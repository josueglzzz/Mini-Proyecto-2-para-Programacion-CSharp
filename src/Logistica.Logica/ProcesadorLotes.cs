using System;
using System.Globalization;
using Logistica.Dominio;

namespace Logistica.Logica
{
    /// <summary>
    /// Orquesta el procesamiento completo de un lote:
    /// parseo (Reto 1) → arreglo dinámico (Reto 2) → cálculo (Reto 3) →
    /// ordenamiento de mayor a menor total → totales del lote.
    /// </summary>
    public class ProcesadorLotes
    {
        private readonly ParserLotes _parser;
        private readonly CalculadoraTarifas _calculadora;

        public ProcesadorLotes(CalculadoraTarifas calculadora)
            : this(new ParserLotes(), calculadora)
        {
        }

        public ProcesadorLotes(ParserLotes parser, CalculadoraTarifas calculadora)
        {
            if (parser == null)
            {
                throw new ArgumentNullException(nameof(parser));
            }
            if (calculadora == null)
            {
                throw new ArgumentNullException(nameof(calculadora));
            }
            _parser = parser;
            _calculadora = calculadora;
        }

        public ResultadoLote Procesar(string textoLote)
        {
            ResultadoParseo parseo = _parser.Analizar(textoLote);
            ArregloPaquetes validos = parseo.PaquetesValidos;

            ArregloPaquetes calculados = new ArregloPaquetes();
            ErrorLinea[] errores = new ErrorLinea[parseo.Errores.Length + validos.Cantidad];
            int totalErrores = 0;

            for (int i = 0; i < parseo.Errores.Length; i++)
            {
                errores[totalErrores] = parseo.Errores[i];
                totalErrores++;
            }

            // Cálculo protegido: un error financiero en un paquete no detiene el lote.
            for (int i = 0; i < validos.Cantidad; i++)
            {
                Paquete paquete = validos.ObtenerEn(i);
                try
                {
                    paquete.Desglose = _calculadora.Calcular(paquete);
                    calculados.Agregar(paquete);
                }
                catch (OverflowException)
                {
                    errores[totalErrores] = CrearErrorFinanciero(paquete);
                    totalErrores++;
                }
                catch (ArgumentException)
                {
                    errores[totalErrores] = CrearErrorFinanciero(paquete);
                    totalErrores++;
                }
            }

            calculados.OrdenarPorTotalDescendente();

            ErrorLinea[] erroresFinales = new ErrorLinea[totalErrores];
            for (int i = 0; i < totalErrores; i++)
            {
                erroresFinales[i] = errores[i];
            }
            OrdenarErroresPorLinea(erroresFinales);

            return new ResultadoLote(
                calculados.ConvertirAArreglo(),
                erroresFinales,
                parseo.TotalLineas,
                calculados.Capacidad,
                calculados.Redimensionamientos,
                DateTime.Now);
        }

        /// <summary>Invierte el orden actual de los paquetes (mayor→menor ⇄ menor→mayor).</summary>
        public Paquete[] InvertirOrden(Paquete[] paquetes)
        {
            ArregloPaquetes arreglo = ArregloPaquetes.DesdeArreglo(paquetes);
            arreglo.Invertir();
            return arreglo.ConvertirAArreglo();
        }

        private static ErrorLinea CrearErrorFinanciero(Paquete paquete)
        {
            string detalle = paquete.Codigo + " (" + paquete.PesoKg.ToString(CultureInfo.InvariantCulture) + " kg)";
            return new ErrorLinea(paquete.NumeroLinea, paquete.Codigo, TipoErrorLinea.ErrorFinanciero, detalle);
        }

        /// <summary>Ordenamiento por selección de los errores según número de línea.</summary>
        private static void OrdenarErroresPorLinea(ErrorLinea[] errores)
        {
            for (int i = 0; i < errores.Length - 1; i++)
            {
                int indiceMenor = i;
                for (int j = i + 1; j < errores.Length; j++)
                {
                    if (errores[j].NumeroLinea < errores[indiceMenor].NumeroLinea)
                    {
                        indiceMenor = j;
                    }
                }

                if (indiceMenor != i)
                {
                    ErrorLinea temporal = errores[i];
                    errores[i] = errores[indiceMenor];
                    errores[indiceMenor] = temporal;
                }
            }
        }
    }
}
