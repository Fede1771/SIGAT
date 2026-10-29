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
            if (EntornoDiseno.Activo) return;
            TemaVisual.CargarPreferencia(SesionServicio.ObtenerInstancia().UsuarioActual?.IdUsuario ?? 0);
            TemaVisual.Formulario(this);
            WindowState = FormWindowState.Maximized;

            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            itemUsuarios.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);
            itemBitacora.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Bitacora);
            itemGestionIdiomas.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);


            this.Load += FrmPrincipal_Load;
            itemRoles.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);
            itemIntegridad.Visible = usuarioLogueado != null && usuarioLogueado.TienePermiso(MatrizRoles.Administracion);
            ConfigurarMarcoVisual();
            this.FormClosed += FrmPrincipal_FormClosed;
            FormClosing += (_, e) =>
            {
                if (formularioActual is FormGestionRoles roles && !roles.ConfirmarSalida()) e.Cancel = true;
            };
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
            if (!CerrarFormularioActual()) return;
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

        // Cada sesión empieza con la preferencia persistida de su propia cuenta.
        private void AplicarIdiomaPorDefecto(List<Idioma> idiomas)
        {
            if (IdiomaManager.ObtenerInstancia().IdiomaActual != null)
            {
                ActualizarIdioma();
                return;
            }

            _idiomaBLL.RestablecerIdiomaUsuario();
        }

        private void OpcionDeIdioma_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem opcionElegida = (ToolStripMenuItem)sender;
            int idIdioma = ((Idioma)opcionElegida.Tag).Id;
            _idiomaBLL.CambiarIdioma(idIdioma);
        }

        private void ItemRoles_Click(object sender, EventArgs e)
        {
            if (PuedeAbrir(MatrizRoles.Administracion)) AbrirFormulario(new FormGestionRoles());
        }

        private void ItemIntegridad_Click(object sender, EventArgs e)
        {
            if (!PuedeAbrir(MatrizRoles.Administracion)) return;
            if (formularioActual is FormGestionRoles roles && !roles.ConfirmarSalida()) return;
            using var dialogo = new FrmIntegridad();
            dialogo.ShowDialog(this);
            if (new IntegridadBLL().Verificar().Correcta) _idiomaBLL.RestablecerIdiomaUsuario();
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

            if (!CerrarFormularioActual()) return;
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

        // Consultamos los permisos cargados en la sesión antes de abrir una sección. Es el control de acceso de la navegación.
        private bool PuedeAbrir(string permiso)
        {
            if (SesionServicio.ObtenerInstancia().UsuarioActual?.TienePermiso(permiso) == true) return true;
            MessageBox.Show("No tiene permiso para acceder a esta función.", "SIGAT");
            return false;
        }

        // Alojamos la pantalla dentro del panel central. Cerramos la anterior para no acumular formularios ni observadores.
        private void AbrirFormulario(Form formHijo)
        {
            if (!CerrarFormularioActual()) { formHijo.Dispose(); return; }
            panelInicio.Visible = false;
            formularioActual = formHijo;
            formHijo.TopLevel = false;
            formHijo.FormBorderStyle = FormBorderStyle.None;
            formHijo.WindowState = FormWindowState.Normal;
            formHijo.MinimumSize = Size.Empty;
            formHijo.Dock = DockStyle.Fill;
            // La pantalla recibe el tema estando oculta, antes de cargar datos o mostrar controles nativos.
            TemaVisual.Aplicar(formHijo);
            panelContenido.SuspendLayout();
            try
            {
                panelContenido.Controls.Add(formHijo);
                SeleccionarSeccion(formHijo.GetType().Name);
                formHijo.Show();
                formHijo.BringToFront();
            }
            finally { panelContenido.ResumeLayout(true); }
        }

        private Form formularioActual;
        private readonly Dictionary<string, BotonNavegacion> navegacion = new();
        private readonly List<(string Seccion, ToolStripMenuItem Menu, IconoVisual Icono)> accesos = new();

        // Permisos y eventos se aplican sobre los controles creados por el diseñador.
        private void ConfigurarMarcoVisual()
        {
            navegacion.Add("Inicio", navInicio);
            navInicio.Click += (_, _) => MostrarInicio();
            navSalir.Click += (_, _) => itemLogout.PerformClick();
            btnIdiomaVisual.Click += (_, _) => MostrarSelectorIdioma();
            btnTemaVisual.Click += CambiarTema_Click;
            foreach (var acceso in new[]
            {
                ("FrmGestionUsuarios", itemUsuarios, IconoVisual.Usuarios, navFrmGestionUsuarios, inicioFrmGestionUsuarios),
                ("FormGestionRoles", itemRoles, IconoVisual.Roles, navFormGestionRoles, inicioFormGestionRoles),
                ("FrmGestionIdiomas", itemGestionIdiomas, IconoVisual.Idiomas, navFrmGestionIdiomas, inicioFrmGestionIdiomas),
                ("FrmBitacora", itemBitacora, IconoVisual.Bitacora, navFrmBitacora, inicioFrmBitacora),
                ("FrmIntegridad", itemIntegridad, IconoVisual.Roles, navFrmIntegridad, inicioFrmIntegridad)
            })
            {
                bool permitido = acceso.Item2.Available;
                acceso.Item4.Visible = acceso.Item5.Visible = permitido;
                var fila = panelNavegacion.GetRow(acceso.Item4);
                panelNavegacion.RowStyles[fila].Height = permitido ? 60 : 0;
                if (!permitido)
                {
                    // El diseñador conserva el catálogo completo; la sesión solo muestra sus accesos.
                    acceso.Item4.Parent.Controls.Remove(acceso.Item4);
                    acceso.Item5.Parent.Controls.Remove(acceso.Item5);
                    acceso.Item4.Dispose();
                    acceso.Item5.Dispose();
                    continue;
                }
                accesos.Add((acceso.Item1, acceso.Item2, acceso.Item3));
                navegacion.Add(acceso.Item1, acceso.Item4);
                acceso.Item4.Click += (_, _) => acceso.Item2.PerformClick();
                acceso.Item5.Click += (_, _) => acceso.Item2.PerformClick();
            }
            MostrarInicio();
            ActualizarMarcoVisual();
        }

        private void CambiarTema_Click(object sender, EventArgs e)
        {
            try
            {
                int id = SesionServicio.ObtenerInstancia().UsuarioActual?.IdUsuario ?? 0;
                TemaVisual.GuardarPreferencia(id, !TemaVisual.EsOscuro);
                ActualizarBotonTema();
                TemaVisual.Aplicar(selectorIdioma);
            }
            catch (Exception ex) when (ex is IOException || ex is UnauthorizedAccessException || ex is InvalidOperationException)
            { MessageBox.Show(this, "No se pudo guardar el tema.\n" + ex.Message, "Apariencia", MessageBoxButtons.OK, MessageBoxIcon.Information); }
        }

        private void MostrarSelectorIdioma()
        {
            foreach (ToolStripItem item in selectorIdioma.Items.Cast<ToolStripItem>().ToArray()) item.Dispose();
            selectorIdioma.Items.Clear();
            foreach (ToolStripItem item in itemIdioma.DropDownItems)
            {
                if (!item.Available) continue;
                if (item is ToolStripSeparator)
                {
                    selectorIdioma.Items.Add(new ToolStripSeparator());
                }
                else if (item is ToolStripMenuItem opcion)
                {
                    var entrada = new ToolStripMenuItem(opcion.Text)
                    {
                        Checked = opcion.Checked, Enabled = opcion.Enabled,
                        Padding = new Padding(10, 6, 10, 6)
                    };
                    entrada.Click += (_, _) => opcion.PerformClick();
                    selectorIdioma.Items.Add(entrada);
                }
            }
            selectorIdioma.MinimumSize = new Size(Math.Max(210, btnIdiomaVisual.Width), 0);
            TemaVisual.Aplicar(selectorIdioma);
            // Un menú independiente evita que el MenuStrip oculto lo reposicione sobre el logo.
            selectorIdioma.Show(btnIdiomaVisual, new Point(btnIdiomaVisual.Width, btnIdiomaVisual.Height + 6), ToolStripDropDownDirection.BelowLeft);
        }

        private void MostrarInicio()
        {
            if (!CerrarFormularioActual()) return;
            panelInicio.Visible = true;
            panelInicio.BringToFront();
            TemaVisual.Aplicar(panelInicio);
            SeleccionarSeccion("Inicio");
        }

        private bool CerrarFormularioActual()
        {
            if (formularioActual == null) return true;
            if (formularioActual is FormGestionRoles roles && !roles.ConfirmarSalida()) return false;
            formularioActual.Close();
            formularioActual.Dispose();
            formularioActual = null;
            return true;
        }

        private void SeleccionarSeccion(string section)
        {
            foreach (var entry in navegacion)
            {
                entry.Value.Seleccionado = entry.Key == section;
                entry.Value.Invalidate();
            }
            if (navegacion.TryGetValue(section, out var active)) lblSeccion.Text = active.Text;
        }

        private void ActualizarMarcoVisual()
        {
            if (lblCuenta == null) return;
            ActualizarBotonTema();
            var user = SesionServicio.ObtenerInstancia().UsuarioActual;
            lblCuenta.Text = user?.NombreUsuario ?? "";
            lblRoles.Text = user == null || user.Roles.Count == 0
                ? IdiomaManager.ObtenerInstancia().Traducir(Name, "sin_roles", "Sin roles asignados")
                : string.Join(Environment.NewLine, user.Roles.Select(r => r.Nombre));
            string idioma = IdiomaManager.ObtenerInstancia().IdiomaActual?.NombreNativo;
            btnIdiomaVisual.Text = IdiomaManager.ObtenerInstancia().Traducir(Name, "menu_idioma", "Idioma")
                + (string.IsNullOrEmpty(idioma) ? "" : " · " + idioma);
            foreach (var entry in navegacion)
            {
                entry.Value.Text = IdiomaManager.ObtenerInstancia().Traducir(Name, entry.Value.Tag.ToString(), entry.Value.Text);
                if (entry.Value.Seleccionado) lblSeccion.Text = entry.Value.Text;
            }
        }

        // El botón muestra el modo al que vamos a cambiar, no el modo que ya está activo.
        private void ActualizarBotonTema()
        {
            if (btnTemaVisual == null) return;
            btnTemaVisual.Text = IdiomaManager.ObtenerInstancia().Traducir(Name,
                TemaVisual.EsOscuro ? "tema_claro" : "tema_oscuro", TemaVisual.EsOscuro ? "Modo claro" : "Modo oscuro");
            btnTemaVisual.AccessibleName = btnTemaVisual.Text;
        }
    }
}
