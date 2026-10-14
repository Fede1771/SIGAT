using System.ComponentModel;
using System.Reflection;
using System.Windows.Forms;
using SIGAT.UI;
using SIGAT.BE;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class DisenadorTests
{
    private sealed class ContextoDiseno : LicenseContext
    {
        public override LicenseUsageMode UsageMode => LicenseUsageMode.Designtime;
    }

    [TestMethod]
    public void TodosLosFormulariosMuestranControlesSinSesionEnModoDiseno()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            LicenseContext anterior = LicenseManager.CurrentContext;
            try
            {
                SIGAT.SERVICIOS.SesionServicio.ObtenerInstancia().CerrarSesion();
                LicenseManager.CurrentContext = new ContextoDiseno();
                Type[] formularios = typeof(FrmPrincipal).Assembly.GetTypes()
                    .Where(t => t.IsSubclassOf(typeof(Form)) && !t.IsAbstract).ToArray();
                Assert.HasCount(9, formularios);
                foreach (Type tipo in formularios)
                {
                    using var formulario = (Form)Activator.CreateInstance(tipo)!;
                    Assert.IsTrue(formulario.Controls.Count > 0, tipo.Name + " debe tener diseño sin ejecutar su lógica.");
                    Assert.IsNotNull(tipo.GetMethod("InitializeComponent", BindingFlags.NonPublic | BindingFlags.Instance));
                    Assert.IsTrue(formulario.Controls.Cast<Control>().All(c => !c.IsDisposed));
                    formulario.PerformLayout();
                    void VerificarFilas(Control control)
                    {
                        if (control is TableLayoutPanel tabla && tipo != typeof(FrmLogin))
                        {
                            foreach (RowStyle fila in tabla.RowStyles)
                                Assert.IsFalse(fila.SizeType == SizeType.Absolute && fila.Height == 20F,
                                    tipo.Name + "/" + tabla.Name + ": las filas de contenido no deben quedar fijadas en 20 píxeles.");
                        }
                        foreach (Control hijo in control.Controls) VerificarFilas(hijo);
                    }
                    VerificarFilas(formulario);
                    Assert.AreEqual(10F, formulario.Font.Size, tipo.Name + " debe conservar la fuente base.");
                    if (tipo != typeof(DlgNombreRol) && tipo != typeof(DlgFamiliaPermisos))
                    {
                        var raiz = formulario.Controls.Cast<Control>().First(c => c is TableLayoutPanel);
                        Assert.AreEqual(DockStyle.Fill, raiz.Dock, tipo.Name + " debe ocupar toda la ventana.");
                        formulario.ClientSize = new System.Drawing.Size(1400, 850);
                        formulario.PerformLayout();
                        Assert.AreEqual(formulario.ClientSize.Width - formulario.Padding.Horizontal,
                            raiz.Width, tipo.Name + " debe adaptarse al ancho disponible.");
                    }
                    if (formulario is FrmPrincipal)
                    {
                        var marca = formulario.Controls.Find("label2", true)[0];
                        var titulo = formulario.Controls.Find("label8", true)[0];
                        Assert.IsTrue(marca.Font.Bold && marca.Font.Size >= 22F, "La marca debe conservar su jerarquía visual.");
                        Assert.IsTrue(titulo.Font.Bold && titulo.Font.Size >= 22F, "El título de inicio no debe perder su fuente.");
                    }
                    if (formulario is FrmGestionIdiomas)
                    {
                        var grilla = (DataGridView)formulario.Controls.Find("dgvTraducciones", true)[0];
                        Assert.HasCount(8, grilla.Columns, "El diseñador debe conservar las columnas de traducciones.");
                        Assert.IsFalse(grilla.Columns["Texto"]!.ReadOnly);
                        Assert.IsTrue(grilla.Columns["TextoBase"]!.ReadOnly);
                        Assert.IsFalse(grilla.AutoGenerateColumns);
                    }
                    if (formulario is FrmGestionUsuarios)
                    {
                        var grilla = (DataGridView)formulario.Controls.Find("dgvUsuarios", true)[0];
                        Assert.IsFalse(grilla.AutoGenerateColumns);
                        Assert.HasCount(6, grilla.Columns);
                        Assert.IsInstanceOfType<DataGridViewTextBoxColumn>(grilla.Columns["Activo"]!);
                        Assert.IsFalse(grilla.Columns.Contains("Password"));
                        Assert.IsFalse(grilla.Columns.Contains("EsAdministradorOriginal"));
                    }
                    if (formulario is FrmBitacora)
                    {
                        var grilla = (DataGridView)formulario.Controls.Find("dgv", true)[0];
                        Assert.IsFalse(grilla.AutoGenerateColumns);
                        Assert.HasCount(6, grilla.Columns);
                        Assert.IsTrue(grilla.Columns.Contains("DigitoVerificador"));
                    }
                    Assert.IsNull(SIGAT.SERVICIOS.SesionServicio.ObtenerInstancia().UsuarioActual);
                }
            }
            catch (Exception ex) { error = ex; }
            finally { LicenseManager.CurrentContext = anterior; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(15)), "El diseño no debe quedar esperando una consulta SQL.");
        if (error != null) throw new AssertFailedException("No se pudo construir un formulario en modo diseño.", error);
    }

    [TestMethod]
    public void EstadoDeUsuarioSeFormateaComoTextoSinErrorDeTipo()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var form = new FrmGestionUsuarios();
                var grilla = (DataGridView)form.Controls.Find("dgvUsuarios", true)[0];
                grilla.BindingContext = new BindingContext();
                grilla.DataError += (_, e) => throw new AssertFailedException("La grilla no debe fallar al formatear un estado.", e.Exception);
                grilla.DataSource = new List<Usuario>
                {
                    new() { NombreUsuario = "activo", Activo = true },
                    new() { NombreUsuario = "inactivo", Activo = false }
                };
                Assert.AreEqual("Activo", grilla.Rows[0].Cells["Activo"].FormattedValue);
                Assert.AreEqual("Inactivo", grilla.Rows[1].Cells["Activo"].FormattedValue);
                Assert.IsFalse(grilla.Columns.Contains("Password"));
                Assert.IsFalse(grilla.Columns.Contains("EsAdministradorOriginal"));
            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        hilo.Join();
        if (error != null) throw new AssertFailedException("Falló el formato de la grilla de usuarios.", error);
    }

    [TestMethod]
    public void DialogoDeFamiliaConservaSoloLasAsignacionesDirectas()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var dialogo = new DlgFamiliaPermisos();
                var hoja = new PermisoSimple { Nombre = "Consulta" };
                var otra = new PermisoSimple { Nombre = "Otra" };
                var familia = new PermisoCompuesto { Nombre = "Familia" };
                familia.Agregar(hoja);
                dialogo.Cargar(familia, new Permiso[] { familia, hoja, otra });
                var lista = (CheckedListBox)dialogo.Controls.Find("lista", true)[0];
                Assert.HasCount(2, lista.Items);
                Assert.AreSame(hoja, dialogo.Seleccionados.Single());
                lista.SetItemChecked(0, false);
                lista.SetItemChecked(1, true);
                Assert.AreSame(otra, dialogo.Seleccionados.Single());
                Assert.AreSame(hoja, familia.ObtenerHijos().Single(), "Abrir y marcar el diálogo no modifica el modelo antes de confirmar.");
                Assert.AreEqual(DialogResult.OK, ((Button)dialogo.AcceptButton!).DialogResult);
                Assert.AreEqual(DialogResult.Cancel, ((Button)dialogo.CancelButton!).DialogResult);
            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        hilo.Join();
        if (error != null) throw new AssertFailedException("No se conservaron las asignaciones del diálogo.", error);
    }
}
