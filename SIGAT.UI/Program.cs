using SIGAT.BLL;
using SIGAT.SERVICIOS;
using SIGAT.BE.Integridad;

namespace SIGAT.UI
{
    internal static class Program
    {
        [STAThread]
        // Siempre presentamos el login. La recuperación requiere primero autenticar a un administrador de SIGAT.
        static void Main()
        {
            ApplicationConfiguration.Initialize();

            // 1. Interceptar excepciones de los hilos de la UI (Formularios)
            Application.ThreadException += new ThreadExceptionEventHandler(ManejarErrorDeInterfaz);

            // 2. Interceptar excepciones de hilos de fondo o tareas asíncronas
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(ManejarErrorDeFondo);

            // Arrancamos el sistema mostrando la pantalla de login
            try
            {
                Application.Run(new FrmLogin());
            }
            catch (Exception ex)
            {
                MessageBox.Show("No se pudo iniciar SIGAT.\n" + ex.Message,
                    "Inicio bloqueado", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // Un fallo al escribir el log no debe impedir elegir el respaldo de recuperación.
        internal static DialogResult MostrarRecuperacion(IWin32Window propietario, InformeIntegridad informe)
        {
            if (!IntegridadBLL.EsAdministradorAplicacion())
            {
                MostrarAvisoMantenimiento(propietario);
                return DialogResult.Cancel;
            }
            try { IntegridadBLL.RegistrarIncidente(informe); } catch { }
            using var recuperar = new FrmIntegridad(informe, bloquearInicio: true);
            return recuperar.ShowDialog(propietario);
        }

        internal static void MostrarAvisoMantenimiento(IWin32Window propietario)
            => MessageBox.Show(propietario, "En mantenimiento, estamos resolviendo el inconveniente",
                "Mantenimiento", MessageBoxButtons.OK, MessageBoxIcon.Information);

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
            if (excepcion is IntegridadException integridad)
            {
                if (MostrarRecuperacion(Form.ActiveForm, integridad.Informe) == DialogResult.OK) Application.Restart();
                Application.Exit();
                return;
            }
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
