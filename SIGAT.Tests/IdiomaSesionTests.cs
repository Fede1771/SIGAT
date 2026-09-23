using SIGAT.BE;
using SIGAT.BE.Idiomas;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;
using SIGAT.UI;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class IdiomaSesionTests
{
    [TestCleanup]
    public void Limpiar() => SesionServicio.ObtenerInstancia().CerrarSesion();

    [TestMethod]
    public void NuevaSesionNoHeredaIdiomaTraduccionesNiObservadores()
    {
        var sesion = SesionServicio.ObtenerInstancia();
        var manager = IdiomaManager.ObtenerInstancia();
        sesion.IniciarSesion(new Usuario());
        var observador = new Observador();
        manager.Suscribir(observador);
        manager.CargarIdioma(new Idioma { Id = 2, Codigo = "en" },
            new Dictionary<string, string> { ["Form.boton"] = "Save" });
        Assert.AreEqual("Save", manager.Traducir("Form", "boton", "Guardar"));
        Assert.AreEqual(2, IdiomaManager.ObtenerInstancia().IdiomaActual!.Id);
        sesion.CerrarSesion();
        Assert.IsNull(manager.IdiomaActual);
        Assert.AreEqual("Guardar", manager.Traducir("Form", "boton", "Guardar"));
        sesion.IniciarSesion(new Usuario());
        manager.CargarIdioma(new Idioma { Id = 1, Codigo = "es" }, new());
        Assert.AreEqual(1, observador.Notificaciones);
        // Incluso una sustitución directa del usuario inicia un estado limpio.
        sesion.IniciarSesion(new Usuario());
        Assert.IsNull(manager.IdiomaActual);
    }

    [TestMethod]
    [DataRow("es", "Español *", "Español")]
    [DataRow("en", "English .", "English")]
    [DataRow("pt-BR", "Português ()", "Português")]
    [DataRow("ru", "Русский -", "Русский")]
    public void EtiquetaSoloMarcaElIdiomaActivo(string codigo, string activa, string inactiva)
    {
        var idioma = new Idioma { Id = 1, Codigo = codigo, Nombre = "Nombre" };
        Assert.AreEqual(activa, PresentacionIdioma.Etiqueta(idioma, 1));
        Assert.AreEqual(inactiva, PresentacionIdioma.Etiqueta(idioma, 2));
        Assert.AreEqual(inactiva, PresentacionIdioma.Etiqueta(idioma, null));
        Assert.AreEqual("Nombre", idioma.Nombre);
        Assert.AreEqual(codigo, idioma.Codigo);
    }

    private sealed class Observador : IIdiomaObserver
    {
        public int Notificaciones { get; private set; }
        public void ActualizarIdioma() => Notificaciones++;
    }
}
