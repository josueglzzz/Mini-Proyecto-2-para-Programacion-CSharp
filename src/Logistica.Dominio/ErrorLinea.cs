using System;

namespace Logistica.Dominio
{
    /// <summary>
    /// Representa una línea del lote que no pasó la validación.
    /// Se almacena en un arreglo paralelo al de paquetes válidos (Reto 1).
    /// </summary>
    public class ErrorLinea
    {
        private readonly int _numeroLinea;
        private readonly string _contenidoOriginal;
        private readonly TipoErrorLinea _tipo;
        private readonly string _detalle;

        public ErrorLinea(int numeroLinea, string contenidoOriginal, TipoErrorLinea tipo, string detalle)
        {
            if (numeroLinea <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(numeroLinea), "El número de línea debe ser mayor a cero.");
            }

            _numeroLinea = numeroLinea;
            _contenidoOriginal = contenidoOriginal ?? string.Empty;
            _tipo = tipo;
            _detalle = detalle ?? string.Empty;
        }

        public int NumeroLinea
        {
            get { return _numeroLinea; }
        }

        public string ContenidoOriginal
        {
            get { return _contenidoOriginal; }
        }

        public TipoErrorLinea Tipo
        {
            get { return _tipo; }
        }

        /// <summary>Fragmento o valor que provocó el error (por ejemplo "ABC" o "-5.0").</summary>
        public string Detalle
        {
            get { return _detalle; }
        }
    }
}
