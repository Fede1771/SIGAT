using Microsoft.Data.SqlClient;

namespace SIGAT.DAL
{
    internal static class ConexionBD
    {
        private static readonly string CadenaConexion = @"Data Source=.\SQLEXPRESS;Initial Catalog=SIGAT;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion() => new SqlConnection(CadenaConexion);
    }
}