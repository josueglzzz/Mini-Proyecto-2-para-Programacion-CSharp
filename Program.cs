using System;
using System.Reflection;

namespace Logistica.Pruebas
{
    /// <summary>
    /// Ejecuta automáticamente todas las clases cuyo nombre empieza con
    /// "Pruebas" y que tengan un método estático Ejecutar(Verificador).
    /// Uso: dotnet run --project tests/Logistica.Pruebas
    /// </summary>
    public static class Program
    {
        public static int Main()
        {
            Verificador verificador = new Verificador();
            Type[] tipos = typeof(Program).Assembly.GetTypes();

            for (int i = 0; i < tipos.Length; i++)
            {
                Type tipo = tipos[i];
                if (!tipo.IsClass || !tipo.Name.StartsWith("Pruebas", StringComparison.Ordinal))
                {
                    continue;
                }

                MethodInfo metodo = tipo.GetMethod("Ejecutar", BindingFlags.Public | BindingFlags.Static);
                if (metodo == null)
                {
                    continue;
                }

                Console.WriteLine();
                Console.WriteLine("== " + tipo.Name + " ==");
                try
                {
                    metodo.Invoke(null, new object[] { verificador });
                }
                catch (TargetInvocationException ex)
                {
                    Exception causa = ex.InnerException ?? ex;
                    verificador.Verdadero(false, "Excepción no controlada en " + tipo.Name + ": " + causa.GetType().Name + " - " + causa.Message);
                }
            }

            Console.WriteLine();
            Console.WriteLine("Resultado: " + verificador.Aprobadas + " aprobadas, " + verificador.Fallidas + " fallidas.");
            return verificador.Fallidas == 0 ? 0 : 1;
        }
    }
}
