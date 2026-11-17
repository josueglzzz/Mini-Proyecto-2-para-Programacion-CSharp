using System;

namespace Logistica.Dominio
{
    public enum MotivoErrorConfiguracion
    {
        ArchivoNoEncontrado,
        AccesoDenegado,
        FormatoInvalido,
        ValorInvalido
    }

    /// <summary>
    /// Error al cargar appsettings.json. El motivo permite que la interfaz
    /// muestre un diagnóstico claro en el idioma activo.
    /// </summary>
    public class ConfiguracionException : Exception
    {
        private readonly MotivoErrorConfiguracion _motivo;
        private readonly string _ruta;

        public ConfiguracionException(MotivoErrorConfiguracion motivo, string ruta, string mensaje, Exception causa)
            : base(mensaje, causa)
        {
            _motivo = motivo;
            _ruta = ruta ?? string.Empty;
        }

        public MotivoErrorConfiguracion Motivo
        {
            get { return _motivo; }
        }

        public string Ruta
        {
            get { return _ruta; }
        }
    }
}
