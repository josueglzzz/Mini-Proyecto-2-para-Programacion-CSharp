using System;
using System.IO;
using System.Text.Json;
using Logistica.Dominio;

namespace Logistica.Persistencia
{
    /// <summary>
    /// RF1: lee appsettings.json con la biblioteca estándar System.Text.Json.
    /// Cualquier problema (archivo ausente, JSON mal formado, valores fuera de
    /// rango) se reporta como ConfiguracionException con un motivo claro.
    /// </summary>
    public class ConfiguracionRepositorio
    {
        public const string NombreArchivo = "appsettings.json";
        public const string CarpetaDatos = "LogisticaFacturacion";

        private readonly string _rutaArchivo;
        private readonly string _carpetaDatosPredeterminada;

        /// <summary>Usa appsettings.json junto al ejecutable y %LOCALAPPDATA% para datos.</summary>
        public ConfiguracionRepositorio()
            : this(
                Path.Combine(AppContext.BaseDirectory, NombreArchivo),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), CarpetaDatos))
        {
        }

        public ConfiguracionRepositorio(string rutaArchivo, string carpetaDatosPredeterminada)
        {
            if (string.IsNullOrWhiteSpace(rutaArchivo))
            {
                throw new ArgumentException("La ruta del archivo de configuración es obligatoria.", nameof(rutaArchivo));
            }
            _rutaArchivo = rutaArchivo;
            _carpetaDatosPredeterminada = carpetaDatosPredeterminada;
        }

        public string RutaArchivo
        {
            get { return _rutaArchivo; }
        }

        public ConfiguracionAplicacion Cargar()
        {
            if (!File.Exists(_rutaArchivo))
            {
                throw new ConfiguracionException(
                    MotivoErrorConfiguracion.ArchivoNoEncontrado,
                    _rutaArchivo,
                    "No se encontró el archivo de configuración: " + _rutaArchivo,
                    null);
            }

            string contenido;
            try
            {
                contenido = File.ReadAllText(_rutaArchivo);
            }
            catch (UnauthorizedAccessException ex)
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.AccesoDenegado, _rutaArchivo, "Sin permisos para leer " + _rutaArchivo, ex);
            }
            catch (IOException ex)
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.AccesoDenegado, _rutaArchivo, "No se pudo leer " + _rutaArchivo + ": " + ex.Message, ex);
            }

            return Interpretar(contenido);
        }

        /// <summary>Convierte el texto JSON en objetos de dominio validados.</summary>
        public ConfiguracionAplicacion Interpretar(string contenidoJson)
        {
            JsonDocumentOptions opciones = new JsonDocumentOptions();
            opciones.AllowTrailingCommas = true;
            opciones.CommentHandling = JsonCommentHandling.Skip;

            try
            {
                using (JsonDocument documento = JsonDocument.Parse(contenidoJson, opciones))
                {
                    JsonElement raiz = documento.RootElement;
                    JsonElement seccionTarifas = ObtenerSeccion(raiz, "Tarifas", true);

                    ConfiguracionTarifas tarifas = new ConfiguracionTarifas(
                        LeerDecimal(seccionTarifas, "PrecioBasePorKg", true, 0m),
                        LeerDecimal(seccionTarifas, "TasaIva", true, 0m),
                        LeerDecimal(seccionTarifas, "CostoFijoEnvio", true, 0m));

                    tarifas.RecargoZonaA = LeerDecimal(seccionTarifas, "RecargoZonaA", false, tarifas.RecargoZonaA);
                    tarifas.RecargoZonaB = LeerDecimal(seccionTarifas, "RecargoZonaB", false, tarifas.RecargoZonaB);
                    tarifas.RecargoZonaC = LeerDecimal(seccionTarifas, "RecargoZonaC", false, tarifas.RecargoZonaC);
                    tarifas.PesoLimiteKg = LeerDecimal(seccionTarifas, "PesoLimiteKg", false, tarifas.PesoLimiteKg);
                    tarifas.PenalizacionSobrepeso = LeerDecimal(seccionTarifas, "PenalizacionSobrepeso", false, tarifas.PenalizacionSobrepeso);
                    tarifas.UmbralSeguro = LeerDecimal(seccionTarifas, "UmbralSeguro", false, tarifas.UmbralSeguro);
                    tarifas.TasaSeguro = LeerDecimal(seccionTarifas, "TasaSeguro", false, tarifas.TasaSeguro);

                    JsonElement seccionBaseDatos = ObtenerSeccion(raiz, "BaseDatos", false);
                    string archivoBaseDatos = LeerTexto(seccionBaseDatos, "Archivo", "logistica_envios.db");

                    JsonElement seccionInterfaz = ObtenerSeccion(raiz, "Interfaz", false);
                    string idioma = LeerTexto(seccionInterfaz, "IdiomaPredeterminado", "es");

                    return new ConfiguracionAplicacion(tarifas, ResolverRutaBaseDatos(archivoBaseDatos), idioma);
                }
            }
            catch (JsonException ex)
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.FormatoInvalido, _rutaArchivo, "El JSON no tiene un formato válido: " + ex.Message, ex);
            }
            catch (ArgumentOutOfRangeException ex)
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.ValorInvalido, _rutaArchivo, ex.Message, ex);
            }
        }

        private string ResolverRutaBaseDatos(string archivo)
        {
            if (Path.IsPathRooted(archivo) || string.IsNullOrWhiteSpace(_carpetaDatosPredeterminada))
            {
                return archivo;
            }
            return Path.Combine(_carpetaDatosPredeterminada, archivo);
        }

        private JsonElement ObtenerSeccion(JsonElement raiz, string nombre, bool obligatoria)
        {
            JsonElement seccion;
            if (raiz.ValueKind == JsonValueKind.Object
                && raiz.TryGetProperty(nombre, out seccion)
                && seccion.ValueKind == JsonValueKind.Object)
            {
                return seccion;
            }

            if (obligatoria)
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.ValorInvalido, _rutaArchivo, "Falta la sección \"" + nombre + "\" en " + NombreArchivo, null);
            }
            return default(JsonElement);
        }

        private decimal LeerDecimal(JsonElement seccion, string nombre, bool obligatorio, decimal predeterminado)
        {
            JsonElement valor;
            if (seccion.ValueKind != JsonValueKind.Object || !seccion.TryGetProperty(nombre, out valor))
            {
                if (obligatorio)
                {
                    throw new ConfiguracionException(MotivoErrorConfiguracion.ValorInvalido, _rutaArchivo, "Falta el parámetro obligatorio \"" + nombre + "\".", null);
                }
                return predeterminado;
            }

            decimal numero;
            if (valor.ValueKind != JsonValueKind.Number || !valor.TryGetDecimal(out numero))
            {
                throw new ConfiguracionException(MotivoErrorConfiguracion.ValorInvalido, _rutaArchivo, "El parámetro \"" + nombre + "\" debe ser numérico.", null);
            }
            return numero;
        }

        private static string LeerTexto(JsonElement seccion, string nombre, string predeterminado)
        {
            JsonElement valor;
            if (seccion.ValueKind == JsonValueKind.Object
                && seccion.TryGetProperty(nombre, out valor)
                && valor.ValueKind == JsonValueKind.String)
            {
                string texto = valor.GetString();
                if (!string.IsNullOrWhiteSpace(texto))
                {
                    return texto.Trim();
                }
            }
            return predeterminado;
        }
    }
}
