using System;
using System.Threading;
using System.Windows.Forms;
using SIGAT.BLL;

namespace SIGAT.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize(); // Tu configuración original intacta

            // 1. Interceptar excepciones de los hilos de la UI (Formularios)
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalExceptionHandler);

            // 2. Interceptar excepciones de hilos de fondo o tareas asíncronas
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(NonUIExceptionHandler);

            Application.Run(new FrmLogin());
        }

        // Método que atrapa errores de la interfaz
        static void GlobalExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            ManejarErrorCritico(e.Exception);
        }

        // Método que atrapa errores fuera de la interfaz
        static void NonUIExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                ManejarErrorCritico(ex);
            }
        }

        // Lógica central para registrar y mostrar el error sin que el sistema muera
        static void ManejarErrorCritico(Exception ex)
        {
            try
            {
                BitacoraBLL bitacoraBLL = new BitacoraBLL();
                var sesion = SIGAT.SERVICIOS.SesionServicio.ObtenerInstancia().UsuarioActual;
                string usuario = sesion != null ? sesion.NombreUsuario : "Sistema";

                bitacoraBLL.Registrar(usuario, "Error Crítico del Sistema", $"Detalle técnico: {ex.Message}");
            }
            catch
            {
                // Si la bitácora también falla, lo ignoramos para evitar un bucle infinito
            }

            MessageBox.Show("Se ha producido un error inesperado. El incidente fue registrado en la bitácora del sistema.\n\n" +
                            "Mensaje técnico: " + ex.Message,
                            "Excepción Controlada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}