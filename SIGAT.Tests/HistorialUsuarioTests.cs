using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using SIGAT.UI;
using System.Windows.Forms;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class HistorialUsuarioTests
{
    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void VersionaCuentasYRolesYRestauraSinCambiarLaClave()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_HISTORIAL_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        string? baseAnterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        var sesion = SesionServicio.ObtenerInstancia();
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", new SqlConnectionStringBuilder(cadena).InitialCatalog);
            var servicio = new UsuarioBLL();
            var dal = new UsuarioDAL(cadena);
            var roles = new RolesBLL(new RolesDAL(cadena));
            var administrador = new RolesDAL(cadena).Cargar().Usuarios.Single(cuenta => cuenta.IdUsuario == 1);
            sesion.IniciarSesion(administrador);
            Assert.AreEqual(OperacionUsuarioVersion.EstadoInicial, servicio.ConsultarHistorial(1).Versiones.Last().Operacion);

            var usuario = new Usuario { NombreUsuario = "hist_" + Guid.NewGuid().ToString("N"), Nombre = "Ana", Apellido = "Pérez", Activo = true };
            servicio.CrearUsuario(usuario, "ClaveInicialDePrueba");
            var edicion = new EdicionRoles(roles.CargarParaGestion());
            edicion.AsignarRolUsuario(usuario.IdUsuario, 5, true);
            edicion.Guardar(roles);
            usuario.Apellido = "Gómez";
            servicio.ActualizarUsuario(usuario, "ClaveNuevaDePrueba");
            servicio.ActualizarUsuario(usuario, "ClaveNuevaDePrueba"); // La misma clave y los mismos datos no generan otra versión.
            servicio.BajaLogicaUsuario(usuario.IdUsuario, usuario.NombreUsuario);

            var historial = servicio.ConsultarHistorial(usuario.IdUsuario);
            CollectionAssert.AreEqual(new[] { OperacionUsuarioVersion.Baja, OperacionUsuarioVersion.Edicion,
                OperacionUsuarioVersion.Roles, OperacionUsuarioVersion.Alta }, historial.Versiones.Select(v => v.Operacion).ToArray());
            Assert.IsTrue(historial.Versiones.All(v => v.Autor.IdUsuario == 1 && v.FechaUtc.Kind == DateTimeKind.Utc));
            Assert.IsTrue(historial.Versiones[1].CambioClave);
            Assert.AreEqual("Pérez", historial.Versiones[2].Estado.Apellido);
            Assert.AreEqual(5, historial.Versiones[2].Estado.Roles.Single().Id);
            ComprobarDialogo(servicio, usuario.IdUsuario);

            sesion.IniciarSesion(usuario);
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.ConsultarHistorial(usuario.IdUsuario));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.RestaurarVersion(usuario.IdUsuario, 2, historial.VersionSeguridad));
            sesion.IniciarSesion(administrador);
            var borrador = new EdicionRoles(roles.CargarParaGestion());
            borrador.AsignarRolUsuario(usuario.IdUsuario, 5, false);
            servicio.RestaurarVersion(usuario.IdUsuario, 2, historial.VersionSeguridad);
            Assert.ThrowsExactly<InvalidOperationException>(() => borrador.Guardar(roles));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.RestaurarVersion(usuario.IdUsuario, 1, historial.VersionSeguridad));

            var restaurado = servicio.ConsultarHistorial(usuario.IdUsuario);
            Assert.AreEqual(OperacionUsuarioVersion.Restauracion, restaurado.Versiones[0].Operacion);
            Assert.AreEqual(2, restaurado.Versiones[0].VersionOrigen);
            Assert.IsTrue(restaurado.EstadoActual!.Activo);
            Assert.AreEqual("Pérez", restaurado.EstadoActual.Apellido);
            Assert.AreEqual(5, restaurado.EstadoActual.Roles.Single().Id);
            Assert.AreEqual(HashHelper.ObtenerHashSHA256("ClaveNuevaDePrueba"), dal.ObtenerPorNombreUsuario(usuario.NombreUsuario)!.Password);
            Assert.IsTrue(new IntegridadDAL(cadena).Verificar().Correcta);

            using var conexion = new SqlConnection(cadena);
            conexion.Open();
            using var comando = new SqlCommand("""
                CREATE TRIGGER dbo.TR_PruebaFallaHistorial ON dbo.UsuarioVersion AFTER INSERT AS
                THROW 50001, 'Fallo de historial para la prueba.', 1;
                """, conexion);
            comando.ExecuteNonQuery();
            try
            {
                usuario.Apellido = "No persistir";
                Assert.ThrowsExactly<SqlException>(() => servicio.ActualizarUsuario(usuario, ""));
            }
            finally
            {
                comando.CommandText = "DROP TRIGGER dbo.TR_PruebaFallaHistorial";
                comando.ExecuteNonQuery();
            }
            Assert.AreEqual("Pérez", dal.ObtenerPorNombreUsuario(usuario.NombreUsuario)!.Apellido);
            Assert.AreEqual(restaurado.VersionSeguridad, servicio.ConsultarHistorial(usuario.IdUsuario).VersionSeguridad);
        }
        finally
        {
            sesion.CerrarSesion();
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", baseAnterior);
        }
    }

    private static void ComprobarDialogo(UsuarioBLL servicio, int idUsuario)
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var dialogo = new DlgHistorialUsuario();
                dialogo.Configurar(servicio, idUsuario, false);
                dialogo.Shown += (_, _) =>
                {
                    try
                    {
                        var versiones = (DataGridView)dialogo.Controls.Find("dgvVersiones", true).Single();
                        var detalle = (DataGridView)dialogo.Controls.Find("dgvDetalle", true).Single();
                        var restaurar = (Button)dialogo.Controls.Find("btnRestaurar", true).Single();
                        Assert.AreEqual(4, versiones.Rows.Count);
                        Assert.AreEqual(4, versiones.Rows[0].Cells["Version"].Value);
                        Assert.IsFalse(restaurar.Enabled);
                        versiones.CurrentCell = versiones.Rows[2].Cells["Version"];
                        Assert.AreEqual("Pérez", detalle.Rows[2].Cells["Seleccionada"].Value);
                        Assert.AreEqual("Gómez", detalle.Rows[2].Cells["Vigente"].Value);
                        Assert.IsTrue(restaurar.Enabled);
                        Assert.IsFalse(string.IsNullOrEmpty(detalle.Rows[4].Cells["Seleccionada"].ToolTipText),
                            "El cambio de roles directos debe identificarse sin modificar sus datos.");
                        var idiomas = SIGAT.SERVICIOS.Idiomas.IdiomaManager.ObtenerInstancia();
                        idiomas.CargarIdioma(new SIGAT.BE.Idiomas.Idioma { Id = 1, Nombre = "Prueba", Codigo = "es" },
                            new Dictionary<string, string>
                            {
                                [dialogo.Name + ".ayudaRestaurar"] = "Restoring creates a new version."
                            });
                        Assert.AreEqual(2, versiones.CurrentRow!.Cells["Version"].Value,
                            "Cambiar el idioma debe conservar la versión seleccionada.");
                        Assert.AreEqual("Restoring creates a new version.", dialogo.Controls.Find("lblRestauracion", true).Single().Text);
                        Assert.IsTrue(restaurar.Enabled);
                    }
                    catch (Exception ex) { error = ex; }
                    finally { dialogo.Close(); }
                };
                dialogo.ShowDialog();
            }
            catch (Exception ex) { error = ex; }
        }) { IsBackground = true };
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(20)), "El diálogo debe cargar el historial y responder.");
        if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
    }
}
