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
            itemUsuarios.Visible = usuarioLogueado != null && usuarioLogueado.TienePatente("Gestión de Usuarios");
            itemBitacora.Visible = usuarioLogueado != null && usuarioLogueado.TienePatente("Bitácora");
            itemGestionIdiomas.Visible = usuarioLogueado != null && usuarioLogueado.TienePatente("Gestión de Idiomas");

            this.Tag = "titulo_ventana";
            itemSistema.Tag = "menu_sistema";
            itemUsuarios.Tag = "menu_usuarios";
            itemBitacora.Tag = "menu_bitacora";
            itemLogout.Tag = "menu_logout";
            itemIdioma.Tag = "menu_idioma";

            this.Load += FrmPrincipal_Load;
            ToolStripMenuItem itemRoles = new ToolStripMenuItem("Gestión de roles y permisos");
            itemRoles.Visible = usuarioLogueado != null && usuarioLogueado.TienePatente("Gestión de Roles");
            itemRoles.Click += (sender, e) =>
            {
                if (PuedeAbrir("Gestión de Roles")) AbrirFormulario(new FormGestionRoles());
            };
            itemSistema.DropDownItems.Add(itemRoles);
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
                opcion.Tag = idioma;
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
                if (string.Equals(idioma.Codigo, "es", StringComparison.OrdinalIgnoreCase))
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
            int idIdioma = ((Idioma)opcionElegida.Tag).Id;
            _idiomaBLL.CambiarIdioma(idIdioma);
        }

        private void ItemGestionIdiomas_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir("Gestión de Idiomas")) return;
            AbrirFormulario(new FrmGestionIdiomas());
        }

        private void ItemUsuarios_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir("Gestión de Usuarios")) return;
            AbrirFormulario(new FrmGestionUsuarios());
        }

        private void ItemBitacora_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir("Bitácora")) return;
            AbrirFormulario(new FrmBitacora());
        }

        private void ItemLogout_Click(object sender, EventArgs e)
        {
            string nombreUsuario = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;

            _bitacora.Registrar(nombreUsuario, "Logout", "Cierre de sesión seguro.");
            foreach (Form hijo in MdiChildren) hijo.Close();
            SesionServicio.ObtenerInstancia().CerrarSesion();

            this.Hide();
            FrmLogin frmLogin = new FrmLogin();
            frmLogin.ShowDialog();
            this.Close();
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);

            int? idActivo = IdiomaManager.ObtenerInstancia().IdiomaActual?.Id;
            foreach (ToolStripItem item in itemIdioma.DropDownItems)
            {
                if (item is ToolStripMenuItem opcion && opcion.Tag is Idioma idioma)
                {
                    opcion.Text = PresentacionIdioma.Etiqueta(idioma, idActivo);
                    opcion.Checked = idioma.Id == idActivo;
                    opcion.AccessibleName = opcion.Text;
                }
            }

            string txtUsuario = IdiomaManager.ObtenerInstancia().Traducir(this.Name, "titulo_usuario", "Usuario");
            string txtPerfil = IdiomaManager.ObtenerInstancia().Traducir(this.Name, "titulo_perfil", "Perfil");

            string nombreUsu = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;
            string nombrePer = SesionServicio.ObtenerInstancia().PerfilActual.NombrePerfil;

            string nombresRoles = "";
            foreach (Rol rol in SesionServicio.ObtenerInstancia().UsuarioActual.Roles)
            {
                if (nombresRoles != "") nombresRoles += ", ";
                nombresRoles += rol.Nombre;
            }
            this.Text = "SIGAT - " + txtUsuario + ": " + nombreUsu + " | " + txtPerfil + ": " + nombrePer
                + " | Roles: " + (nombresRoles == "" ? "Sin roles asignados" : nombresRoles);
        }

        private bool PuedeAbrir(string permiso)
        {
            if (SesionServicio.ObtenerInstancia().UsuarioActual?.TienePatente(permiso) == true) return true;
            MessageBox.Show("No tiene permiso para acceder a esta función.", "SIGAT");
            return false;
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
