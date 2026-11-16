using System;

namespace Logistica.Dominio
{
    /// <summary>
    /// Parámetros financieros leídos del archivo appsettings.json.
    /// Cada propiedad valida su rango para evitar cálculos con datos incoherentes.
    /// </summary>
    public class ConfiguracionTarifas
    {
        private decimal _precioBasePorKg;
        private decimal _tasaIva;
        private decimal _costoFijoEnvio;
        private decimal _recargoZonaA = 0.05m;
        private decimal _recargoZonaB = 0.10m;
        private decimal _recargoZonaC = 0.18m;
        private decimal _pesoLimiteKg = 20m;
        private decimal _penalizacionSobrepeso = 0.25m;
        private decimal _umbralSeguro = 1000m;
        private decimal _tasaSeguro = 0.02m;

        public ConfiguracionTarifas(decimal precioBasePorKg, decimal tasaIva, decimal costoFijoEnvio)
        {
            PrecioBasePorKg = precioBasePorKg;
            TasaIva = tasaIva;
            CostoFijoEnvio = costoFijoEnvio;
        }

        public decimal PrecioBasePorKg
        {
            get { return _precioBasePorKg; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PrecioBasePorKg), "El precio base por kg debe ser mayor a cero.");
                }
                _precioBasePorKg = value;
            }
        }

        /// <summary>Tasa de IVA expresada como fracción (0.16 = 16%).</summary>
        public decimal TasaIva
        {
            get { return _tasaIva; }
            set
            {
                ValidarPorcentaje(value, nameof(TasaIva));
                _tasaIva = value;
            }
        }

        public decimal CostoFijoEnvio
        {
            get { return _costoFijoEnvio; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(CostoFijoEnvio), "El costo fijo de envío no puede ser negativo.");
                }
                _costoFijoEnvio = value;
            }
        }

        public decimal RecargoZonaA
        {
            get { return _recargoZonaA; }
            set
            {
                ValidarPorcentaje(value, nameof(RecargoZonaA));
                _recargoZonaA = value;
            }
        }

        public decimal RecargoZonaB
        {
            get { return _recargoZonaB; }
            set
            {
                ValidarPorcentaje(value, nameof(RecargoZonaB));
                _recargoZonaB = value;
            }
        }

        public decimal RecargoZonaC
        {
            get { return _recargoZonaC; }
            set
            {
                ValidarPorcentaje(value, nameof(RecargoZonaC));
                _recargoZonaC = value;
            }
        }

        /// <summary>Peso a partir del cual se cobra penalización por kilogramo extra.</summary>
        public decimal PesoLimiteKg
        {
            get { return _pesoLimiteKg; }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(PesoLimiteKg), "El peso límite debe ser mayor a cero.");
                }
                _pesoLimiteKg = value;
            }
        }

        /// <summary>Fracción del precio base que se cobra por cada kg excedente (0.25 = 25%).</summary>
        public decimal PenalizacionSobrepeso
        {
            get { return _penalizacionSobrepeso; }
            set
            {
                ValidarPorcentaje(value, nameof(PenalizacionSobrepeso));
                _penalizacionSobrepeso = value;
            }
        }

        /// <summary>Valor declarado a partir del cual se cobra seguro obligatorio.</summary>
        public decimal UmbralSeguro
        {
            get { return _umbralSeguro; }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(UmbralSeguro), "El umbral del seguro no puede ser negativo.");
                }
                _umbralSeguro = value;
            }
        }

        public decimal TasaSeguro
        {
            get { return _tasaSeguro; }
            set
            {
                ValidarPorcentaje(value, nameof(TasaSeguro));
                _tasaSeguro = value;
            }
        }

        private static void ValidarPorcentaje(decimal valor, string nombre)
        {
            if (valor < 0 || valor > 1)
            {
                throw new ArgumentOutOfRangeException(nombre, "El valor de " + nombre + " debe estar entre 0 y 1 (ejemplo: 0.16 = 16%).");
            }
        }
    }
}
