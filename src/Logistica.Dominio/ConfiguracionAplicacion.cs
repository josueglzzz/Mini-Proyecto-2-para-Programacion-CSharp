using System;

namespace Logistica.Dominio
{
    /// <summary>
    /// Configuración global leída de appsettings.json: tarifas, base de datos
    /// e idioma inicial de la interfaz.
    /// </summary>
    public class ConfiguracionAplicacion
    {
        private readonly ConfiguracionTarifas _tarifas;
        private readonly string _rutaBaseDatos;
        private readonly string _idiomaPredeterminado;

        public ConfiguracionAplicacion(ConfiguracionTarifas tarifas, string rutaBaseDatos, string idiomaPredeterminado)
        {
            if (tarifas == null)
            {
                throw new ArgumentNullException(nameof(tarifas));
            }
            if (string.IsNullOrWhiteSpace(rutaBaseDatos))
            {
                throw new ArgumentException("La ruta de la base de datos es obligatoria.", nameof(rutaBaseDatos));
            }

            _tarifas = tarifas;
            _rutaBaseDatos = rutaBaseDatos;
            _idiomaPredeterminado = string.IsNullOrWhiteSpace(idiomaPredeterminado) ? "es" : idiomaPredeterminado.Trim().ToLowerInvariant();
        }

        public ConfiguracionTarifas Tarifas
        {
            get { return _tarifas; }
        }

        /// <summary>Ruta absoluta del archivo SQLite.</summary>
        public string RutaBaseDatos
        {
            get { return _rutaBaseDatos; }
        }

        /// <summary>"es" o "en".</summary>
        public string IdiomaPredeterminado
        {
            get { return _idiomaPredeterminado; }
        }
    }
}
