using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BE.Integridad;
using SIGAT.SERVICIOS;
using SIGAT.UI;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class RecuperacionInterfazTests
{
    [TestMethod]
    public void SinSesionYConUsuarioComunSoloSeMuestraElAvisoDeMantenimiento()
    {
        EnInterfaz(() =>
        {
            var sesion = SesionServicio.ObtenerInstancia();
            foreach (bool conUsuario in new[] { false, true })
            {
                sesion.CerrarSesion();
                if (conUsuario) sesion.IniciarSesion(new Usuario { NombreUsuario = "comun", Activo = true });
                using var owner = new Form { ShowInTaskbar = false, Opacity = 0 };
                owner.Show();
                bool mostroAviso = false;
                bool abrioRecuperacion = false;
                string texto = "";
                uint hilo = GetCurrentThreadId();
                using var timer = new System.Windows.Forms.Timer { Interval = 25 };
                timer.Tick += (_, _) =>
                {
                    abrioRecuperacion |= Application.OpenForms.OfType<FrmIntegridad>().Any();
                    EnumThreadWindows(hilo, (ventana, _) =>
                    {
                        var titulo = new StringBuilder(100);
                        GetWindowText(ventana, titulo, titulo.Capacity);
                        if (titulo.ToString() != "Mantenimiento") return true;
                        mostroAviso = true;
                        EnumChildWindows(ventana, (hijo, _) =>
                        {
                            var contenido = new StringBuilder(500);
                            GetWindowText(hijo, contenido, contenido.Capacity);
                            texto += contenido.ToString();
                            return true;
                        }, IntPtr.Zero);
                        PostMessage(ventana, 0x0010, IntPtr.Zero, IntPtr.Zero); // Solo el diálogo del hilo de esta prueba.
                        return true;
                    }, IntPtr.Zero);
                };
                timer.Start();
                var programa = typeof(FrmLogin).Assembly.GetType("SIGAT.UI.Program")!;
                var mostrar = programa.GetMethod("MostrarRecuperacion", BindingFlags.Static | BindingFlags.NonPublic)!;
                var resultado = (DialogResult)mostrar.Invoke(null, new object[] { owner, new InformeIntegridad() })!;
                timer.Stop();
                Assert.IsTrue(mostroAviso);
                StringAssert.Contains(texto, "En mantenimiento, estamos resolviendo el inconveniente");
                Assert.IsFalse(abrioRecuperacion);
                Assert.AreEqual(DialogResult.Cancel, resultado);
                owner.Close();
            }
        });
    }

    [TestMethod]
    public void UnaSesionRestringidaNoPuedeAbrirLaVentanaPrincipal()
    {
        EnInterfaz(() =>
        {
            SesionServicio.ObtenerInstancia().IniciarSesionRecuperacion(new Usuario { Activo = true });
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => new FrmPrincipal());
        });
    }

    private static void EnInterfaz(Action prueba)
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try { prueba(); }
            catch (Exception ex) { error = ex; }
            finally { SesionServicio.ObtenerInstancia().CerrarSesion(); }
        }) { IsBackground = true };
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(15)), "La interfaz debe responder sin abrir la recuperación.");
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
    }

    private delegate bool VentanaCallback(IntPtr ventana, IntPtr parametro);
    [DllImport("kernel32.dll")] private static extern uint GetCurrentThreadId();
    [DllImport("user32.dll")] private static extern bool EnumThreadWindows(uint hilo, VentanaCallback callback, IntPtr parametro);
    [DllImport("user32.dll")] private static extern bool EnumChildWindows(IntPtr ventana, VentanaCallback callback, IntPtr parametro);
    [DllImport("user32.dll", CharSet = CharSet.Unicode)] private static extern int GetWindowText(IntPtr ventana, StringBuilder texto, int maximo);
    [DllImport("user32.dll")] private static extern bool PostMessage(IntPtr ventana, uint mensaje, IntPtr wParam, IntPtr lParam);
}
