using System.Data;
using Microsoft.Data.SqlClient;
using SIGAT.BE;

namespace SIGAT.DAL
{
    public class UsuarioDAL
    {
        private readonly string? cadena;
        private readonly IntegridadDAL integridad;
        public UsuarioDAL(string? cadenaConexion = null) { cadena = cadenaConexion; integridad = new IntegridadDAL(cadenaConexion); }
        private SqlConnection Conectar()
        {
            return cadena == null ? ConexionBD.ObtenerConexion() : new SqlConnection(cadena);
        }

        public Usuario? ObtenerPorNombreUsuario(string nombreUsuario)
        {
            return integridad.LeerVerificado((conexion, transaccion) =>
            {
                using SqlCommand comando = new SqlCommand(@"SELECT IdUsuario,NombreUsuario,Password,Nombre,Apellido,Activo,DigitoVerificador
                    FROM dbo.Usuarios WHERE NombreUsuario=@Usuario", conexion, transaccion);
                comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = nombreUsuario;
                using SqlDataReader lector = comando.ExecuteReader();
                return lector.Read() ? MapearUsuario(lector) : null;
            });
        }

        public List<Usuario> ObtenerTodos()
        {
            return integridad.LeerVerificado((conexion, transaccion) =>
            {
                List<Usuario> usuarios = new List<Usuario>();
                using SqlCommand comando = new SqlCommand(@"SELECT IdUsuario,NombreUsuario,Password,Nombre,Apellido,Activo,DigitoVerificador
                    FROM dbo.Usuarios ORDER BY NombreUsuario", conexion, transaccion);
                using SqlDataReader lector = comando.ExecuteReader();
                while (lector.Read()) usuarios.Add(MapearUsuario(lector));
                return usuarios;
            });
        }

        // La transacción confirma el alta completa o revierte sus cambios. La cuenta inicial es un caso separado del alta común.
        public void Insertar(Usuario usuario, bool administradorInicial = false)
        {
            integridad.Cambiar(administradorInicial ? new[] { "Usuarios", "UsuarioRol" } : new[] { "Usuarios" }, (conexion, transaccion) =>
            {
                BloquearSeguridad(conexion, transaccion);
                if (administradorInicial)
                {
                    using SqlCommand comprobar = new SqlCommand("SELECT COUNT(*) FROM dbo.Usuarios WITH (UPDLOCK,HOLDLOCK)", conexion, transaccion);
                    if ((int)comprobar.ExecuteScalar() != 0)
                        throw new InvalidOperationException("El administrador inicial solo puede crearse cuando no existen cuentas.");
                }
                using SqlCommand comando = new SqlCommand(@"INSERT dbo.Usuarios (NombreUsuario,Password,Nombre,Apellido,Activo)
                    OUTPUT INSERTED.IdUsuario VALUES (@Usuario,@Password,@Nombre,@Apellido,@Activo)", conexion, transaccion);
                Parametros(comando, usuario);
                comando.Parameters.Add("@Password", SqlDbType.VarChar, 256).Value = usuario.Password;
                int id = Convert.ToInt32(comando.ExecuteScalar());
                if (administradorInicial)
                {
                    using SqlCommand asignar = new SqlCommand("INSERT dbo.UsuarioRol (IdUsuario,IdRol) VALUES (@Id,1)", conexion, transaccion);
                    asignar.Parameters.Add("@Id", SqlDbType.Int).Value = id;
                    asignar.ExecuteNonQuery();
                }
                usuario.IdUsuario = id;
                return 0;
            });
        }

        // La protección del administrador se verifica también en el DAL. Deshabilitar un botón no alcanza para proteger los datos.
        public void Actualizar(Usuario usuario)
        {
            if (!usuario.Activo && EsAdministradorOriginal(usuario.IdUsuario))
                throw new InvalidOperationException("No se puede desactivar al administrador original.");
            integridad.Cambiar("Usuarios", (conexion, transaccion) =>
            {
                BloquearSeguridad(conexion, transaccion);
                using SqlCommand comando = new SqlCommand(@"UPDATE dbo.Usuarios SET NombreUsuario=@Usuario,
                    Password=CASE WHEN @Password='' THEN Password ELSE @Password END,
                    Nombre=@Nombre,Apellido=@Apellido,Activo=@Activo,DigitoVerificador=NULL WHERE IdUsuario=@Id", conexion, transaccion);
                Parametros(comando, usuario);
                comando.Parameters.Add("@Password", SqlDbType.VarChar, 256).Value = usuario.Password ?? "";
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = usuario.IdUsuario;
                if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El usuario ya no existe.");
                if (!usuario.Activo) ExigirAdministradorActivo(conexion, transaccion);
                return 0;
            });
        }

        // La baja es lógica: actualizamos Activo a cero. No borramos físicamente la cuenta.
        public void Eliminar(int idUsuario)
        {
            if (EsAdministradorOriginal(idUsuario))
                throw new InvalidOperationException("No se puede dar de baja al administrador original.");
            integridad.Cambiar("Usuarios", (conexion, transaccion) =>
            {
                BloquearSeguridad(conexion, transaccion);
                using SqlCommand comando = new SqlCommand("UPDATE dbo.Usuarios SET Activo=0,DigitoVerificador=NULL WHERE IdUsuario=@Id", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
                if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El usuario ya no existe.");
                ExigirAdministradorActivo(conexion, transaccion);
                return 0;
            });
        }

        public bool EsAdministradorOriginal(int idUsuario)
        {
            using SqlConnection conexion = Conectar();
            conexion.Open();
            using SqlCommand comando = new SqlCommand("SELECT COUNT(*) FROM dbo.AdministradorOriginal WHERE IdUsuario=@Id", conexion);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
            return (int)comando.ExecuteScalar() > 0;
        }

        private static void BloquearSeguridad(SqlConnection conexion, SqlTransaction transaccion)
        {
            // Mismo bloqueo que el guardado de roles: evita conflictos y bajas simultáneas.
            using SqlCommand comando = new SqlCommand("UPDATE dbo.SeguridadVersion WITH (UPDLOCK) SET Version=Version+1 WHERE Id=1", conexion, transaccion);
            if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("Falta instalar la matriz de roles.");
        }

        private static void ExigirAdministradorActivo(SqlConnection conexion, SqlTransaction transaccion)
        {
            using SqlCommand comando = new SqlCommand(@"SELECT COUNT(*) FROM dbo.Usuarios u
                JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
                JOIN dbo.RolPermiso rp ON rp.IdRol=ur.IdRol
                WHERE u.Activo=1 AND rp.IdPermiso=7", conexion, transaccion);
            if ((int)comando.ExecuteScalar() == 0)
                throw new InvalidOperationException("No se puede dar de baja al último administrador activo.");
        }

        // Los datos viajan como parámetros, separados del texto SQL. Así no se interpretan como instrucciones de la consulta.
        private static void Parametros(SqlCommand comando, Usuario usuario)
        {
            comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario.NombreUsuario;
            comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = usuario.Nombre;
            comando.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = usuario.Apellido;
            comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = usuario.Activo;
        }

        // Convertimos una fila SQL en una entidad. La carga de roles se realiza aparte para no mezclar cuenta y catálogo.
        private static Usuario MapearUsuario(SqlDataReader lector)
        {
            return new Usuario
            {
                IdUsuario=lector.GetInt32(0), NombreUsuario=lector.GetString(1), Password=lector.GetString(2),
                Nombre=lector.GetString(3), Apellido=lector.GetString(4), Activo=lector.GetBoolean(5), DigitoVerificador=lector.IsDBNull(6)?null:lector.GetString(6)
            };
        }
    }
}
