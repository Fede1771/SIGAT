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

            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            itemUsuarios.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);
            itemBitacora.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Bitacora);
            itemGestionIdiomas.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);

            this.Tag = "titulo_ventana";
            itemSistema.Tag = "menu_sistema";
            itemUsuarios.Tag = "menu_usuarios";
            itemBitacora.Tag = "menu_bitacora";
            itemLogout.Tag = "menu_logout";
            itemIdioma.Tag = "menu_idioma";

            this.Load += FrmPrincipal_Load;
            ToolStripMenuItem itemRoles = new ToolStripMenuItem("Gestión de roles y permisos");
            itemRoles.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);
            itemRoles.Click += (sender, e) =>
            {
                if (PuedeAbrir(MatrizRoles.Administracion)) AbrirFormulario(new FormGestionRoles());
            };
            itemSistema.DropDownItems.Add(itemRoles);
            CrearMarcoVisual(itemRoles);
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
            CerrarFormularioActual();
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
            if (!PuedeAbrir(MatrizRoles.Administracion)) return;
            AbrirFormulario(new FrmGestionIdiomas());
        }

        private void ItemUsuarios_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir(MatrizRoles.Administracion)) return;
            AbrirFormulario(new FrmGestionUsuarios());
        }

        private void ItemBitacora_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir(MatrizRoles.Bitacora)) return;
            AbrirFormulario(new FrmBitacora());
        }

        private void ItemLogout_Click(object sender, EventArgs e)
        {
            string nombreUsuario = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;

            _bitacora.Registrar(nombreUsuario, "Logout", "Cierre de sesión seguro.");
            CerrarFormularioActual();
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


            string nombreUsu = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;


            string nombresRoles = "";
            foreach (Rol rol in SesionServicio.ObtenerInstancia().UsuarioActual.Roles)
            {
                if (nombresRoles != "") nombresRoles += ", ";
                nombresRoles += rol.Nombre;
            }
            this.Text = "SIGAT - " + txtUsuario + ": " + nombreUsu
                + " | Roles: " + (nombresRoles == "" ? "Sin roles asignados" : nombresRoles);
            ActualizarMarcoVisual();
        }

        private bool PuedeAbrir(string permiso)
        {
            if (SesionServicio.ObtenerInstancia().UsuarioActual?.TienePermiso(permiso) == true) return true;
            MessageBox.Show("No tiene permiso para acceder a esta función.", "SIGAT");
            return false;
        }

        private void AbrirFormulario(Form formHijo)
        {
            CerrarFormularioActual();
            foreach (Control control in panelContenido.Controls.Cast<Control>().ToArray()) control.Dispose();
            formularioActual = formHijo;
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.WindowState = FormWindowState.Normal;
            formHijo.MinimumSize = Size.Empty;
            formHijo.Dock = DockStyle.Fill;
            panelContenido.Controls.Add(formHijo);
            SeleccionarSeccion(formHijo.GetType().Name);
            formHijo.Show();
        }
    }
}
