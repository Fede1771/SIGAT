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
                var jerarquia = (System.Windows.Forms.TreeView)form.Controls.Find("tvRolesJerarquia", true)[0];
                var usuario = (System.Windows.Forms.TreeView)form.Controls.Find("tvUsuarioPermisos", true)[0];
                Assert.AreEqual(5, jerarquia.Nodes.Count);
                var catalogo = (System.Windows.Forms.TreeView)form.Controls.Find("tvCatalogoGeneral", true)[0];
                Assert.HasCount(2, catalogo.Nodes);
                Assert.AreEqual("PERMISOS SIMPLES", catalogo.Nodes[0].Text);
                Assert.AreEqual("PERMISOS COMPUESTOS (FAMILIAS)", catalogo.Nodes[1].Text);
                foreach (System.Windows.Forms.TreeNode grupo in catalogo.Nodes)
                    foreach (System.Windows.Forms.TreeNode componente in grupo.Nodes)
                        Assert.IsInstanceOfType<Permiso>(componente.Tag);
                Assert.HasCount(1, form.Controls.Find("btnEditarFamilia", true));
                Assert.AreEqual(6, usuario.Nodes[0].Nodes.Count);
                Assert.IsInstanceOfType<Rol>(usuario.Nodes[0].Tag);
                var combo = (System.Windows.Forms.ComboBox)form.Controls.Find("cbUsuarios", true)[0];
                Assert.AreEqual(3, combo.Items.Count);
                Assert.IsEmpty(form.Controls.Find("btnCrearRolAnidado", true));
                Assert.IsEmpty(form.Controls.Find("btnCrearRol", true));
                Assert.IsEmpty(form.Controls.Find("btnEliminarRol", true));
                foreach (string boton in new[] { "btnAsignarARol", "btnQuitarDeRol" })
                    Assert.HasCount(1, form.Controls.Find(boton, true));
                var distribucion = (System.Windows.Forms.TableLayoutPanel)combo.Parent!.Parent!;
                Assert.AreEqual(1, distribucion.GetColumn(combo.Parent));
                var destino = form.Controls.Find("cbRolDestino", true)[0];
                Assert.AreEqual(2, distribucion.GetColumn(destino.Parent!.Parent!));
                Assert.AreEqual(1, distribucion.GetRowSpan(destino.Parent!.Parent!));
                Assert.AreEqual(2, distribucion.GetRowSpan(jerarquia.Parent!.Parent!));
                Assert.AreEqual(0, distribucion.GetColumn(jerarquia.Parent!.Parent!));
                Assert.AreEqual(2, distribucion.GetColumn(catalogo.Parent!.Parent!));
                Assert.AreEqual(1, distribucion.GetRow(catalogo.Parent!.Parent!));
                Assert.AreEqual("cuentaRegistrada", ((Usuario)combo.Items[0]).NombreUsuario);
                Assert.AreEqual(42, ((Usuario)combo.Items[0]).IdUsuario);
                Assert.AreEqual(1, cuenta.Roles.Count);
                combo.SelectedIndex = 1;
                Assert.AreEqual("Auditor", usuario.Nodes[0].Text);
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
