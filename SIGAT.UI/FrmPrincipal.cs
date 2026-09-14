using SIGAT.BE;
using SIGAT.BE.Idiomas;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmPrincipal : Form, IIdiomaObserver
    {
        private BitacoraBLL _bitacora = new BitacoraBLL();
        private IdiomaBLL _idiomaBLL = new IdiomaBLL();

        public FrmPrincipal()
        {
            InitializeComponent();

            foreach (Control controlActual in this.Controls)
            {
                if (controlActual is MdiClient)
                {
                    controlActual.BackColor = Color.FromArgb(245, 247, 250);
                    break;
                }
            }

            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            bool esAdministrador = usuarioLogueado != null && usuarioLogueado.Perfil.NombrePerfil == "Administrador";
            if (!esAdministrador)
            {
                itemUsuarios.Visible = false;
            }

            this.Tag = "titulo_ventana";
            itemSistema.Tag = "menu_sistema";
            itemUsuarios.Tag = "menu_usuarios";
            itemBitacora.Tag = "menu_bitacora";
            itemLogout.Tag = "menu_logout";
            itemIdioma.Tag = "menu_idioma";

            this.Load += FrmPrincipal_Load;
            this.FormClosed += FrmPrincipal_FormClosed;
        }

        private void FrmPrincipal_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);

            List<Idioma> idiomas = _idiomaBLL.ObtenerIdiomas();
            CargarMenuDeIdiomas(idiomas);
            AplicarIdiomaPorDefecto(idiomas);
        }

        private void FrmPrincipal_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void CargarMenuDeIdiomas(List<Idioma> idiomas)
        {
            itemIdioma.DropDownItems.Clear();

            foreach (Idioma idioma in idiomas)
            {
                ToolStripMenuItem opcion = new ToolStripMenuItem(idioma.Nombre);
                opcion.Tag = idioma.Id;
                opcion.Click += OpcionDeIdioma_Click;
                itemIdioma.DropDownItems.Add(opcion);
            }

            itemIdioma.DropDownItems.Add(new ToolStripSeparator());
            itemIdioma.DropDownItems.Add(itemGestionIdiomas);
        }

        // Al arrancar, se aplica Español automaticamente
        private void AplicarIdiomaPorDefecto(List<Idioma> idiomas)
        {
            if (IdiomaManager.ObtenerInstancia().IdiomaActual != null)
            {
                ActualizarIdioma();
                return;
            }

            foreach (Idioma idioma in idiomas)
            {
                if (idioma.Nombre == "Español")
                {
                    _idiomaBLL.CambiarIdioma(idioma.Id);
                    return;
                }
            }

            if (idiomas.Count > 0)
            {
                _idiomaBLL.CambiarIdioma(idiomas[0].Id);
            }
        }

        private void OpcionDeIdioma_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem opcionElegida = (ToolStripMenuItem)sender;
            int idIdioma = (int)opcionElegida.Tag;
            _idiomaBLL.CambiarIdioma(idIdioma);
        }

        private void ItemGestionIdiomas_Click(object sender, EventArgs e)
        {
            AbrirFormulario(new FrmGestionIdiomas());
        }

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

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);

            string txtUsuario = IdiomaManager.ObtenerInstancia().Traducir(this.Name, "titulo_usuario", "Usuario");
            string txtPerfil = IdiomaManager.ObtenerInstancia().Traducir(this.Name, "titulo_perfil", "Perfil");

            string nombreUsu = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;
            string nombrePer = SesionServicio.ObtenerInstancia().PerfilActual.NombrePerfil;

            this.Text = "SIGAT - " + txtUsuario + ": " + nombreUsu + " | " + txtPerfil + ": " + nombrePer;
        }

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