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
    public void MatrizContieneExactamenteLosComponentesSolicitados()
    {
        EstadoRoles estado = MatrizRoles.CrearCatalogo();
        string[] nombres = { "Administrador", "Responsable de Inventario", "Técnico de Mantenimiento", "Mesa de Ayuda", "Auditor" };
        int[][] ids = { new[] {1,3,4,5,6,7}, new[] {3,4,6}, new[] {5,2}, new[] {4,2}, new[] {1,2,6} };
        Assert.AreEqual(5, estado.Roles.Count);
        Assert.AreEqual(7, estado.Permisos.Count);
        int simples = 0;
        foreach (Permiso permiso in estado.Permisos)
        {
            if (permiso is PermisoSimple) simples++;
            Assert.IsEmpty(permiso.ObtenerHijos());
        }
        Assert.AreEqual(2, simples);
        for (int i = 0; i < nombres.Length; i++)
        {
            Rol rol = estado.Roles[i];
            Assert.AreEqual(nombres[i], rol.Nombre);
            Assert.AreEqual(ids[i].Length, rol.Permisos.Count);
            foreach (Permiso permiso in estado.Permisos)
            {
                bool esperado = false;
                foreach (int id in ids[i]) if (permiso.Id == id) esperado = true;
                Assert.AreEqual(esperado, rol.TienePermiso(permiso.Nombre), rol.Nombre + ": " + permiso.Nombre);
            }
        }
        MatrizRoles.ValidarCatalogo(estado);
    }

    [TestMethod]
    public void AdmiteRolesNuevosYProtegeCatalogoBase()
    {
        EstadoRoles estado = MatrizRoles.CrearCatalogo();
        estado.Roles.Add(new Rol { Id = 99, Nombre = "Extra" });
        MatrizRoles.ValidarCatalogo(estado);
        estado = MatrizRoles.CrearCatalogo();
        estado.Roles[0].AgregarPermiso(estado.Permisos[1]);
        Assert.ThrowsExactly<InvalidOperationException>(() => MatrizRoles.ValidarCatalogo(estado));
        estado = MatrizRoles.CrearCatalogo();
        estado.Permisos[2].Agregar(estado.Permisos[1]);
        Assert.ThrowsExactly<InvalidOperationException>(() => MatrizRoles.ValidarCatalogo(estado));
        estado = MatrizRoles.CrearCatalogo();
        estado.Permisos[2] = new PermisoSimple { Id = 3, Nombre = "Gestión de activos" };
        Assert.ThrowsExactly<InvalidOperationException>(() => MatrizRoles.ValidarCatalogo(estado));
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
            throw new InvalidOperationException("Solo se admite una base de pruebas.");
        RolesDAL dal = new RolesDAL(cadena);
        RolesBLL servicio = new RolesBLL(dal);
        EstadoRoles original = dal.Cargar();
        MatrizRoles.ValidarCatalogo(original);
        try
        {
            Usuario admin = Buscar(original, 1);
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            EstadoRoles estado = servicio.CargarParaGestion();
            EstadoRoles desactualizado = dal.Cargar();
            Buscar(estado, 2).AsignarRol(estado.Roles[4]);
            Buscar(estado, 3).AsignarRol(estado.Roles[3]);
            servicio.Guardar(estado);
            Assert.ThrowsExactly<InvalidOperationException>(() => dal.Guardar(desactualizado));
            SesionServicio.ObtenerInstancia().CerrarSesion();
            Usuario ingreso = new Usuario { IdUsuario = 2 };
            servicio.CargarRolesUsuario(ingreso);
            SesionServicio.ObtenerInstancia().IniciarSesion(ingreso);
            Assert.IsTrue(ingreso.TienePermiso("Ver bitácora"));
            Assert.IsTrue(ingreso.TienePermiso("Consultar inventario"));
            Assert.IsTrue(ingreso.TienePermiso("Reportes"));
            Assert.IsFalse(ingreso.TienePermiso(MatrizRoles.Administracion));
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.CargarParaGestion());
            Assert.IsTrue(Buscar(dal.Cargar(), 3).TienePermiso("Movimientos"));
            SesionServicio.ObtenerInstancia().IniciarSesion(admin);
            estado = dal.Cargar();
            Usuario objetivo = Buscar(estado, 2);
            foreach (Rol rol in objetivo.Roles) objetivo.QuitarRol(rol);
            servicio.Guardar(estado);
            servicio.CargarRolesUsuario(ingreso);
            Assert.IsEmpty(ingreso.Roles);
            EstadoRoles sinGestor = dal.Cargar();
            foreach (Rol rol in Buscar(sinGestor, 1).Roles) Buscar(sinGestor, 1).QuitarRol(rol);
            Assert.ThrowsExactly<InvalidOperationException>(() => servicio.Guardar(sinGestor));
            // Un error de FK después del DELETE no debe perder las asignaciones.
            estado = dal.Cargar();
            long version = estado.Version;
            Buscar(estado, 2).AsignarRol(new Rol { Id = 999, Nombre = "Inválido" });
            Assert.ThrowsExactly<SqlException>(() => dal.Guardar(estado));
            Assert.AreEqual(version, dal.Cargar().Version);
            Assert.IsTrue(Buscar(dal.Cargar(), 1).TienePermiso(MatrizRoles.Administracion));
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
