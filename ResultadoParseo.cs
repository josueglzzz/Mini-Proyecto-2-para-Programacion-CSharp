using Logistica.Dominio;

namespace Logistica.Logica
{
    /// <summary>
    /// Salida del parser: arreglo dinámico de paquetes válidos, arreglo
    /// paralelo de errores y la matriz bidimensional de campos limpios.
    /// </summary>
    public class ResultadoParseo
    {
        private readonly ArregloPaquetes _paquetesValidos;
        private readonly ErrorLinea[] _errores;
        private readonly string[,] _matrizCampos;
        private readonly int _totalLineas;

        public ResultadoParseo(ArregloPaquetes paquetesValidos, ErrorLinea[] errores, string[,] matrizCampos, int totalLineas)
        {
            _paquetesValidos = paquetesValidos;
            _errores = errores;
            _matrizCampos = matrizCampos;
            _totalLineas = totalLineas;
        }

        public ArregloPaquetes PaquetesValidos
        {
            get { return _paquetesValidos; }
        }

        public ErrorLinea[] Errores
        {
            get { return _errores; }
        }

        /// <summary>Matriz [línea, campo] con los fragmentos ya limpios (string[,]).</summary>
        public string[,] MatrizCampos
        {
            get { return _matrizCampos; }
        }

        public int TotalLineas
        {
            get { return _totalLineas; }
        }
    }
}
