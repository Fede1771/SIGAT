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
    public void LoginSoloDesplazaCuandoElContenidoNoCabe()
    {
        EnInterfaz(() =>
        {
            foreach (float escala in new[] { 1f, 1.25f, 1.5f })
            {
                using var login = (FrmLogin)typeof(FrmLogin).GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic,
                    null, new[] { typeof(bool) }, null)!.Invoke(new object[] { false });
                login.Scale(new SizeF(escala, escala));
                login.Font = new Font("Segoe UI", 10F * escala);
                Preparar(login, new Size((int)(460 * escala), (int)(610 * escala)));
                var visor = (Panel)login.Controls.Find("visorAdaptativo", true)[0];
                Assert.IsFalse(visor.VerticalScroll.Visible, "El login no necesita scroll a escala " + escala);
                Assert.IsFalse(visor.HorizontalScroll.Visible);
                var ingresar = login.Controls.Find("btnLogin", true)[0];
                Assert.IsTrue(visor.RectangleToScreen(visor.ClientRectangle).Contains(ingresar.RectangleToScreen(ingresar.ClientRectangle)),
                    $"Ingresar debe quedar accesible a escala {escala}: botón {ingresar.RectangleToScreen(ingresar.ClientRectangle)}, visor {visor.RectangleToScreen(visor.ClientRectangle)}, scroll {visor.AutoScrollPosition}.");
                Guardar(login, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "login-sin-scroll-" + escala);
                login.MinimumSize = Size.Empty;
                login.ClientSize = new Size((int)(460 * escala), (int)(300 * escala));
                Application.DoEvents();
                Assert.IsTrue(visor.VerticalScroll.Visible, "En ventanas muy bajas el contenido debe seguir siendo accesible.");
                login.Controls.Find("txtUsuario", true)[0].Focus();
                ingresar.Focus();
                Application.DoEvents();
                Assert.IsTrue(visor.RectangleToScreen(visor.ClientRectangle).Contains(ingresar.RectangleToScreen(ingresar.ClientRectangle)),
                    $"Ingresar debe quedar accesible a escala {escala}: botón {ingresar.RectangleToScreen(ingresar.ClientRectangle)}, visor {visor.RectangleToScreen(visor.ClientRectangle)}, scroll {visor.AutoScrollPosition}.");
                login.ClientSize = new Size((int)(460 * escala), (int)(610 * escala));
                Application.DoEvents();
                Assert.IsFalse(visor.VerticalScroll.Visible, "Al recuperar espacio debe desaparecer la barra.");
            }
        });
    }

    [TestMethod]
    public void RolProtegidoSeMuestraSinCasillaYConservaLasAsignacionesEditables()
    {
        EnInterfaz(() =>
        {
            var tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!.GetMethod("CambiarModo")!;
            try
            {
                foreach (bool oscuro in new[] { false, true })
                {
                    tema.Invoke(null, new object[] { oscuro });
                    var datos = EdicionRolesTests.Base();
                    using var form = new FormGestionRoles(); QuitarCarga(form);
                    typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(form, new object[] { datos });
                    Preparar(form, new Size(1240, 820));
                    var protegido = (Label)form.Controls.Find("lblRolProtegido", true)[0];
                    var roles = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0];
                    var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0];
                    int Id(object item) => (int)item.GetType().GetProperty("Id")!.GetValue(item)!;
                    Assert.IsTrue(protegido.Visible);
                    StringAssert.Contains(protegido.Text, "Administrador");
                    StringAssert.Contains(protegido.Text, "Protegido");
                    Assert.IsFalse(protegido.TabStop);
                    Assert.AreEqual(AccessibleRole.StaticText, protegido.AccessibleRole);
                    Assert.AreNotEqual(roles.BackColor, protegido.BackColor, "La asignación fija debe distinguirse con fondo gris.");
                    Assert.IsFalse(roles.Items.Cast<object>().Any(item => Id(item) == 1), "La cuenta original no debe mostrar una casilla para Administrador.");
                    Assert.IsFalse(form.TieneCambios);
                    roles.SetItemChecked(0, true); Application.DoEvents();
                    Assert.IsTrue(form.TieneCambios, "Los otros roles siguen siendo editables.");
                    form.DescartarCambios();
                    Assert.IsFalse(form.TieneCambios);
                    Assert.IsTrue(protegido.Visible);
                    Assert.IsTrue(datos.Usuarios[0].Roles.Any(r => r.Id == 1));
                    Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "admin-protegido-" + (oscuro ? "oscuro" : "claro"));
                    usuarios.SelectedIndex = 1; Application.DoEvents();
                    Assert.IsFalse(protegido.Visible, "La asignación fija corresponde solo al administrador original.");
                    Assert.IsTrue(roles.Items.Cast<object>().Any(item => Id(item) == 1), "En otra cuenta Administrador sigue disponible según la lógica actual.");
                }
            }
            finally { tema.Invoke(null, new object[] { false }); }
        });
    }

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
                Assert.IsFalse(form.Controls.Find("paginaRoles", true)[0] is TabPage);
                var tree = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                tree.SelectedNode = tree.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text.StartsWith(familia.Nombre)).Nodes[0];
                var quitar = form.Controls.Find("btnQuitarAsignacion", true)[0];
                Assert.IsFalse(quitar.Enabled, "Un permiso heredado no se quita como relación directa.");
                Assert.IsTrue(form.Controls.Find("rutaRol", true)[0].Text.Contains("También se obtiene por"));
                ((CheckBox)form.Controls.Find("mostrarDetalleRol", true)[0]).Checked = true;
                Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-v2-oscuro-herencia");
                OpcionRoles(form, "btnAbrirOrigenRol").PerformClick();
                Application.DoEvents();
                Assert.IsTrue(form.Controls.Find("paginaRoles", true)[0].Visible);
                Assert.IsTrue(form.Controls.Find("paginaFamilias", true)[0].Visible);
                Assert.AreEqual("Contenido de: " + familia.Nombre, form.Controls.Find("tituloFamilias", true)[0].Text);
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
    public void ArbolCompletoRespetaMatrizAnidamientoYDestinoDeLasAcciones()
    {
        EnInterfaz(() =>
        {
            var datos = EdicionRolesTests.Base();
            var familia = new PermisoCompuesto { Id = 8, Nombre = "Familia de prueba" };
            var hija = new PermisoCompuesto { Id = 9, Nombre = "Subfamilia de prueba" };
            hija.Agregar(datos.Permisos[0]); familia.Agregar(hija);
            datos.Permisos.AddRange(new[] { familia, hija });
            datos.Roles[1].AgregarPermiso(familia);
            datos.Roles[1].AgregarRol(datos.Roles[4]);
            using var form = new FormGestionRoles(); QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { datos });
            Preparar(form, new Size(1500, 1000));
            var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
            var quitar = (Button)form.Controls.Find("btnQuitarAsignacion", true)[0];
            var agregar = (Button)form.Controls.Find("btnAgregarPermisoRol", true)[0];
            Assert.IsEmpty(form.Controls.Find("listaRoles", true));
            CollectionAssert.AreEqual(datos.Roles.Select(r => r.Nombre).ToArray(), arbol.Nodes.Cast<TreeNode>().Select(n => n.Text).ToArray());
            // Las raíces identifican roles; no son relaciones que puedan quitarse.
            arbol.SelectedNode = arbol.Nodes[0];
            Assert.IsFalse(quitar.Enabled); Assert.IsFalse(agregar.Enabled);
            arbol.SelectedNode = arbol.Nodes[1];
            Assert.IsFalse(quitar.Enabled); Assert.IsTrue(agregar.Enabled);
            var heredado = arbol.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text.StartsWith(familia.Nombre)).Nodes[0].Nodes[0];
            arbol.SelectedNode = heredado;
            Assert.IsFalse(quitar.Enabled);
            Assert.IsTrue(OpcionRoles(form, "btnAbrirOrigenRol").Enabled);
            Assert.IsTrue(form.Controls.Find("rutaRol", true)[0].Text.Contains("Subfamilia de prueba"));
            // Seleccionar Auditor incluido conserva Responsable como destino, no edita Auditor.
            var incluido = arbol.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text.StartsWith("Auditor"));
            arbol.SelectedNode = incluido.Nodes[0]; Assert.IsFalse(quitar.Enabled);
            OpcionRoles(form, "btnAbrirOrigenRol").PerformClick();
            Assert.AreSame(arbol.Nodes[4], arbol.SelectedNode, "Abrir origen debe seleccionar el rol raíz correspondiente.");
            incluido = arbol.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text.StartsWith("Auditor"));
            arbol.SelectedNode = incluido; Assert.IsTrue(quitar.Enabled);
            quitar.PerformClick(); Application.DoEvents();
            Assert.IsTrue(form.TieneCambios);
            Assert.IsFalse(arbol.Nodes[1].Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Auditor")));
            Assert.HasCount(3, arbol.Nodes[4].Nodes, "Auditor mantiene sus propios permisos.");
            Assert.HasCount(1, datos.Roles[1].Roles, "El borrador no modifica la fuente confirmada.");

            // Revisar y cancelar Guardar conserva el borrador y no consulta SQL.
            string resumen = "";
            using var cancelar = new System.Windows.Forms.Timer { Interval = 25 };
            cancelar.Tick += (_, _) =>
            {
                var dialogo = Application.OpenForms.OfType<DlgFamiliaPermisos>().FirstOrDefault();
                if (dialogo == null) return;
                resumen = dialogo.Controls.Find("resumenCambios", true)[0].Text;
                cancelar.Stop(); dialogo.DialogResult = DialogResult.Cancel;
            };
            cancelar.Start();
            bool guardado = (bool)typeof(FormGestionRoles).GetMethod("GuardarCambios", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, null)!;
            Assert.IsFalse(guardado); Assert.IsTrue(form.TieneCambios);
            StringAssert.Contains(resumen, "Auditor");
            form.DescartarCambios();
            Assert.IsFalse(form.TieneCambios);
            Assert.IsTrue(arbol.Nodes[1].Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Auditor")));
            arbol.SelectedNode = arbol.Nodes[0].Nodes[0]; Assert.IsFalse(quitar.Enabled);
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "arbol-completo");

            // Buscar por una hoja mantiene la ruta completa y no altera el borrador.
            ((TextBox)form.Controls.Find("buscarRoles", true)[0]).Text = hija.Nombre;
            var espera = System.Diagnostics.Stopwatch.StartNew();
            while (arbol.Nodes.Count != 1 && espera.ElapsedMilliseconds < 3000) { Application.DoEvents(); Thread.Sleep(10); }
            Assert.HasCount(1, arbol.Nodes);
            Assert.AreEqual("Responsable de Inventario", arbol.Nodes[0].Text);
            Assert.IsTrue(arbol.Nodes[0].Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Movimientos")), "El filtro conserva la rama completa.");
            Assert.IsFalse(form.TieneCambios);
        });
    }

    [TestMethod]
    public void AgregarDesdeUnaFamiliaDelArbolAnidaEnLaSeleccionYDescartarRestauraLasRamas()
    {
        EnInterfaz(() =>
        {
            var datos = EdicionRolesTests.Base();
            datos.Roles[1].AgregarRol(datos.Roles[3]);
            using var form = new FormGestionRoles(); QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { datos });
            Preparar(form, new Size(1500, 1000));
            var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
            var agregar = (Button)form.Controls.Find("btnAgregarPermisoRol", true)[0];
            var incluir = OpcionRoles(form, "btnIncluirRol");
            TreeNode Hijo(TreeNode padre, string nombre) => padre.Nodes.Cast<TreeNode>().Single(n => n.Text == nombre);

            string[] Elegir(object boton, string nombre, bool aceptar = true)
            {
                string[] opciones = Array.Empty<string>();
                Exception? error = null;
                using var aceptarSelector = new System.Windows.Forms.Timer { Interval = 25 };
                aceptarSelector.Tick += (_, _) =>
                {
                    var selector = Application.OpenForms.OfType<DlgFamiliaPermisos>().FirstOrDefault();
                    if (selector == null) return;
                    aceptarSelector.Stop();
                    try
                    {
                        var lista = (CheckedListBox)selector.Controls.Find("lista", true)[0];
                        opciones = lista.Items.Cast<object>().Select(o => (string)o.GetType().GetProperty("Texto")!.GetValue(o)!).ToArray();
                        int indice = Array.FindIndex(opciones, s => s.EndsWith(" · " + nombre));
                        Assert.IsTrue(indice >= 0, "El selector debe ofrecer " + nombre);
                        lista.SetItemChecked(indice, true);
                    }
                    catch (Exception ex) { error = ex; }
                    finally { selector.DialogResult = aceptar && error == null ? DialogResult.OK : DialogResult.Cancel; }
                };
                Assert.IsTrue(boton is Button b ? b.Enabled : ((ToolStripMenuItem)boton).Enabled);
                aceptarSelector.Start();
                if (boton is Button accion) accion.PerformClick(); else ((ToolStripMenuItem)boton).PerformClick();
                Application.DoEvents();
                if (error != null) throw error;
                return opciones;
            }

            var gestion = Hijo(arbol.Nodes[1], "Gestión de activos");
            arbol.SelectedNode = gestion;
            Assert.IsFalse(incluir.Enabled, "Las familias agrupan permisos y familias; los roles se incluyen en roles.");
            StringAssert.Contains(form.Controls.Find("tituloRoles", true)[0].Text, "Gestión de activos");
            Elegir(agregar, "Movimientos");
            gestion = Hijo(arbol.Nodes[1], "Gestión de activos");
            var movimientos = Hijo(gestion, "Movimientos");
            Assert.AreSame(gestion, movimientos.Parent);
            Assert.IsTrue(gestion.IsExpanded);
            Assert.IsTrue(form.TieneCambios);
            Assert.IsEmpty(datos.Permisos.Single(p => p.Id == 3).ObtenerHijos(), "La fuente confirmada sigue intacta.");

            arbol.SelectedNode = movimientos;
            var ofrecidos = Elegir(agregar, "Reportes");
            Assert.IsFalse(ofrecidos.Any(s => s.EndsWith(" · Gestión de activos") || s.EndsWith(" · Movimientos")), "El selector excluye ancestros y el propio destino.");
            gestion = Hijo(arbol.Nodes[1], "Gestión de activos");
            var reportes = Hijo(Hijo(gestion, "Movimientos"), "Reportes");
            Assert.AreSame(gestion, reportes.Parent!.Parent);
            Assert.HasCount(1, Hijo(arbol.Nodes[3], "Movimientos").Nodes, "Una familia compartida muestra los mismos hijos en todas sus ramas.");

            arbol.SelectedNode = gestion;
            Elegir(agregar, "Ver bitácora", aceptar: false);
            Assert.IsFalse(gestion.Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Ver bitácora")), "Cancelar el selector no cambia la familia.");
            // Un rol incluido también recibe sus hijos en su propia rama, sin agregarlos a la raíz.
            arbol.SelectedNode = Hijo(arbol.Nodes[1], "Mesa de Ayuda");
            ofrecidos = Elegir(incluir, "Técnico de Mantenimiento");
            Assert.IsFalse(ofrecidos.Any(s => s.EndsWith(" · Responsable de Inventario") || s.EndsWith(" · Mesa de Ayuda")));
            var mesa = Hijo(arbol.Nodes[1], "Mesa de Ayuda");
            Assert.AreSame(mesa, Hijo(mesa, "Técnico de Mantenimiento").Parent);
            Assert.IsFalse(arbol.Nodes[1].Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Técnico")), "El nuevo rol se incluye bajo Mesa de Ayuda, no en Responsable.");

            arbol.SelectedNode = Hijo(arbol.Nodes[4], "Ver bitácora");
            Assert.IsFalse(agregar.Enabled); Assert.IsFalse(incluir.Enabled);
            arbol.SelectedNode = Hijo(arbol.Nodes[0], "Gestión de activos");
            Assert.IsFalse(agregar.Enabled, "La rama Administrador conserva su protección de edición.");
            arbol.SelectedNode = Hijo(arbol.Nodes[1], "Gestión de activos");
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "familia-anidada-desde-arbol");
            form.DescartarCambios();
            Assert.IsFalse(form.TieneCambios);
            Assert.IsEmpty(Hijo(arbol.Nodes[1], "Gestión de activos").Nodes);
            Assert.IsFalse(Hijo(arbol.Nodes[1], "Mesa de Ayuda").Nodes.Cast<TreeNode>().Any(n => n.Text.StartsWith("Técnico")));
            Assert.IsEmpty(datos.Roles[3].Roles);
        });
    }

    [TestMethod]
    public void VistaMinimalMantieneLosCincoRolesYLosDetallesOpcionalesSinAlterarElBorrador()
    {
        EnInterfaz(() =>
        {
            var tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!.GetMethod("CambiarModo")!;
            try
            {
                foreach (bool oscuro in new[] { false, true })
                {
                    tema.Invoke(null, new object[] { oscuro });
                    using var form = new FormGestionRoles(); QuitarCarga(form);
                    typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .Invoke(form, new object[] { EdicionRolesTests.Base() });
                    Preparar(form, new Size(1440, 900));
                    var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                    Assert.HasCount(5, arbol.Nodes);
                    Assert.IsTrue(arbol.Nodes.Cast<TreeNode>().All(n => !n.IsExpanded), "El árbol comienza con sus cinco raíces contraídas.");
                    Assert.AreEqual("Gestión de activos", arbol.Nodes[0].Nodes[1].Text);
                    StringAssert.Contains(arbol.Nodes[0].Nodes[1].ToolTipText, "Familia");
                    Assert.HasCount(3, form.Controls.Find("accionesRol", true)[0].Controls, "Solo Agregar, Quitar y Más opciones permanecen en la barra.");
                    Assert.HasCount(3, form.Controls.Find("accionesFamilia", true)[0].Controls);
                    Assert.IsFalse(form.Controls.Find("tvUsuarioPermisos", true)[0].Visible);
                    Assert.IsFalse(form.Controls.Find("rutaRol", true)[0].Visible);
                    Assert.IsFalse(form.Controls.Find("txtImpacto", true)[0].Visible);
                    Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "minimal-inicial-" + (oscuro ? "oscuro" : "claro"));
                    var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0];
                    usuarios.SelectedIndex = 1;
                    ((CheckedListBox)form.Controls.Find("rolesUsuario", true)[0]).SetItemChecked(1, true);
                    Application.DoEvents();
                    Assert.IsTrue(form.TieneCambios);
                    arbol.SelectedNode = arbol.Nodes[1]; arbol.Nodes[1].Expand();
                    var seleccionado = arbol.SelectedNode;
                    foreach (string nombre in new[] { "mostrarPermisosUsuario", "mostrarDetalleRol", "mostrarDetalleFamilia" })
                        ((CheckBox)form.Controls.Find(nombre, true)[0]).Checked = true;
                    Application.DoEvents();
                    Assert.IsTrue(form.Controls.Find("tvUsuarioPermisos", true)[0].Visible);
                    Assert.IsTrue(form.Controls.Find("rutaRol", true)[0].Visible);
                    Assert.IsTrue(form.Controls.Find("txtImpacto", true)[0].Visible);
                    Assert.AreSame(seleccionado, arbol.SelectedNode);
                    Assert.IsTrue(form.TieneCambios, "Desplegar información conserva la edición pendiente.");
                    ((Button)form.Controls.Find("btnMasOpcionesRol", true)[0]).PerformClick(); Application.DoEvents();
                    var incluir = OpcionRoles(form, "btnIncluirRol");
                    var menu = (ContextMenuStrip)incluir.Owner!;
                    Assert.IsTrue(menu.Visible); Assert.IsTrue(incluir.Enabled);
                    Assert.AreSame(menu, OpcionRoles(form, "btnAbrirOrigenRol").Owner);
                    menu.Close();
                    foreach (string nombre in new[] { "mostrarPermisosUsuario", "mostrarDetalleRol", "mostrarDetalleFamilia" })
                        ((CheckBox)form.Controls.Find(nombre, true)[0]).Checked = false;
                    Application.DoEvents();
                    Assert.IsFalse(form.Controls.Find("rutaRol", true)[0].Visible);
                    Assert.IsFalse(form.Controls.Find("txtImpacto", true)[0].Visible);
                    Assert.IsTrue(form.TieneCambios);
                    form.DescartarCambios();
                    Assert.IsFalse(form.TieneCambios);
                    Assert.IsTrue(arbol.Nodes[1].IsExpanded, "Descartar mantiene las ramas que el usuario estaba recorriendo.");
                    arbol.SelectedNode = arbol.Nodes[0];
                    Assert.IsFalse(OpcionRoles(form, "btnIncluirRol").Enabled);
                    Assert.IsFalse(form.Controls.Find("btnAgregarPermisoRol", true)[0].Enabled);
                    Assert.IsFalse(OpcionRoles(form, "btnEliminarFamilia").Enabled, "El elemento base sigue protegido.");
                }
            }
            finally { tema.Invoke(null, new object[] { false }); }
        });
    }

    [TestMethod]
    public void MasOpcionesPermiteCrearSubfamiliasYDescartarSinCambiarLaFuenteConfirmada()
    {
        EnInterfaz(() =>
        {
            var datos = EdicionRolesTests.Base();
            using var form = new FormGestionRoles(); QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { datos });
            Preparar(form, new Size(1440, 900));
            var lista = (ListBox)form.Controls.Find("listaFamilias", true)[0];
            lista.SelectedIndex = Enumerable.Range(0, lista.Items.Count).Single(i =>
                (int)lista.Items[i].GetType().GetProperty("Id")!.GetValue(lista.Items[i])! == 3);
            ((Button)form.Controls.Find("btnMasOpcionesFamilia", true)[0]).PerformClick(); Application.DoEvents();
            var crear = OpcionRoles(form, "btnCrearFamiliaAnidada");
            var menu = (ContextMenuStrip)crear.Owner!;
            Assert.IsTrue(menu.Visible); Assert.IsTrue(crear.Enabled);
            Assert.IsFalse(OpcionRoles(form, "btnEliminarFamilia").Enabled);
            using var aceptar = new System.Windows.Forms.Timer { Interval = 25 };
            aceptar.Tick += (_, _) =>
            {
                var dialogo = Application.OpenForms.OfType<DlgNombreRol>().FirstOrDefault();
                if (dialogo == null) return;
                aceptar.Stop(); dialogo.Controls.Find("txtNombre", true)[0].Text = "Subfamilia desde el menú";
                dialogo.DialogResult = DialogResult.OK;
            };
            aceptar.Start(); crear.PerformClick(); menu.Close(); Application.DoEvents();
            Assert.IsTrue(form.TieneCambios);
            Assert.HasCount(7, datos.Permisos);
            Assert.IsEmpty(datos.Permisos.Single(p => p.Id == 3).ObtenerHijos());
            Assert.IsFalse(OpcionRoles(form, "btnEliminarFamilia").Enabled, "La subfamilia recién creada ya está en uso.");
            var tree = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
            var gestion = tree.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text == "Gestión de activos");
            Assert.AreEqual("Subfamilia desde el menú", gestion.Nodes[0].Text);
            form.DescartarCambios();
            Assert.IsFalse(form.TieneCambios);
            Assert.HasCount(7, lista.Items);
            Assert.IsEmpty(tree.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text == "Gestión de activos").Nodes);
        });
    }

    [TestMethod]
    public void TresColumnasPermitenEditarDescartarYRecorrerJerarquiasSinSql()
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
            Assert.IsEmpty(form.Controls.Find("vistasRoles", true));
            Assert.HasCount(3, form.Controls.Find("columnasRoles", true)[0].Controls);
            foreach (var size in new[] { new Size(1440, 900), new Size(864, 650), new Size(480, 640), new Size(1240, 820) })
            {
                form.MinimumSize = Size.Empty;
                form.ClientSize = size;
                for (int i = 0; i < 3; i++)
                {
                    Application.DoEvents();
                    string suf = new[] { "Usuarios", "Roles", "Familias" }[i];
                    if (i == 0) ((CheckBox)form.Controls.Find("mostrarPermisosUsuario", true)[0]).Checked = true;
                    if (i == 1)
                    {
                        var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                        arbol.SelectedNode = arbol.Nodes[1];
                    }
                    else
                    {
                        var lista = (ListBox)form.Controls.Find("lista" + suf, true)[0];
                        int buscado = i == 0 ? 2 : 8;
                        lista.SelectedIndex = Enumerable.Range(0, lista.Items.Count).Single(n => (int)lista.Items[n].GetType().GetProperty("Id")!.GetValue(lista.Items[n])! == buscado);
                    }
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
    public void ColumnasSimultaneasConEscaladoYDesplazamientoIndependiente()
    {
        EnInterfaz(() =>
        {
            foreach (float escala in new[] { 1f, 1.25f, 1.5f })
            {
                using var form = new FormGestionRoles(); QuitarCarga(form);
                typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .Invoke(form, new object[] { EdicionRolesTests.Base() });
                // Escalar controles y tipografía simula las medidas del escritorio con escalado de Windows.
                form.Scale(new SizeF(escala, escala));
                form.Font = new Font("Segoe UI", 10F * escala);
                Preparar(form, new Size(escala == 1f ? 1120 : escala == 1.25f ? 1440 : 1680, (int)(780 * escala)));
                Application.DoEvents();
                var visor = (Panel)form.Controls.Find("visorColumnas", true)[0];
                var columnas = form.Controls.Find("columnasRoles", true)[0];
                Assert.HasCount(3, columnas.Controls);
                foreach (var c in columnas.Controls.Cast<Control>())
                {
                    Assert.IsTrue(c.Visible);
                    Assert.IsTrue(c.Width >= 270 * escala, c.Name + " debe conservar un ancho legible.");
                    Assert.IsTrue(visor.RectangleToScreen(visor.ClientRectangle).Contains(c.RectangleToScreen(c.ClientRectangle)),
                        "Las tres columnas deben verse completas en escritorio: " + c.Name);
                }
                foreach (string n in new[] { "contenedorDetalleUsuarios", "contenedorDetalleRoles", "contenedorDetalleFamilias" })
                    Assert.IsTrue(((Panel)form.Controls.Find(n, true)[0]).AutoScroll);
                Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "columnas-escala-" + escala);
                form.MinimumSize = Size.Empty; form.ClientSize = new Size(480, 620); Application.DoEvents();
                Assert.IsTrue(visor.HorizontalScroll.Visible, "Una ventana excepcionalmente pequeña debe desplazar las columnas, no ocultarlas.");
                foreach (var c in columnas.Controls.Cast<Control>()) Assert.IsTrue(c.Visible);
                foreach (string n in new[] { "btnGuardarCambios", "btnDescartarCambios", "btnRecargarDatos" })
                {
                    var boton = form.Controls.Find(n, true)[0];
                    Assert.IsTrue(form.RectangleToScreen(form.ClientRectangle).Contains(boton.RectangleToScreen(boton.ClientRectangle)), n);
                }
            }
        });
    }

    [TestMethod]
    public void CambiarContextosConservaBorradorYSeleccionesIndependientes()
    {
        EnInterfaz(() =>
        {
            using var form = new FormGestionRoles(); QuitarCarga(form);
            var original = EdicionRolesTests.Base();
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { original });
            Preparar(form, new Size(1240, 820));
            var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0]; usuarios.SelectedIndex = 1;
            var cuentas = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0]; cuentas.SetItemChecked(1, true); Application.DoEvents();
            var seleccion = usuarios.SelectedItem;
            var roles = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0]; roles.SelectedNode = roles.Nodes[3];
            var familias = (ListBox)form.Controls.Find("listaFamilias", true)[0]; familias.SelectedIndex = 2;
            Application.DoEvents();
            Assert.IsTrue(form.TieneCambios);
            Assert.AreSame(seleccion, usuarios.SelectedItem);
            Assert.IsTrue(cuentas.GetItemChecked(1));
            Assert.HasCount(1, original.Usuarios[1].Roles, "Los cambios siguen en memoria y no alteran los datos confirmados.");
            form.DescartarCambios();
            Assert.IsFalse(form.TieneCambios); Assert.IsFalse(cuentas.GetItemChecked(1));
        });
    }

    [TestMethod]
    public void MenuContraibleGanaEspacioSinCerrarElModuloNiPerderBorrador()
    {
        EnInterfaz(() =>
        {
            var datos = EdicionRolesTests.Base(); SesionServicio.ObtenerInstancia().IniciarSesion(datos.Usuarios[0]);
            using var principal = new FrmPrincipal(); QuitarCarga(principal); Preparar(principal, new Size(1366, 850));
            var form = new FormGestionRoles(); QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { datos });
            typeof(FrmPrincipal).GetMethod("AbrirFormulario", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(principal, new object[] { form });
            var usuario = (ListBox)form.Controls.Find("listaUsuarios", true)[0]; usuario.SelectedIndex = 1;
            ((CheckedListBox)form.Controls.Find("rolesUsuario", true)[0]).SetItemChecked(1, true); Application.DoEvents();
            var host = principal.Controls.Find("panelContenido", true)[0]; int ancho = host.Width;
            var menu = (Button)principal.Controls.Find("btnAlternarMenu", true)[0]; menu.PerformClick(); Application.DoEvents();
            Assert.IsTrue(host.Width > ancho + 100); Assert.AreSame(form, host.Controls[0]); Assert.IsTrue(form.TieneCambios);
            foreach (var boton in principal.Controls.Find("panelNavegacion", true)[0].Controls.OfType<BotonNavegacion>())
            { Assert.IsTrue(boton.MostrarSoloIcono); Assert.IsFalse(string.IsNullOrWhiteSpace(boton.AccessibleName)); }
            Guardar(principal, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "columnas-menu-contraido");
            menu.PerformClick(); Application.DoEvents(); Assert.AreEqual(ancho, host.Width); Assert.IsTrue(form.TieneCambios);
            form.DescartarCambios();
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
            // La prueba bombea mensajes sin Application.Run: instalamos el contexto que usa la aplicación real.
            SynchronizationContext.SetSynchronizationContext(new WindowsFormsSynchronizationContext());
            var tarea = (Task)typeof(FrmIntegridad).GetMethod("Operar", BindingFlags.Instance | BindingFlags.NonPublic)!
                .Invoke(form, new object[] { (Func<string>)(() => ""), false })!;
            var limite = DateTime.UtcNow.AddSeconds(5);
            while (!tarea.IsCompleted && DateTime.UtcNow < limite) { Application.DoEvents(); Thread.Sleep(10); }
            Assert.IsTrue(tarea.IsCompleted, "La operación debe finalizar en el hilo de la interfaz.");
            tarea.GetAwaiter().GetResult();
            Assert.IsFalse(form.Controls.Find("btnContinuar", true)[0].Enabled);
            Assert.IsTrue(form.Controls.Find("btnRecuperar", true)[0].Enabled);
            Assert.IsTrue(form.Controls.Find("btnRecalcular", true)[0].Enabled);
            Assert.IsFalse(form.Controls.Find("btnBackupVerificado", true)[0].Enabled);
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "integridad");
            var avanzadas = (CheckBox)form.Controls.Find("chkOpcionesAvanzadas", true)[0];
            var completas = (CheckBox)form.Controls.Find("completas", true)[0];
            Assert.IsFalse(form.Controls.Find("panelAvanzado", true)[0].Visible);
            avanzadas.Checked = true;
            Application.DoEvents();
            Assert.IsTrue(completas.Visible);
            Assert.IsTrue(form.Controls.Find("btnRecalcular", true)[0].Visible);
            completas.Checked = true;
            avanzadas.Checked = false;
            Assert.IsFalse(completas.Checked, "Ocultar las opciones avanzadas no debe dejar una recuperación completa seleccionada en secreto.");
            Assert.IsFalse(form.Controls.Find("btnContinuar", true)[0].Enabled);
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
                if (form == usuarios)
                {
                    gridUsers.CurrentCell = gridUsers.Rows[0].Cells["NombreUsuario"];
                    typeof(FrmGestionUsuarios).GetMethod("SeleccionarFila", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(usuarios, null);
                    StringAssert.Contains(usuarios.Controls.Find("label9", true)[0].Text, "admin");
                    StringAssert.Contains(usuarios.Controls.Find("lblAyudaClave", true)[0].Text, "conservar");
                    usuarios.Controls.Find("txtPass", true)[0].Text = "Borrador sin guardar";
                    Guardar(form, directory, "usuarios-edicion");
                    typeof(FrmGestionUsuarios).GetMethod("Limpiar", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(usuarios, null);
                    Assert.AreEqual("", usuarios.Controls.Find("txtPass", true)[0].Text);
                    Assert.AreEqual("Nuevo usuario", usuarios.Controls.Find("label9", true)[0].Text);
                    var datos = gridUsers.DataSource;
                    gridUsers.DataSource = new List<Usuario>();
                    StringAssert.Contains(usuarios.Controls.Find("lblEstadoLista", true)[0].Text, "No hay usuarios");
                    gridUsers.DataSource = datos;
                }
                if (form == bitacora)
                {
                    var datos = gridLog.DataSource;
                    gridLog.DataSource = new List<Bitacora>();
                    Assert.IsTrue(bitacora.Controls.Find("lblVacio", true)[0].Visible);
                    Guardar(form, directory, "bitacora-vacia");
                    gridLog.DataSource = datos;
                    Assert.IsFalse(bitacora.Controls.Find("lblVacio", true)[0].Visible,
                        "El aviso debe desaparecer al recuperar resultados.");
                }
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

    [TestMethod]
    public void HistorialConservaComparacionYSeleccionAlCambiarTemaYTamano()
    {
        EnInterfaz(() =>
        {
            var tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!.GetMethod("CambiarModo")!;
            using var dialogo = new DlgHistorialUsuario();
            var carga = typeof(DlgHistorialUsuario).GetMethod("CargarHistorial", BindingFlags.Instance | BindingFlags.NonPublic)!;
            dialogo.Shown -= (EventHandler)carga.CreateDelegate(typeof(EventHandler), dialogo);
            var inicial = new EstadoUsuarioVersion("ana", "Ana", "Pérez", true, Array.Empty<RolVersionUsuario>());
            var editado = inicial with { Apellido = "Gómez" };
            var actual = editado with { Activo = false };
            var autor = new AutorCambioUsuario(1, "admin");
            var historial = new HistorialUsuario(10, actual, new[]
            {
                new UsuarioVersion(2, 5, DateTime.UtcNow, autor, OperacionUsuarioVersion.Baja, actual, true, false, null),
                new UsuarioVersion(2, 4, DateTime.UtcNow.AddMinutes(-1), autor, OperacionUsuarioVersion.Edicion, editado, true, false, null),
                new UsuarioVersion(2, 3, DateTime.UtcNow.AddMinutes(-2), autor, OperacionUsuarioVersion.Edicion, inicial, true, true, null),
                new UsuarioVersion(2, 2, DateTime.UtcNow.AddMinutes(-3), autor, OperacionUsuarioVersion.Edicion, inicial, true, false, null),
                new UsuarioVersion(2, 1, DateTime.UtcNow.AddMinutes(-4), autor, OperacionUsuarioVersion.Alta, inicial with { Nombre = "A." }, true, false, null)
            });
            var cuentas = (ComboBox)dialogo.Controls.Find("cmbCuentas", true).Single();
            cuentas.DisplayMember = "Value";
            cuentas.DataSource = new[] { KeyValuePair.Create(new CuentaConHistorial(2, "ana", true), "ana · #2") };
            typeof(DlgHistorialUsuario).GetField("historial", BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(dialogo, historial);
            typeof(DlgHistorialUsuario).GetMethod("MostrarVersiones", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(dialogo, new object?[] { null });
            var versiones = (DataGridView)dialogo.Controls.Find("dgvVersiones", true).Single();
            var detalle = (DataGridView)dialogo.Controls.Find("dgvDetalle", true).Single();
            var restaurar = (Button)dialogo.Controls.Find("btnRestaurar", true).Single();
            Preparar(dialogo, new Size(920, 640));
            Assert.IsFalse(restaurar.Enabled, "No se restaura un estado idéntico al actual.");
            StringAssert.Contains(dialogo.Controls.Find("lblRestauracion", true).Single().Text, "coincide");
            versiones.CurrentCell = versiones.Rows[1].Cells["Version"];
            try
            {
                foreach (bool oscuro in new[] { false, true, false })
                {
                    tema.Invoke(null, new object[] { oscuro });
                    foreach (var size in new[] { new Size(920, 640), new Size(700, 515) })
                    {
                        dialogo.ClientSize = size;
                        Application.DoEvents();
                        ComprobarGrillas(dialogo);
                        Assert.AreEqual(4, versiones.CurrentRow!.Cells["Version"].Value);
                        Assert.IsTrue(restaurar.Enabled);
                        Assert.AreEqual("Pérez", detalle.Rows[2].Cells["Anterior"].Value);
                        Assert.AreEqual("Gómez", detalle.Rows[2].Cells["Seleccionada"].Value);
                        var cambio = detalle.Rows[2].Cells["Seleccionada"];
                        var estilo = new DataGridViewCellStyle(detalle.DefaultCellStyle);
                        var formato = new DataGridViewCellFormattingEventArgs(2, 2, cambio.Value, typeof(string), estilo);
                        typeof(DataGridView).GetMethod("OnCellFormatting", BindingFlags.Instance | BindingFlags.NonPublic,
                            null, new[] { typeof(DataGridViewCellFormattingEventArgs) }, null)!.Invoke(detalle, new object[] { formato });
                        Assert.IsTrue(formato.CellStyle.Font!.Bold);
                        Assert.AreNotEqual(detalle.DefaultCellStyle.BackColor, formato.CellStyle.BackColor);
                        Assert.IsFalse(string.IsNullOrEmpty(cambio.ToolTipText));
                        if (size.Width == 920)
                            foreach (var grilla in new[] { versiones, detalle })
                            {
                                var ultimaFila = grilla.GetRowDisplayRectangle(4, false);
                                Assert.IsTrue(ultimaFila.Height > 0 && ultimaFila.Bottom <= grilla.ClientSize.Height,
                                    "Las cinco filas deben entrar completas en el tamaño habitual: " + grilla.Name);
                            }
                        Guardar(dialogo, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "historial-" + (oscuro ? "oscuro" : "claro") + "-" + size.Width);
                    }
                }
            }
            finally { tema.Invoke(null, new object[] { false }); }
        });
    }

    [TestMethod]
    public void BuscarEnUnaColumnaConservaLosOtrosArbolesYElBorrador()
    {
        EnInterfaz(() =>
        {
            using var form = new FormGestionRoles(); QuitarCarga(form);
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!
                .Invoke(form, new object[] { EdicionRolesTests.Base() });
            Preparar(form, new Size(1440, 900));
            var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0];
            usuarios.SelectedIndex = 1;
            var marcas = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0];
            marcas.SetItemChecked(1, true); Application.DoEvents();
            var treeRoles = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
            var rol = treeRoles.SelectedNode;
            var nodoRol = treeRoles.Nodes[0];
            var familia = ((ListBox)form.Controls.Find("listaFamilias", true)[0]).SelectedItem;
            var filtro = (TextBox)form.Controls.Find("buscarUsuarios", true)[0];
            filtro.Text = "nadie-con-ese-nombre";
            var espera = System.Diagnostics.Stopwatch.StartNew();
            while (usuarios.Items.Count != 0 && espera.ElapsedMilliseconds < 3000) { Application.DoEvents(); Thread.Sleep(10); }
            Assert.IsEmpty(usuarios.Items);
            Assert.AreSame(rol, treeRoles.SelectedNode);
            Assert.AreSame(nodoRol, treeRoles.Nodes[0], "Buscar usuarios no reconstruye el árbol de otro contexto.");
            Assert.AreSame(familia, ((ListBox)form.Controls.Find("listaFamilias", true)[0]).SelectedItem);
            Assert.IsTrue(form.TieneCambios);
            filtro.Clear();
            espera.Restart();
            while (usuarios.Items.Count == 0 && espera.ElapsedMilliseconds < 3000) { Application.DoEvents(); Thread.Sleep(10); }
            usuarios.SelectedIndex = 1;
            Assert.IsTrue(marcas.GetItemChecked(1), "El filtro no descarta las asignaciones pendientes.");
            form.DescartarCambios();
            Assert.IsFalse(marcas.GetItemChecked(1));
            Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-mejorados");
        });
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void CargaRolesSinBloquearLaEdicionYTraduceSinConsultarOtraVezSql()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        var nombreBase = new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cadena).InitialCatalog;
        string? anterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", nombreBase);
            EnInterfaz(() =>
            {
                var datos = new SIGAT.DAL.RolesDAL(cadena).Cargar();
                SesionServicio.ObtenerInstancia().IniciarSesion(datos.Usuarios.Single(u => u.IdUsuario == datos.IdAdministradorOriginal));
                using var form = new FormGestionRoles { ShowInTaskbar = false, Opacity = 0, ClientSize = new Size(1500, 900) };
                var campo = typeof(FormGestionRoles).GetField("consultando", BindingFlags.Instance | BindingFlags.NonPublic)!;
                var tiempo = System.Diagnostics.Stopwatch.StartNew();
                form.Show();
                Assert.IsTrue((bool)campo.GetValue(form)!);
                Assert.IsFalse(form.Controls.Find("visorColumnas", true)[0].Enabled);
                Assert.IsFalse(form.Controls.Find("btnGuardarCambios", true)[0].Enabled);
                bool respondio = false;
                form.BeginInvoke((Action)(() => respondio = true));
                while ((bool)campo.GetValue(form)! && tiempo.Elapsed < TimeSpan.FromSeconds(30))
                { Application.DoEvents(); Thread.Sleep(10); }
                Assert.IsTrue(respondio);
                Assert.IsFalse((bool)campo.GetValue(form)!, "La carga debe finalizar.");
                Assert.IsTrue(form.Controls.Find("visorColumnas", true)[0].Enabled);
                Assert.HasCount(datos.Usuarios.Count, ((ListBox)form.Controls.Find("listaUsuarios", true)[0]).Items);
                Assert.HasCount(3, form.Controls.Find("columnasRoles", true)[0].Controls);
                Assert.IsFalse(form.TieneCambios);
                Assert.IsFalse(form.Controls.Find("btnAgregarPermisoRol", true)[0].Enabled, "Administrador sigue protegido.");
                // Una actualización de idioma ya no necesita conectarse a SQL.
                Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", "SIGAT_Roles_Pruebas_NoExiste_SoloLectura");
                form.ActualizarIdioma();
                Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", nombreBase);
                Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "roles-carga-real");
                Console.WriteLine($"Carga de roles en base de prueba: {tiempo.ElapsedMilliseconds} ms.");
            });
        }
        finally { Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", anterior); }
    }

    [TestMethod]
    public void RedisenoIdentificaDestinoFamiliasCompartidasYResumenSinGuardar()
    {
        EnInterfaz(() =>
        {
            var tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!.GetMethod("CambiarModo")!;
            try
            {
                foreach (bool oscuro in new[] { false, true })
                {
                    tema.Invoke(null, new object[] { oscuro });
                    var datos = EdicionRolesTests.Base();
                    datos.Usuarios[0].NombreUsuario = "admin"; datos.Usuarios[1].NombreUsuario = "Toby";
                    var familia = new PermisoCompuesto { Id = 8, Nombre = "Equipos de oficina" };
                    var hija = new PermisoCompuesto { Id = 9, Nombre = "Portátiles" };
                    hija.Agregar(datos.Permisos[1]); familia.Agregar(hija);
                    datos.Permisos.AddRange(new[] { familia, hija });
                    datos.Roles[1].AgregarPermiso(familia); datos.Roles[3].AgregarPermiso(familia);
                    datos.Roles[1].AgregarRol(datos.Roles[2]);
                    using var form = new FormGestionRoles(); QuitarCarga(form);
                    typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .Invoke(form, new object[] { datos });
                    Preparar(form, new Size(1600, 900));
                    var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0]; usuarios.SelectedIndex = 1;
                    var marcas = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0]; marcas.SetItemChecked(1, true);
                    Application.DoEvents();
                    var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                    var rama = arbol.Nodes[1].Nodes.Cast<TreeNode>().Single(n => n.Text == familia.Nombre);
                    arbol.Nodes[1].Expand(); rama.Expand(); rama.Nodes[0].Expand(); arbol.SelectedNode = rama.Nodes[0];
                    var catalogo = (ListBox)form.Controls.Find("listaFamilias", true)[0];
                    catalogo.SelectedIndex = Enumerable.Range(0, catalogo.Items.Count).Single(i =>
                        (int)catalogo.Items[i].GetType().GetProperty("Id")!.GetValue(catalogo.Items[i])! == familia.Id);
                    ((TreeView)form.Controls.Find("tvCatalogoGeneral", true)[0]).ExpandAll();
                    StringAssert.Contains(form.Controls.Find("lblDestinoRol", true)[0].Text, "Responsable de Inventario");
                    StringAssert.Contains(form.Controls.Find("lblDestinoRol", true)[0].Text, "Equipos de oficina");
                    StringAssert.Contains(form.Controls.Find("lblDestinoRol", true)[0].Text, "Portátiles");
                    Assert.IsFalse(form.Controls.Find("btnIncluirRolVisible", true)[0].Enabled, "Una familia no recibe roles.");
                    Assert.IsTrue(form.Controls.Find("btnSubfamiliaVisible", true)[0].Enabled);
                    Assert.IsTrue(form.Controls.Find("btnVerResumen", true)[0].Enabled);
                    Assert.AreEqual(DrawMode.OwnerDrawFixed, catalogo.DrawMode);
                    foreach (var size in new[] { new Size(1600, 900), new Size(1366, 768), new Size(1040, 740) })
                    {
                        form.ClientSize = size; Application.DoEvents();
                        Assert.HasCount(3, form.Controls.Find("columnasRoles", true)[0].Controls);
                        Assert.HasCount(5, arbol.Nodes);
                        foreach (var nombre in new[] { "btnGuardarCambios", "btnDescartarCambios", "btnRecargarDatos" })
                        {
                            var boton = form.Controls.Find(nombre, true)[0];
                            Assert.IsTrue(form.ClientRectangle.Contains(form.RectangleToClient(boton.RectangleToScreen(boton.ClientRectangle))));
                        }
                        Guardar(form, Environment.GetEnvironmentVariable("SIGAT_VISUAL_PREVIEW"), "rediseno-" + (oscuro ? "oscuro" : "claro") + "-" + size.Width);
                    }
                    bool vioResumen = false;
                    using var cerrar = new System.Windows.Forms.Timer { Interval = 25 };
                    cerrar.Tick += (_, _) =>
                    {
                        var dialogo = Application.OpenForms.OfType<DlgFamiliaPermisos>().FirstOrDefault();
                        if (dialogo == null) return;
                        cerrar.Stop();
                        vioResumen = dialogo.Controls.Find("resumenCambios", true)[0].Text.Contains("Toby");
                        dialogo.DialogResult = DialogResult.OK;
                    };
                    cerrar.Start();
                    typeof(LinkLabel).GetMethod("OnLinkClicked", BindingFlags.NonPublic | BindingFlags.Instance)!
                        .Invoke(form.Controls.Find("btnVerResumen", true)[0], new object[] { new LinkLabelLinkClickedEventArgs(new LinkLabel.Link()) });
                    Assert.IsTrue(vioResumen);
                    Assert.IsTrue(form.TieneCambios, "Cerrar el resumen no confirma ni descarta la edición.");
                    Assert.HasCount(1, familia.ObtenerHijos());
                    form.DescartarCambios();
                    Assert.IsFalse(form.TieneCambios);
                    Assert.IsFalse(form.Controls.Find("btnVerResumen", true)[0].Enabled);
                    Assert.HasCount(5, arbol.Nodes);
                    arbol.SelectedNode = arbol.Nodes[0];
                    Assert.IsFalse(form.Controls.Find("btnAgregarPermisoRol", true)[0].Enabled);
                    Assert.IsFalse(form.Controls.Find("btnIncluirRolVisible", true)[0].Enabled);
                }
            }
            finally { tema.Invoke(null, new object[] { false }); }
        });
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void BotonesDelRedisenoGuardanLasTresColumnasYDescartarNoEscribeSql()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        var dal = new SIGAT.DAL.RolesDAL(cadena);
        var original = dal.Cargar();
        string? baseAnterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", new Microsoft.Data.SqlClient.SqlConnectionStringBuilder(cadena).InitialCatalog);
        try
        {
            EnInterfaz(() =>
            {
                SesionServicio.ObtenerInstancia().IniciarSesion(original.Usuarios.Single(u => u.IdUsuario == original.IdAdministradorOriginal));
                using var form = new FormGestionRoles(); QuitarCarga(form);
                typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(form, new object[] { dal.Cargar() });
                Preparar(form, new Size(1500, 900));
                var usuarios = (ListBox)form.Controls.Find("listaUsuarios", true)[0];
                var marcas = (CheckedListBox)form.Controls.Find("rolesUsuario", true)[0];
                var catalogo = (ListBox)form.Controls.Find("listaFamilias", true)[0];
                var arbol = (TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                int usuario = original.Usuarios.First(u => u.IdUsuario != original.IdAdministradorOriginal && !u.Roles.Any(r => r.Id == 2)).IdUsuario;
                int Indice(ListBox lista, int id) => Enumerable.Range(0, lista.Items.Count).Single(i =>
                    (int)lista.Items[i].GetType().GetProperty("Id")!.GetValue(lista.Items[i])! == id);
                // Los diálogos se aceptan como en el recorrido real: se ejercitan los botones y sus eventos.
                void AceptarDialogo(string boton, Action<Form> completar)
                {
                    Exception? error = null;
                    using var aceptar = new System.Windows.Forms.Timer { Interval = 25 };
                    aceptar.Tick += (_, _) =>
                    {
                        var dialogo = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f is DlgNombreRol or DlgFamiliaPermisos);
                        if (dialogo == null) return;
                        aceptar.Stop();
                        try { completar(dialogo); }
                        catch (Exception ex) { error = ex; }
                        finally { dialogo.DialogResult = error == null ? DialogResult.OK : DialogResult.Cancel; }
                    };
                    var accion = (Button)form.Controls.Find(boton, true)[0];
                    Assert.IsTrue(accion.Enabled && accion.Visible, boton);
                    aceptar.Start(); accion.PerformClick(); Application.DoEvents();
                    if (error != null) throw error;
                }
                void ElegirComponente(Form dialogo, string nombre)
                {
                    var lista = (CheckedListBox)dialogo.Controls.Find("lista", true)[0];
                    int indice = Enumerable.Range(0, lista.Items.Count).Single(i =>
                        ((string)lista.Items[i].GetType().GetProperty("Texto")!.GetValue(lista.Items[i])!).EndsWith(" · " + nombre));
                    lista.SetItemChecked(indice, true);
                }
                string nombre = "Subfamilia UI " + Guid.NewGuid().ToString("N")[..8];
                long version = original.Version;
                foreach (bool guardar in new[] { false, true })
                {
                    usuarios.SelectedIndex = Indice(usuarios, usuario);
                    marcas.SetItemChecked(Indice(marcas, 2), true); Application.DoEvents();
                    catalogo.SelectedIndex = Indice(catalogo, 3);
                    AceptarDialogo("btnSubfamiliaVisible", d => d.Controls.Find("txtNombre", true)[0].Text = nombre);
                    AceptarDialogo("btnAgregarComponenteFamilia", d => ElegirComponente(d, "Consultar inventario"));
                    arbol.SelectedNode = arbol.Nodes[1];
                    AceptarDialogo("btnIncluirRolVisible", d => ElegirComponente(d, "Técnico de Mantenimiento"));
                    Assert.IsTrue(form.TieneCambios);
                    Assert.AreEqual(version, dal.Cargar().Version, "Editar las tres columnas conserva SQL hasta guardar.");
                    Assert.IsFalse(dal.Cargar().Permisos.Any(p => p.Nombre == nombre));
                    if (!guardar)
                    {
                        form.DescartarCambios(); Application.DoEvents();
                        Assert.IsFalse(form.TieneCambios);
                        Assert.IsFalse(marcas.GetItemChecked(Indice(marcas, 2)));
                        Assert.IsFalse(arbol.Nodes[1].Nodes.Cast<TreeNode>().Any(n => n.Text == "Técnico de Mantenimiento"));
                        Assert.AreEqual(version, dal.Cargar().Version);
                        continue;
                    }
                    AceptarDialogo("btnGuardarCambios", d => StringAssert.Contains(d.Controls.Find("resumenCambios", true)[0].Text, nombre));
                    Assert.IsFalse(form.TieneCambios);
                    var confirmado = dal.Cargar();
                    Assert.AreEqual(version + 1, confirmado.Version);
                    Assert.IsTrue(confirmado.Usuarios.Single(u => u.IdUsuario == usuario).Roles.Any(r => r.Id == 2));
                    Assert.IsTrue(confirmado.Roles.Single(r => r.Id == 2).Roles.Any(r => r.Id == 3));
                    var hija = confirmado.Permisos.Single(p => p.Nombre == nombre);
                    Assert.AreEqual(2, hija.ObtenerHijos().Single().Id);
                    Assert.IsTrue(confirmado.Permisos.Single(p => p.Id == 3).ObtenerHijos().Any(p => p.Id == hija.Id));
                    Assert.IsTrue(new SIGAT.DAL.IntegridadDAL(cadena).Verificar().Correcta);
                    Assert.HasCount(5, arbol.Nodes);
                    arbol.SelectedNode = arbol.Nodes[0];
                    Assert.IsFalse(form.Controls.Find("btnAgregarPermisoRol", true)[0].Enabled);
                    Assert.IsFalse(form.Controls.Find("btnIncluirRolVisible", true)[0].Enabled);
                }
            });
        }
        finally
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", baseAnterior);
            original.Version = dal.Cargar().Version;
            dal.Guardar(original);
        }
    }

    private static ToolStripMenuItem OpcionRoles(FormGestionRoles form, string nombre) =>
        (ToolStripMenuItem)typeof(FormGestionRoles).GetField(nombre, BindingFlags.NonPublic | BindingFlags.Instance)!.GetValue(form)!;

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
