using System;
using System.Globalization;
using System.IO;
using Logistica.Dominio;
using Microsoft.Data.Sqlite;

namespace Logistica.Persistencia
{
    /// <summary>
    /// Persistencia en base de datos SQLite de los lotes procesados (envíos)
    /// y de las recepciones de paquetes. Cada lote se guarda dentro de una
    /// transacción: o se guarda completo o no se guarda nada.
    /// </summary>
    public class RepositorioEnvios
    {
        private const string FormatoFecha = "yyyy-MM-dd HH:mm:ss";

        private const string ScriptEsquema =
            "CREATE TABLE IF NOT EXISTS Lotes (" +
            " Id INTEGER PRIMARY KEY AUTOINCREMENT," +
            " FechaProceso TEXT NOT NULL," +
            " TotalLineas INTEGER NOT NULL," +
            " PaquetesValidos INTEGER NOT NULL," +
            " PaquetesConError INTEGER NOT NULL," +
            " SumaBaseImponible REAL NOT NULL," +
            " SumaIva REAL NOT NULL," +
            " SumaTotal REAL NOT NULL);" +
            "CREATE TABLE IF NOT EXISTS Paquetes (" +
            " Id INTEGER PRIMARY KEY AUTOINCREMENT," +
            " LoteId INTEGER NOT NULL REFERENCES Lotes(Id) ON DELETE CASCADE," +
            " NumeroLinea INTEGER NOT NULL," +
            " Codigo TEXT NOT NULL," +
            " Cliente TEXT NOT NULL," +
            " Zona TEXT NOT NULL," +
            " PesoKg REAL NOT NULL," +
            " ValorDeclarado REAL NOT NULL," +
            " SubtotalPeso REAL NOT NULL," +
            " PorcentajeRecargoZona REAL NOT NULL," +
            " RecargoZona REAL NOT NULL," +
            " KilosExcedentes REAL NOT NULL," +
            " PenalizacionSobrepeso REAL NOT NULL," +
            " Seguro REAL NOT NULL," +
            " CostoFijoEnvio REAL NOT NULL," +
            " BaseImponible REAL NOT NULL," +
            " Iva REAL NOT NULL," +
            " Total REAL NOT NULL," +
            " Estado TEXT NOT NULL DEFAULT 'EnTransito'," +
            " FechaRecepcion TEXT NULL);" +
            "CREATE TABLE IF NOT EXISTS ErroresLote (" +
            " Id INTEGER PRIMARY KEY AUTOINCREMENT," +
            " LoteId INTEGER NOT NULL REFERENCES Lotes(Id) ON DELETE CASCADE," +
            " NumeroLinea INTEGER NOT NULL," +
            " TipoError TEXT NOT NULL," +
            " Detalle TEXT NOT NULL," +
            " ContenidoOriginal TEXT NOT NULL);" +
            "CREATE INDEX IF NOT EXISTS IX_Paquetes_LoteId ON Paquetes(LoteId);" +
            "CREATE INDEX IF NOT EXISTS IX_ErroresLote_LoteId ON ErroresLote(LoteId);";

        private readonly string _rutaArchivo;
        private readonly string _cadenaConexion;

        public RepositorioEnvios(string rutaArchivo)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException("La ruta de la base de datos es obligatoria.", nameof(rutaArchivo));
            }
            _rutaArchivo = rutaArchivo;
            _cadenaConexion = "Data Source=" + rutaArchivo;
        }

        public string RutaArchivo
        {
            get { return _rutaArchivo; }
        }

        /// <summary>Crea la carpeta y las tablas si todavía no existen.</summary>
        public void InicializarBaseDatos()
        {
            try
            {
                string carpeta = Path.GetDirectoryName(Path.GetFullPath(_rutaArchivo));
                if (!string.IsNullOrEmpty(carpeta) && !Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                using (SqliteConnection conexion = AbrirConexion())
                using (SqliteCommand comando = conexion.CreateCommand())
                {
                    comando.CommandText = ScriptEsquema;
                    comando.ExecuteNonQuery();
                }
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.AccesoDenegado, "Sin permisos para crear la base de datos en " + _rutaArchivo, ex);
            }
            catch (IOException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.RutaInvalida, "No se pudo preparar la carpeta de la base de datos: " + ex.Message, ex);
            }
            catch (SqliteException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.BaseDatos, "Error al crear las tablas: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Registra el lote completo (envíos y errores) en una sola transacción.
        /// Devuelve el Id generado y lo asigna al lote y a cada paquete.
        /// </summary>
        public long GuardarLote(ResultadoLote resultado)
        {
            if (resultado == null)
            {
                throw new ArgumentNullException(nameof(resultado));
            }

            try
            {
                using (SqliteConnection conexion = AbrirConexion())
                using (SqliteTransaction transaccion = conexion.BeginTransaction())
                {
                    try
                    {
                        long loteId = InsertarLote(conexion, transaccion, resultado);

                        Paquete[] paquetes = resultado.Paquetes;
                        for (int i = 0; i < paquetes.Length; i++)
                        {
                            paquetes[i].LoteId = loteId;
                            paquetes[i].Id = InsertarPaquete(conexion, transaccion, loteId, paquetes[i]);
                        }

                        ErrorLinea[] errores = resultado.Errores;
                        for (int i = 0; i < errores.Length; i++)
                        {
                            InsertarError(conexion, transaccion, loteId, errores[i]);
                        }

                        transaccion.Commit();
                        resultado.Id = loteId;
                        return loteId;
                    }
                    catch
                    {
                        transaccion.Rollback();
                        throw;
                    }
                }
            }
            catch (SqliteException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.BaseDatos, "No se pudo guardar el lote: " + ex.Message, ex);
            }
        }

        /// <summary>Historial de lotes del más reciente al más antiguo.</summary>
        public ResumenLote[] ObtenerHistorial()
        {
            try
            {
                using (SqliteConnection conexion = AbrirConexion())
                {
                    int cantidad = ContarRegistros(conexion, "SELECT COUNT(*) FROM Lotes", 0);
                    ResumenLote[] historial = new ResumenLote[cantidad];

                    using (SqliteCommand comando = conexion.CreateCommand())
                    {
                        comando.CommandText =
                            "SELECT l.Id, l.FechaProceso, l.TotalLineas, l.PaquetesValidos, l.PaquetesConError, l.SumaTotal," +
                            " (SELECT COUNT(*) FROM Paquetes p WHERE p.LoteId = l.Id AND p.Estado = 'Entregado')" +
                            " FROM Lotes l ORDER BY l.Id DESC";

                        using (SqliteDataReader lector = comando.ExecuteReader())
                        {
                            int indice = 0;
                            while (lector.Read() && indice < cantidad)
                            {
                                historial[indice] = new ResumenLote(
                                    Convert.ToInt64(lector.GetValue(0), CultureInfo.InvariantCulture),
                                    LeerFecha(lector.GetString(1)),
                                    Convert.ToInt32(lector.GetValue(2), CultureInfo.InvariantCulture),
                                    Convert.ToInt32(lector.GetValue(3), CultureInfo.InvariantCulture),
                                    Convert.ToInt32(lector.GetValue(4), CultureInfo.InvariantCulture),
                                    LeerMonto(lector.GetValue(5)),
                                    Convert.ToInt32(lector.GetValue(6), CultureInfo.InvariantCulture));
                                indice++;
                            }
                        }
                    }
                    return historial;
                }
            }
            catch (SqliteException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.BaseDatos, "No se pudo leer el historial: " + ex.Message, ex);
            }
        }

        /// <summary>Recupera los paquetes de un lote guardado, con su desglose y estado.</summary>
        public Paquete[] ObtenerPaquetesDeLote(long loteId)
        {
            try
            {
                using (SqliteConnection conexion = AbrirConexion())
                {
                    int cantidad = ContarRegistros(conexion, "SELECT COUNT(*) FROM Paquetes WHERE LoteId = @loteId", loteId);
                    Paquete[] paquetes = new Paquete[cantidad];

                    using (SqliteCommand comando = conexion.CreateCommand())
                    {
                        comando.CommandText =
                            "SELECT Id, LoteId, NumeroLinea, Codigo, Cliente, Zona, PesoKg, ValorDeclarado," +
                            " SubtotalPeso, PorcentajeRecargoZona, RecargoZona, KilosExcedentes, PenalizacionSobrepeso," +
                            " Seguro, CostoFijoEnvio, BaseImponible, Iva, Total, Estado, FechaRecepcion" +
                            " FROM Paquetes WHERE LoteId = @loteId ORDER BY Total DESC";
                        comando.Parameters.AddWithValue("@loteId", loteId);

                        using (SqliteDataReader lector = comando.ExecuteReader())
                        {
                            int indice = 0;
                            while (lector.Read() && indice < cantidad)
                            {
                                paquetes[indice] = LeerPaquete(lector);
                                indice++;
                            }
                        }
                    }
                    return paquetes;
                }
            }
            catch (SqliteException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.BaseDatos, "No se pudieron leer los paquetes del lote: " + ex.Message, ex);
            }
        }

        /// <summary>
        /// Registra la recepción (entrega) de un paquete: actualiza la base de
        /// datos y después el objeto en memoria.
        /// </summary>
        public void RegistrarRecepcion(Paquete paquete, DateTime fechaRecepcion)
        {
            if (paquete == null)
            {
                throw new ArgumentNullException(nameof(paquete));
            }
            if (paquete.Id <= 0)
            {
                throw new InvalidOperationException("El paquete " + paquete.Codigo + " no ha sido guardado en la base de datos.");
            }
            if (paquete.Estado == EstadoEnvio.Entregado)
            {
                throw new InvalidOperationException("El paquete " + paquete.Codigo + " ya fue entregado.");
            }

            try
            {
                using (SqliteConnection conexion = AbrirConexion())
                using (SqliteCommand comando = conexion.CreateCommand())
                {
                    comando.CommandText =
                        "UPDATE Paquetes SET Estado = 'Entregado', FechaRecepcion = @fecha" +
                        " WHERE Id = @id AND Estado = 'EnTransito'";
                    comando.Parameters.AddWithValue("@fecha", fechaRecepcion.ToString(FormatoFecha, CultureInfo.InvariantCulture));
                    comando.Parameters.AddWithValue("@id", paquete.Id);

                    int filasAfectadas = comando.ExecuteNonQuery();
                    if (filasAfectadas == 0)
                    {
                        throw new InvalidOperationException("El paquete " + paquete.Codigo + " no existe o ya fue entregado.");
                    }
                }
                paquete.RegistrarRecepcion(fechaRecepcion);
            }
            catch (SqliteException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.BaseDatos, "No se pudo registrar la recepción: " + ex.Message, ex);
            }
        }

        private SqliteConnection AbrirConexion()
        {
            SqliteConnection conexion = new SqliteConnection(_cadenaConexion);
            conexion.Open();
            using (SqliteCommand pragma = conexion.CreateCommand())
            {
                pragma.CommandText = "PRAGMA foreign_keys = ON;";
                pragma.ExecuteNonQuery();
            }
            return conexion;
        }

        private static long InsertarLote(SqliteConnection conexion, SqliteTransaction transaccion, ResultadoLote resultado)
        {
            using (SqliteCommand comando = conexion.CreateCommand())
            {
                comando.Transaction = transaccion;
                comando.CommandText =
                    "INSERT INTO Lotes (FechaProceso, TotalLineas, PaquetesValidos, PaquetesConError, SumaBaseImponible, SumaIva, SumaTotal)" +
                    " VALUES (@fecha, @lineas, @validos, @errores, @base, @iva, @total);" +
                    " SELECT last_insert_rowid();";
                comando.Parameters.AddWithValue("@fecha", resultado.FechaProceso.ToString(FormatoFecha, CultureInfo.InvariantCulture));
                comando.Parameters.AddWithValue("@lineas", resultado.TotalLineas);
                comando.Parameters.AddWithValue("@validos", resultado.CantidadValidos);
                comando.Parameters.AddWithValue("@errores", resultado.CantidadErrores);
                comando.Parameters.AddWithValue("@base", (double)resultado.SumaBaseImponible);
                comando.Parameters.AddWithValue("@iva", (double)resultado.SumaIva);
                comando.Parameters.AddWithValue("@total", (double)resultado.SumaTotal);
                return Convert.ToInt64(comando.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
        }

        private static long InsertarPaquete(SqliteConnection conexion, SqliteTransaction transaccion, long loteId, Paquete paquete)
        {
            DesgloseCosto d = paquete.Desglose;
            if (d == null)
            {
                throw new InvalidOperationException("El paquete " + paquete.Codigo + " no tiene desglose calculado.");
            }

            using (SqliteCommand comando = conexion.CreateCommand())
            {
                comando.Transaction = transaccion;
                comando.CommandText =
                    "INSERT INTO Paquetes (LoteId, NumeroLinea, Codigo, Cliente, Zona, PesoKg, ValorDeclarado, SubtotalPeso," +
                    " PorcentajeRecargoZona, RecargoZona, KilosExcedentes, PenalizacionSobrepeso, Seguro, CostoFijoEnvio," +
                    " BaseImponible, Iva, Total, Estado, FechaRecepcion)" +
                    " VALUES (@lote, @linea, @codigo, @cliente, @zona, @peso, @valor, @subtotal, @porcentaje, @recargo," +
                    " @excedentes, @penalizacion, @seguro, @fijo, @base, @iva, @total, @estado, @recepcion);" +
                    " SELECT last_insert_rowid();";
                comando.Parameters.AddWithValue("@lote", loteId);
                comando.Parameters.AddWithValue("@linea", paquete.NumeroLinea);
                comando.Parameters.AddWithValue("@codigo", paquete.Codigo);
                comando.Parameters.AddWithValue("@cliente", paquete.Cliente);
                comando.Parameters.AddWithValue("@zona", paquete.Zona.ToString());
                comando.Parameters.AddWithValue("@peso", (double)paquete.PesoKg);
                comando.Parameters.AddWithValue("@valor", (double)paquete.ValorDeclarado);
                comando.Parameters.AddWithValue("@subtotal", (double)d.SubtotalPeso);
                comando.Parameters.AddWithValue("@porcentaje", (double)d.PorcentajeRecargoZona);
                comando.Parameters.AddWithValue("@recargo", (double)d.RecargoZona);
                comando.Parameters.AddWithValue("@excedentes", (double)d.KilosExcedentes);
                comando.Parameters.AddWithValue("@penalizacion", (double)d.PenalizacionSobrepeso);
                comando.Parameters.AddWithValue("@seguro", (double)d.Seguro);
                comando.Parameters.AddWithValue("@fijo", (double)d.CostoFijoEnvio);
                comando.Parameters.AddWithValue("@base", (double)d.BaseImponible);
                comando.Parameters.AddWithValue("@iva", (double)d.Iva);
                comando.Parameters.AddWithValue("@total", (double)d.Total);
                comando.Parameters.AddWithValue("@estado", paquete.Estado.ToString());
                if (paquete.FechaRecepcion.HasValue)
                {
                    comando.Parameters.AddWithValue("@recepcion", paquete.FechaRecepcion.Value.ToString(FormatoFecha, CultureInfo.InvariantCulture));
                }
                else
                {
                    comando.Parameters.AddWithValue("@recepcion", DBNull.Value);
                }
                return Convert.ToInt64(comando.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
        }

        private static void InsertarError(SqliteConnection conexion, SqliteTransaction transaccion, long loteId, ErrorLinea error)
        {
            using (SqliteCommand comando = conexion.CreateCommand())
            {
                comando.Transaction = transaccion;
                comando.CommandText =
                    "INSERT INTO ErroresLote (LoteId, NumeroLinea, TipoError, Detalle, ContenidoOriginal)" +
                    " VALUES (@lote, @linea, @tipo, @detalle, @contenido)";
                comando.Parameters.AddWithValue("@lote", loteId);
                comando.Parameters.AddWithValue("@linea", error.NumeroLinea);
                comando.Parameters.AddWithValue("@tipo", error.Tipo.ToString());
                comando.Parameters.AddWithValue("@detalle", error.Detalle);
                comando.Parameters.AddWithValue("@contenido", error.ContenidoOriginal);
                comando.ExecuteNonQuery();
            }
        }

        private static int ContarRegistros(SqliteConnection conexion, string consulta, long loteId)
        {
            using (SqliteCommand comando = conexion.CreateCommand())
            {
                comando.CommandText = consulta;
                if (consulta.Contains("@loteId"))
                {
                    comando.Parameters.AddWithValue("@loteId", loteId);
                }
                return Convert.ToInt32(comando.ExecuteScalar(), CultureInfo.InvariantCulture);
            }
        }

        private static Paquete LeerPaquete(SqliteDataReader lector)
        {
            ZonaEnvio zona = (ZonaEnvio)Enum.Parse(typeof(ZonaEnvio), lector.GetString(5));
            Paquete paquete = new Paquete(lector.GetString(3), lector.GetString(4), zona, LeerMonto(lector.GetValue(6)), LeerMonto(lector.GetValue(7)));
            paquete.Id = Convert.ToInt64(lector.GetValue(0), CultureInfo.InvariantCulture);
            paquete.LoteId = Convert.ToInt64(lector.GetValue(1), CultureInfo.InvariantCulture);
            paquete.NumeroLinea = Convert.ToInt32(lector.GetValue(2), CultureInfo.InvariantCulture);
            paquete.Desglose = new DesgloseCosto(
                LeerMonto(lector.GetValue(8)),
                LeerMonto(lector.GetValue(9)),
                LeerMonto(lector.GetValue(10)),
                LeerMonto(lector.GetValue(11)),
                LeerMonto(lector.GetValue(12)),
                LeerMonto(lector.GetValue(13)),
                LeerMonto(lector.GetValue(14)),
                LeerMonto(lector.GetValue(15)),
                LeerMonto(lector.GetValue(16)),
                LeerMonto(lector.GetValue(17)));
            paquete.Estado = (EstadoEnvio)Enum.Parse(typeof(EstadoEnvio), lector.GetString(18));
            if (!lector.IsDBNull(19))
            {
                paquete.FechaRecepcion = LeerFecha(lector.GetString(19));
            }
            return paquete;
        }

        private static decimal LeerMonto(object valor)
        {
            if (valor == null || valor is DBNull)
            {
                return 0m;
            }
            return Math.Round(Convert.ToDecimal(valor, CultureInfo.InvariantCulture), 4, MidpointRounding.AwayFromZero);
        }

        private static DateTime LeerFecha(string texto)
        {
            return DateTime.ParseExact(texto, FormatoFecha, CultureInfo.InvariantCulture);
        }
    }
}
