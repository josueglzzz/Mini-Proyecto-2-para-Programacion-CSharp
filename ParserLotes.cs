using System;
using System.Globalization;
using System.Text.RegularExpressions;
using Logistica.Dominio;

namespace Logistica.Logica
{
    /// <summary>
    /// RETO 1: sanitización y parseo de texto con tolerancia a fallos.
    /// Recorre el lote línea por línea, limpia cada fragmento, valida tipos
    /// y guarda las líneas corruptas en un arreglo paralelo de errores sin
    /// detener el procesamiento del resto del lote.
    /// Formato esperado: CODIGO;CLIENTE;ZONA;PESO_KG;VALOR_DECLARADO
    /// </summary>
    public class ParserLotes
    {
        public const char Separador = ';';
        public const int NumeroCampos = 5;

        public const int CampoCodigo = 0;
        public const int CampoCliente = 1;
        public const int CampoZona = 2;
        public const int CampoPeso = 3;
        public const int CampoValor = 4;

        private const int LongitudMaximaCliente = 80;

        // Patrones de cadena: código tipo "PACK-001" / "COD-101" y nombre con letras.
        private static readonly Regex _patronCodigo = new Regex(@"^[A-Z]{2,6}-\d{1,6}$");
        private static readonly Regex _patronCliente = new Regex(@"^[\p{L}][\p{L} .'\-]*$");
        private static readonly Regex _patronMiles = new Regex(@"^\d{1,3}(,\d{3})+(\.\d+)?$");

        public ResultadoParseo Analizar(string textoLote)
        {
            if (textoLote == null || textoLote.Trim().Length == 0)
            {
                return new ResultadoParseo(new ArregloPaquetes(), new ErrorLinea[0], new string[0, NumeroCampos], 0);
            }

            // Normaliza saltos de línea de Windows/Mac y elimina saltos sobrantes al final.
            string textoNormalizado = textoLote.Replace("\r\n", "\n").Replace('\r', '\n').TrimEnd();
            string[] lineas = textoNormalizado.Split('\n');

            string[,] matrizCampos = new string[lineas.Length, NumeroCampos];
            ErrorLinea[] erroresTemporales = new ErrorLinea[lineas.Length];
            int totalErrores = 0;
            ArregloPaquetes paquetesValidos = new ArregloPaquetes();

            for (int i = 0; i < lineas.Length; i++)
            {
                int numeroLinea = i + 1;
                string lineaOriginal = lineas[i];
                ErrorLinea error = AnalizarLinea(lineaOriginal, numeroLinea, i, matrizCampos, paquetesValidos);

                if (error != null)
                {
                    erroresTemporales[totalErrores] = error;
                    totalErrores++;
                }
            }

            // Recorta el arreglo de errores a su tamaño real.
            ErrorLinea[] errores = new ErrorLinea[totalErrores];
            for (int i = 0; i < totalErrores; i++)
            {
                errores[i] = erroresTemporales[i];
            }

            return new ResultadoParseo(paquetesValidos, errores, matrizCampos, lineas.Length);
        }

        /// <summary>
        /// Valida una línea. Devuelve null si la línea es válida (y agrega el
        /// paquete al arreglo) o un ErrorLinea con el motivo del descarte.
        /// </summary>
        private ErrorLinea AnalizarLinea(string lineaOriginal, int numeroLinea, int fila, string[,] matrizCampos, ArregloPaquetes paquetesValidos)
        {
            string linea = lineaOriginal.Trim();

            if (linea.Length == 0)
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.LineaVacia, string.Empty);
            }

            // Tolera un separador sobrante al final: "A;B;C;D;E;"
            if (linea.EndsWith(Separador.ToString()))
            {
                linea = linea.Substring(0, linea.Length - 1);
            }

            string[] fragmentos = linea.Split(Separador);
            if (fragmentos.Length != NumeroCampos)
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.CamposIncompletos, fragmentos.Length.ToString(CultureInfo.InvariantCulture));
            }

            for (int c = 0; c < NumeroCampos; c++)
            {
                matrizCampos[fila, c] = LimpiarFragmento(fragmentos[c]);
            }

            string codigo = matrizCampos[fila, CampoCodigo].ToUpperInvariant();
            if (!_patronCodigo.IsMatch(codigo))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.CodigoInvalido, codigo);
            }
            if (paquetesValidos.ContieneCodigo(codigo))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.CodigoDuplicado, codigo);
            }

            string cliente = matrizCampos[fila, CampoCliente];
            if (cliente.Length == 0 || cliente.Length > LongitudMaximaCliente || !_patronCliente.IsMatch(cliente))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.ClienteInvalido, cliente);
            }

            ZonaEnvio zona;
            if (!IntentarObtenerZona(matrizCampos[fila, CampoZona], out zona))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.ZonaInvalida, matrizCampos[fila, CampoZona]);
            }

            decimal peso;
            if (!IntentarConvertirNumero(matrizCampos[fila, CampoPeso], out peso))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.PesoNoNumerico, matrizCampos[fila, CampoPeso]);
            }
            if (peso <= 0)
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.PesoNoPositivo, matrizCampos[fila, CampoPeso]);
            }

            decimal valor;
            if (!IntentarConvertirNumero(matrizCampos[fila, CampoValor], out valor))
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.ValorNoNumerico, matrizCampos[fila, CampoValor]);
            }
            if (valor < 0)
            {
                return new ErrorLinea(numeroLinea, lineaOriginal, TipoErrorLinea.ValorNegativo, matrizCampos[fila, CampoValor]);
            }

            Paquete paquete = new Paquete(codigo, cliente, zona, peso, valor);
            paquete.NumeroLinea = numeroLinea;
            paquetesValidos.Agregar(paquete);
            return null;
        }

        /// <summary>
        /// Limpieza de un fragmento: quita espacios extremos, tabuladores y
        /// espacios dobles internos.
        /// </summary>
        public static string LimpiarFragmento(string fragmento)
        {
            if (fragmento == null)
            {
                return string.Empty;
            }

            string limpio = fragmento.Replace('\t', ' ').Trim();
            while (limpio.Contains("  "))
            {
                limpio = limpio.Replace("  ", " ");
            }
            return limpio;
        }

        /// <summary>
        /// Acepta "Zona A", "zona a", "A", "Zone B"... (selección múltiple con switch).
        /// </summary>
        public static bool IntentarObtenerZona(string texto, out ZonaEnvio zona)
        {
            zona = ZonaEnvio.A;
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            string clave = texto.ToUpperInvariant().Replace("ZONA", string.Empty).Replace("ZONE", string.Empty).Trim();

            switch (clave)
            {
                case "A":
                    zona = ZonaEnvio.A;
                    return true;
                case "B":
                    zona = ZonaEnvio.B;
                    return true;
                case "C":
                    zona = ZonaEnvio.C;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Convierte texto a decimal con cultura invariante (punto decimal).
        /// Elimina "$" y separadores de miles válidos (1,500.00). Las
        /// excepciones de formato y desbordamiento se capturan aquí para que
        /// una línea defectuosa no detenga el lote.
        /// </summary>
        public static bool IntentarConvertirNumero(string texto, out decimal resultado)
        {
            resultado = 0m;
            if (string.IsNullOrWhiteSpace(texto))
            {
                return false;
            }

            string normalizado = texto.Replace("$", string.Empty).Replace(" ", string.Empty);
            if (_patronMiles.IsMatch(normalizado))
            {
                normalizado = normalizado.Replace(",", string.Empty);
            }

            try
            {
                resultado = decimal.Parse(
                    normalizado,
                    NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
            catch (OverflowException)
            {
                return false;
            }
        }
    }
}
