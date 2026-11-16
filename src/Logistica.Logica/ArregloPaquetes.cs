using System;
using Logistica.Dominio;

namespace Logistica.Logica
{
    /// <summary>
    /// RETO 2: arreglo nativo de paquetes con redimensionamiento manual,
    /// ordenamiento por burbuja (mayor a menor total) e inversión.
    /// No usa List&lt;T&gt; ni Array.Resize: todo se hace elemento por elemento.
    /// </summary>
    public class ArregloPaquetes
    {
        public const int CapacidadInicial = 5;

        private Paquete[] _elementos;
        private int _cantidad;
        private int _redimensionamientos;

        public ArregloPaquetes()
        {
            _elementos = new Paquete[CapacidadInicial];
            _cantidad = 0;
            _redimensionamientos = 0;
        }

        /// <summary>Número de paquetes realmente almacenados.</summary>
        public int Cantidad
        {
            get { return _cantidad; }
        }

        /// <summary>Tamaño físico actual del arreglo interno.</summary>
        public int Capacidad
        {
            get { return _elementos.Length; }
        }

        public int Redimensionamientos
        {
            get { return _redimensionamientos; }
        }

        public void Agregar(Paquete paquete)
        {
            if (paquete == null)
            {
                throw new ArgumentNullException(nameof(paquete));
            }

            if (_cantidad == _elementos.Length)
            {
                Redimensionar();
            }

            _elementos[_cantidad] = paquete;
            _cantidad++;
        }

        public Paquete ObtenerEn(int indice)
        {
            if (indice < 0 || indice >= _cantidad)
            {
                throw new IndexOutOfRangeException("Índice fuera del rango de paquetes almacenados: " + indice);
            }
            return _elementos[indice];
        }

        /// <summary>Búsqueda lineal (ciclo controlado por condición) de un código.</summary>
        public bool ContieneCodigo(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                return false;
            }

            string codigoNormalizado = codigo.Trim().ToUpperInvariant();
            int indice = 0;
            bool encontrado = false;

            while (indice < _cantidad && !encontrado)
            {
                encontrado = _elementos[indice].Codigo == codigoNormalizado;
                indice++;
            }

            return encontrado;
        }

        /// <summary>
        /// Crea un arreglo con el doble de capacidad y copia elemento por elemento.
        /// </summary>
        private void Redimensionar()
        {
            int nuevaCapacidad = _elementos.Length * 2;
            Paquete[] nuevoArreglo = new Paquete[nuevaCapacidad];

            for (int i = 0; i < _cantidad; i++)
            {
                nuevoArreglo[i] = _elementos[i];
            }

            _elementos = nuevoArreglo;
            _redimensionamientos++;
        }

        /// <summary>
        /// Ordenamiento burbuja optimizado: de mayor a menor costo total.
        /// Se detiene antes si en una pasada no hubo intercambios.
        /// </summary>
        public void OrdenarPorTotalDescendente()
        {
            bool huboIntercambio;
            int pasada = 0;

            do
            {
                huboIntercambio = false;
                for (int j = 0; j < _cantidad - 1 - pasada; j++)
                {
                    if (_elementos[j].TotalAPagar < _elementos[j + 1].TotalAPagar)
                    {
                        Paquete temporal = _elementos[j];
                        _elementos[j] = _elementos[j + 1];
                        _elementos[j + 1] = temporal;
                        huboIntercambio = true;
                    }
                }
                pasada++;
            }
            while (huboIntercambio && pasada < _cantidad);
        }

        /// <summary>Invierte el orden de los elementos usando dos índices que se cruzan.</summary>
        public void Invertir()
        {
            int izquierda = 0;
            int derecha = _cantidad - 1;

            while (izquierda < derecha)
            {
                Paquete temporal = _elementos[izquierda];
                _elementos[izquierda] = _elementos[derecha];
                _elementos[derecha] = temporal;
                izquierda++;
                derecha--;
            }
        }

        /// <summary>Devuelve una copia exacta (sin posiciones vacías) del contenido.</summary>
        public Paquete[] ConvertirAArreglo()
        {
            Paquete[] copia = new Paquete[_cantidad];
            for (int i = 0; i < _cantidad; i++)
            {
                copia[i] = _elementos[i];
            }
            return copia;
        }

        /// <summary>Construye un ArregloPaquetes a partir de un arreglo existente.</summary>
        public static ArregloPaquetes DesdeArreglo(Paquete[] paquetes)
        {
            if (paquetes == null)
            {
                throw new ArgumentNullException(nameof(paquetes));
            }

            ArregloPaquetes arreglo = new ArregloPaquetes();
            foreach (Paquete paquete in paquetes)
            {
                arreglo.Agregar(paquete);
            }
            return arreglo;
        }
    }
}
