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
                IF OBJECT_ID('dbo.SeguridadVersion', 'U') IS NULL
                    THROW 50001, 'Ejecute Querys/09_Roles_Persistentes.sql antes de iniciar SIGAT.', 1;
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
            using (SqlCommand comando = new SqlCommand("SELECT IdPadre, IdHijo FROM dbo.PermisoHijo ORDER BY IdPadre, IdHijo", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                    permisos[lector.GetInt32(0)].Agregar(permisos[lector.GetInt32(1)]);
            }
            Dictionary<int, Rol> roles = new Dictionary<int, Rol>();
            using (SqlCommand comando = new SqlCommand("SELECT Id, IdFamilia, IdUsuarioPersonal FROM dbo.Rol ORDER BY Id", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Rol rol = new Rol((PermisoCompuesto)permisos[lector.GetInt32(1)])
                    {
                        Id = lector.GetInt32(0),
                        IdUsuarioPersonal = lector.IsDBNull(2) ? null : lector.GetInt32(2)
                    };
                    roles.Add(rol.Id, rol);
                    estado.Roles.Add(rol);
                }
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
            // Catálogo pequeño: se guarda como una unidad, con transacción y control de versión.
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
            Ejecutar(conexion, transaccion, "DELETE FROM dbo.UsuarioRol; DELETE FROM dbo.Rol; DELETE FROM dbo.PermisoHijo; DELETE FROM dbo.Permiso;");
            foreach (Permiso permiso in estado.Permisos)
            {
                using SqlCommand comando = new SqlCommand("INSERT dbo.Permiso (Id, Nombre, EsCompuesto) VALUES (@Id, @Nombre, @Compuesto)", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = permiso.Id;
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar, 150).Value = permiso.Nombre;
                comando.Parameters.Add("@Compuesto", SqlDbType.Bit).Value = permiso is PermisoCompuesto;
                comando.ExecuteNonQuery();
            }
            foreach (Permiso padre in estado.Permisos)
            {
                foreach (Permiso hijo in padre.ObtenerHijos())
                    InsertarPar(conexion, transaccion, "INSERT dbo.PermisoHijo (IdPadre, IdHijo) VALUES (@Uno, @Dos)", padre.Id, hijo.Id);
            }
            foreach (Rol rol in estado.Roles)
            {
                using SqlCommand comando = new SqlCommand("INSERT dbo.Rol (Id, IdFamilia, IdUsuarioPersonal) VALUES (@Id, @Familia, @Usuario)", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = rol.Id;
                comando.Parameters.Add("@Familia", SqlDbType.Int).Value = rol.Familia.Id;
                comando.Parameters.Add("@Usuario", SqlDbType.Int).Value = (object?)rol.IdUsuarioPersonal ?? DBNull.Value;
                comando.ExecuteNonQuery();
            }
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
