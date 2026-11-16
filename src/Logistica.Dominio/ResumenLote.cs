using System;

namespace Logistica.Dominio
{
    /// <summary>Fila del historial de lotes guardados en la base de datos.</summary>
    public class ResumenLote
    {
        private readonly long _id;
        private readonly DateTime _fecha;
        private readonly int _totalLineas;
        private readonly int _paquetesValidos;
        private readonly int _paquetesConError;
        private readonly decimal _total;
        private readonly int _paquetesEntregados;

        public ResumenLote(long id, DateTime fecha, int totalLineas, int paquetesValidos, int paquetesConError, decimal total, int paquetesEntregados)
        {
            _id = id;
            _fecha = fecha;
            _totalLineas = totalLineas;
            _paquetesValidos = paquetesValidos;
            _paquetesConError = paquetesConError;
            _total = total;
            _paquetesEntregados = paquetesEntregados;
        }

        public long Id
        {
            get { return _id; }
        }

        public DateTime Fecha
        {
            get { return _fecha; }
        }

        public int TotalLineas
        {
            get { return _totalLineas; }
        }

        public int PaquetesValidos
        {
            get { return _paquetesValidos; }
        }

        public int PaquetesConError
        {
            get { return _paquetesConError; }
        }

        public decimal Total
        {
            get { return _total; }
        }

        public int PaquetesEntregados
        {
            get { return _paquetesEntregados; }
        }
    }
}
