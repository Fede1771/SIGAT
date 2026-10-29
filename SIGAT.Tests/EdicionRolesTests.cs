using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.Tests;

[TestClass]
public class EdicionRolesTests
{
    internal static EstadoRoles Base()
    {
        var e = MatrizRoles.CrearCatalogo();
        e.IdAdministradorOriginal = 1;
        e.Version = 42;
        var admin = new Usuario { IdUsuario = 1, NombreUsuario = "admin", Activo = true };
        admin.AsignarRol(e.Roles[0]); e.Usuarios.Add(admin);
        var lector = new Usuario { IdUsuario = 2, NombreUsuario = "lector", Activo = true };
        lector.AsignarRol(e.Roles[4]); e.Usuarios.Add(lector);
        return e;
    }

    [TestMethod]
    public void CopiaConservaIdentidadCompartidaSinModificarElOriginal()
    {
        var original = Base();
        var f = new PermisoCompuesto { Id = 8, Nombre = "Compartida" };
        f.Agregar(original.Permisos[0]); original.Permisos.Add(f);
        original.Roles[1].AgregarPermiso(f); original.Roles[4].AgregarPermiso(f);
        var ed = new EdicionRoles(original);
        var copia = ed.Estado.Permisos.Single(p => p.Id == 8);
        Assert.AreNotSame(f, copia);
        Assert.AreSame(copia, ed.Estado.Roles[1].Permisos.Single(p => p.Id == 8));
        Assert.AreSame(copia, ed.Estado.Roles[4].Permisos.Single(p => p.Id == 8));
        Assert.AreSame(ed.Estado.Roles[4], ed.Estado.Usuarios[1].Roles[0]);
        ed.AgregarAFamilia(8, new[] { 2 });
        Assert.HasCount(1, f.ObtenerHijos());
        Assert.HasCount(2, ed.Estado.Permisos.Single(p => p.Id == 8).ObtenerHijos());
        CollectionAssert.Contains(ed.Impacto(8).ToList(), "Usuario: lector");
    }

    [TestMethod]
    public void DescartarRestauraCatalogoAsignacionesYVersion()
    {
        var original = Base(); var ed = new EdicionRoles(original);
        int f = ed.CrearFamilia("Nueva");
        ed.AgregarAFamilia(f, new[] { 1, 2 });
        ed.AgregarAlRol(2, new[] { f }, new[] { 5 });
        ed.AsignarRolUsuario(2, 2, true);
        Assert.IsTrue(ed.TieneCambios);
        Assert.HasCount(7, original.Permisos);
        Assert.HasCount(1, original.Usuarios[1].Roles);
        ed.Descartar();
        Assert.IsFalse(ed.TieneCambios);
        Assert.HasCount(7, ed.Estado.Permisos);
        Assert.HasCount(1, ed.Estado.Usuarios[1].Roles);
        Assert.AreEqual(42L, ed.Estado.Version);
    }

    [TestMethod]
    public void FamiliasYRolesAnidadosRechazanCiclosYOperacionesParciales()
    {
        var ed = new EdicionRoles(Base());
        int a = ed.CrearFamilia("A"), b = ed.CrearFamilia("B", a), c = ed.CrearFamilia("C", b);
        int cambios = ed.Cambios.Count;
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.AgregarAFamilia(c, new[] { 1, a }));
        Assert.IsEmpty(ed.Estado.Permisos.Single(p => p.Id == c).ObtenerHijos());
        Assert.AreEqual(cambios, ed.Cambios.Count);
        ed.AgregarAlRol(2, Array.Empty<int>(), new[] { 3 });
        ed.AgregarAlRol(3, Array.Empty<int>(), new[] { 4 });
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.AgregarAlRol(4, new[] { 1 }, new[] { 2 }));
        Assert.IsFalse(ed.Estado.Roles.Single(r => r.Id == 4).Permisos.Any(p => p.Id == 1));
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.AgregarAlRol(2, Array.Empty<int>(), new[] { 2 }));
    }

    [TestMethod]
    public void ProteccionesOriginalesSeValidanAntesDeCambiarElBorrador()
    {
        var ed = new EdicionRoles(Base());
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.AsignarRolUsuario(1, 1, false));
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.AgregarAlRol(1, new[] { 2 }, Array.Empty<int>()));
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.QuitarDelRol(1, 1, false));
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.EliminarFamilia(3));
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.CrearFamilia("Reportes"));
        Assert.IsFalse(ed.TieneCambios);
    }

    [TestMethod]
    public void RevertirElUltimoCambioNoDejaUnaEdicionPendiente()
    {
        var ed = new EdicionRoles(Base());
        ed.AsignarRolUsuario(2, 2, true);
        Assert.IsTrue(ed.TieneCambios);
        ed.AsignarRolUsuario(2, 2, false);
        Assert.IsFalse(ed.TieneCambios);
        int f = ed.CrearFamilia("Temporal");
        ed.EliminarFamilia(f);
        Assert.IsFalse(ed.TieneCambios);
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    [DoNotParallelize]
    public void GuardarConfirmaElBorradorYUnConflictoConservaLaEdicionPendiente()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_ROLES_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        SqlPruebas.Preparar(cadena);
        var dal = new SIGAT.DAL.RolesDAL(cadena);
        var servicio = new RolesBLL(dal);
        var original = dal.Cargar();
        var sesion = SIGAT.SERVICIOS.SesionServicio.ObtenerInstancia();
        var anterior = sesion.UsuarioActual;
        try
        {
            sesion.IniciarSesion(original.Usuarios.Single(u => u.IdUsuario == original.IdAdministradorOriginal));
            var ed = new EdicionRoles(servicio.CargarParaGestion());
            long version = ed.Estado.Version;
            // Usuarios: solo Guardar escribe la asignación. Descartar vuelve a la confirmada.
            int usuario = original.Usuarios.First(u => u.IdUsuario != original.IdAdministradorOriginal && !u.Roles.Any(r => r.Id == 2)).IdUsuario;
            ed.AsignarRolUsuario(usuario, 2, true);
            Assert.IsFalse(dal.Cargar().Usuarios.Single(u => u.IdUsuario == usuario).Roles.Any(r => r.Id == 2));
            ed.Descartar();
            Assert.IsFalse(ed.Estado.Usuarios.Single(u => u.IdUsuario == usuario).Roles.Any(r => r.Id == 2));
            ed.AsignarRolUsuario(usuario, 2, true);
            ed.Guardar(servicio);
            Assert.IsTrue(dal.Cargar().Usuarios.Single(u => u.IdUsuario == usuario).Roles.Any(r => r.Id == 2));
            Assert.IsFalse(ed.TieneCambios);
            ed.AsignarRolUsuario(usuario, 2, false); ed.Descartar();
            Assert.IsTrue(ed.Estado.Usuarios.Single(u => u.IdUsuario == usuario).Roles.Any(r => r.Id == 2));
            // Familias: guardar conserva la jerarquía y descartar no crea registros.
            string nombre = "Borrador " + Guid.NewGuid().ToString("N");
            int id = ed.CrearFamilia(nombre);
            Assert.IsFalse(dal.Cargar().Permisos.Any(p => p.Nombre == nombre));
            ed.Descartar();
            Assert.IsFalse(ed.Estado.Permisos.Any(p => p.Nombre == nombre));
            var obsoleto = new EdicionRoles(servicio.CargarParaGestion());
            id = ed.CrearFamilia(nombre);
            int hija = ed.CrearFamilia(nombre + " hija", id);
            ed.AgregarAFamilia(hija, new[] { 1, 2 });
            ed.Guardar(servicio);
            var recuperado = dal.Cargar();
            Assert.AreEqual(hija, recuperado.Permisos.Single(p => p.Id == id).ObtenerHijos().Single().Id);
            Assert.HasCount(2, recuperado.Permisos.Single(p => p.Id == hija).ObtenerHijos());
            // Roles: relaciones de permisos y de roles incluidos se guardan juntas.
            ed.AgregarAlRol(5, new[] { id }, new[] { 3 });
            Assert.IsFalse(dal.Cargar().Roles.Single(r => r.Id == 5).Roles.Any(r => r.Id == 3));
            ed.Descartar();
            Assert.IsFalse(ed.Estado.Roles.Single(r => r.Id == 5).Roles.Any(r => r.Id == 3));
            ed.AgregarAlRol(5, new[] { id }, new[] { 3 });
            ed.Guardar(servicio);
            Assert.IsFalse(ed.TieneCambios);
            Assert.AreEqual(version + 3, ed.Estado.Version);
            recuperado = dal.Cargar();
            var auditor = recuperado.Roles.Single(r => r.Id == 5);
            Assert.IsTrue(auditor.Permisos.Any(p => p.Id == id));
            Assert.IsTrue(auditor.Roles.Any(r => r.Id == 3));
            Assert.IsTrue(auditor.TienePermiso("Mantenimiento"));
            // Las protecciones también se comprueban al guardar, aunque alguien
            // altere directamente un objeto del borrador y eluda la interfaz.
            var protegido = new EdicionRoles(servicio.CargarParaGestion());
            protegido.CrearFamilia("No persistir " + Guid.NewGuid().ToString("N"));
            var adminOriginal = protegido.Estado.Usuarios.Single(u => u.IdUsuario == original.IdAdministradorOriginal);
            adminOriginal.QuitarRol(protegido.Estado.Roles.Single(r => r.Id == 1));
            Assert.ThrowsExactly<InvalidOperationException>(() => protegido.Guardar(servicio));
            Assert.AreEqual(version + 3, dal.Cargar().Version);
            Assert.IsTrue(dal.Cargar().Usuarios.Single(u => u.IdUsuario == original.IdAdministradorOriginal).Roles.Any(r => r.Id == 1));
            string otro = "Obsoleto " + Guid.NewGuid().ToString("N");
            obsoleto.CrearFamilia(otro);
            Assert.ThrowsExactly<InvalidOperationException>(() => obsoleto.Guardar(servicio));
            Assert.IsTrue(obsoleto.TieneCambios);
            Assert.IsFalse(dal.Cargar().Permisos.Any(p => p.Nombre == otro));
        }
        finally
        {
            original.Version = dal.Cargar().Version;
            dal.Guardar(original);
            if (anterior == null) sesion.CerrarSesion(); else sesion.IniciarSesion(anterior);
        }
    }

    [TestMethod]
    public void QuitarRelacionConservaFamiliaYPermisoHeredadoPorOtraRuta()
    {
        var ed = new EdicionRoles(Base());
        int a = ed.CrearFamilia("A"), b = ed.CrearFamilia("B");
        ed.AgregarAFamilia(a, new[] { 1 }); ed.AgregarAFamilia(b, new[] { 1 });
        ed.AgregarAlRol(2, new[] { a, b }, Array.Empty<int>());
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.QuitarDelRol(2, 1, false));
        ed.QuitarDelRol(2, a, false);
        Assert.IsTrue(ed.Estado.Roles.Single(r => r.Id == 2).TienePermiso("Ver bitácora"));
        Assert.HasCount(1, ed.Estado.Permisos.Single(p => p.Id == a).ObtenerHijos());
        Assert.ThrowsExactly<InvalidOperationException>(() => ed.EliminarFamilia(b));
        ed.EliminarFamilia(a);
        Assert.IsTrue(ed.Estado.Permisos.Any(p => p.Id == 1));
    }
}
