using System;

namespace Logistica.Dominio
{
    /// <summary>
    /// Entidad principal: un paquete/factura de flete capturado en un lote.
    /// Aplica encapsulamiento estricto: campos privados y propiedades públicas
    /// que validan los datos antes de aceptarlos.
    /// </summary>
    public class Paquete
    {
        private long _id;
        private long _loteId;
        private int _numeroLinea;
        private string _codigo;
        private string _cliente;
        private ZonaEnvio _zona;
        private decimal _pesoKg;
        private decimal _valorDeclarado;
        private DesgloseCosto _desglose;
        private EstadoEnvio _estado;
        private DateTime? _fechaRecepcion;

        public Paquete(string codigo, string cliente, ZonaEnvio zona, decimal pesoKg, decimal valorDeclarado)
        {
            Codigo = codigo;
            Cliente = cliente;
            Zona = zona;
            PesoKg = pesoKg;
            ValorDeclarado = valorDeclarado;
            _estado = EstadoEnvio.EnTransito;
        }

        /// <summary>Identificador asignado por la base de datos (0 si aún no se guarda).</summary>
        public long Id
        {
            get { return _id; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "El Id no puede ser negativo.");
                }
                _id = value;
            }
        }

        public long LoteId
        {
            get { return _loteId; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "El Id de lote no puede ser negativo.");
                }
                _loteId = value;
            }
        }

        /// <summary>Línea del texto de entrada de la que provino el paquete.</summary>
        public int NumeroLinea
        {
            get { return _numeroLinea; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "El número de línea no puede ser negativo.");
                }
                _numeroLinea = value;
            }
        }

        public string Codigo
        {
            get { return _codigo; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El código del paquete es obligatorio.", nameof(value));
                }
                _codigo = value.Trim().ToUpperInvariant();
            }
        }

        public string Cliente
        {
            get { return _cliente; }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("El nombre del cliente es obligatorio.", nameof(value));
                }
                _cliente = value.Trim();
            }
        }

        public ZonaEnvio Zona
        {
            get { return _zona; }
            set { _zona = value; }
        }

        public decimal PesoKg
        {
            get { return _pesoKg; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "El peso debe ser mayor a cero.");
                }
                _pesoKg = value;
            }
        }

        public decimal ValorDeclarado
        {
            get { return _valorDeclarado; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), "El valor declarado no puede ser negativo.");
                }
                _valorDeclarado = value;
            }
        }

        /// <summary>Desglose financiero; es null hasta que la calculadora lo asigna.</summary>
        public DesgloseCosto Desglose
        {
            get { return _desglose; }
            set { _desglose = value; }
        }

        public EstadoEnvio Estado
        {
            get { return _estado; }
            set { _estado = value; }
        }

        public DateTime? FechaRecepcion
        {
            get { return _fechaRecepcion; }
            set { _fechaRecepcion = value; }
        }

        /// <summary>Total a pagar o 0 si todavía no se calcula el desglose.</summary>
        public decimal TotalAPagar
        {
            get { return _desglose == null ? 0m : _desglose.Total; }
        }

        /// <summary>Registra la recepción del paquete (cierra la transacción de envío).</summary>
        public void RegistrarRecepcion(DateTime fecha)
        {
            if (_estado == EstadoEnvio.Entregado)
            {
                throw new InvalidOperationException("El paquete " + _codigo + " ya fue entregado.");
            }
            _estado = EstadoEnvio.Entregado;
            _fechaRecepcion = fecha;
        }

        public override string ToString()
        {
            return _codigo + " - " + _cliente;
        }
    }
}
