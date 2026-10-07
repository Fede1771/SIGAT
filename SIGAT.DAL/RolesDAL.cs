using System.Data;
using Microsoft.Data.SqlClient;
using SIGAT.BE;

namespace SIGAT.DAL
{
    public class RolesDAL
    {
        private readonly string? cadena;
        public RolesDAL(string? cadenaConexion = null) { cadena = cadenaConexion; }
        private SqlConnection Conectar()
        {
            return cadena == null ? ConexionBD.ObtenerConexion() : new SqlConnection(cadena);
        }

        private static void ConfigurarConexion(SqlConnection conexion)
        {
            using SqlCommand comando = new SqlCommand(@"SET ANSI_NULLS ON; SET ANSI_PADDING ON;
                SET ANSI_WARNINGS ON; SET ARITHABORT ON; SET CONCAT_NULL_YIELDS_NULL ON;
                SET QUOTED_IDENTIFIER ON; SET NUMERIC_ROUNDABORT OFF;", conexion);
            comando.ExecuteNonQuery();
        }

        public EstadoRoles Cargar()
        {
            using SqlConnection conexion = Conectar();
            conexion.Open();
            ConfigurarConexion(conexion);
            using SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            EstadoRoles estado = new EstadoRoles();
            using (SqlCommand comando = new SqlCommand(@"
                IF COL_LENGTH('dbo.SeguridadVersion', 'Esquema') IS NULL
                    THROW 50001, 'Ejecute Querys/10_Matriz_Roles_Exacta.sql antes de iniciar SIGAT.', 1;
                SELECT Version FROM dbo.SeguridadVersion WITH (HOLDLOCK) WHERE Id = 1;", conexion, transaccion))
            {
                estado.Version = Convert.ToInt64(comando.ExecuteScalar());
            }
            Dictionary<int, Permiso> permisos = new Dictionary<int, Permiso>();
            using (SqlCommand comando = new SqlCommand("SELECT Id, Nombre, EsCompuesto FROM dbo.Permiso ORDER BY Id", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Permiso permiso = lector.GetBoolean(2) ? new PermisoCompuesto() : new PermisoSimple();
                    permiso.Id = lector.GetInt32(0);
                    permiso.Nombre = lector.GetString(1);
                    permisos.Add(permiso.Id, permiso);
                    estado.Permisos.Add(permiso);
                }
            }
            Dictionary<int, Rol> roles = new Dictionary<int, Rol>();
            using (SqlCommand comando = new SqlCommand("SELECT Id, Nombre FROM dbo.Rol ORDER BY Id", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Rol rol = new Rol { Id = lector.GetInt32(0), Nombre = lector.GetString(1) };
                    roles.Add(rol.Id, rol);
                    estado.Roles.Add(rol);
                }
            }
            using (SqlCommand comando = new SqlCommand("SELECT IdRol, IdPermiso FROM dbo.RolPermiso ORDER BY IdRol, IdPermiso", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read()) roles[lector.GetInt32(0)].AgregarPermiso(permisos[lector.GetInt32(1)]);
            }
            Dictionary<int, Usuario> usuarios = new Dictionary<int, Usuario>();
            using (SqlCommand comando = new SqlCommand(@"SELECT u.IdUsuario, u.NombreUsuario, u.Activo, u.IdPerfil, p.NombrePerfil
                FROM dbo.Usuarios u JOIN dbo.Perfiles p ON p.IdPerfil = u.IdPerfil ORDER BY u.NombreUsuario", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Usuario usuario = new Usuario
                    {
                        IdUsuario = lector.GetInt32(0), NombreUsuario = lector.GetString(1),
                        Activo = lector.GetBoolean(2), IdPerfil = lector.GetInt32(3),
                        Perfil = new Perfil { IdPerfil = lector.GetInt32(3), NombrePerfil = lector.GetString(4) }
                    };
                    usuarios.Add(usuario.IdUsuario, usuario);
                    estado.Usuarios.Add(usuario);
                }
            }
            using (SqlCommand comando = new SqlCommand("SELECT IdUsuario, IdRol FROM dbo.UsuarioRol ORDER BY IdUsuario, IdRol", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read()) usuarios[lector.GetInt32(0)].AsignarRol(roles[lector.GetInt32(1)]);
            }
            transaccion.Commit();
            return estado;
        }

        public void Guardar(EstadoRoles estado)
        {
            // Solo cambian las asignaciones a usuarios; el catálogo de la matriz es fijo.
            // Dos administradores no pueden sobrescribirse sin recibir un conflicto.
            using SqlConnection conexion = Conectar();
            conexion.Open();
            ConfigurarConexion(conexion);
            using SqlTransaction transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
            using (SqlCommand comando = new SqlCommand(@"UPDATE dbo.SeguridadVersion WITH (UPDLOCK)
                SET Version = Version + 1 WHERE Id = 1 AND Version = @Version", conexion, transaccion))
            {
                comando.Parameters.Add("@Version", SqlDbType.BigInt).Value = estado.Version;
                if (comando.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException("Otro usuario modificó los roles. Se recargarán los datos; vuelva a realizar el cambio.");
            }
            Ejecutar(conexion, transaccion, "DELETE FROM dbo.UsuarioRol;");
            foreach (Usuario usuario in estado.Usuarios)
            {
                foreach (Rol rol in usuario.Roles)
                    InsertarPar(conexion, transaccion, "INSERT dbo.UsuarioRol (IdUsuario, IdRol) VALUES (@Uno, @Dos)", usuario.IdUsuario, rol.Id);
            }
            transaccion.Commit();
            estado.Version++;
        }

        private static void Ejecutar(SqlConnection conexion, SqlTransaction transaccion, string sql)
        {
            using SqlCommand comando = new SqlCommand(sql, conexion, transaccion);
            comando.ExecuteNonQuery();
        }

        private static void InsertarPar(SqlConnection conexion, SqlTransaction transaccion, string sql, int uno, int dos)
        {
            using SqlCommand comando = new SqlCommand(sql, conexion, transaccion);
            comando.Parameters.Add("@Uno", SqlDbType.Int).Value = uno;
            comando.Parameters.Add("@Dos", SqlDbType.Int).Value = dos;
            comando.ExecuteNonQuery();
        }
    }
}
