using System;
using System.IO;
using Logistica.Dominio;
using Logistica.Logica;
using Logistica.Persistencia;

namespace Logistica.Pruebas
{
    /// <summary>Pruebas de JSON, CSV y base de datos (Parte 2).</summary>
    public static class PruebasPersistencia
    {
        private const string JsonValido =
            "{\n" +
            "  // Los comentarios se permiten\n" +
            "  \"Tarifas\": { \"PrecioBasePorKg\": 25.0, \"TasaIva\": 0.16, \"CostoFijoEnvio\": 50.0, \"RecargoZonaC\": 0.20 },\n" +
            "  \"BaseDatos\": { \"Archivo\": \"prueba.db\" },\n" +
            "  \"Interfaz\": { \"IdiomaPredeterminado\": \"EN\" }\n" +
            "}";

        public static void Ejecutar(Verificador v)
        {
            string carpeta = Path.Combine(Path.GetTempPath(), "LogisticaPruebas_" + Guid.NewGuid().ToString("N"));
            Directory.CreateDirectory(carpeta);
            try
            {
                ProbarConfiguracion(v, carpeta);
                ProbarCsv(v, carpeta);
                ProbarBaseDatos(v, carpeta);
            }
            finally
            {
                try
                {
                    Directory.Delete(carpeta, true);
                }
                catch (IOException)
                {
                    // El archivo de SQLite puede seguir bloqueado unos milisegundos.
                }
            }
        }

        private static void ProbarConfiguracion(Verificador v, string carpeta)
        {
            Console.WriteLine(" Configuración JSON");
            string ruta = Path.Combine(carpeta, "appsettings.json");
            File.WriteAllText(ruta, JsonValido);

            ConfiguracionRepositorio repositorio = new ConfiguracionRepositorio(ruta, carpeta);
            ConfiguracionAplicacion configuracion = repositorio.Cargar();
            v.Igual(25.0m, configuracion.Tarifas.PrecioBasePorKg, "Lee PrecioBasePorKg");
            v.Igual(0.16m, configuracion.Tarifas.TasaIva, "Lee TasaIva");
            v.Igual(50.0m, configuracion.Tarifas.CostoFijoEnvio, "Lee CostoFijoEnvio");
            v.Igual(0.20m, configuracion.Tarifas.RecargoZonaC, "Lee recargo opcional de zona C");
            v.Igual(0.05m, configuracion.Tarifas.RecargoZonaA, "Usa valor predeterminado para zona A");
            v.Igual("en", configuracion.IdiomaPredeterminado, "Normaliza el idioma a minúsculas");
            v.Igual(Path.Combine(carpeta, "prueba.db"), configuracion.RutaBaseDatos, "Resuelve la ruta relativa de la base de datos");

            ConfiguracionRepositorio inexistente = new ConfiguracionRepositorio(Path.Combine(carpeta, "no_existe.json"), carpeta);
            VerificarMotivo(v, inexistente, null, MotivoErrorConfiguracion.ArchivoNoEncontrado, "Archivo inexistente se informa");
            VerificarMotivo(v, repositorio, "{ \"Tarifas\": { ", MotivoErrorConfiguracion.FormatoInvalido, "JSON mal formado se informa");
            VerificarMotivo(v, repositorio, "{ \"Tarifas\": { \"TasaIva\": 0.16, \"CostoFijoEnvio\": 50 } }", MotivoErrorConfiguracion.ValorInvalido, "Falta PrecioBasePorKg");
            VerificarMotivo(v, repositorio, "{ \"Tarifas\": { \"PrecioBasePorKg\": \"veinte\", \"TasaIva\": 0.16, \"CostoFijoEnvio\": 50 } }", MotivoErrorConfiguracion.ValorInvalido, "Precio no numérico");
            VerificarMotivo(v, repositorio, "{ \"Tarifas\": { \"PrecioBasePorKg\": 25, \"TasaIva\": 16, \"CostoFijoEnvio\": 50 } }", MotivoErrorConfiguracion.ValorInvalido, "IVA fuera de rango (16 en vez de 0.16)");
            VerificarMotivo(v, repositorio, "{ }", MotivoErrorConfiguracion.ValorInvalido, "Falta la sección Tarifas");
        }

        private static void VerificarMotivo(Verificador v, ConfiguracionRepositorio repositorio, string json, MotivoErrorConfiguracion esperado, string descripcion)
        {
            try
            {
                if (json == null)
                {
                    repositorio.Cargar();
                }
                else
                {
                    repositorio.Interpretar(json);
                }
                v.Verdadero(false, descripcion + " (no se lanzó excepción)");
            }
            catch (ConfiguracionException ex)
            {
                v.Verdadero(ex.Motivo == esperado, descripcion + " [" + ex.Motivo + "]");
            }
        }

        private static ResultadoLote CrearLote()
        {
            ProcesadorLotes procesador = new ProcesadorLotes(new CalculadoraTarifas(new ConfiguracionTarifas(25m, 0.16m, 50m)));
            return procesador.Procesar(
                "PACK-001;Juan Perez;Zona A;12.5;150.0\n" +
                "PACK-002;Maria Lopez;Zona C;-5.0;200.0\n" +
                "PACK-010;Rosa Vega;Zona C;25;1500\n");
        }

        private static string[] Encabezados()
        {
            return new string[] { "Codigo", "Cliente", "Zona", "Peso", "Valor", "Subtotal", "Recargo", "Sobrepeso", "Seguro", "Fijo", "IVA", "Total" };
        }

        private static void ProbarCsv(Verificador v, string carpeta)
        {
            Console.WriteLine(" Exportación CSV");
            ResultadoLote lote = CrearLote();
            string ruta = Path.Combine(carpeta, "reporte.csv");

            ExportadorCsv exportador = new ExportadorCsv();
            exportador.Exportar(lote, ruta, Encabezados(), "TOTAL LOTE");

            string[] lineas = File.ReadAllLines(ruta);
            v.Igual(4, lineas.Length, "Encabezado + 2 paquetes + totales");
            v.Igual("Codigo,Cliente,Zona,Peso,Valor,Subtotal,Recargo,Sobrepeso,Seguro,Fijo,IVA,Total", lineas[0], "Encabezado separado por comas");
            v.Igual("PACK-010,Rosa Vega,Zona C,25,1500.00,625.00,112.50,31.25,30.00,50.00,135.80,984.55", lineas[1], "Fila del paquete más caro primero");
            v.Igual("TOTAL LOTE,,,,1650.00,937.50,128.13,31.25,30.00,100.00,196.30,1423.18", lineas[3], "Fila de totales");

            v.Igual("\"Perez, Juan\"", ExportadorCsv.EscaparCampo("Perez, Juan"), "Escapa comas con comillas");
            v.Igual("\"Dijo \"\"hola\"\"\"", ExportadorCsv.EscaparCampo("Dijo \"hola\""), "Duplica comillas internas");

            try
            {
                exportador.Exportar(lote, Path.Combine(carpeta, "no", "existe", "r.csv"), Encabezados(), "TOTAL");
                v.Verdadero(false, "Carpeta inexistente debe fallar");
            }
            catch (PersistenciaException ex)
            {
                v.Verdadero(ex.Tipo == TipoFalloPersistencia.RutaInvalida, "Carpeta inexistente se informa como ruta inválida");
            }
        }

        private static void ProbarBaseDatos(Verificador v, string carpeta)
        {
            Console.WriteLine(" Base de datos SQLite");
            RepositorioEnvios repositorio = new RepositorioEnvios(Path.Combine(carpeta, "datos", "envios.db"));
            repositorio.InicializarBaseDatos();
            repositorio.InicializarBaseDatos();
            v.Verdadero(File.Exists(repositorio.RutaArchivo), "Crea carpeta y archivo de base de datos (idempotente)");

            ResultadoLote lote = CrearLote();
            long loteId = repositorio.GuardarLote(lote);
            v.Verdadero(loteId > 0, "Guarda el lote y devuelve Id");
            v.Verdadero(lote.Paquetes[0].Id > 0, "Asigna Id a los paquetes");

            repositorio.GuardarLote(CrearLote());
            ResumenLote[] historial = repositorio.ObtenerHistorial();
            v.Igual(2, historial.Length, "Historial con 2 lotes");
            v.Verdadero(historial[0].Id > historial[1].Id, "Historial del más reciente al más antiguo");
            v.Igual(1, historial[0].PaquetesConError, "Guarda el número de errores");
            v.Igual(1423.18m, historial[0].Total, "Guarda el total del lote");

            Paquete[] paquetes = repositorio.ObtenerPaquetesDeLote(loteId);
            v.Igual(2, paquetes.Length, "Recupera 2 paquetes del lote");
            v.Igual("PACK-010", paquetes[0].Codigo, "Recupera ordenado por total");
            v.Igual(984.55m, paquetes[0].Desglose.Total, "Recupera el desglose");
            v.Verdadero(paquetes[0].Zona == ZonaEnvio.C, "Recupera la zona");
            v.Verdadero(paquetes[0].Estado == EstadoEnvio.EnTransito, "Estado inicial: en tránsito");

            repositorio.RegistrarRecepcion(paquetes[0], new DateTime(2026, 11, 11, 10, 30, 0));
            Paquete[] recargados = repositorio.ObtenerPaquetesDeLote(loteId);
            v.Verdadero(recargados[0].Estado == EstadoEnvio.Entregado, "Recepción guardada en la base de datos");
            v.Verdadero(recargados[0].FechaRecepcion.HasValue && recargados[0].FechaRecepcion.Value.Hour == 10, "Guarda la fecha de recepción");
            v.LanzaExcepcion<InvalidOperationException>(() => repositorio.RegistrarRecepcion(recargados[0], DateTime.Now), "No permite recibir dos veces");

            ResumenLote[] historialFinal = repositorio.ObtenerHistorial();
            v.Igual(1, historialFinal[1].PaquetesEntregados, "El historial cuenta paquetes entregados");
        }
    }
}
