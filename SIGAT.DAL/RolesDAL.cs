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
                IF OBJECT_ID('dbo.RolHijo','U') IS NULL
                    THROW 50001, 'Ejecute Querys/13_Roles_Editables_Admin_Protegido.sql antes de iniciar SIGAT.', 1;
                SELECT Version FROM dbo.SeguridadVersion WITH (HOLDLOCK) WHERE Id = 1;", conexion, transaccion))
            {
                estado.Version = Convert.ToInt64(comando.ExecuteScalar());
            }
            using (SqlCommand comando = new SqlCommand("SELECT IdUsuario FROM dbo.AdministradorOriginal WHERE Id=1", conexion, transaccion))
                estado.IdAdministradorOriginal = Convert.ToInt32(comando.ExecuteScalar());
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
            using (SqlCommand comando = new SqlCommand("SELECT IdPadre,IdHijo FROM dbo.RolHijo", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
                while (lector.Read()) roles[lector.GetInt32(0)].AgregarRol(roles[lector.GetInt32(1)]);
            Dictionary<int, Usuario> usuarios = new Dictionary<int, Usuario>();
            using (SqlCommand comando = new SqlCommand(@"SELECT u.IdUsuario, u.NombreUsuario, u.Activo
                FROM dbo.Usuarios u ORDER BY u.NombreUsuario", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
            {
                while (lector.Read())
                {
                    Usuario usuario = new Usuario
                    {
                        IdUsuario = lector.GetInt32(0), NombreUsuario = lector.GetString(1),
                        Activo = lector.GetBoolean(2)

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
            // Catálogo, jerarquía y asignaciones se guardan juntos.
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
            int protegido;
            using (SqlCommand comando = new SqlCommand("SELECT IdUsuario FROM dbo.AdministradorOriginal WHERE Id=1", conexion, transaccion))
                protegido = Convert.ToInt32(comando.ExecuteScalar());
            if (!estado.Usuarios.Any(u => u.IdUsuario == protegido && u.Activo && u.Roles.Any(r => r.Id == 1)))
                throw new InvalidOperationException("No se puede quitar el rol Administrador a la cuenta original.");
            Ejecutar(conexion, transaccion, "DELETE FROM dbo.UsuarioRol; DELETE FROM dbo.RolHijo; DELETE FROM dbo.RolPermiso;");
            foreach (Rol rol in estado.Roles)
            {
                using SqlCommand comando = new SqlCommand(@"UPDATE dbo.Rol SET Nombre=@Nombre WHERE Id=@Id;
                    IF @@ROWCOUNT=0 INSERT dbo.Rol(Id,Nombre) VALUES (@Id,@Nombre)", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = rol.Id;
                comando.Parameters.Add("@Nombre", SqlDbType.NVarChar,150).Value = rol.Nombre;
                comando.ExecuteNonQuery();
            }
            var existentes = new List<int>();
            using (SqlCommand comando = new SqlCommand("SELECT Id FROM dbo.Rol", conexion, transaccion))
            using (SqlDataReader lector = comando.ExecuteReader())
                while (lector.Read()) existentes.Add(lector.GetInt32(0));
            foreach (int id in existentes.Where(id => !estado.Roles.Any(r => r.Id == id)))
            {
                using SqlCommand comando = new SqlCommand("DELETE dbo.Rol WHERE Id=@Id", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                comando.ExecuteNonQuery();
            }
            foreach (Rol rol in estado.Roles)
            {
                foreach (Permiso permiso in rol.Permisos)
                    InsertarPar(conexion, transaccion, "INSERT dbo.RolPermiso VALUES (@Uno,@Dos)", rol.Id, permiso.Id);
                foreach (Rol hijo in rol.Roles)
                    InsertarPar(conexion, transaccion, "INSERT dbo.RolHijo VALUES (@Uno,@Dos)", rol.Id, hijo.Id);
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
