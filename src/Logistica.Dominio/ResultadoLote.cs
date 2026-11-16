using System;

namespace Logistica.Dominio
{
    /// <summary>
    /// Resultado completo del procesamiento de un lote: paquetes válidos
    /// (ordenados), arreglo paralelo de errores y totales del lote.
    /// </summary>
    public class ResultadoLote
    {
        private long _id;
        private readonly DateTime _fechaProceso;
        private Paquete[] _paquetes;
        private readonly ErrorLinea[] _errores;
        private readonly int _totalLineas;
        private readonly int _capacidadArreglo;
        private readonly int _redimensionamientos;

        public ResultadoLote(
            Paquete[] paquetes,
            ErrorLinea[] errores,
            int totalLineas,
            int capacidadArreglo,
            int redimensionamientos,
            DateTime fechaProceso)
        {
            if (paquetes == null)
            {
                throw new ArgumentNullException(nameof(paquetes));
            }
            if (errores == null)
            {
                throw new ArgumentNullException(nameof(errores));
            }

            _paquetes = paquetes;
            _errores = errores;
            _totalLineas = totalLineas;
            _capacidadArreglo = capacidadArreglo;
            _redimensionamientos = redimensionamientos;
            _fechaProceso = fechaProceso;
        }

        /// <summary>Id del lote en la base de datos (0 mientras no se guarda).</summary>
        public long Id
        {
            get { return _id; }
            set { _id = value; }
        }

        public DateTime FechaProceso
        {
            get { return _fechaProceso; }
        }

        public Paquete[] Paquetes
        {
            get { return _paquetes; }
        }

        public ErrorLinea[] Errores
        {
            get { return _errores; }
        }

        public int TotalLineas
        {
            get { return _totalLineas; }
        }

        /// <summary>Capacidad final del arreglo dinámico (evidencia del Reto 2).</summary>
        public int CapacidadArreglo
        {
            get { return _capacidadArreglo; }
        }

        /// <summary>Número de veces que se duplicó el arreglo base.</summary>
        public int Redimensionamientos
        {
            get { return _redimensionamientos; }
        }

        public int CantidadValidos
        {
            get { return _paquetes.Length; }
        }

        public int CantidadErrores
        {
            get { return _errores.Length; }
        }

        public decimal SumaBaseImponible
        {
            get { return Sumar(0); }
        }

        public decimal SumaIva
        {
            get { return Sumar(1); }
        }

        public decimal SumaTotal
        {
            get { return Sumar(2); }
        }

        /// <summary>Permite reemplazar el orden de los paquetes (por ejemplo, al invertirlo).</summary>
        public void ReemplazarPaquetes(Paquete[] paquetesReordenados)
        {
            if (paquetesReordenados == null || paquetesReordenados.Length != _paquetes.Length)
            {
                throw new ArgumentException("El arreglo reordenado debe tener los mismos elementos.", nameof(paquetesReordenados));
            }
            _paquetes = paquetesReordenados;
        }

        private decimal Sumar(int concepto)
        {
            decimal suma = 0m;
            for (int i = 0; i < _paquetes.Length; i++)
            {
                DesgloseCosto desglose = _paquetes[i].Desglose;
                if (desglose == null)
                {
                    continue;
                }

                switch (concepto)
                {
                    case 0:
                        suma += desglose.BaseImponible;
                        break;
                    case 1:
                        suma += desglose.Iva;
                        break;
                    default:
                        suma += desglose.Total;
                        break;
                }
            }
            return suma;
        }
    }
}
