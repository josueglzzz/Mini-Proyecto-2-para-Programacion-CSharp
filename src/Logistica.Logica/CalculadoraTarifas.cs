using System;
using Logistica.Dominio;

namespace Logistica.Logica
{
    /// <summary>
    /// RETO 3: cálculo de impuestos escalonados y tarifa regional.
    /// Servicio de dominio independiente de la interfaz gráfica: recibe las
    /// constantes leídas del JSON y devuelve el desglose exacto de cada paquete.
    /// </summary>
    public class CalculadoraTarifas
    {
        private const int Decimales = 2;

        private readonly ConfiguracionTarifas _configuracion;

        public CalculadoraTarifas(ConfiguracionTarifas configuracion)
        {
            if (configuracion == null)
            {
                throw new ArgumentNullException(nameof(configuracion));
            }
            _configuracion = configuracion;
        }

        public ConfiguracionTarifas Configuracion
        {
            get { return _configuracion; }
        }

        /// <summary>
        /// Calcula el desglose. Lanza OverflowException si los montos rebasan
        /// la capacidad de decimal (error financiero) y ArgumentException si el
        /// paquete trae datos imposibles.
        /// </summary>
        public DesgloseCosto Calcular(Paquete paquete)
        {
            if (paquete == null)
            {
                throw new ArgumentNullException(nameof(paquete));
            }
            if (paquete.PesoKg <= 0 || paquete.ValorDeclarado < 0)
            {
                throw new ArgumentException("El paquete " + paquete.Codigo + " tiene peso o valor inválido.");
            }

            decimal precioBase = _configuracion.PrecioBasePorKg;

            // 1) Subtotal por peso.
            decimal subtotalPeso = Redondear(paquete.PesoKg * precioBase);

            // 2) Recargo por zona (selección múltiple).
            decimal porcentajeZona = ObtenerPorcentajeZona(paquete.Zona);
            decimal recargoZona = Redondear(subtotalPeso * porcentajeZona);

            // 3) Penalización por sobrepeso: cada kg extra paga 25% del precio base.
            decimal kilosExcedentes = 0m;
            decimal penalizacion = 0m;
            if (paquete.PesoKg > _configuracion.PesoLimiteKg && _configuracion.PenalizacionSobrepeso > 0)
            {
                kilosExcedentes = paquete.PesoKg - _configuracion.PesoLimiteKg;
                penalizacion = Redondear(kilosExcedentes * precioBase * _configuracion.PenalizacionSobrepeso);
            }

            // 4) Seguro obligatorio si el valor declarado supera el umbral.
            decimal seguro = 0m;
            if (paquete.ValorDeclarado > _configuracion.UmbralSeguro && _configuracion.TasaSeguro > 0)
            {
                seguro = Redondear(paquete.ValorDeclarado * _configuracion.TasaSeguro);
            }

            // 5) Base imponible, IVA y total.
            decimal costoFijo = _configuracion.CostoFijoEnvio;
            decimal baseImponible = subtotalPeso + recargoZona + penalizacion + seguro + costoFijo;
            decimal iva = Redondear(baseImponible * _configuracion.TasaIva);
            decimal total = baseImponible + iva;

            return new DesgloseCosto(
                subtotalPeso,
                porcentajeZona,
                recargoZona,
                kilosExcedentes,
                penalizacion,
                seguro,
                costoFijo,
                baseImponible,
                iva,
                total);
        }

        public decimal ObtenerPorcentajeZona(ZonaEnvio zona)
        {
            switch (zona)
            {
                case ZonaEnvio.A:
                    return _configuracion.RecargoZonaA;
                case ZonaEnvio.B:
                    return _configuracion.RecargoZonaB;
                case ZonaEnvio.C:
                    return _configuracion.RecargoZonaC;
                default:
                    throw new ArgumentOutOfRangeException(nameof(zona), "Zona no soportada: " + zona);
            }
        }

        private static decimal Redondear(decimal valor)
        {
            return Math.Round(valor, Decimales, MidpointRounding.AwayFromZero);
        }
    }
}
