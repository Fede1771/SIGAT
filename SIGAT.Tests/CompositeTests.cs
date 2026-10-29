using SIGAT.BE;
using SIGAT.BLL;

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
        var hoja = new PermisoSimple { Nombre = "Ver bitácora" };
        var familia = new PermisoCompuesto();
        Assert.ThrowsExactly<InvalidOperationException>(() => hoja.Agregar(familia));
        Assert.ThrowsExactly<InvalidOperationException>(() => hoja.Quitar(familia));
        familia.ObtenerHijos().Add(familia);
        Assert.AreEqual(0, familia.ObtenerHijos().Count);
    }

    [TestMethod]
    public void PermisosCompartidosSeActualizanSinDuplicarAsignaciones()
    {
        var gestion = new PermisoCompuesto { Nombre = "Gestión de activos" };
        var admin = new Rol { Nombre = "Administrador" };
        var usuario = new Usuario();
        admin.AgregarPermiso(gestion);
        usuario.AsignarRol(admin);
        usuario.AsignarRol(admin);
        var patente = new PermisoSimple { Nombre = "Ver bitácora" };
        gestion.Agregar(patente);
        gestion.Agregar(patente);
        Assert.IsTrue(usuario.TienePermiso("ver bitácora"));
        Assert.AreEqual(1, usuario.Roles.Count);
        Assert.AreEqual(1, gestion.ObtenerHijos().Count);
        gestion.Quitar(patente);
        Assert.IsFalse(usuario.TienePermiso("Ver bitácora"));
    }

    [TestMethod]
    public void QuitarRolDeUsuarioNoModificaOtrosUsuarios()
    {
        var rol = new Rol();
        rol.AgregarPermiso(new PermisoSimple { Nombre = "Ver bitácora" });
        var uno = new Usuario();
        var dos = new Usuario();
        uno.AsignarRol(rol);
        dos.AsignarRol(rol);
        uno.QuitarRol(rol);
        Assert.IsFalse(uno.TienePermiso("Ver bitácora"));
        Assert.IsTrue(dos.TienePermiso("Ver bitácora"));
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
                var cargar = typeof(SIGAT.UI.FormGestionRoles).GetMethod("InicializarDatos",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                var cuenta = new Usuario { IdUsuario = 42, NombreUsuario = "cuentaRegistrada", Activo = true };
                EstadoRoles datos = MatrizRoles.CrearCatalogo();
                cuenta.AsignarRol(datos.Roles[0]);
                datos.Usuarios.Add(cuenta);
                Usuario auditor = new Usuario { IdUsuario = 43, NombreUsuario = "auditorRegistrado", Activo = true };
                auditor.AsignarRol(datos.Roles[4]);
                datos.Usuarios.Add(auditor);
                datos.Usuarios.Add(new Usuario { IdUsuario = 44, NombreUsuario = "inactivo", Activo = false });
                cargar.Invoke(form, new object[] { datos });
                var tabs = (System.Windows.Forms.TabControl)form.Controls.Find("vistasRoles", true).Single();
                Assert.HasCount(3, tabs.TabPages);
                var usuarios = (System.Windows.Forms.ListBox)form.Controls.Find("listaUsuarios", true).Single();
                Assert.HasCount(3, usuarios.Items);
                Assert.HasCount(5, ((System.Windows.Forms.ListBox)form.Controls.Find("listaRoles", true).Single()).Items);
                Assert.HasCount(7, ((System.Windows.Forms.ListBox)form.Controls.Find("listaFamilias", true).Single()).Items);
                Assert.IsFalse(form.TieneCambios);
                Assert.IsFalse(form.Controls.Find("btnGuardarCambios", true).Single().Enabled);
                Assert.IsEmpty(form.Controls.Find("cbRolDestino", true));
                Assert.IsFalse(form.Controls.Find("btnAgregarPermisoRol", true).Single().Enabled, "Administrador debe ser de solo lectura.");
                Assert.AreEqual(1, cuenta.Roles.Count);
                cargar.Invoke(form, new object[] { new EstadoRoles() });
                Assert.IsEmpty(usuarios.Items);

            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        hilo.Join();
        if (error != null) throw error;
    }
}
