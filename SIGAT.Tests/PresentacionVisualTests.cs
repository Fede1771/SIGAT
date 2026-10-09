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
            foreach (string column in new[] { "IdIdioma", "IdControl", "DigitoVerificador" }) gridLanguages.Columns[column]!.Visible = false;
            using var bitacora = new FrmBitacora();
            var gridLog = (DataGridView)bitacora.Controls.Find("dgv", true)[0];
            gridLog.DataSource = new List<Bitacora> { new() { Fecha = DateTime.Now, Usuario = "admin", Actividad = "Login", InformacionAsociada = "Inicio de sesión correcto" } };
            using var roles = new FormGestionRoles();
            typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(roles, new object[] { catalog });
            foreach (var (form, name) in new[] { ((Form)usuarios, "usuarios"), (idiomas, "idiomas"), (bitacora, "bitacora"), (roles, "roles") })
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
            foreach (var (form, name) in new[] { ((Form)usuarios, "usuarios"), (idiomas, "idiomas"), (bitacora, "bitacora"), (roles, "roles") })
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
                var rect = new Rectangle(form.PointToClient(grid.PointToScreen(Point.Empty)), grid.Size);
                Assert.IsTrue(form.ClientRectangle.Contains(rect), $"La grilla {grid.Name} debe quedar dentro de la pantalla: {rect} / {form.ClientRectangle}.");
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
