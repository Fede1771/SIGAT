using Microsoft.Data.SqlClient;
using SIGAT.DAL;

namespace SIGAT.Tests;

internal static class SqlPruebas
{
    // Solo inicializamos bases exclusivas de pruebas sin referencia. Una base dañada debe fallar, nunca recalcularse aquí.
    internal static void Preparar(string cadena)
    {
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        var servicio = new IntegridadDAL(cadena);
        if (!servicio.Verificar().Configurada) servicio.InicializarBaseConfiable();
        Assert.IsTrue(servicio.Verificar().Correcta, "La prueba debe comenzar con una referencia válida.");
    }
}
