using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BE.Integridad;
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
        Assert.AreNotEqual(VerificadorSHA256.Traduccion(1, 2, "Texto", "Completa"), VerificadorSHA256.Traduccion(1, 2, "Texto", "Pendiente"));
    }

    [TestMethod]
    public void Sha256EsEstableEntreCulturasYConUnicode()
    {
        var anterior = CultureInfo.CurrentCulture;
        var fecha = new DateTime(2026, 10, 10, 12, 34, 56, 997);
        string original = VerificadorSHA256.Bitacora(4, fecha, "admin", "Prueba", "ñ中文");
        try
        {
            CultureInfo.CurrentCulture = new CultureInfo("ar-SA");
            Assert.AreEqual(original, VerificadorSHA256.Bitacora(4, fecha, "admin", "Prueba", "ñ中文"));
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
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base de pruebas.");
        void Sql(string texto)
        {
            using var c = new SqlConnection(cadena); c.Open();
            using var cmd = new SqlCommand(texto, c); cmd.ExecuteNonQuery();
        }
        var integridad = new IntegridadDAL(cadena);
        var idioma = new IdiomaDAL(cadena);
        var bitacora = new BitacoraDAL(cadena);
        // Fila legada justo antes del redondeo a un nuevo segundo de SQL datetime.
        DateTime fecha = new DateTime(2026, 10, 10, 12, 0, 2).AddMilliseconds(999);
        string cifrado = new EncriptadorServicio().Encriptar("Registro original")!;
        string antiguo = new EncriptadorServicio().CalcularDV(fecha.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) + "adminPrueba" + cifrado)!;
        using (var c = new SqlConnection(cadena))
        {
            c.Open();
            using var cmd = new SqlCommand("INSERT dbo.Bitacora(Fecha,Usuario,Actividad,InformacionAsociada,DigitoVerificador) VALUES(@Fecha,'admin','Prueba',@Info,@DV)", c);
            cmd.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = fecha;
            cmd.Parameters.AddWithValue("@Info", cifrado); cmd.Parameters.AddWithValue("@DV", antiguo); cmd.ExecuteNonQuery();
        }
        string backup = integridad.InicializarBaseConfiable();
        Assert.IsTrue(integridad.Verificar().Correcta, "La migración debe corregir el formato legado, incluidos los pendientes.");
        Assert.ThrowsExactly<InvalidOperationException>(() => integridad.InicializarBaseConfiable());
        idioma.ActualizarTraduccion(1, 1, "Texto correcto ñ中文", "Completa", "");
        bitacora.Insertar(new Bitacora { Fecha = fecha, Usuario = "admin", Actividad = "Nuevo", InformacionAsociada = "Información correcta" });
        Assert.IsTrue(integridad.Verificar().Correcta);
        backup = integridad.CrearRespaldoConfiable();

        Sql("UPDATE dbo.Traduccion SET Texto=N'Texto alterado' WHERE Id_Idioma=1 AND Id_Control=1");
        Assert.IsFalse(integridad.Verificar().Correcta);
        Assert.ThrowsExactly<IntegridadException>(() => idioma.ObtenerTraduccionesPorIdioma(1));
        Assert.ThrowsExactly<IntegridadException>(() => idioma.ActualizarTraduccion(1, 1, "Aceptar alteración", "Completa", ""));
        Assert.ThrowsExactly<IntegridadException>(() => integridad.CrearRespaldoConfiable());
        // Un .bak físicamente válido puede contener datos cuyo SHA-256 está roto.
        string corrupto = Path.Combine(Path.GetDirectoryName(backup)!, Path.GetFileNameWithoutExtension(backup) + "_corrupto.bak");
        using (var c = new SqlConnection(cadena))
        {
            c.Open();
            using var cmd = new SqlCommand("BACKUP DATABASE [" + c.Database + "] TO DISK=@Ruta WITH COPY_ONLY,CHECKSUM", c);
            cmd.Parameters.AddWithValue("@Ruta", corrupto); cmd.ExecuteNonQuery();
        }
        Assert.ThrowsExactly<IntegridadException>(() => integridad.Recuperar(corrupto));
        Assert.IsFalse(integridad.Verificar().Correcta, "Un respaldo corrupto no debe modificar ni legitimar los datos actuales.");
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Assert.AreEqual("Texto correcto ñ中文", idioma.ObtenerTraduccionesPorIdioma(1).Single(t => t.IdControl == 1).Texto);

        Sql("UPDATE dbo.Bitacora SET DigitoVerificador='DV roto' WHERE IdBitacora=1");
        Assert.ThrowsExactly<IntegridadException>(() => bitacora.Buscar(null, null, "", ""));
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Sql("UPDATE dbo.Bitacora SET InformacionAsociada='cifrado inválido' WHERE IdBitacora=1");
        Assert.IsTrue(integridad.Verificar().Problemas.Any(p => p.Motivo.Contains("cifrada")));
        Assert.ThrowsExactly<IntegridadException>(() => bitacora.Buscar(null, null, "", ""));
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);

        Sql("DELETE dbo.Bitacora WHERE IdBitacora=1");
        Assert.IsTrue(integridad.Verificar().Problemas.Any(p => p.Registro == "Tabla"));
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Sql("DELETE dbo.Traduccion WHERE Id_Idioma=1 AND Id_Control=1");
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);
        Sql("UPDATE dbo.IntegridadTabla SET HashVertical=REPLICATE('0',64) WHERE Tabla='Traduccion'");
        Assert.IsTrue(integridad.Recuperar(backup).Correcta);

        // Un respaldo anterior no debe reemplazar silenciosamente una actualización legítima.
        idioma.ActualizarTraduccion(1, 1, "Cambio legítimo posterior", "Completa", "");
        Sql("UPDATE dbo.Traduccion SET Texto='Corrupción posterior' WHERE Id_Idioma=1 AND Id_Control=1");
        Assert.ThrowsExactly<InvalidOperationException>(() => integridad.Recuperar(backup));
        Assert.IsFalse(integridad.Verificar().Correcta);
        Assert.IsTrue(integridad.Recuperar(backup, restaurarTablasCompletas: true).Correcta);
        Assert.AreEqual("Texto correcto ñ中文", idioma.ObtenerTraduccionesPorIdioma(1).Single(t => t.IdControl == 1).Texto);
        Assert.IsTrue(bitacora.Buscar(null, null, "", "").Count == 2);
        bitacora.Insertar(new Bitacora { Fecha=DateTime.Now.AddDays(-200),Usuario="admin",Actividad="Antiguo",InformacionAsociada="Archivar" });
        Assert.AreEqual(1, integridad.ArchivarBitacora(180));
        Assert.IsTrue(integridad.Verificar().Correcta, "El archivo legítimo debe actualizar el control del conjunto.");
    }
}
