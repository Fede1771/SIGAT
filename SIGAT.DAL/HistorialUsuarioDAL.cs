using System.Data;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using SIGAT.BE;

namespace SIGAT.DAL;

internal static class HistorialUsuarioDAL
{
    internal static void ExigirEsquema(SqlConnection conexion, SqlTransaction transaccion)
    {
        using var comando = new SqlCommand("SELECT OBJECT_ID(N'dbo.UsuarioVersion', N'U')", conexion, transaccion);
        if (comando.ExecuteScalar() is DBNull)
            throw new InvalidOperationException("Falta instalar el historial de usuarios. Ejecute Querys/02_Actualizar_Base_Existente.sql.");
    }

    internal static Dictionary<int, EstadoUsuarioVersion> Capturar(SqlConnection conexion, SqlTransaction transaccion, int? id = null)
    {
        using var comando = new SqlCommand("""
            SELECT u.IdUsuario, u.NombreUsuario, u.Nombre, u.Apellido, u.Activo,
                (SELECT r.Id, r.Nombre FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.Id=ur.IdRol
                 WHERE ur.IdUsuario=u.IdUsuario ORDER BY r.Id FOR JSON PATH)
            FROM dbo.Usuarios u WHERE @Id IS NULL OR u.IdUsuario=@Id
            """, conexion, transaccion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = (object?)id ?? DBNull.Value;
        using var lector = comando.ExecuteReader();
        var estados = new Dictionary<int, EstadoUsuarioVersion>();
        while (lector.Read())
            estados.Add(lector.GetInt32(0), LeerEstado(lector, 1));
        return estados;
    }

    internal static void Registrar(SqlConnection conexion, SqlTransaction transaccion, int id,
        EstadoUsuarioVersion estado, OperacionUsuarioVersion operacion, AutorCambioUsuario? autor,
        bool cambioClave = false, int? versionOrigen = null, bool existe = true)
    {
        using var comando = new SqlCommand("""
            INSERT dbo.UsuarioVersion (IdUsuario, Version, IdActor, Actor, Operacion,
                NombreUsuario, Nombre, Apellido, Activo, Roles, Existe, CambioClave, VersionOrigen)
            SELECT @Id, ISNULL(MAX(Version), 0) + 1, @IdActor, COALESCE(@Actor, ORIGINAL_LOGIN()), @Operacion,
                @Usuario, @Nombre, @Apellido, @Activo, @Roles, @Existe, @CambioClave, @VersionOrigen
            FROM dbo.UsuarioVersion WITH (UPDLOCK, HOLDLOCK) WHERE IdUsuario=@Id
            """, conexion, transaccion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        comando.Parameters.Add("@IdActor", SqlDbType.Int).Value = (object?)autor?.IdUsuario ?? DBNull.Value;
        comando.Parameters.Add("@Actor", SqlDbType.NVarChar, 256).Value = (object?)autor?.Nombre ?? DBNull.Value;
        comando.Parameters.Add("@Operacion", SqlDbType.VarChar, 24).Value = operacion.ToString();
        comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = estado.NombreUsuario;
        comando.Parameters.Add("@Nombre", SqlDbType.VarChar, 100).Value = estado.Nombre;
        comando.Parameters.Add("@Apellido", SqlDbType.VarChar, 100).Value = estado.Apellido;
        comando.Parameters.Add("@Activo", SqlDbType.Bit).Value = estado.Activo;
        comando.Parameters.Add("@Roles", SqlDbType.NVarChar, -1).Value = JsonSerializer.Serialize(estado.Roles);
        comando.Parameters.Add("@Existe", SqlDbType.Bit).Value = existe;
        comando.Parameters.Add("@CambioClave", SqlDbType.Bit).Value = cambioClave;
        comando.Parameters.Add("@VersionOrigen", SqlDbType.Int).Value = (object?)versionOrigen ?? DBNull.Value;
        comando.ExecuteNonQuery();
    }

    internal static List<CuentaConHistorial> ListarCuentas(SqlConnection conexion, SqlTransaction transaccion)
    {
        using var comando = new SqlCommand("""
            WITH Ultimas AS (
                SELECT IdUsuario, NombreUsuario, Existe, ROW_NUMBER() OVER (PARTITION BY IdUsuario ORDER BY Version DESC) AS Fila
                FROM dbo.UsuarioVersion
            )
            SELECT IdUsuario, NombreUsuario, Existe FROM Ultimas WHERE Fila=1 ORDER BY NombreUsuario, IdUsuario
            """, conexion, transaccion);
        using var lector = comando.ExecuteReader();
        var cuentas = new List<CuentaConHistorial>();
        while (lector.Read())
            cuentas.Add(new(lector.GetInt32(0), lector.GetString(1), lector.GetBoolean(2)));
        return cuentas;
    }

    internal static List<UsuarioVersion> Consultar(SqlConnection conexion, SqlTransaction transaccion, int id, int? numero = null)
    {
        using var comando = new SqlCommand("""
            SELECT Version, FechaUtc, IdActor, Actor, Operacion, Existe, CambioClave, VersionOrigen,
                NombreUsuario, Nombre, Apellido, Activo, Roles
            FROM dbo.UsuarioVersion WHERE IdUsuario=@Id AND (@Numero IS NULL OR Version=@Numero)
            ORDER BY Version DESC
            """, conexion, transaccion);
        comando.Parameters.Add("@Id", SqlDbType.Int).Value = id;
        comando.Parameters.Add("@Numero", SqlDbType.Int).Value = (object?)numero ?? DBNull.Value;
        using var lector = comando.ExecuteReader();
        var versiones = new List<UsuarioVersion>();
        while (lector.Read())
            versiones.Add(new(id, lector.GetInt32(0), DateTime.SpecifyKind(lector.GetDateTime(1), DateTimeKind.Utc),
                new(lector.IsDBNull(2) ? null : lector.GetInt32(2), lector.GetString(3)),
                Enum.Parse<OperacionUsuarioVersion>(lector.GetString(4)), LeerEstado(lector, 8),
                lector.GetBoolean(5), lector.GetBoolean(6), lector.IsDBNull(7) ? null : lector.GetInt32(7)));
        return versiones;
    }

    private static EstadoUsuarioVersion LeerEstado(SqlDataReader lector, int columna) =>
        new(lector.GetString(columna), lector.GetString(columna + 1), lector.GetString(columna + 2),
            lector.GetBoolean(columna + 3), lector.IsDBNull(columna + 4) ? [] :
                JsonSerializer.Deserialize<RolVersionUsuario[]>(lector.GetString(columna + 4))!);
}
