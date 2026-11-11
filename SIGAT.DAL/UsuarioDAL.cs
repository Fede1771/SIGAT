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
        public void Insertar(Usuario usuario, bool administradorInicial = false, AutorCambioUsuario? autor = null)
        {
            usuario.IdUsuario = integridad.Cambiar(administradorInicial ? new[] { "Usuarios", "UsuarioRol" } : new[] { "Usuarios" }, (conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                AvanzarVersionSeguridad(conexion, transaccion);
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
                HistorialUsuarioDAL.Registrar(conexion, transaccion, id,
                    HistorialUsuarioDAL.Capturar(conexion, transaccion, id)[id], OperacionUsuarioVersion.Alta, autor);
                return id;
            });
        }

        // La protección del administrador se verifica también en el DAL. Deshabilitar un botón no alcanza para proteger los datos.
        public void Actualizar(Usuario usuario, AutorCambioUsuario? autor = null)
        {
            if (!usuario.Activo && EsAdministradorOriginal(usuario.IdUsuario))
                throw new InvalidOperationException("No se puede desactivar al administrador original.");
            integridad.Cambiar("Usuarios", (conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                AvanzarVersionSeguridad(conexion, transaccion);
                var anterior = LeerEstadoActual(conexion, transaccion, usuario.IdUsuario);
                bool cambioClave = !string.IsNullOrEmpty(usuario.Password) &&
                    CambioClave(conexion, transaccion, usuario.IdUsuario, usuario.Password);
                using SqlCommand comando = new SqlCommand(@"UPDATE dbo.Usuarios SET NombreUsuario=@Usuario,
                    Password=CASE WHEN @Password='' THEN Password ELSE @Password END,
                    Nombre=@Nombre,Apellido=@Apellido,Activo=@Activo,DigitoVerificador=NULL WHERE IdUsuario=@Id", conexion, transaccion);
                Parametros(comando, usuario);
                comando.Parameters.Add("@Password", SqlDbType.VarChar, 256).Value = usuario.Password ?? "";
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = usuario.IdUsuario;
                if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El usuario ya no existe.");
                if (!usuario.Activo) ExigirAdministradorActivo(conexion, transaccion);
                var actual = LeerEstadoActual(conexion, transaccion, usuario.IdUsuario);
                if (cambioClave || !anterior.CoincideCon(actual))
                {
                    var operacion = anterior.Activo == actual.Activo ? OperacionUsuarioVersion.Edicion
                        : actual.Activo ? OperacionUsuarioVersion.Reactivacion : OperacionUsuarioVersion.Baja;
                    HistorialUsuarioDAL.Registrar(conexion, transaccion, usuario.IdUsuario, actual, operacion, autor, cambioClave);
                }
                return 0;
            });
        }

        // La baja es lógica: actualizamos Activo a cero. No borramos físicamente la cuenta.
        public void Eliminar(int idUsuario, AutorCambioUsuario? autor = null)
        {
            if (EsAdministradorOriginal(idUsuario))
                throw new InvalidOperationException("No se puede dar de baja al administrador original.");
            integridad.Cambiar("Usuarios", (conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                AvanzarVersionSeguridad(conexion, transaccion);
                var anterior = LeerEstadoActual(conexion, transaccion, idUsuario);
                using SqlCommand comando = new SqlCommand("UPDATE dbo.Usuarios SET Activo=0,DigitoVerificador=NULL WHERE IdUsuario=@Id", conexion, transaccion);
                comando.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
                if (comando.ExecuteNonQuery() != 1) throw new InvalidOperationException("El usuario ya no existe.");
                ExigirAdministradorActivo(conexion, transaccion);
                if (anterior.Activo)
                    HistorialUsuarioDAL.Registrar(conexion, transaccion, idUsuario, anterior with { Activo = false },
                        OperacionUsuarioVersion.Baja, autor);
                return 0;
            });
        }

        public IReadOnlyList<CuentaConHistorial> ListarCuentasConHistorial() =>
            integridad.LeerVerificado((conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                return HistorialUsuarioDAL.ListarCuentas(conexion, transaccion);
            });

        public HistorialUsuario ConsultarHistorial(int idUsuario) =>
            integridad.LeerVerificado((conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                using var version = new SqlCommand("SELECT Version FROM dbo.SeguridadVersion WHERE Id=1", conexion, transaccion);
                return new HistorialUsuario(Convert.ToInt64(version.ExecuteScalar()),
                    HistorialUsuarioDAL.Capturar(conexion, transaccion, idUsuario).GetValueOrDefault(idUsuario),
                    HistorialUsuarioDAL.Consultar(conexion, transaccion, idUsuario));
            });

        public void RestaurarVersion(int idUsuario, int numero, long versionSeguridad, AutorCambioUsuario autor)
        {
            integridad.Cambiar(new[] { "Usuarios", "UsuarioRol" }, (conexion, transaccion) =>
            {
                HistorialUsuarioDAL.ExigirEsquema(conexion, transaccion);
                AvanzarVersionSeguridad(conexion, transaccion, versionSeguridad);
                var anterior = LeerEstadoActual(conexion, transaccion, idUsuario);
                var version = HistorialUsuarioDAL.Consultar(conexion, transaccion, idUsuario, numero).SingleOrDefault();
                if (version?.Existe != true)
                    throw new InvalidOperationException("La versión no contiene una cuenta que pueda restaurarse.");
                var destino = version.Estado;
                if (anterior.CoincideCon(destino))
                    throw new InvalidOperationException("La cuenta ya tiene los datos y roles de esa versión.");

                using var original = new SqlCommand("SELECT IdUsuario FROM dbo.AdministradorOriginal WHERE Id=1", conexion, transaccion);
                if (Convert.ToInt32(original.ExecuteScalar()) == idUsuario &&
                    (!destino.Activo || !destino.Roles.Any(rol => rol.Id == 1)))
                    throw new InvalidOperationException("El administrador original debe permanecer activo y conservar el rol Administrador.");

                using var actualizar = new SqlCommand("""
                    IF EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario=@Usuario AND IdUsuario<>@Id)
                        THROW 50001, 'El nombre de usuario de esa versión ya está en uso.', 1;
                    UPDATE dbo.Usuarios SET NombreUsuario=@Usuario, Nombre=@Nombre, Apellido=@Apellido,
                        Activo=@Activo, DigitoVerificador=NULL WHERE IdUsuario=@Id;
                    """, conexion, transaccion);
                Parametros(actualizar, destino.NombreUsuario, destino.Nombre, destino.Apellido, destino.Activo);
                actualizar.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
                actualizar.ExecuteNonQuery();

                if (!anterior.MismosRolesQue(destino))
                    RestaurarRoles(conexion, transaccion, idUsuario, destino.Roles);
                ExigirAdministradorActivo(conexion, transaccion);
                HistorialUsuarioDAL.Registrar(conexion, transaccion, idUsuario,
                    LeerEstadoActual(conexion, transaccion, idUsuario), OperacionUsuarioVersion.Restauracion, autor,
                    versionOrigen: numero);
                return 0;
            });
        }

        private static void RestaurarRoles(SqlConnection conexion, SqlTransaction transaccion, int idUsuario,
            IReadOnlyList<RolVersionUsuario> roles)
        {
            using var quitar = new SqlCommand("DELETE dbo.UsuarioRol WHERE IdUsuario=@Id", conexion, transaccion);
            quitar.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
            quitar.ExecuteNonQuery();
            using var asignar = new SqlCommand("""
                INSERT dbo.UsuarioRol(IdUsuario,IdRol) SELECT @Id, Id FROM dbo.Rol WHERE Id=@Rol
                """, conexion, transaccion);
            asignar.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
            var idRol = asignar.Parameters.Add("@Rol", SqlDbType.Int);
            foreach (var rol in roles)
            {
                idRol.Value = rol.Id;
                if (asignar.ExecuteNonQuery() != 1)
                    throw new InvalidOperationException($"El rol {rol.Nombre} de esa versión ya no existe.");
            }
        }

        private static EstadoUsuarioVersion LeerEstadoActual(SqlConnection conexion, SqlTransaction transaccion, int id) =>
            HistorialUsuarioDAL.Capturar(conexion, transaccion, id).GetValueOrDefault(id)
            ?? throw new InvalidOperationException("El usuario ya no existe.");

        private static bool CambioClave(SqlConnection conexion, SqlTransaction transaccion, int id, string nueva)
        {
            using var comando = new SqlCommand("SELECT Password FROM dbo.Usuarios WHERE IdUsuario=@Id", conexion, transaccion);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            return !string.Equals((string?)comando.ExecuteScalar(), nueva, StringComparison.Ordinal);
        }

        public bool EsAdministradorOriginal(int idUsuario)
        {
            using SqlConnection conexion = Conectar();
            conexion.Open();
            using SqlCommand comando = new SqlCommand("SELECT COUNT(*) FROM dbo.AdministradorOriginal WHERE IdUsuario=@Id", conexion);
            comando.Parameters.Add("@Id", SqlDbType.Int).Value = idUsuario;
            return (int)comando.ExecuteScalar() > 0;
        }

        internal static void AvanzarVersionSeguridad(SqlConnection conexion, SqlTransaction transaccion, long? esperada = null)
        {
            // Mismo bloqueo que el guardado de roles: evita conflictos y bajas simultáneas.
            using var comando = new SqlCommand("""
                UPDATE dbo.SeguridadVersion WITH (UPDLOCK) SET Version=Version+1
                WHERE Id=1 AND (@Esperada IS NULL OR Version=@Esperada)
                """, conexion, transaccion);
            comando.Parameters.Add("@Esperada", SqlDbType.BigInt).Value = (object?)esperada ?? DBNull.Value;
            if (comando.ExecuteNonQuery() != 1)
                throw new InvalidOperationException(esperada.HasValue
                    ? "Otra operación modificó los datos de seguridad. Recargue antes de continuar."
                    : "Falta instalar la matriz de roles.");
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
        private static void Parametros(SqlCommand comando, Usuario usuario) =>
            Parametros(comando, usuario.NombreUsuario, usuario.Nombre, usuario.Apellido, usuario.Activo);

        private static void Parametros(SqlCommand comando, string usuario, string nombre, string apellido, bool activo)
        {
            comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = usuario;
            comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = nombre;
            comando.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = apellido;
            comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = activo;
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
