using System;

namespace Logistica.Pruebas
{
    /// <summary>Mini marco de pruebas: registra aciertos y fallos.</summary>
    public class Verificador
    {
        private int _aprobadas;
        private int _fallidas;

        public int Aprobadas
        {
            get { return _aprobadas; }
        }

        public int Fallidas
        {
            get { return _fallidas; }
        }

        public void Verdadero(bool condicion, string descripcion)
        {
            if (condicion)
            {
                _aprobadas++;
                Console.WriteLine("  [OK]    " + descripcion);
            }
            else
            {
                _fallidas++;
                Console.WriteLine("  [FALLA] " + descripcion);
            }
        }

        public void Igual(decimal esperado, decimal obtenido, string descripcion)
        {
            Verdadero(esperado == obtenido, descripcion + " (esperado " + esperado + ", obtenido " + obtenido + ")");
        }

        public void Igual(int esperado, int obtenido, string descripcion)
        {
            Verdadero(esperado == obtenido, descripcion + " (esperado " + esperado + ", obtenido " + obtenido + ")");
        }

        public void Igual(string esperado, string obtenido, string descripcion)
        {
            Verdadero(esperado == obtenido, descripcion + " (esperado \"" + esperado + "\", obtenido \"" + obtenido + "\")");
        }

        public void LanzaExcepcion<TExcepcion>(Action accion, string descripcion) where TExcepcion : Exception
        {
            try
            {
                accion();
                Verdadero(false, descripcion + " (no se lanzó " + typeof(TExcepcion).Name + ")");
            }
            catch (TExcepcion)
            {
                Verdadero(true, descripcion);
            }
            catch (Exception ex)
            {
                Verdadero(false, descripcion + " (se lanzó " + ex.GetType().Name + ")");
            }
        }
    }
}
