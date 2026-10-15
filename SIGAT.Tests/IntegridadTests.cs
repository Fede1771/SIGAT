using System.Globalization;
using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BE.Integridad;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class IntegridadTests
{
    [TestMethod]
    public void CanonicoDistingueNulosVaciosYLimitesDeCampos()
    {
        Assert.AreNotEqual(VerificadorSHA256.Canonico("ab", "c"), VerificadorSHA256.Canonico("a", "bc"));
        Assert.AreNotEqual(VerificadorSHA256.Canonico((object?)null), VerificadorSHA256.Canonico(""));
        string hash = VerificadorSHA256.Usuario(1, "admin", "hash", "Ana", "Pérez", true);
        Assert.AreNotEqual(hash, VerificadorSHA256.Usuario(1, "admin", "otro", "Ana", "Pérez", true));
        Assert.AreNotEqual(hash, VerificadorSHA256.Usuario(1, "admin", "hash", "Ana", "Pérez", false));
        Assert.AreNotEqual(VerificadorSHA256.UsuarioRol(1, 2), VerificadorSHA256.UsuarioRol(2, 1));
        Assert.AreEqual(64, hash.Length);
    }

    [TestMethod]
    public void Sha256EsEstableEntreCulturasYConUnicode()
    {
        var anterior = CultureInfo.CurrentCulture;
        string original = VerificadorSHA256.Usuario(4, "ñ中文", "hash", "Ana", "Pérez", true);
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            Assert.AreEqual(original, VerificadorSHA256.Usuario(4, "ñ中文", "hash", "Ana", "Pérez", true));
        }
        finally { CultureInfo.CurrentCulture = anterior; }
        Assert.AreEqual("ba7816bf8f01cfea414140de5dae2223b00361a396177a9cb410ff61f20015ad", VerificadorSHA256.Hexadecimal("abc"));
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void DetectaCorrupcionBloqueaOperacionesYRecuperaDesdeBackupComprobado()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_INTEGRIDAD_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere base exclusiva de pruebas."); return; }
        string nombreBase = new SqlConnectionStringBuilder(cadena).InitialCatalog;
        if (!nombreBase.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base de pruebas.");
        void Sql(string texto)
        {
            using var c = new SqlConnection(cadena); c.Open();
            using var cmd = new SqlCommand(texto, c); cmd.ExecuteNonQuery();
        }
        var integridad = new IntegridadDAL(cadena);
        var idioma = new IdiomaDAL(cadena);
        var bitacora = new BitacoraDAL(cadena);
        var usuarios = new UsuarioDAL(cadena);
        var roles = new RolesDAL(cadena);
        string backupInicial = integridad.InicializarBaseConfiable();
        Assert.IsTrue(File.Exists(backupInicial));
        Assert.ThrowsExactly<InvalidOperationException>(() => integridad.InicializarBaseConfiable());
        var cuenta = new Usuario { NombreUsuario="dv_prueba",Nombre="Ana",Apellido="Pérez",Password=HashHelper.ObtenerHashSHA256("Inicial123"),Activo=true };
        usuarios.Insertar(cuenta);
        var estado = roles.Cargar();
        estado.Usuarios.Single(u => u.IdUsuario == cuenta.IdUsuario).AsignarRol(estado.Roles.Single(r => r.Id==5));
        roles.Guardar(estado);
        string carpeta = Path.Combine(Path.GetTempPath(), "SIGAT_DV_"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(carpeta);
        string backup = integridad.CrearRespaldoConfiable(Path.Combine(carpeta,"confiable.bak"));
        Assert.AreEqual(Path.Combine(carpeta,"confiable.bak"), backup);
        Assert.ThrowsExactly<InvalidOperationException>(() => integridad.CrearRespaldoConfiable(backup));
        Assert.ThrowsExactly<ArgumentException>(() => integridad.CrearRespaldoConfiable("relativo.bak"));
        Assert.IsTrue(integridad.Verificar().Correcta);

        Sql($"UPDATE dbo.Usuarios SET Apellido=N'ALTERADO' WHERE IdUsuario={cuenta.IdUsuario}");
        var informe = integridad.Verificar();
        Assert.IsTrue(informe.Problemas.Any(p => p.Tabla=="Usuarios" && p.Registro==cuenta.IdUsuario.ToString()));
        Assert.IsTrue(informe.Problemas.Any(p => p.Tabla=="Usuarios" && p.Registro=="Tabla"));
        Assert.ThrowsExactly<IntegridadException>(() => usuarios.ObtenerTodos());
        Assert.ThrowsExactly<IntegridadException>(() => usuarios.ObtenerPorNombreUsuario("dv_prueba"));
        Assert.ThrowsExactly<IntegridadException>(() => roles.Cargar());
        Assert.ThrowsExactly<IntegridadException>(() => roles.Guardar(estado));
        Assert.ThrowsExactly<IntegridadException>(() => usuarios.Actualizar(cuenta));
        Assert.ThrowsExactly<IntegridadException>(() => integridad.CrearRespaldoConfiable());
        // Un .bak físicamente válido también debe rechazarse si contiene cuentas adulteradas.
        using (var c = new SqlConnection(cadena))
        {
            c.Open();
            using var consultar = new SqlCommand("SELECT CONVERT(nvarchar(2000),SERVERPROPERTY('InstanceDefaultBackupPath'))", c);
            string corrupto = Path.Combine((string)consultar.ExecuteScalar()!, nombreBase + "_corrupto.bak");
            using var guardar = new SqlCommand("BACKUP DATABASE [" + nombreBase + "] TO DISK=@Ruta WITH COPY_ONLY,CHECKSUM,INIT", c);
            guardar.Parameters.AddWithValue("@Ruta", corrupto);
            guardar.ExecuteNonQuery();
            Assert.ThrowsExactly<IntegridadException>(() => integridad.Recuperar(corrupto));
            Assert.IsFalse(integridad.Verificar().Correcta, "Un respaldo con daño lógico no debe legitimar la alteración.");
        }
        // La auditoría y las traducciones continúan operativas aun si las cuentas están bloqueadas.
        bitacora.Insertar(new Bitacora { Fecha=DateTime.Now,Usuario="admin",Actividad="Incidente",InformacionAsociada="Cuenta alterada" });
        Assert.AreEqual("Cuenta alterada",bitacora.Buscar(null,null,"admin","Incidente").First().InformacionAsociada);
        idioma.ActualizarTraduccion(1,1,"Texto sin DVH","Completa");
        Assert.IsTrue(idioma.ObtenerTraduccionesPorIdioma(1).Any(t=>t.Texto=="Texto sin DVH"));
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Assert.AreEqual("Pérez",usuarios.ObtenerPorNombreUsuario("dv_prueba")!.Apellido);
        Assert.AreEqual("Cuenta alterada",bitacora.Buscar(null,null,"admin","Incidente").First().InformacionAsociada);

        Sql($"UPDATE dbo.Usuarios SET DigitoVerificador=REPLICATE('0',64) WHERE IdUsuario={cuenta.IdUsuario}");
        Assert.IsFalse(integridad.Verificar().Correcta);
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Sql($"DELETE dbo.UsuarioRol WHERE IdUsuario={cuenta.IdUsuario}");
        Assert.IsTrue(integridad.Verificar().Problemas.Any(p=>p.Tabla=="UsuarioRol" && p.Registro=="Tabla"));
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Sql($"DELETE dbo.UsuarioRol WHERE IdUsuario={cuenta.IdUsuario}; DELETE dbo.Usuarios WHERE IdUsuario={cuenta.IdUsuario}");
        Assert.IsTrue(integridad.Recuperar(backup).Correcta, "La cuenta se repone con IDENTITY_INSERT antes de sus asignaciones.");

        cuenta.Nombre="Cambio legítimo posterior";cuenta.Password="";usuarios.Actualizar(cuenta);
        Sql($"UPDATE dbo.Usuarios SET Apellido=N'OTRA ALTERACION' WHERE IdUsuario={cuenta.IdUsuario}");
        Assert.ThrowsExactly<InvalidOperationException>(()=>integridad.Recuperar(backup));
        Assert.IsTrue(integridad.Recuperar(backup,restaurarTablasCompletas:true).Correcta);
        Assert.AreEqual("Ana",usuarios.ObtenerPorNombreUsuario("dv_prueba")!.Nombre);

        Sql($"UPDATE dbo.Usuarios SET Apellido=N'Aceptado conscientemente' WHERE IdUsuario={cuenta.IdUsuario}");
        SesionServicio.ObtenerInstancia().CerrarSesion();
        var servicio = new IntegridadBLL(integridad,new BitacoraBLL(bitacora));
        Guid IdBase()
        {
            using var c = new SqlConnection(cadena); c.Open();
            using var cmd = new SqlCommand("SELECT TOP(1) IdBase FROM dbo.IntegridadTabla", c);
            return (Guid)cmd.ExecuteScalar()!;
        }
        Guid referencia = IdBase();
        Assert.IsTrue(servicio.RecalcularIntegridad().Correcta);
        Assert.AreEqual(referencia, IdBase(), "El recálculo conserva la identidad de la base y sus respaldos.");
        Assert.AreEqual("Aceptado conscientemente",usuarios.ObtenerPorNombreUsuario("dv_prueba")!.Apellido);
        Assert.IsTrue(bitacora.Buscar(null,null,null,"RecalculoIntegridad").Any());
        Sql("DELETE dbo.IntegridadTabla WHERE Tabla='UsuarioRol'");
        Assert.IsFalse(integridad.Verificar().Correcta);
        Assert.ThrowsExactly<InvalidOperationException>(() => integridad.Recuperar(backup),
            "Sin un DVV, la recuperación parcial debe explicar la limitación sin aceptar datos.");
        Assert.IsTrue(integridad.Recuperar(backup, true).Correcta, "El respaldo comprobado repone el DVV faltante en recuperación completa.");
        Sql("DELETE dbo.IntegridadTabla WHERE Tabla='UsuarioRol'");
        Assert.IsTrue(servicio.RecalcularIntegridad().Correcta,"El recálculo repone un registro DVV faltante.");
        var noAdmin=new Usuario { IdUsuario=cuenta.IdUsuario,NombreUsuario="dv_prueba" };
        SesionServicio.ObtenerInstancia().IniciarSesion(noAdmin);
        try { Assert.ThrowsExactly<UnauthorizedAccessException>(()=>servicio.RecalcularIntegridad()); }
        finally { SesionServicio.ObtenerInstancia().CerrarSesion(); }
        string log=Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),"SIGAT","logs","integridad.log");
        StringAssert.Contains(File.ReadAllText(log),"Recálculo administrativo");

        // El login debe verificar antes de autenticar y no crear una sesión si se alteraron las asignaciones.
        string? previa=Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS",nombreBase);
            Sql($"UPDATE dbo.UsuarioRol SET DigitoVerificador=REPLICATE('0',64) WHERE IdUsuario={cuenta.IdUsuario}");
            Assert.ThrowsExactly<IntegridadException>(()=>new UsuarioBLL().Autenticar("dv_prueba","Inicial123",out _));
            Assert.IsNull(SesionServicio.ObtenerInstancia().UsuarioActual);
            Assert.IsTrue(integridad.Recuperar(backup,true).Correcta);
        }
        finally { Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS",previa); }
        Sql("DROP TABLE dbo.IntegridadTabla");
        Assert.IsFalse(integridad.Verificar().Configurada);
        Assert.IsTrue(servicio.RecalcularIntegridad().Correcta, "El recálculo administrativo puede crear los controles faltantes.");
        Assert.AreNotEqual(Guid.Empty, IdBase());
    }
}
