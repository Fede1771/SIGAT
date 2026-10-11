using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class PreferenciaIdiomaTests
{
    [TestCleanup]
    public void LimpiarSesion() => SesionServicio.ObtenerInstancia().CerrarSesion();

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void PreferenciasPersistenPorCuentaSinHeredarseEntreSesiones()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_IDIOMA_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Configurar una base exclusiva de pruebas en SIGAT_IDIOMA_TEST_CONNECTION."); return; }
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        var dal = new IdiomaDAL(cadena);
        var servicio = new IdiomaBLL(dal);
        int es = dal.ObtenerIdIdiomaPorCodigo("es"), en = dal.ObtenerIdIdiomaPorCodigo("en"), pt = dal.ObtenerIdIdiomaPorCodigo("pt");
        var sesion = SesionServicio.ObtenerInstancia();
        var manager = IdiomaManager.ObtenerInstancia();
        void Entrar(int id, string nombre, IdiomaBLL bll)
        {
            sesion.CerrarSesion();
            Assert.IsNull(manager.IdiomaActual);
            sesion.IniciarSesion(new Usuario { IdUsuario = id, NombreUsuario = nombre });
            bll.RestablecerIdiomaUsuario();
        }
        try
        {
            Entrar(1, "admin", servicio);
            Assert.AreEqual(es, manager.IdiomaActual!.Id);
            Assert.IsNull(dal.ObtenerIdiomaPreferido(1));
            servicio.CambiarIdioma(en);
            Assert.AreEqual(en, dal.ObtenerIdiomaPreferido(1));
            Entrar(2, "valen1", servicio);
            Assert.AreEqual(es, manager.IdiomaActual!.Id, "Otra cuenta no debe heredar el inglés de admin.");
            servicio.CambiarIdioma(pt);
            Assert.AreEqual(pt, dal.ObtenerIdiomaPreferido(2));

            // Nuevos servicios y conexiones recuperan las preferencias guardadas en SQL.
            var reabierto = new IdiomaBLL(new IdiomaDAL(cadena));
            Entrar(1, "admin_renombrado", reabierto);
            Assert.AreEqual(en, manager.IdiomaActual!.Id, "El idioma pertenece al ID, aunque cambie el nombre.");
            Assert.AreEqual("System", manager.Traducir("FrmPrincipal", "menu_sistema", "Sistema"));
            Entrar(2, "valen1", reabierto);
            Assert.AreEqual(pt, manager.IdiomaActual!.Id);
            Assert.AreEqual(en, dal.ObtenerIdiomaPreferido(1));
            Entrar(3, "nuevo", reabierto);
            Assert.AreEqual(es, manager.IdiomaActual!.Id);

            dal.CambiarEstadoIdioma(en, false);
            Entrar(1, "admin", reabierto);
            Assert.AreEqual(es, manager.IdiomaActual!.Id);
            Assert.AreEqual(en, dal.ObtenerIdiomaPreferido(1), "El respaldo no debe borrar una preferencia inactiva.");
            Assert.ThrowsExactly<InvalidOperationException>(() => reabierto.CambiarIdioma(en));
            Assert.AreEqual(es, manager.IdiomaActual!.Id);
            dal.CambiarEstadoIdioma(en, true);
            reabierto.RestablecerIdiomaUsuario();
            Assert.AreEqual(en, manager.IdiomaActual!.Id);
            Assert.ThrowsExactly<SqlException>(() => dal.GuardarIdiomaPreferido(4, pt));
            Assert.IsNull(dal.ObtenerIdiomaPreferido(4));
        }
        finally { dal.CambiarEstadoIdioma(en, true); }
    }
}
