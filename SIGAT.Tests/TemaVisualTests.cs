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
    public void TextoDeshabilitadoEsLegibleEnOscuroSinHabilitarLasAcciones()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var form = new Form();
                var boton = new Button { Text = "Deactivate", Enabled = false, FlatStyle = FlatStyle.Flat,
                    BackColor = Color.White, ForeColor = Color.Black, Size = new Size(150, 40) };
                var casilla = new CheckBox { Text = "Active", Enabled = false, Checked = true,
                    BackColor = Color.White, ForeColor = Color.Black, Size = new Size(130, 30), Top = 50 };
                form.Controls.AddRange(new Control[] { boton, casilla });
                Invocar("CambiarModo", true);
                Invocar("Aplicar", form);
                foreach (Control control in new Control[] { boton, casilla })
                {
                    control.CreateControl();
                    using var bitmap = new Bitmap(control.Width, control.Height);
                    control.DrawToBitmap(bitmap, control.ClientRectangle);
                    int claros = 0;
                    int izquierda = control is CheckBox ? 24 : 5;
                    for (int y = 5; y < bitmap.Height - 5; y++)
                        for (int x = izquierda; x < bitmap.Width - 5; x++)
                        {
                            Color pixel = bitmap.GetPixel(x, y);
                            if (Math.Abs(pixel.R - 173) <= 3 && Math.Abs(pixel.G - 188) <= 3 && Math.Abs(pixel.B - 206) <= 3)
                                claros++;
                        }
                    Assert.IsTrue(claros > 10, "El texto deshabilitado debe dibujarse con el gris claro del tema.");
                    Assert.IsFalse(control.Enabled);
                }
                Assert.IsTrue(casilla.Checked);
                Invocar("CambiarModo", false);
                Invocar("Aplicar", form);
                Assert.AreEqual(Color.Black, boton.ForeColor);
                Assert.AreEqual(Color.Black, casilla.ForeColor);
            }
            catch (Exception ex) { error = ex; }
            finally { Invocar("CambiarModo", false); }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start(); hilo.Join();
        if (error != null) throw error;
    }

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
    public void PantallasRecibenTemaOscuroAntesDeLaCargaYDeMostrarse()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                var admin = new Usuario { IdUsuario = int.MaxValue - 60, NombreUsuario = "tema_sin_destello", Activo = true };
                var catalogo = MatrizRoles.CrearCatalogo();
                admin.AsignarRol(catalogo.Roles[0]);
                catalogo.Usuarios.Add(admin);
                SesionServicio.ObtenerInstancia().IniciarSesion(admin);
                using var principal = new FrmPrincipal();
                QuitarCarga(principal, "FrmPrincipal_Load");
                principal.ShowInTaskbar = false;
                principal.Opacity = 0;
                principal.Show();
                Invocar("CambiarModo", true);
                var abrir = typeof(FrmPrincipal).GetMethod("AbrirFormulario", BindingFlags.Instance | BindingFlags.NonPublic)!;
                int cargas = 0;
                for (int vuelta = 0; vuelta < 3; vuelta++)
                {
                    var usuarios = new FrmGestionUsuarios();
                    QuitarCarga(usuarios, "FrmGestionUsuarios_Load");
                    var roles = new FormGestionRoles();
                    QuitarCarga(roles, "CargarDatos");
                    foreach (Form pantalla in new Form[] { usuarios, roles })
                    {
                        pantalla.Load += (_, _) =>
                        {
                            cargas++;
                            Assert.IsTrue(pantalla.BackColor.GetBrightness() < 0.2, "El fondo debe ser oscuro antes de cargar datos.");
                            if (pantalla is FormGestionRoles)
                            {
                                typeof(FormGestionRoles).GetMethod("InicializarDatos", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(pantalla, new object[] { catalogo });
                                var arbol = (TreeView)pantalla.Controls.Find("tvRolesJerarquia", true)[0];
                                Assert.IsTrue(arbol.BackColor.GetBrightness() < 0.2);
                            }
                        };
                        pantalla.VisibleChanged += (_, _) =>
                        {
                            if (pantalla.Visible) Assert.IsTrue(pantalla.BackColor.GetBrightness() < 0.2, "No debe mostrarse con el fondo claro.");
                        };
                        abrir.Invoke(principal, new object[] { pantalla });
                        Application.DoEvents();
                    }
                    typeof(FrmPrincipal).GetMethod("MostrarInicio", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(principal, null);
                    Assert.IsTrue(principal.Controls.Find("panelContenido", true)[0].Controls[0].BackColor.GetBrightness() < 0.2);
                }
                Assert.AreEqual(6, cargas);
            }
            catch (Exception ex) { error = ex; }
            finally { Invocar("CambiarModo", false); SesionServicio.ObtenerInstancia().CerrarSesion(); }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start(); hilo.Join();
        if (error != null) throw error;
    }

    private static void QuitarCarga(Form form, string metodo)
    {
        var carga = form.GetType().GetMethod(metodo, BindingFlags.Instance | BindingFlags.NonPublic)!;
        form.Load -= (EventHandler)carga.CreateDelegate(typeof(EventHandler), form);
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
