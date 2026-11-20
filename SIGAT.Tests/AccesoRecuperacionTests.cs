using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class AccesoRecuperacionTests
{
    [TestCleanup]
    public void LimpiarSesion() => SesionServicio.ObtenerInstancia().CerrarSesion();

    [TestMethod]
    public void SinLoginNiUsuarioComunPuedenAdministrarLaRecuperacion()
    {
        var sesion = SesionServicio.ObtenerInstancia();
        var servicio = new IntegridadBLL();
        foreach (Usuario? usuario in new Usuario?[] { null, new() { Activo = true }, Administrador(false) })
        {
            sesion.CerrarSesion();
            if (usuario != null) sesion.IniciarSesion(usuario);
            Assert.IsFalse(IntegridadBLL.EsAdministradorAplicacion());
            Assert.IsFalse(servicio.PuedeAdministrar()); // Debe rechazar antes de consultar SQL.
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => servicio.RecalcularIntegridad());
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => servicio.InicializarBaseConfiable());
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => servicio.Recuperar("sin-respaldo.bak"));
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => servicio.CrearRespaldoConfiable());
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => servicio.ArchivarBitacora(180));
        }
    }

    [TestMethod]
    public void LaSesionDeRecuperacionSeDistingueYSeLimpiaAlSalirOAutenticarNormalmente()
    {
        var sesion = SesionServicio.ObtenerInstancia();
        sesion.IniciarSesionRecuperacion(Administrador());
        Assert.IsTrue(sesion.EstaAutenticado);
        Assert.IsTrue(sesion.SoloRecuperacion);
        Assert.IsTrue(IntegridadBLL.EsAdministradorAplicacion());
        sesion.CerrarSesion();
        Assert.IsNull(sesion.UsuarioActual);
        Assert.IsFalse(sesion.SoloRecuperacion);
        sesion.IniciarSesionRecuperacion(Administrador());
        sesion.IniciarSesion(new Usuario { Activo = true });
        Assert.IsFalse(sesion.SoloRecuperacion);
        Assert.IsFalse(IntegridadBLL.EsAdministradorAplicacion());
    }

    private static Usuario Administrador(bool activo = true)
    {
        var usuario = new Usuario { IdUsuario = 1, NombreUsuario = "admin", Activo = activo };
        usuario.AsignarRol(MatrizRoles.CrearCatalogo().Roles.Single(r => r.Id == 1));
        return usuario;
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void LaPrimeraReferenciaTambienExigeElLoginDelAdministradorOriginal()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_LOGIN_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas de login."); return; }
        string nombreBase = new SqlConnectionStringBuilder(cadena).InitialCatalog;
        if (!nombreBase.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        string? anterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", nombreBase);
            const string clave = "PrimeraReferencia123!";
            string comun = "bootstrap_" + Guid.NewGuid().ToString("N")[..8];
            using (var c = new SqlConnection(cadena))
            {
                c.Open();
                using var cmd = new SqlCommand("""
                    UPDATE dbo.Usuarios SET DigitoVerificador=NULL;
                    UPDATE dbo.UsuarioRol SET DigitoVerificador=NULL;
                    DELETE dbo.IntegridadTabla;
                    UPDATE dbo.Usuarios SET Password=@Clave
                    WHERE IdUsuario=(SELECT IdUsuario FROM dbo.AdministradorOriginal WHERE Id=1);
                    INSERT dbo.Usuarios(NombreUsuario,Password,Nombre,Apellido,Activo)
                    VALUES(@Comun,@Clave,'Prueba','Inicial',1);
                    INSERT dbo.UsuarioRol(IdUsuario,IdRol) VALUES(CONVERT(int,SCOPE_IDENTITY()),5);
                    SELECT NombreUsuario FROM dbo.Usuarios
                    WHERE IdUsuario=(SELECT IdUsuario FROM dbo.AdministradorOriginal WHERE Id=1);
                    """, c);
                cmd.Parameters.AddWithValue("@Clave", HashHelper.ObtenerHashSHA256(clave));
                cmd.Parameters.AddWithValue("@Comun", comun);
                string admin = (string)cmd.ExecuteScalar()!;
                var login = new UsuarioBLL();
                Assert.AreEqual(ResultadoLogin.Mantenimiento, login.Autenticar(comun, clave, out _));
                Assert.IsNull(SesionServicio.ObtenerInstancia().UsuarioActual);
                Assert.AreEqual(ResultadoLogin.CredencialesInvalidas, login.Autenticar(admin, "incorrecta", out _));
                Assert.IsNull(SesionServicio.ObtenerInstancia().UsuarioActual);
                Assert.AreEqual(ResultadoLogin.RequiereRecuperacion, login.Autenticar(admin, clave, out _));
                Assert.IsTrue(SesionServicio.ObtenerInstancia().SoloRecuperacion);
            }
            var integridad = new IntegridadDAL(cadena);
            Assert.IsFalse(integridad.Verificar().Configurada);
            new IntegridadBLL(integridad).InicializarBaseConfiable();
            Assert.IsTrue(integridad.Verificar().Correcta);
        }
        finally
        {
            SesionServicio.ObtenerInstancia().CerrarSesion();
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", anterior);
        }
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void ElLoginSoloAutorizaRecuperacionAlAdministradorYNoLegitimaLosDatosDaniados()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_LOGIN_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas de login."); return; }
        SqlPruebas.Preparar(cadena);
        var sesion = SesionServicio.ObtenerInstancia();
        string? anterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", new SqlConnectionStringBuilder(cadena).InitialCatalog);
            var integridad = new IntegridadDAL(cadena);
            var usuarios = new UsuarioDAL(cadena);
            var roles = new RolesDAL(cadena);
            var estado = roles.Cargar();
            var admin = usuarios.ObtenerTodos().Single(u => u.IdUsuario == estado.IdAdministradorOriginal);
            const string clave = "LoginRecuperacion123!";
            admin.Password = HashHelper.ObtenerHashSHA256(clave);
            usuarios.Actualizar(admin);
            Usuario Crear(string nombre, bool activo = true)
            {
                var u = new Usuario { NombreUsuario = nombre, Nombre = "Prueba", Apellido = "Login", Activo = activo,
                    Password = HashHelper.ObtenerHashSHA256(clave) };
                usuarios.Insertar(u);
                return u;
            }
            var comun = Crear("login_comun");
            var delegado = Crear("login_admin_anidado");
            var inactivo = Crear("login_inactivo", false);
            estado = roles.Cargar();
            var rolAnidado = estado.Roles.Single(r => r.Id == 2);
            rolAnidado.AgregarRol(estado.Roles.Single(r => r.Id == 1));
            estado.Usuarios.Single(u => u.IdUsuario == comun.IdUsuario).AsignarRol(estado.Roles.Single(r => r.Id == 5));
            estado.Usuarios.Single(u => u.IdUsuario == delegado.IdUsuario).AsignarRol(rolAnidado);
            roles.Guardar(estado);
            void Sql(string texto)
            {
                using var c = new SqlConnection(cadena); c.Open();
                using var cmd = new SqlCommand(texto, c); cmd.ExecuteNonQuery();
            }
            var login = new UsuarioBLL();
            Assert.AreEqual(ResultadoLogin.Exito, login.Autenticar(admin.NombreUsuario, clave, out _));
            Assert.IsFalse(sesion.SoloRecuperacion);
            Sql($"UPDATE dbo.Usuarios SET Apellido='Adulterado' WHERE IdUsuario={comun.IdUsuario}");
            Assert.AreEqual(ResultadoLogin.Mantenimiento, login.Autenticar(comun.NombreUsuario, clave, out _));
            Assert.IsNull(sesion.UsuarioActual);
            Assert.AreEqual(ResultadoLogin.CredencialesInvalidas, login.Autenticar(admin.NombreUsuario, "incorrecta", out _));
            Assert.IsNull(sesion.UsuarioActual);
            Assert.AreEqual(ResultadoLogin.UsuarioInactivo, login.Autenticar(inactivo.NombreUsuario, clave, out _));
            Assert.IsNull(sesion.UsuarioActual);
            Assert.AreEqual(ResultadoLogin.RequiereRecuperacion, login.Autenticar(delegado.NombreUsuario, clave, out _),
                "La autorización debe respetar también los roles anidados.");
            Assert.IsTrue(sesion.SoloRecuperacion);
            Assert.ThrowsExactly<SIGAT.BE.Integridad.IntegridadException>(() => usuarios.ObtenerTodos());
            Assert.IsFalse(integridad.Verificar().Correcta, "Autenticar no debe recalcular ni aceptar el daño.");
            Assert.AreEqual(ResultadoLogin.RequiereRecuperacion, login.Autenticar(admin.NombreUsuario, clave, out _));
            var servicio = new IntegridadBLL(integridad);
            Assert.IsTrue(servicio.RecalcularIntegridad().Correcta);
            Assert.IsTrue(sesion.SoloRecuperacion, "Reparar no convierte la sesión en un acceso normal.");
            Assert.ThrowsExactly<UnauthorizedAccessException>(() => new RolesBLL().CargarParaGestion());
            Assert.AreEqual(ResultadoLogin.Exito, login.Autenticar(admin.NombreUsuario, clave, out _));
            Assert.IsFalse(sesion.SoloRecuperacion);

            Sql($"UPDATE dbo.Usuarios SET DigitoVerificador=REPLICATE('0',64) WHERE IdUsuario={admin.IdUsuario}");
            Assert.AreEqual(ResultadoLogin.RequiereRecuperacion, login.Autenticar(admin.NombreUsuario, clave, out _),
                "Un dígito alterado en la propia cuenta permite recuperar si el DVV confirma sus datos.");
            Assert.IsTrue(servicio.RecalcularIntegridad().Correcta);
            Sql($"UPDATE dbo.UsuarioRol SET IdRol=1 WHERE IdUsuario={comun.IdUsuario}");
            Assert.AreEqual(ResultadoLogin.Mantenimiento, login.Autenticar(comun.NombreUsuario, clave, out _),
                "Una asignación adulterada no debe convertir a un usuario común en administrador.");
            Assert.IsNull(sesion.UsuarioActual);
            Assert.AreEqual(ResultadoLogin.RequiereRecuperacion, login.Autenticar(admin.NombreUsuario, clave, out _));
            Assert.IsTrue(servicio.RecalcularIntegridad().Correcta);

            string hashLegitimo = HashHelper.ObtenerHashSHA256(clave);
            string hashAdulterado = HashHelper.ObtenerHashSHA256("clave-adulterada");
            Sql($"UPDATE dbo.Usuarios SET Password='{hashAdulterado}' WHERE IdUsuario={admin.IdUsuario}");
            Assert.AreEqual(ResultadoLogin.Mantenimiento, login.Autenticar(admin.NombreUsuario, "clave-adulterada", out _),
                "No se deben aceptar credenciales adulteradas sin un DVH o DVV que las respalde.");
            Assert.IsNull(sesion.UsuarioActual);
            Sql($"UPDATE dbo.Usuarios SET Password='{hashLegitimo}' WHERE IdUsuario={admin.IdUsuario}");
            Assert.IsTrue(integridad.Verificar().Correcta);
            Assert.AreEqual(ResultadoLogin.Exito, login.Autenticar(admin.NombreUsuario, clave, out _));
        }
        finally
        {
            sesion.CerrarSesion();
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", anterior);
        }
    }
}
