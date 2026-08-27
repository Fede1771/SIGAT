using System;
using System.Threading;
using System.Windows.Forms;
using SIGAT.BLL;
using SIGAT.SERVICIOS;

namespace SIGAT.UI
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Interceptar excepciones de los hilos de la UI (Formularios)
            Application.ThreadException += new ThreadExceptionEventHandler(ManejarErrorDeInterfaz);

            // 2. Interceptar excepciones de hilos de fondo o tareas asíncronas
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ManejarErrorDeFondo);

            // Arrancamos el sistema mostrando la pantalla de login
            Application.Run(new FrmLogin());
        }

        // Método que atrapa errores de la interfaz
        static void ManejarErrorDeInterfaz(object sender, ThreadExceptionEventArgs e)
        {
            ManejarErrorCritico(e.Exception);
        }

        // Método que atrapa errores fuera de la interfaz
        static void ManejarErrorDeFondo(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception excepcionFondo)
            {
                ManejarErrorCritico(excepcionFondo);
            }
        }

        // Lógica central para registrar y mostrar el error sin que el sistema muera
        static void ManejarErrorCritico(Exception excepcion)
        {
            try
            {
                BitacoraBLL bitacoraBLL = new BitacoraBLL();

                // Intentamos sacar el usuario logueado. Si es nulo, ponemos "Sistema"
                string nombreUsuario = "Sistema";
                if (SesionServicio.ObtenerInstancia().UsuarioActual != null)
                {
                    nombreUsuario = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;
                }

                bitacoraBLL.Registrar(nombreUsuario, "Error Crítico del Sistema", "Detalle técnico: " + excepcion.Message);
            }
            catch
            {
                // Si la bitácora falla justo al intentar guardar un error, lo ignoramos para evitar un bucle infinito
            }

            MessageBox.Show("Se ha producido un error inesperado. El incidente fue registrado en la bitácora del sistema.\n\n" +
                            "Mensaje técnico: " + excepcion.Message,
                            "Excepción Controlada",
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Error);
        }
    }
}