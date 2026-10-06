using SIGAT.BE;

namespace SIGAT.Tests;

[TestClass]
public class CompositeTests
{
    [TestMethod]
    public void RechazaCiclosDirectosEIndirectos()
    {
        var a = new PermisoCompuesto();
        var b = new PermisoCompuesto();
        var c = new PermisoCompuesto();
        a.Agregar(b);
        b.Agregar(c);
        Assert.ThrowsExactly<InvalidOperationException>(() => a.Agregar(a));
        Assert.ThrowsExactly<InvalidOperationException>(() => c.Agregar(a));
        Assert.AreEqual(0, c.ObtenerHijos().Count);
    }

    [TestMethod]
    public void HojaRechazaHijosYListasNoPermitenSaltarValidacion()
    {
        var hoja = new PermisoSimple { Nombre = "Bitácora" };
        var familia = new PermisoCompuesto();
        Assert.ThrowsExactly<InvalidOperationException>(() => hoja.Agregar(familia));
        Assert.ThrowsExactly<InvalidOperationException>(() => hoja.Quitar(familia));
        familia.ObtenerHijos().Add(familia);
        Assert.AreEqual(0, familia.ObtenerHijos().Count);
    }

    [TestMethod]
    public void PermisosCompartidosSeActualizanSinDuplicarAsignaciones()
    {
        var gestion = new Rol { Nombre = "Gestión" };
        var admin = new Rol { Nombre = "Administrador" };
        var usuario = new Usuario();
        admin.AgregarPermiso(gestion.Familia);
        usuario.AsignarRol(admin);
        usuario.AsignarRol(admin);
        var patente = new PermisoSimple { Nombre = "Bitácora" };
        gestion.AgregarPermiso(patente);
        gestion.AgregarPermiso(patente);
        Assert.IsTrue(usuario.TienePermiso("bitácora"));
        Assert.AreEqual(1, usuario.Roles.Count);
        Assert.AreEqual(1, gestion.Permisos.Count);
        gestion.QuitarPermiso(patente);
        Assert.IsFalse(usuario.TienePermiso("Bitácora"));
    }

    [TestMethod]
    public void QuitarRolDeUsuarioNoModificaOtrosUsuarios()
    {
        var rol = new Rol();
        rol.AgregarPermiso(new PermisoSimple { Nombre = "Bitácora" });
        var uno = new Usuario();
        var dos = new Usuario();
        uno.AsignarRol(rol);
        dos.AsignarRol(rol);
        uno.QuitarRol(rol);
        Assert.IsFalse(uno.TienePermiso("Bitácora"));
        Assert.IsTrue(dos.TienePermiso("Bitácora"));
    }

    [TestMethod]
    public void FormularioCargaDatosYControlesSinBaseDeDatos()
    {
        Exception? error = null;
        var hilo = new Thread(() =>
        {
            try
            {
                using var form = new SIGAT.UI.FormGestionRoles();
                var cargar = typeof(SIGAT.UI.FormGestionRoles).GetMethod("CargarDatos",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                cargar.Invoke(form, new object[] { form, EventArgs.Empty });
                var jerarquia = (System.Windows.Forms.TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                var usuario = (System.Windows.Forms.TreeView)form.Controls.Find("tvUsuarioPermisos", true)[0];
                Assert.AreEqual(3, jerarquia.Nodes.Count);
                Assert.AreEqual(2, usuario.Nodes[0].Nodes.Count);
                Assert.IsInstanceOfType<Rol>(usuario.Nodes[0].Tag);
            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        hilo.Join();
        if (error != null) throw error;
    }
}
