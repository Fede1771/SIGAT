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
    public void FamiliasPersonalizadasAdmitenVariosNivelesYRechazanNombresInvalidos()
    {
        var estado = MatrizRoles.CrearCatalogo();
        var raiz = new PermisoCompuesto { Id = 20, Nombre = "Familia raíz" };
        var rama = new PermisoCompuesto { Id = 21, Nombre = "Familia intermedia" };
        var hoja = new PermisoCompuesto { Id = 22, Nombre = "Familia final" };
        raiz.Agregar(rama); rama.Agregar(hoja); hoja.Agregar(estado.Permisos[1]);
        estado.Permisos.AddRange(new[] { raiz, rama, hoja });
        MatrizRoles.ValidarCatalogo(estado);
        Assert.IsTrue(raiz.ContienePermiso("Consultar inventario"));
        Assert.ThrowsExactly<InvalidOperationException>(() => hoja.Agregar(raiz));
        foreach (string nombre in new[] { "", "  ", "Reportes", "familia raíz", new string('x', 151) })
        {
            hoja.Nombre = nombre;
            Assert.ThrowsExactly<InvalidOperationException>(() => MatrizRoles.ValidarCatalogo(estado));
        }
        hoja.Nombre = "Familia final";
        estado.Permisos.Add(new PermisoSimple { Id = 23, Nombre = "Simple no permitido" });
        Assert.ThrowsExactly<InvalidOperationException>(() => MatrizRoles.ValidarCatalogo(estado));
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void FamiliasPersonalizadasPersistenSeRenombranYSeEliminan()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere base de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        var dal = new RolesDAL(cadena);
        var original = dal.Cargar();
        try
        {
            var estado = dal.Cargar();
            int id = estado.Permisos.Max(p => p.Id) + 1;
            var raiz = new PermisoCompuesto { Id = id, Nombre = "Familia SQL raíz" };
            var rama = new PermisoCompuesto { Id = id + 1, Nombre = "Familia SQL rama" };
            var final = new PermisoCompuesto { Id = id + 2, Nombre = "Familia SQL final" };
            raiz.Agregar(rama); rama.Agregar(final); final.Agregar(estado.Permisos.Single(p => p.Id == 2));
            estado.Permisos.AddRange(new[] { raiz, rama, final });
            estado.Roles.Single(r => r.Id == 5).AgregarPermiso(raiz);
            MatrizRoles.ValidarCatalogo(estado);
            dal.Guardar(estado);
            estado = dal.Cargar();
            var recuperada = estado.Permisos.Single(p => p.Id == id);
            Assert.AreSame(estado.Permisos.Single(p => p.Id == id + 1), recuperada.ObtenerHijos().Single());
            Assert.AreEqual(2, recuperada.ObtenerHijos().Single().ObtenerHijos().Single().ObtenerHijos().Single().Id);
            Assert.IsTrue(estado.Roles.Single(r => r.Id == 5).Permisos.Contains(recuperada));
            recuperada.Nombre = "Familia SQL renombrada";
            dal.Guardar(estado);
            estado = dal.Cargar();
            Assert.AreEqual("Familia SQL renombrada", estado.Permisos.Single(p => p.Id == id).Nombre);
            estado.Roles.Single(r => r.Id == 5).QuitarPermiso(estado.Permisos.Single(p => p.Id == id));
            estado.Permisos.RemoveAll(p => p.Id >= id);
            dal.Guardar(estado);
            Assert.IsFalse(dal.Cargar().Permisos.Any(p => p.Id >= id));
            Assert.IsTrue(new IntegridadDAL(cadena).Verificar().Correcta);
        }
        finally
        {
            original.Version = dal.Cargar().Version;
            dal.Guardar(original);
        }
    }

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
        SqlPruebas.Preparar(cadena);
        var dal = new RolesDAL(cadena);
        var original = dal.Cargar();
        try
        {
            var estado = dal.Cargar();
            var familia = estado.Permisos.Find(p => p.Id == 6)!;
            familia.Agregar(estado.Permisos.Find(p => p.Id == 2)!);
            var nuevo = new Rol { Id = estado.Roles.Max(r => r.Id) + 1, Nombre = "Prueba anidado" };
            nuevo.AgregarRol(estado.Roles.Find(r => r.Id == 5)!);
            estado.Roles.Add(nuevo);
            var otro = estado.Usuarios.First(u => u.IdUsuario != estado.IdAdministradorOriginal);
            otro.AsignarRol(estado.Roles.Find(r => r.Id == 1)!);
            otro.AsignarRol(nuevo);
            dal.Guardar(estado);
            var recargado = dal.Cargar();
            Assert.IsTrue(recargado.Permisos.Find(p => p.Id == 6)!.ContienePermiso("Consultar inventario"));
            Assert.HasCount(1, recargado.Permisos.Find(p => p.Id == 6)!.ObtenerHijos());
            var familiaRecargada = recargado.Permisos.Find(p => p.Id == 6)!;
            familiaRecargada.Quitar(familiaRecargada.ObtenerHijos()[0]);
            dal.Guardar(recargado);
            Assert.IsEmpty(dal.Cargar().Permisos.Find(p => p.Id == 6)!.ObtenerHijos());
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
