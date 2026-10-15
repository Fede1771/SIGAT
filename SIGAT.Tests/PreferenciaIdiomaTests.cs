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
    public void PreferenciaConcurrenteConDesactivacionNoDejaUnIdiomaInactivoEnUso()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_IDIOMA_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere base de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        var dal = new IdiomaDAL(cadena);
        int es = dal.ObtenerIdIdiomaPorCodigo("es");
        int? anterior = dal.ObtenerIdiomaPreferido(3);
        try
        {
            for (int intento = 0; intento < 5; intento++)
            {
                int id = dal.InsertarIdioma(new SIGAT.BE.Idiomas.Idioma
                {
                    Nombre = "Concurrencia " + Guid.NewGuid().ToString("N"),
                    Codigo = "c" + Guid.NewGuid().ToString("N")[..8], Activo = true
                });
                dal.GuardarIdiomaPreferido(3, es);
                using var salida = new Barrier(2);
                Exception? Elegir()
                {
                    salida.SignalAndWait();
                    try { new IdiomaDAL(cadena).GuardarIdiomaPreferido(3, id); return null; }
                    catch (SqlException ex) { return ex; }
                }
                Exception? Desactivar()
                {
                    salida.SignalAndWait();
                    try { new IdiomaDAL(cadena).CambiarEstadoIdioma(id, false); return null; }
                    catch (InvalidOperationException ex) { return ex; }
                }
                var elegir = Task.Run(Elegir);
                var desactivar = Task.Run(Desactivar);
                Assert.IsTrue(Task.WaitAll(new Task[] { elegir, desactivar }, TimeSpan.FromSeconds(30)));
                Assert.IsTrue(elegir.Result == null || desactivar.Result == null, "Una operación debe poder confirmarse.");
                Assert.IsTrue(dal.ObtenerIdiomaPorId(id).Activo || dal.ObtenerIdiomaPreferido(3) != id,
                    "La transacción debe impedir una preferencia hacia un idioma inactivo.");
            }
        }
        finally { dal.GuardarIdiomaPreferido(3, anterior ?? es); }
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void PreferenciasPersistenPorCuentaSinHeredarseEntreSesiones()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_IDIOMA_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Configurar una base exclusiva de pruebas en SIGAT_IDIOMA_TEST_CONNECTION."); return; }
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        SqlPruebas.Preparar(cadena);
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

            Assert.IsTrue(dal.TieneUsuariosAsignados(en));
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.CambiarEstadoIdioma(en, false));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.CambiarEstadoIdioma(en, false));
            Assert.IsTrue(dal.ObtenerIdiomaPorId(en).Activo);
            // No importa que la cuenta esté inactiva: la preferencia sigue perteneciendo a un usuario registrado.
            var usuarios = new UsuarioDAL(cadena);
            var cuenta = usuarios.ObtenerTodos().Single(u => u.IdUsuario == 2);
            cuenta.Activo = false; cuenta.Password = ""; usuarios.Actualizar(cuenta);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.CambiarEstadoIdioma(pt, false));
            cuenta.Activo = true; usuarios.Actualizar(cuenta);
            var vacio = servicio.CrearIdioma("Prueba desactivación", "zz-test", "Prueba", true);
            servicio.CambiarIdioma(vacio);
            // Simulamos una sesión ya cargada cuya preferencia se cambió en otro lugar: la BLL también comprueba IdiomaActual.
            dal.GuardarIdiomaPreferido(3, es);
            Assert.IsFalse(dal.TieneUsuariosAsignados(vacio));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.CambiarEstadoIdioma(vacio, false));
            servicio.CambiarIdioma(es);
            servicio.CambiarEstadoIdioma(vacio, false);
            Assert.IsFalse(dal.ObtenerIdiomaPorId(vacio).Activo);
            servicio.CambiarEstadoIdioma(vacio, true);
            Assert.IsTrue(dal.ObtenerIdiomaPorId(vacio).Activo);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.CambiarEstadoIdioma(es, false));
            Assert.ThrowsExactly<SqlException>(() => dal.GuardarIdiomaPreferido(4, pt));
            Assert.IsNull(dal.ObtenerIdiomaPreferido(4));
        }
        finally { dal.CambiarEstadoIdioma(en, true); }
    }
}
