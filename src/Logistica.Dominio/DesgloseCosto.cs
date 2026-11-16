namespace Logistica.Dominio
{
    /// <summary>
    /// Desglose financiero exacto de un paquete (resultado del Reto 3).
    /// Es inmutable: sólo la calculadora de tarifas lo construye.
    /// </summary>
    public class DesgloseCosto
    {
        private readonly decimal _subtotalPeso;
        private readonly decimal _porcentajeRecargoZona;
        private readonly decimal _recargoZona;
        private readonly decimal _kilosExcedentes;
        private readonly decimal _penalizacionSobrepeso;
        private readonly decimal _seguro;
        private readonly decimal _costoFijoEnvio;
        private readonly decimal _baseImponible;
        private readonly decimal _iva;
        private readonly decimal _total;

        public DesgloseCosto(
            decimal subtotalPeso,
            decimal porcentajeRecargoZona,
            decimal recargoZona,
            decimal kilosExcedentes,
            decimal penalizacionSobrepeso,
            decimal seguro,
            decimal costoFijoEnvio,
            decimal baseImponible,
            decimal iva,
            decimal total)
        {
            _subtotalPeso = subtotalPeso;
            _porcentajeRecargoZona = porcentajeRecargoZona;
            _recargoZona = recargoZona;
            _kilosExcedentes = kilosExcedentes;
            _penalizacionSobrepeso = penalizacionSobrepeso;
            _seguro = seguro;
            _costoFijoEnvio = costoFijoEnvio;
            _baseImponible = baseImponible;
            _iva = iva;
            _total = total;
        }

        /// <summary>Peso (kg) multiplicado por el precio base por kilogramo.</summary>
        public decimal SubtotalPeso
        {
            get { return _subtotalPeso; }
        }

        /// <summary>Porcentaje aplicado según la zona (0.05, 0.10 o 0.18).</summary>
        public decimal PorcentajeRecargoZona
        {
            get { return _porcentajeRecargoZona; }
        }

        public decimal RecargoZona
        {
            get { return _recargoZona; }
        }

        /// <summary>Kilogramos que exceden el límite configurado (0 si no hay sobrepeso).</summary>
        public decimal KilosExcedentes
        {
            get { return _kilosExcedentes; }
        }

        public decimal PenalizacionSobrepeso
        {
            get { return _penalizacionSobrepeso; }
        }

        public decimal Seguro
        {
            get { return _seguro; }
        }

        public decimal CostoFijoEnvio
        {
            get { return _costoFijoEnvio; }
        }

        /// <summary>Suma de todos los conceptos antes de impuestos.</summary>
        public decimal BaseImponible
        {
            get { return _baseImponible; }
        }

        public decimal Iva
        {
            get { return _iva; }
        }

        public decimal Total
        {
            get { return _total; }
        }
    }
}
