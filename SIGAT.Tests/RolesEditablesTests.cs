using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using Microsoft.Data.SqlClient;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class RolesEditablesTests
{
    [TestMethod]
    public void HerenciaTransitivaSinCiclosYAdministradorProtegido()
    {
        var estado = MatrizRoles.CrearCatalogo();
        var padre = new Rol { Id = 6, Nombre = "Supervisor" };
        var hijo = new Rol { Id = 7, Nombre = "Consulta" };
        hijo.AgregarRol(estado.Roles[4]);
        padre.AgregarRol(hijo);
        padre.AgregarRol(hijo);
        Assert.HasCount(1, padre.Roles);
        Assert.IsTrue(padre.TienePermiso(MatrizRoles.Bitacora));
        Assert.ThrowsExactly<InvalidOperationException>(() => hijo.AgregarRol(padre));
        Assert.ThrowsExactly<InvalidOperationException>(() => padre.AgregarRol(padre));
        estado.Roles.AddRange(new[] { padre, hijo });
        MatrizRoles.ValidarCatalogo(estado);
        padre.QuitarRol(hijo);
        Assert.IsFalse(padre.TienePermiso(MatrizRoles.Bitacora));
        estado.IdAdministradorOriginal = 1;
        var original = new Usuario { IdUsuario = 1, Activo = true };
        original.AsignarRol(estado.Roles[0]);
        var otro = new Usuario { IdUsuario = 2, Activo = true };
        otro.AsignarRol(estado.Roles[0]);
        estado.Usuarios.AddRange(new[] { original, otro });
        RolesBLL.Validar(estado);
        original.QuitarRol(estado.Roles[0]);
        Assert.ThrowsExactly<InvalidOperationException>(() => RolesBLL.Validar(estado));
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void RolesAnidadosPersistenYAdminOriginalNoSeDesactivaConOtroAdmin()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere base de pruebas."); return; }
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_"))
            throw new InvalidOperationException("Base de pruebas requerida.");
        var dal = new RolesDAL(cadena);
        var original = dal.Cargar();
        try
        {
            var estado = dal.Cargar();
            var nuevo = new Rol { Id = estado.Roles.Max(r => r.Id) + 1, Nombre = "Prueba anidado" };
            nuevo.AgregarRol(estado.Roles.Find(r => r.Id == 5)!);
            estado.Roles.Add(nuevo);
            var otro = estado.Usuarios.First(u => u.IdUsuario != estado.IdAdministradorOriginal);
            otro.AsignarRol(estado.Roles.Find(r => r.Id == 1)!);
            otro.AsignarRol(nuevo);
            dal.Guardar(estado);
            var recargado = dal.Cargar();
            Assert.IsTrue(recargado.Roles.Find(r => r.Id == nuevo.Id)!.TienePermiso(MatrizRoles.Bitacora));
            var usuarios = new UsuarioDAL(cadena);
            Assert.ThrowsExactly<InvalidOperationException>(() => usuarios.Eliminar(estado.IdAdministradorOriginal));
            var admin = usuarios.ObtenerTodos().Find(u => u.IdUsuario == estado.IdAdministradorOriginal)!;
            admin.Activo = false;
            Assert.ThrowsExactly<InvalidOperationException>(() => usuarios.Actualizar(admin));
            using var conexion = new SqlConnection(cadena);
            conexion.Open();
            using var comando = new SqlCommand("UPDATE dbo.Usuarios SET Activo=0 WHERE IdUsuario=@Id", conexion);
            comando.Parameters.AddWithValue("@Id", estado.IdAdministradorOriginal);
            Assert.ThrowsExactly<SqlException>(() => comando.ExecuteNonQuery());
        }
        finally
        {
            original.Version = dal.Cargar().Version;
            dal.Guardar(original);
        }
    }
}
