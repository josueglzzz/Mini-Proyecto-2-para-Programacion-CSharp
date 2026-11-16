using System;
using System.Globalization;
using System.IO;
using System.Text;
using Logistica.Dominio;

namespace Logistica.Persistencia
{
    /// <summary>
    /// RF6: exporta el lote procesado a un archivo CSV delimitado por comas.
    /// El archivo se escribe dentro de un bloque using para liberar el
    /// descriptor aunque ocurra un error.
    /// </summary>
    public class ExportadorCsv
    {
        public const int NumeroColumnas = 12;
        private const int PrimeraColumnaMonto = 4;
        private const char Delimitador = ',';

        /// <param name="encabezados">12 encabezados en el idioma activo.</param>
        /// <param name="etiquetaTotales">Texto de la fila de totales (por ejemplo "TOTAL LOTE").</param>
        public void Exportar(ResultadoLote resultado, string ruta, string[] encabezados, string etiquetaTotales)
        {
            if (resultado == null || resultado.CantidadValidos == 0)
            {
                throw new PersistenciaException(TipoFalloPersistencia.SinDatos, "No hay paquetes válidos para exportar.", null);
            }
            if (encabezados == null || encabezados.Length != NumeroColumnas)
            {
                throw new ArgumentException("Se requieren exactamente " + NumeroColumnas + " encabezados.", nameof(encabezados));
            }
            if (string.IsNullOrWhiteSpace(ruta))
            {
                throw new PersistenciaException(TipoFalloPersistencia.RutaInvalida, "La ruta de destino está vacía.", null);
            }

            try
            {
                using (StreamWriter escritor = new StreamWriter(ruta, false, new UTF8Encoding(true)))
                {
                    escritor.WriteLine(UnirFila(encabezados));

                    Paquete[] paquetes = resultado.Paquetes;
                    decimal[] sumas = new decimal[NumeroColumnas];
                    for (int i = 0; i < paquetes.Length; i++)
                    {
                        decimal[] montos = ObtenerMontos(paquetes[i]);
                        for (int c = PrimeraColumnaMonto; c < NumeroColumnas; c++)
                        {
                            sumas[c] += montos[c];
                        }
                        escritor.WriteLine(UnirFila(ConstruirFila(paquetes[i], montos)));
                    }

                    // Fila final con la suma de cada columna de montos.
                    string[] filaTotales = new string[NumeroColumnas];
                    for (int c = 0; c < NumeroColumnas; c++)
                    {
                        filaTotales[c] = c >= PrimeraColumnaMonto ? FormatearMonto(sumas[c]) : string.Empty;
                    }
                    filaTotales[0] = etiquetaTotales ?? "TOTAL";
                    escritor.WriteLine(UnirFila(filaTotales));
                }
            }
            catch (DirectoryNotFoundException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.RutaInvalida, "La carpeta de destino no existe: " + ruta, ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.AccesoDenegado, "Sin permisos para escribir en: " + ruta, ex);
            }
            catch (PathTooLongException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.RutaInvalida, "La ruta es demasiado larga: " + ruta, ex);
            }
            catch (IOException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.ArchivoEnUso, "No se pudo escribir el archivo (¿está abierto en otro programa?): " + ex.Message, ex);
            }
            catch (NotSupportedException ex)
            {
                throw new PersistenciaException(TipoFalloPersistencia.RutaInvalida, "Formato de ruta no soportado: " + ruta, ex);
            }
        }

        /// <summary>Montos del paquete indexados por columna (4 a 11).</summary>
        private static decimal[] ObtenerMontos(Paquete paquete)
        {
            decimal[] montos = new decimal[NumeroColumnas];
            DesgloseCosto desglose = paquete.Desglose;
            montos[4] = paquete.ValorDeclarado;
            if (desglose != null)
            {
                montos[5] = desglose.SubtotalPeso;
                montos[6] = desglose.RecargoZona;
                montos[7] = desglose.PenalizacionSobrepeso;
                montos[8] = desglose.Seguro;
                montos[9] = desglose.CostoFijoEnvio;
                montos[10] = desglose.Iva;
                montos[11] = desglose.Total;
            }
            return montos;
        }

        private static string[] ConstruirFila(Paquete paquete, decimal[] montos)
        {
            string[] fila = new string[NumeroColumnas];
            fila[0] = paquete.Codigo;
            fila[1] = paquete.Cliente;
            fila[2] = "Zona " + paquete.Zona;
            fila[3] = paquete.PesoKg.ToString("0.###", CultureInfo.InvariantCulture);
            for (int c = PrimeraColumnaMonto; c < NumeroColumnas; c++)
            {
                fila[c] = FormatearMonto(montos[c]);
            }
            return fila;
        }

        private static string UnirFila(string[] campos)
        {
            StringBuilder linea = new StringBuilder();
            for (int i = 0; i < campos.Length; i++)
            {
                if (i > 0)
                {
                    linea.Append(Delimitador);
                }
                linea.Append(EscaparCampo(campos[i]));
            }
            return linea.ToString();
        }

        /// <summary>
        /// Encierra entre comillas los campos con comas, comillas o saltos de
        /// línea y duplica las comillas internas (estándar RFC 4180).
        /// </summary>
        public static string EscaparCampo(string campo)
        {
            if (campo == null)
            {
                return string.Empty;
            }

            bool requiereComillas = campo.IndexOf(Delimitador) >= 0
                || campo.IndexOf('"') >= 0
                || campo.IndexOf('\n') >= 0
                || campo.IndexOf('\r') >= 0;

            if (!requiereComillas)
            {
                return campo;
            }
            return "\"" + campo.Replace("\"", "\"\"") + "\"";
        }

        private static string FormatearMonto(decimal monto)
        {
            return monto.ToString("0.00", CultureInfo.InvariantCulture);
        }
    }
}
