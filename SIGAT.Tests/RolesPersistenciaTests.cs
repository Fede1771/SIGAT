using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using Microsoft.Data.SqlClient;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class RolesPersistenciaTests
{
    [TestMethod]
    public void FamiliaConNombreDePermisoNoAutorizaYNoSePuedeQuitarUltimoGestor()
    {
        EstadoRoles estado = new EstadoRoles();
        Usuario usuario = new Usuario { Activo = true };
        Rol rol = new Rol { Nombre = "Gestión de Roles" };
        usuario.AsignarRol(rol);
        estado.Usuarios.Add(usuario);
        estado.Roles.Add(rol);
        Assert.IsFalse(usuario.TienePatente("Gestión de Roles"));
        Assert.ThrowsExactly<InvalidOperationException>(() => RolesBLL.Validar(estado));
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void AsignacionesPersistenTrasNuevaSesionYConflictosNoSobrescriben()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena))
        {
            Assert.Inconclusive("Configurar SIGAT_ROLES_TEST_CONNECTION con una base exclusiva de pruebas.");
            return;
        }
        var builder = new SqlConnectionStringBuilder(cadena);
        if (!builder.InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Esta prueba solo admite una base SIGAT_Roles_Pruebas_.");
        RolesDAL dal = new RolesDAL(cadena);
        RolesBLL servicio = new RolesBLL(dal);
        EstadoRoles original = dal.Cargar();
        try
        {
            Usuario admin = Buscar(original, 1);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            EstadoRoles estado = servicio.CargarParaGestion();
            EstadoRoles desactualizado = dal.Cargar();
            Rol nuevo = new Rol(new PermisoCompuesto { Id = 101, Nombre = "Supervisor de prueba" }) { Id = 100 };
            nuevo.AgregarPermiso(estado.Permisos[4]);
            estado.Roles.Add(nuevo);
            estado.Permisos.Add(nuevo.Familia);
            Buscar(estado, 2).AsignarRol(nuevo);
            Rol personal = new Rol(new PermisoCompuesto { Id = 103, Nombre = "Asignaciones directas" })
                { Id = 104, IdUsuarioPersonal = 2 };
            personal.AgregarPermiso(nuevo.Familia);
            personal.AgregarPermiso(estado.Permisos[1]);
            estado.Roles.Add(personal);
            estado.Permisos.Add(personal.Familia);
            Buscar(estado, 2).AsignarRol(personal);
            servicio.Guardar(estado);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.Guardar(desactualizado));

            SesionServicio.ObtenerInstancia().CerrarSesion();
            Usuario ingreso = new Usuario { IdUsuario = 2 };
            servicio.CargarRolesUsuario(ingreso);
            SesionServicio.ObtenerInstancia().IniciarSesion(ingreso);
            Assert.IsTrue(ingreso.TienePatente("Gestión de Usuarios"));
            Assert.IsTrue(ingreso.TienePatente("Control de Cambios"));
            Rol? recuperado = null;
            Rol? personalRecuperado = null;
            foreach (Rol rol in ingreso.Roles)
            {
                if (rol.Id == 100) recuperado = rol;
                if (rol.IdUsuarioPersonal == 2) personalRecuperado = rol;
            }
            Assert.IsNotNull(recuperado);
            Assert.IsNotNull(personalRecuperado);
            bool comparteFamilia = false;
            foreach (Permiso permiso in personalRecuperado.Permisos)
                if (ReferenceEquals(permiso, recuperado.Familia)) comparteFamilia = true;
            Assert.IsTrue(comparteFamilia, "Los árboles compartidos deben conservar la identidad al reconstruirse.");
            Assert.IsFalse(ingreso.TienePatente("Gestión de Roles"));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.CargarParaGestion());
            Assert.IsTrue(Buscar(dal.Cargar(), 3).Roles.Count > 0, "Las cuentas inactivas conservan asignaciones.");

            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            estado = dal.Cargar();
            Usuario objetivo = Buscar(estado, 2);
            foreach (Rol rol in objetivo.Roles) objetivo.QuitarRol(rol);
            servicio.Guardar(estado);
            servicio.CargarRolesUsuario(ingreso);
            Assert.IsEmpty(ingreso.Roles, "El perfil no debe reponer roles quitados.");
            EstadoRoles sinGestor = dal.Cargar();
            foreach (Rol rol in Buscar(sinGestor, 1).Roles) Buscar(sinGestor, 1).QuitarRol(rol);
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.Guardar(sinGestor));

            // Fuerza un error SQL después del DELETE: la transacción debe restaurar todo.
            estado = dal.Cargar();
            long version = estado.Version;
            estado.Permisos.Add(new PermisoSimple { Id = estado.Permisos[0].Id, Nombre = "Duplicado" });
            Assert.ThrowsExactly<SqlException>(() => dal.Guardar(estado));
            Assert.AreEqual(version, dal.Cargar().Version);
            Assert.IsTrue(Buscar(dal.Cargar(), 1).TienePatente("Gestión de Roles"));
        }
        finally
        {
            original.Version = dal.Cargar().Version;
            dal.Guardar(original);
            SesionServicio.ObtenerInstancia().CerrarSesion();
        }
    }

    private static Usuario Buscar(EstadoRoles estado, int id)
    {
        foreach (Usuario usuario in estado.Usuarios)
            if (usuario.IdUsuario == id) return usuario;
        throw new InvalidOperationException("Falta el usuario de prueba.");
    }
}
