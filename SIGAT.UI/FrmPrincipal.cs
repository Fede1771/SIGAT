using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using System.Resources;
using System.Reflection;

namespace SIGAT.UI
{
    public partial class FrmPrincipal : Form
    {
        private BitacoraBLL _bitacora = new BitacoraBLL();

        public FrmPrincipal()
        {
            InitializeComponent();

            // ESTÉTICA: Pintamos el fondo MDI de un gris muy suave y moderno
            foreach (Control controlActual in this.Controls)
            {
                if (controlActual is MdiClient)
                {
                    controlActual.BackColor = Color.FromArgb(245, 247, 250);
                    break;
                }
            }

            // Control de Permisos: Ocultamos Gestión de Usuarios si no es Administrador
            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            if (usuarioLogueado != null && usuarioLogueado.Perfil.NombrePerfil != "Administrador")
            {
                itemUsuarios.Visible = false;
            }

            // Nos suscribimos al evento de cambio de idioma
            IdiomaServicio.ObtenerInstancia().IdiomaCambiado += new Action(ActualizarTextos);
            ActualizarTextos();
        }

        // EVENTOS DEL MENÚ ----------------------------------------------------

        private void ItemUsuarios_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmGestionUsuarios());
        }

        private void ItemBitacora_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmBitacora());
        }

        private void ItemLogout_Click(object sender, EventArgs e)
        {
            string nombreUsuario = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;

            _bitacora.Registrar(nombreUsuario, "Logout", "Cierre de sesión seguro.");
            SesionServicio.ObtenerInstancia().CerrarSesion();

            this.Hide();
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            this.Close();
        }

        private void ItemEspañol_Click(object sender, EventArgs e)
        {
            IdiomaServicio.ObtenerInstancia().CambiarIdioma("es-AR");
        }

        private void ItemIngles_Click(object sender, EventArgs e)
        {
            IdiomaServicio.ObtenerInstancia().CambiarIdioma("en-US");
        }

        // ---------------------------------------------------------------------

        private void ActualizarTextos()
        {
            ResourceManager rm = new ResourceManager("SIGAT.UI.Idiomas", Assembly.GetExecutingAssembly());

            // Traducimos el título de la ventana
            string txtUsuario = rm.GetString("TituloUsuario") ?? "Usuario";
            string txtPerfil = rm.GetString("TituloPerfil") ?? "Perfil";

            string nombreUsu = SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario ?? "";
            string nombrePer = SesionServicio.ObtenerInstancia().PerfilActual?.NombrePerfil ?? "";

            this.Text = "SIGAT - " + txtUsuario + ": " + nombreUsu + " | " + txtPerfil + ": " + nombrePer;

            // Traducimos las opciones del menú
            itemSistema.Text = rm.GetString("MenuSistema") ?? "Sistema";
            itemUsuarios.Text = rm.GetString("MenuUsuarios") ?? "Gestión de Usuarios";
            itemBitacora.Text = rm.GetString("MenuBitacora") ?? "Bitácora";
            itemLogout.Text = rm.GetString("MenuLogout") ?? "Cerrar Sesión";
            itemIdioma.Text = rm.GetString("MenuIdioma") ?? "Idioma";
        }

        // Hace que cualquier ventana hija se abra maximizada y cierre las demás
        private void AbrirFormulario(Form formHijo)
        {
            foreach (Form formularioAbierto in this.MdiChildren)
            {
                formularioAbierto.Close();
            }

            formHijo.MdiParent = this;
            formHijo.WindowState = FormWindowState.Maximized;
            formHijo.Show();
        }
    }
}
