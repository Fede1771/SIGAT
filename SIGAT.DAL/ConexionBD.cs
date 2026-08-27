using Microsoft.Data.SqlClient;

namespace SIGAT.DAL
{
    internal static class ConexionBD
    {
        // IMPORTANTE: Cambiar ".\SQLEXPRESS" por "(local)" cuando estés en la facultad
        private static readonly string CadenaConexion = @"Data Source=.\SQLEXPRESS;Initial Catalog=SIGAT;Integrated Security=True;TrustServerCertificate=True;";

        public static SqlConnection ObtenerConexion()
        {
            // Creamos y devolvemos una nueva conexión a la base de datos
            return new SqlConnection(CadenaConexion);
        }
    }
}