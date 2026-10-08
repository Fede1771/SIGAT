using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using Microsoft.Data.SqlClient;
using System.Windows.Forms;
using System.Drawing;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class UsuariosTests
{
    [TestMethod]
    public void ValidaCamposObligatorios()
    {
        Assert.ThrowsExactly<ArgumentException>(() => UsuarioBLL.ValidarDatos(new Usuario()));
        var usuario = new Usuario { NombreUsuario = " prueba ", Nombre = " Ana ", Apellido = " Pérez " };
        UsuarioBLL.ValidarDatos(usuario);
        Assert.AreEqual("prueba", usuario.NombreUsuario);
        Assert.AreEqual("Ana", usuario.Nombre);
    }

    [TestMethod]
    public void GrillaExplicitaSinPerfilNiPasswordYSeleccionLimpiaClave()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var form = new SIGAT.UI.FrmGestionUsuarios();
                var grid = (DataGridView)form.Controls.Find("dgvUsuarios", true)[0];
                Assert.AreEqual(6, grid.Columns.Count);
                Assert.IsFalse(grid.AutoGenerateColumns);
                Assert.IsFalse(grid.Columns.Contains("Id"));
                Assert.IsFalse(grid.Columns.Contains("Password"));
                Assert.IsFalse(grid.Columns.Contains("Perfil"));
                Assert.AreEqual(0, form.Controls.Find("cmbPerfil", true).Length);
                Usuario cuenta = new Usuario { IdUsuario = 42, NombreUsuario = "usuario_prueba", Nombre = "Ana", Apellido = "Pérez", Activo = true };
                cuenta.AsignarRol(MatrizRoles.CrearCatalogo().Roles[4]);
                grid.DataSource = new List<Usuario> { cuenta };
                grid.CurrentCell = grid.Rows[0].Cells[0];
                var pass = (TextBox)form.Controls.Find("txtPass", true)[0];
                pass.Text = "claveOtraCuenta";
                typeof(SIGAT.UI.FrmGestionUsuarios).GetMethod("SeleccionarFila", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.Invoke(form, null);
                Assert.AreEqual("", pass.Text);
                Assert.AreEqual("usuario_prueba", ((TextBox)form.Controls.Find("txtUsername", true)[0]).Text);
                string? imagen = Environment.GetEnvironmentVariable("SIGAT_USUARIOS_PREVIEW");
                if (!string.IsNullOrWhiteSpace(imagen))
                {
                    // Renderizar datos de prueba sin ejecutar la carga conectada a SIGAT.
                    var carga = typeof(SIGAT.UI.FrmGestionUsuarios).GetMethod("FrmGestionUsuarios_Load", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                    form.Load -= (EventHandler)carga.CreateDelegate(typeof(EventHandler), form);
                    using var principal = new Form { IsMdiContainer = true, Size = new Size(1440, 900), ShowInTaskbar = false, Opacity = 0 };
                    var menu = new MenuStrip();
                    menu.Items.Add("Sistema");
                    menu.Items.Add("Idioma");
                    principal.MainMenuStrip = menu;
                    principal.Controls.Add(menu);
                    principal.Show();
                    form.MdiParent = principal;
                    form.WindowState = FormWindowState.Maximized;
                    form.Show();
                    form.PerformLayout();
                    Rectangle tabla = new Rectangle(form.PointToClient(grid.PointToScreen(Point.Empty)), grid.Size);
                    Assert.IsTrue(tabla.Top > 60, "La tabla debe quedar debajo del encabezado y dentro del área visible.");
                    Assert.IsTrue(form.ClientRectangle.Contains(tabla), "La tabla no debe quedar recortada en MDI.");
                    using Bitmap bitmap = new Bitmap(principal.Width, principal.Height);
                    principal.DrawToBitmap(bitmap, new Rectangle(0, 0, principal.Width, principal.Height));
                    bitmap.Save(imagen);
                }
            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start(); hilo.Join();
        if (error != null) throw error;
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void AltaSinRolesEdicionClaveBajaReactivacionYProteccionAdministrador()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere base exclusiva de pruebas."); return; }
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Base de pruebas requerida.");
        var dal = new UsuarioDAL(cadena);
        Usuario nuevo = new Usuario { NombreUsuario = "crud_" + Guid.NewGuid().ToString("N").Substring(0, 10), Nombre = "Ana", Apellido = "Pérez", Activo = true, Password = HashHelper.ObtenerHashSHA256("Inicial123") };
        try
        {
            dal.Insertar(nuevo);
            Assert.IsTrue(nuevo.IdUsuario > 0);
            var roles = new RolesBLL(new RolesDAL(cadena));
            roles.CargarRolesUsuario(nuevo);
            Assert.IsEmpty(nuevo.Roles);
            string hash = nuevo.Password;
            nuevo.Nombre = "Ana María";
            nuevo.Password = "";
            dal.Actualizar(nuevo);
            Assert.AreEqual(hash, dal.ObtenerPorNombreUsuario(nuevo.NombreUsuario)!.Password);
            Assert.AreEqual("Ana María", dal.ObtenerPorNombreUsuario(nuevo.NombreUsuario)!.Nombre);
            nuevo.Password = HashHelper.ObtenerHashSHA256("Nueva123");
            dal.Actualizar(nuevo);
            Assert.AreEqual(nuevo.Password, dal.ObtenerPorNombreUsuario(nuevo.NombreUsuario)!.Password);
            dal.Eliminar(nuevo.IdUsuario);
            Assert.IsFalse(dal.ObtenerPorNombreUsuario(nuevo.NombreUsuario)!.Activo);
            nuevo.Activo = true;
            nuevo.Password = "";
            dal.Actualizar(nuevo);
            Assert.IsTrue(dal.ObtenerPorNombreUsuario(nuevo.NombreUsuario)!.Activo);
            Usuario? admin = null;
            foreach (Usuario candidato in dal.ObtenerTodos()) if (candidato.IdUsuario == 1) admin = candidato;
            Assert.IsNotNull(admin);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.Eliminar(admin.IdUsuario));
            admin.Activo = false;
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.Actualizar(admin));
            Assert.IsTrue(dal.ObtenerPorNombreUsuario(admin.NombreUsuario)!.Activo);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.Insertar(nuevo, true));
        }
        finally
        {
            using var conexion = new SqlConnection(cadena);
            conexion.Open();
            using var comando = new SqlCommand("DELETE FROM dbo.Usuarios WHERE IdUsuario=@Id", conexion);
            comando.Parameters.AddWithValue("@Id", nuevo.IdUsuario);
            comando.ExecuteNonQuery();
        }
    }
}
