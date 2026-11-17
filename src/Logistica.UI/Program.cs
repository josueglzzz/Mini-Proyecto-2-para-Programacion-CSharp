using System;
using System.Threading;
using System.Windows.Forms;

namespace Logistica.UI
{
    internal static class Program
    {
        /// <summary>
        /// Punto de entrada. Registra manejadores globales para que ningún
        /// error inesperado cierre la aplicación sin un diagnóstico claro.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += Dominio_UnhandledException;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new FormPrincipal());
        }

        private static void Application_ThreadException(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(
                "Ocurrió un error inesperado / An unexpected error occurred:\r\n\r\n" + e.Exception.Message,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }

        private static void Dominio_UnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Exception excepcion = e.ExceptionObject as Exception;
            string mensaje = excepcion != null ? excepcion.Message : "Error desconocido / Unknown error";
            MessageBox.Show(
                "Error crítico / Critical error:\r\n\r\n" + mensaje,
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
    }
}
