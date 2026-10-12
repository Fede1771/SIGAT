using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using SIGAT.UI;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class TemaVisualTests
{
    private static readonly Type Tema = typeof(FrmPrincipal).Assembly.GetType("SIGAT.UI.TemaVisual")!;
    private static object? Invocar(string nombre, params object[] argumentos) => Tema.GetMethod(nombre)!.Invoke(null, argumentos);

    [TestMethod]
    public void AlternarTemaConservaCamposAsignacionesYColoresOriginales()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                Invocar("CambiarModo", false);
                using var usuarios = new FrmGestionUsuarios();
                var nombre = (TextBox)usuarios.Controls.Find("txtNombre", true)[0];
                nombre.Text = "Edición pendiente";
                var clave = (TextBox)usuarios.Controls.Find("txtPass", true)[0];
                clave.Text = "Clave pendiente";
                var grilla = (DataGridView)usuarios.Controls.Find("dgvUsuarios", true)[0];
                var datos = new List<Usuario> { new() { IdUsuario = 7, NombreUsuario = "prueba", Activo = true } };
                grilla.DataSource = datos;
                grilla.CurrentCell = grilla.Rows[0].Cells[1];
                Color fondo = usuarios.BackColor, cabecera = grilla.ColumnHeadersDefaultCellStyle.BackColor;
                Invocar("Aplicar", usuarios);
                Invocar("CambiarModo", true);
                Invocar("Aplicar", usuarios);
                Assert.IsTrue(usuarios.BackColor.GetBrightness() < 0.2);
                Assert.IsTrue(nombre.ForeColor.GetBrightness() > 0.6);
                Assert.AreEqual("Edición pendiente", nombre.Text);
                Assert.AreEqual("Clave pendiente", clave.Text);
                Assert.AreSame(datos, grilla.DataSource);
                Assert.AreEqual(1, grilla.CurrentCell.ColumnIndex);
                Invocar("CambiarModo", false);
                Invocar("Aplicar", usuarios);
                Assert.AreEqual(fondo, usuarios.BackColor);
                Assert.AreEqual(cabecera, grilla.ColumnHeadersDefaultCellStyle.BackColor);

                var admin = new Usuario { IdUsuario = int.MaxValue - 50, NombreUsuario = "prueba_tema", Activo = true };
                admin.AsignarRol(MatrizRoles.CrearCatalogo().Roles[0]);
                SesionServicio.ObtenerInstancia().IniciarSesion(admin);
                using var principal = new FrmPrincipal();
                var carga = typeof(FrmPrincipal).GetMethod("FrmPrincipal_Load", BindingFlags.NonPublic | BindingFlags.Instance)!;
                principal.Load -= (EventHandler)carga.CreateDelegate(typeof(EventHandler), principal);
                principal.WindowState = FormWindowState.Normal;
                principal.Size = new Size(1440, 900);
                principal.ShowInTaskbar = false;
                principal.Opacity = 0;
                principal.Show();
                Invocar("CambiarModo", true);
                typeof(FrmPrincipal).GetMethod("ActualizarBotonTema", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(principal, null);
                Assert.HasCount(1, principal.Controls.Find("btnTemaVisual", true));
                string? destino = Environment.GetEnvironmentVariable("SIGAT_TEMA_PREVIEW");
                if (!string.IsNullOrWhiteSpace(destino))
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(destino)!);
                    using var bitmap = new Bitmap(principal.Width, principal.Height);
                    principal.DrawToBitmap(bitmap, new Rectangle(Point.Empty, principal.Size));
                    bitmap.Save(destino);
                }
            }
            catch (Exception ex) { error = ex; }
            finally { Invocar("CambiarModo", false); SesionServicio.ObtenerInstancia().CerrarSesion(); }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start(); hilo.Join();
        if (error != null) throw error;
    }

    [TestMethod]
    public void PreferenciaSeGuardaPorIdDeCuentaEnElCliente()
    {
        int uno = int.MaxValue - 51, dos = int.MaxValue - 52;
        string ruta = (string)Invocar("RutaPreferencia", uno)!;
        string rutaDos = (string)Invocar("RutaPreferencia", dos)!;
        try
        {
            Invocar("GuardarPreferencia", uno, true);
            Invocar("GuardarPreferencia", dos, false);
            Invocar("CargarPreferencia", uno);
            Assert.IsTrue((bool)Tema.GetProperty("EsOscuro")!.GetValue(null)!);
            Invocar("CargarPreferencia", dos);
            Assert.IsFalse((bool)Tema.GetProperty("EsOscuro")!.GetValue(null)!);
        }
        finally
        {
            File.Delete(ruta); File.Delete(rutaDos);
            Invocar("CambiarModo", false);
        }
    }
}
