using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BE.Idiomas;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using SIGAT.UI;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class PresentacionVisualTests
{
    [TestMethod]
    public void HerenciaAbreLaFamiliaDeOrigenYElSelectorConservaMarcasAlFiltrar()
    {
        EnInterfaz(() =>
        {
            var tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!.GetMethod("CambiarModo")!;
            tema.Invoke(null, new object[] { true });
            try
            {
                var datos = EdicionRolesTests.Base();
                var familia = new PermisoCompuesto { Id = 8, Nombre = "Accesos de consulta" };
                familia.Agregar(datos.Permisos[0]); familia.Agregar(datos.Permisos[1]);
                datos.Permisos.Add(familia);
                datos.Roles[1].AgregarPermiso(familia); datos.Roles[1].AgregarRol(datos.Roles[4]);
                using var form = new FormGestionRoles(); QuitarCarga(form);
                typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, new object[] { datos });
                Preparar(form, new Size(1240, 820));
                var tabs = (TabControl)form.Controls.Find("vistasRoles", true)[0]; tabs.SelectedIndex = 1;
                ((ListBox)form.Controls.Find("listaRoles", true)[0]).SelectedIndex = 1;
                var tree = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                tree.SelectedNode = tree.Nodes.Cast<TreeNode>().Single(n => n.Text.StartsWith(familia.Nombre)).Nodes[0];
                var quitar = form.Controls.Find("btnQuitarAsignacion", true)[0];
                Assert.IsFalse(quitar.Enabled, "Un permiso heredado no se quita como relación directa.");
                Assert.IsTrue(form.Controls.Find("rutaRol", true)[0].Text.Contains("También se obtiene por"));
                Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-oscuro-herencia");
                ((Button)form.Controls.Find("btnAbrirOrigenRol", true)[0]).PerformClick();
                Application.DoEvents();
                Assert.AreEqual(2, tabs.SelectedIndex);
                Assert.AreEqual(familia.Nombre, form.Controls.Find("tituloFamilias", true)[0].Text);
                var componentes = (TreeView)form.Controls.Find("tvCatalogoGeneral", true)[0];
                componentes.SelectedNode = componentes.Nodes[0];
                ((Button)form.Controls.Find("btnQuitarComponenteFamilia", true)[0]).PerformClick();
                Application.DoEvents();
                Assert.IsTrue(form.TieneCambios);
                Assert.HasCount(2, familia.ObtenerHijos(), "La fuente confirmada no cambia antes de guardar.");
                Assert.HasCount(1, componentes.Nodes);
                Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-oscuro-familias");
                form.DescartarCambios();
                Assert.HasCount(2, componentes.Nodes);
                using var selector = new DlgFamiliaPermisos();
                selector.CargarOpciones(new object[] { familia, datos.Roles[1] }, "Agregar componentes");
                Preparar(selector, new Size(720, 500));
                var lista = (CheckedListBox)typeof(DlgFamiliaPermisos).GetField("lista", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(selector)!;
                lista.SetItemChecked(0, true); lista.SelectedIndex = 0;
                var buscar = (TextBox)selector.Controls.Find("buscarComponentes", true)[0];
                buscar.Text = "Responsable";
                Assert.HasCount(1, selector.SeleccionadosObjetos);
                buscar.Clear();
                Assert.IsTrue(lista.GetItemChecked(0));
                Guardar(selector, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-selector");
                using var resumen = new DlgFamiliaPermisos(); resumen.CargarResumen("Revisar cambios", "Cambio A\r\nRol afectado B");
                Assert.IsTrue(((TextBox)resumen.Controls.Find("resumenCambios", true)[0]).ReadOnly);
            }
            finally { tema.Invoke(null, new object[] { false }); }
        });
    }

    [TestMethod]
    public void TresVistasPermitenEditarDescartarYRecorrerJerarquiasSinSql()
    {
        EnInterfaz(() =>
        {
            var datos = EdicionRolesTests.Base();
            var familia = new PermisoCompuesto { Id = 8, Nombre = "Familia compartida" };
            var hija = new PermisoCompuesto { Id = 9, Nombre = "Subfamilia" };
            familia.Agregar(hija);
            datos.Permisos.AddRange(new[] { familia, hija });
            for (int i = 0; i < 50; i++)
            {
                var p = new PermisoSimple { Id = 10 + i, Nombre = "Componente " + i };
                // La matriz permite crear familias, no permisos simples adicionales:
                // para la vista de volumen se reutilizan familias personalizadas.
                var f = new PermisoCompuesto { Id = p.Id, Nombre = p.Nombre };
                hija.Agregar(f); datos.Permisos.Add(f);
            }
            datos.Roles[4].AgregarPermiso(familia);
            datos.Roles[1].AgregarRol(datos.Roles[4]);
            using var form = new FormGestionRoles();
            QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, new object[] { datos });
            Preparar(form, new Size(1240, 820));
            var tabs = (TabControl)form.Controls.Find("vistasRoles", true)[0];
            Assert.HasCount(3, tabs.TabPages);
            foreach (var size in new[] { new Size(1440, 900), new Size(864, 650), new Size(480, 640), new Size(1240, 820) })
            {
                form.MinimumSize = Size.Empty;
                form.ClientSize = size;
                for (int i = 0; i < 3; i++)
                {
                    tabs.SelectedIndex = i;
                    Application.DoEvents();
                    string suf = new[] { "Usuarios", "Roles", "Familias" }[i];
                    var lista = (ListBox)form.Controls.Find("lista" + suf, true)[0];
                    int buscado = i == 0 ? 2 : i == 1 ? 2 : 8;
                    lista.SelectedIndex = Enumerable.Range(0, lista.Items.Count).Single(n => (int)lista.Items[n].GetType().GetProperty("Id")!.GetValue(lista.Items[n])! == buscado);
                    typeof(Control).GetMethod("OnKeyDown", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(lista, new object[] { new KeyEventArgs(Keys.Enter) });
                    Application.DoEvents();
                    var tree = (TreeView)form.Controls.Find(new[] { "tvUsuarioPermisos", "tvRolesJerarquia", "tvCatalogoGeneral" }[i], true)[0];
                    Assert.IsTrue(tree.Height >= 140, "El árbol necesita una altura legible en " + size);
                    IEnumerable<TreeNode> Todos(TreeNodeCollection ns)
                    { foreach (TreeNode n in ns) { yield return n; foreach (var h in Todos(n.Nodes)) yield return h; } }
                    var ultimo = Todos(tree.Nodes).First(n => n.Text.StartsWith("Componente 49"));
                    ultimo.EnsureVisible();
                    Application.DoEvents();
                    Assert.IsTrue(ultimo.Bounds.Top >= 0 && ultimo.Bounds.Bottom <= tree.ClientSize.Height);
                    foreach (string nombre in new[] { "btnGuardarCambios", "btnDescartarCambios", "btnRecargarDatos" })
                    {
                        var boton = form.Controls.Find(nombre, true)[0];
                        Assert.IsTrue(form.ClientRectangle.Contains(form.RectangleToClient(boton.RectangleToScreen(boton.ClientRectangle))), nombre + " debe permanecer visible.");
                    }
                    Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-" + i + "-" + size.Width);
                }
            }
            tabs.SelectedIndex = 0;
            var roles = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0];
            roles.SetItemChecked(1, true);
            Application.DoEvents();
            Assert.IsTrue(form.TieneCambios);
            Assert.IsTrue(form.Controls.Find("btnGuardarCambios", true)[0].Enabled);
            Assert.HasCount(1, datos.Usuarios[1].Roles, "La edición no modifica los objetos de la sesión.");
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-pendientes");
            form.DescartarCambios();
            Assert.IsFalse(form.TieneCambios);
            Assert.IsFalse(roles.GetItemChecked(1));
        });
    }

    [TestMethod]
    public void VistasCompactasConservanAccionesYListasAccesiblesAlRedimensionar()
    {
        EnInterfaz(() =>
        {
            var catalogo = MatrizRoles.CrearCatalogo();
            var admin = new Usuario { IdUsuario = 1, NombreUsuario = "admin", Activo = true };
            admin.AsignarRol(catalogo.Roles[0]);
            catalogo.Usuarios.Add(admin);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            using var usuarios = new FrmGestionUsuarios();
            using var idiomas = new FrmGestionIdiomas();
            using var bitacora = new FrmBitacora();
            using var integridad = new FrmIntegridad(new SIGAT.BE.Integridad.InformeIntegridad());
            foreach (var form in new Form[] { usuarios, idiomas, bitacora, integridad })
            {
                QuitarCarga(form);
                // Integridad consulta SQL en Shown: esta prueba comprueba únicamente distribución.
                var key = typeof(Form).GetField("s_shownEvent", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
                var events = (EventHandlerList)typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form)!;
                if (form == integridad) events.RemoveHandler(key, events[key]);
                form.MinimumSize = Size.Empty;
                Preparar(form, new Size(1600, 900));
                foreach (var size in new[] { new Size(864, 600), new Size(480, 600), new Size(1600, 900), new Size(864, 600) })
                {
                    form.ClientSize = size;
                    Application.DoEvents();
                    var visor = (ScrollableControl)form.Controls.Find("visorAdaptativo", true)[0];
                    foreach (var nombre in form == usuarios ? new[] { "dgvUsuarios", "btnGuardar" } :
                        form == idiomas ? new[] { "dgvTraducciones", "btnNuevoIdioma" } :
                        form == bitacora ? new[] { "dgv", "btnBuscar" } : new[] { "grilla", "btnRecuperar", "btnContinuar" })
                    {
                        var control = form.Controls.Find(nombre, true)[0];
                        Assert.IsTrue(control.Height >= (control is TreeView || control is DataGridView ? 140 : 24), nombre + " necesita una altura legible: " + control.Size + " en " + size + "; padre " + control.Parent.Size + "; raíz " + visor.Controls[0].Size);
                        visor.ScrollControlIntoView(control);
                        Application.DoEvents();
                        var rect = visor.RectangleToClient(control.RectangleToScreen(control.ClientRectangle));
                        // Desplazar el visor exterior como con su barra: ScrollControlIntoView
                        // no resuelve todos los niveles de contenedores de WinForms.
                        visor.AutoScrollPosition = new Point(0, Math.Max(0, rect.Top - visor.AutoScrollPosition.Y - 30));
                        Application.DoEvents();
                        rect = visor.RectangleToClient(control.RectangleToScreen(control.ClientRectangle));
                        Assert.IsTrue(rect.Left >= 0 && rect.Right <= visor.ClientSize.Width + 1, nombre + " se corta horizontalmente en " + size);
                        Assert.IsTrue(rect.Bottom > 0 && rect.Top < visor.ClientSize.Height, nombre + " debe ser alcanzable con scroll: " + rect + "; vista " + visor.ClientSize + "; raíz " + visor.Controls[0].Bounds + "; ventana " + size + "; editor " + control.Parent.Bounds + "; scroll " + visor.AutoScrollPosition + "; rango " + visor.VerticalScroll.Maximum + "/" + visor.VerticalScroll.LargeChange + "; mínimo " + visor.AutoScrollMinSize);
                    }
                    var preview = Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW");
                    Guardar(form, preview, form.Name + "-" + size.Width);
                }
            }
        });
    }

    [TestMethod]
    public void EncabezadoPrincipalConservaAccionesEnVentanaCompacta()
    {
        EnInterfaz(() =>
        {
            var admin = new Usuario { NombreUsuario = "admin" };
            admin.AsignarRol(MatrizRoles.CrearCatalogo().Roles[0]);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            using var form = new FrmPrincipal();
            QuitarCarga(form);
            Preparar(form, new Size(864, 600));
            foreach (var size in new[] { new Size(760, 520), new Size(1440, 900), new Size(864, 600) })
            {
                form.ClientSize = size;
                Application.DoEvents();
                foreach (string nombre in new[] { "btnTemaVisual", "btnIdiomaVisual" })
                {
                    var boton = form.Controls.Find(nombre, true)[0];
                    var rect = form.RectangleToClient(boton.RectangleToScreen(boton.ClientRectangle));
                    Assert.IsTrue(form.ClientRectangle.Contains(rect), nombre + " no debe quedar fuera del encabezado.");
                }
            }
        });
    }

    [TestMethod]
    public void RecuperacionNoPermiteContinuarConDatosDaniados()
    {
        EnInterfaz(() =>
        {
            var informe = new SIGAT.BE.Integridad.InformeIntegridad { Configurada = true };
            informe.Problemas.Add(new("Usuarios", "2", "El SHA-256 del registro no coincide."));
            informe.Problemas.Add(new("UsuarioRol", "Tabla", "El conjunto de registros no coincide con el DVV."));
            using var form = new FrmIntegridad(informe, bloquearInicio: true);
            var key = typeof(Form).GetField("s_shownEvent", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
            var events = (EventHandlerList)typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form)!;
            events.RemoveHandler(key, events[key]); // Datos locales, sin llamadas SQL.
            typeof(FrmIntegridad).GetField("administrador", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(form, true);
            Preparar(form, new Size(1040, 670));
            var tarea = (Task)typeof(FrmIntegridad).GetMethod("Operar", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(form, new object[] { (Func<string>)(() => ""), false })!;
            var limite = DateTime.UtcNow.AddSeconds(5);
            while (!tarea.IsCompleted && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            Assert.IsTrue(tarea.IsCompleted);
            tarea.GetAwaiter().GetResult();
            Assert.IsFalse(form.Controls.Find("btnContinuar", true)[0].Enabled);
            Assert.IsTrue(form.Controls.Find("btnRecuperar", true)[0].Enabled);
            Assert.IsTrue(form.Controls.Find("btnRecalcular", true)[0].Enabled);
            Assert.IsFalse(form.Controls.Find("btnBackupVerificado", true)[0].Enabled);
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "integridad");
        });
    }

    [TestMethod]
    public void SelectorDeIdiomaSeAbreDebajoDelBotonYConservaLasOpciones()
    {
        EnInterfaz(() =>
        {
            var admin = new Usuario { NombreUsuario = "admin" };
            admin.AsignarRol(MatrizRoles.CrearCatalogo().Roles[0]);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            using var principal = new FrmPrincipal();
            QuitarCarga(principal);
            Preparar(principal, new Size(1200, 800));
            var flags = BindingFlags.Instance | BindingFlags.NonPublic;
            var source = (ToolStripMenuItem)typeof(FrmPrincipal).GetField("itemIdioma", flags)!.GetValue(principal)!;
            source.DropDownItems.Clear();
            bool selected = false;
            var spanish = new ToolStripMenuItem("Español") { Checked = true };
            var english = new ToolStripMenuItem("English");
            english.Click += (_, _) => selected = true;
            source.DropDownItems.AddRange(new ToolStripItem[] { spanish, english });
            var popup = (ContextMenuStrip)typeof(FrmPrincipal).GetField("selectorIdioma", flags)!.GetValue(principal)!;
            popup.Opacity = 0;
            var button = (Button)principal.Controls.Find("btnIdiomaVisual", true)[0];
            button.PerformClick();
            Application.DoEvents();
            var buttonBounds = button.RectangleToScreen(button.ClientRectangle);
            Assert.IsTrue(popup.Visible);
            Assert.IsTrue(popup.Top >= buttonBounds.Bottom, "El selector debe abrirse debajo del botón de idioma.");
            Assert.IsTrue(Math.Abs(popup.Right - buttonBounds.Right) <= 4, "El selector debe alinearse con el borde derecho del botón.");
            Assert.IsTrue(((ToolStripMenuItem)popup.Items[0]).Checked);
            popup.Items[1].PerformClick();
            Assert.IsTrue(selected, "La opción debe ejecutar la acción de cambio de idioma original.");
            popup.Close();
        });
    }

    [TestMethod]
    public void NavegacionRespetaPermisosYCierraLaPantallaAnterior()
    {
        EnInterfaz(() =>
        {
            var user = new Usuario { NombreUsuario = "auditor" };
            user.AsignarRol(MatrizRoles.CrearCatalogo().Roles[4]);
            SesionServicio.ObtenerInstancia().IniciarSesion(user);
            using var principal = new FrmPrincipal();
            Assert.HasCount(1, principal.Controls.Find("navFrmBitacora", true));
            Assert.IsEmpty(principal.Controls.Find("navFrmGestionUsuarios", true));
            Assert.IsEmpty(principal.Controls.Find("navFormGestionRoles", true));
            Assert.IsEmpty(principal.Controls.Find("navFrmGestionIdiomas", true));
            var open = typeof(FrmPrincipal).GetMethod("AbrirFormulario", BindingFlags.Instance | BindingFlags.NonPublic)!;
            var first = new Form();
            var second = new Form();
            open.Invoke(principal, new object[] { first });
            open.Invoke(principal, new object[] { second });
            Assert.IsTrue(first.IsDisposed, "Cambiar de sección debe cerrar la pantalla anterior.");
            var host = principal.Controls.Find("panelContenido", true)[0];
            Assert.AreSame(second, host.Controls[0]);
            Assert.IsFalse(second.TopLevel);
            Assert.AreEqual(DockStyle.Fill, second.Dock);
        });
    }

    [TestMethod]
    public void PantallasSeAdaptanYPermitenRevisarSuAspectoSinSql()
    {
        EnInterfaz(() =>
        {
            var catalog = MatrizRoles.CrearCatalogo();
            var admin = new Usuario { IdUsuario = 1, NombreUsuario = "admin", Nombre = "Ana", Apellido = "Pérez", Activo = true };
            admin.AsignarRol(catalog.Roles[0]);
            catalog.Usuarios.Add(admin);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            string? directory = Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW");
            if (directory != null) Directory.CreateDirectory(directory);
            using var login = (FrmLogin)typeof(FrmLogin).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(bool) }, null)!.Invoke(new object[] { false });
            Preparar(login, login.ClientSize);
            Guardar(login, directory, "login");
            var loginButton = login.Controls.Find("btnLogin", true)[0];
            var username = login.Controls.Find("txtUsuario", true)[0];
            Assert.IsTrue(loginButton.Height <= 60, "Ingresar debe conservar una altura proporcionada.");
            Assert.AreEqual(username.Width, loginButton.Width, "Campos e ingreso deben tener el mismo ancho.");
            using var principal = new FrmPrincipal();
            QuitarCarga(principal);
            Preparar(principal, new Size(1440, 900));
            Guardar(principal, directory, "inicio");
            using var usuarios = new FrmGestionUsuarios();
            var gridUsers = (DataGridView)usuarios.Controls.Find("dgvUsuarios", true)[0];
            gridUsers.DataSource = new List<Usuario> { admin, new Usuario { NombreUsuario = "valen1", Nombre = "Valen", Apellido = "García", Activo = false } };
            using var idiomas = new FrmGestionIdiomas();
            var gridLanguages = (DataGridView)idiomas.Controls.Find("dgvTraducciones", true)[0];
            gridLanguages.DataSource = new List<Traduccion>
            {
                new() { FormNombre = "FrmPrincipal", ControlNombre = "menu_usuarios", TextoBase = "Gestión de usuarios", Texto = "User management", Estado = "Completa" },
                new() { FormNombre = "FrmPrincipal", ControlNombre = "menu_roles", TextoBase = "Roles y permisos", Texto = "", Estado = "Pendiente" }
            };
            foreach (string column in new[] { "IdIdioma", "IdControl" }) gridLanguages.Columns[column]!.Visible = false;
            using var bitacora = new FrmBitacora();
            var gridLog = (DataGridView)bitacora.Controls.Find("dgv", true)[0];
            gridLog.DataSource = new List<Bitacora> { new() { Fecha = DateTime.Now, Usuario = "admin", Actividad = "Login", InformacionAsociada = "Inicio de sesión correcto" } };
            foreach (var (form, name) in new[] { ((Form)usuarios, "usuarios"), (idiomas, "idiomas"), (bitacora, "bitacora") })
            {
                QuitarCarga(form);
                Preparar(form, new Size(1160, 750));
                Guardar(form, directory, name);
                ComprobarGrillas(form);
                form.MinimumSize = Size.Empty;
                form.ClientSize = new Size(864, 650);
                form.PerformLayout();
                Application.DoEvents();
                Guardar(form, directory, name + "-compacto");
                ComprobarGrillas(form);
                if (name == "bitacora")
                {
                    var search = form.Controls.Find("btnBuscar", true)[0];
                    var rect = new Rectangle(search.Parent!.PointToClient(search.PointToScreen(Point.Empty)), search.Size);
                    Assert.IsTrue(search.Parent.ClientRectangle.Contains(rect), "Buscar debe seguir visible cuando los filtros se acomodan en dos filas.");
                }
            }
            var open = typeof(FrmPrincipal).GetMethod("AbrirFormulario", BindingFlags.Instance | BindingFlags.NonPublic)!;
            foreach (var (form, name) in new[] { ((Form)usuarios, "usuarios"), (idiomas, "idiomas"), (bitacora, "bitacora") })
            {
                form.Hide();
                open.Invoke(principal, new object[] { form });
                principal.PerformLayout();
                Application.DoEvents();
                ComprobarGrillas(form);
                Guardar(principal, directory, "sigat-" + name);

            }
        });
    }

    private static void ComprobarGrillas(Control parent)
    {
        foreach (Control child in parent.Controls)
        {
            if (child is DataGridView grid)
            {
                var form = grid.FindForm()!;
                var visor = form.Controls.Find("visorAdaptativo", true).FirstOrDefault();
                Control contenido = visor?.Controls[0] ?? form;
                var rect = new Rectangle(contenido.PointToClient(grid.PointToScreen(Point.Empty)), grid.Size);
                Assert.IsTrue(contenido.ClientRectangle.Contains(rect), $"La grilla {grid.Name} debe quedar dentro del contenido desplazable: {rect} / {contenido.ClientRectangle}.");
                Assert.IsTrue(grid.Height >= 140, $"La grilla {grid.Name} debe conservar espacio para leer filas: {grid.Height} px.");
            }
            ComprobarGrillas(child);
        }
    }

    private static void QuitarCarga(Form form)
    {
        // Las vistas se renderizan con datos locales, sin ejecutar cargas ni traducciones SQL.
        var key = typeof(Form).GetField("s_loadEvent", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
        var events = (EventHandlerList)typeof(Component).GetProperty("Events", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(form)!;
        events.RemoveHandler(key, events[key]);
    }

    private static void Preparar(Form form, Size size)
    {
        form.WindowState = FormWindowState.Normal;
        form.ClientSize = size;
        form.ShowInTaskbar = false;
        form.Opacity = 0;
        form.Show();
        form.PerformLayout();
        Application.DoEvents();
    }

    private static void Guardar(Form form, string? directory, string name)
    {
        if (directory == null) return;
        Directory.CreateDirectory(directory);
        using var bitmap = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(bitmap, new Rectangle(Point.Empty, bitmap.Size));
        bitmap.Save(Path.Combine(directory, name + ".png"));
    }

    private static void EnInterfaz(Action action)
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            try { Application.EnableVisualStyles(); action(); }
            catch (Exception ex) { error = ex; }
            finally { SesionServicio.ObtenerInstancia().CerrarSesion(); }
        });
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.IsTrue(thread.Join(TimeSpan.FromSeconds(30)), "La prueba visual no debe quedar bloqueada.");
        if (error != null) throw error;
    }
}
