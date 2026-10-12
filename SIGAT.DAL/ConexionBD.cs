using Microsoft.Data.SqlClient;

namespace SIGAT.DAL
{
    internal static class ConexionBD
    {
        private static readonly string CadenaConexion = @"Data Source=.\SQLEXPRESS;Initial Catalog=SIGAT;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            string? basePruebas = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
            if (!string.IsNullOrWhiteSpace(basePruebas))
            {
                if (!basePruebas.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal) ||
                    basePruebas.Any(c => !char.IsLetterOrDigit(c) && c != '_'))
                    throw new InvalidOperationException("SIGAT_BASE_PRUEBAS solo admite el nombre de una base exclusiva SIGAT_Roles_Pruebas_.");
                var configuracion = new SqlConnectionStringBuilder(CadenaConexion) { InitialCatalog = basePruebas };
                return new SqlConnection(configuracion.ConnectionString);
            }
            // Creamos y devolvemos una nueva conexión a la base de datos
            return new SqlConnection(CadenaConexion);
        }
    }
}
