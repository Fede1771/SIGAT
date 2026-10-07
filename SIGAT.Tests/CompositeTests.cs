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
                var cargar = typeof(SIGAT.UI.FormGestionRoles).GetMethod("InicializarDatos",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!;
                var cuenta = new Usuario { IdUsuario = 42, NombreUsuario = "cuentaRegistrada", Activo = true,
                    Perfil = new Perfil { NombrePerfil = "Administrador" } };
                EstadoRoles datos = new EstadoRoles();
                Rol admin = new Rol { Id = 1, Nombre = "Administrador" };
                Rol operador = new Rol { Id = 2, Nombre = "Operador" };
                datos.Roles.Add(admin);
                datos.Roles.Add(operador);
                datos.Roles.Add(new Rol { Id = 3, Nombre = "Gestión" });
                foreach (string nombre in new[] { "Gestión de Roles", "Gestión de Usuarios", "Gestión de Idiomas", "Bitácora" })
                {
                    Permiso patente = new PermisoSimple { Nombre = nombre };
                    admin.AgregarPermiso(patente);
                    datos.Permisos.Add(patente);
                }
                cuenta.AsignarRol(admin);
                datos.Usuarios.AddRange(new List<Usuario> { cuenta,
                    new Usuario { IdUsuario = 43, NombreUsuario = "operadorRegistrado", Activo = true,
                        Perfil = new Perfil { NombrePerfil = "Operador" } },
                    new Usuario { IdUsuario = 44, NombreUsuario = "inactivo", Activo = false } });
                datos.Usuarios[1].AsignarRol(operador);
                cargar.Invoke(form, new object[] { datos });
                var jerarquia = (System.Windows.Forms.TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                var usuario = (System.Windows.Forms.TreeView)form.Controls.Find("tvUsuarioPermisos", true)[0];
                Assert.AreEqual(3, jerarquia.Nodes.Count);
                Assert.AreEqual(4, usuario.Nodes[0].Nodes.Count);
                Assert.IsInstanceOfType<Rol>(usuario.Nodes[0].Tag);
                var combo = (System.Windows.Forms.ComboBox)form.Controls.Find("cbUsuarios", true)[0];
                Assert.AreEqual(2, combo.Items.Count);
                Assert.AreEqual("cuentaRegistrada", ((Usuario)combo.Items[0]).NombreUsuario);
                Assert.AreEqual(42, ((Usuario)combo.Items[0]).IdUsuario);
                Assert.AreEqual(1, cuenta.Roles.Count);
                combo.SelectedIndex = 1;
                Assert.AreEqual("Operador", usuario.Nodes[0].Text);
                cargar.Invoke(form, new object[] { new EstadoRoles() });
                Assert.AreEqual(0, combo.Items.Count);
                Assert.AreEqual(0, usuario.Nodes.Count);
            }
            catch (Exception ex) { error = ex; }
        });
        hilo.SetApartmentState(ApartmentState.STA);
        hilo.Start();
        hilo.Join();
        if (error != null) throw error;
    }
}
