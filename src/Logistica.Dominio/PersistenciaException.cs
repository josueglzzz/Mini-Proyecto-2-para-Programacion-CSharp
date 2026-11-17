using System;

namespace Logistica.Dominio
{
    public enum TipoFalloPersistencia
    {
        RutaInvalida,
        AccesoDenegado,
        ArchivoEnUso,
        BaseDatos,
        SinDatos
    }

    /// <summary>
    /// Error de lectura/escritura en CSV o base de datos, envuelto para que la
    /// interfaz no dependa de excepciones técnicas de bajo nivel.
    /// </summary>
    public class PersistenciaException : Exception
    {
        private readonly TipoFalloPersistencia _tipo;

        public PersistenciaException(TipoFalloPersistencia tipo, string mensaje, Exception causa)
            : base(mensaje, causa)
        {
            _tipo = tipo;
        }

        public TipoFalloPersistencia Tipo
        {
            get { return _tipo; }
        }
    }
}
