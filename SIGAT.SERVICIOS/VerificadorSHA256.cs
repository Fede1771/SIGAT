using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SIGAT.SERVICIOS;

public static class VerificadorSHA256
{
    // Longitud + valor evita confundir campos que contienen separadores. También distinguimos null de texto vacío y fijamos el formato de fechas.
    public static string Canonico(params object?[] valores) => string.Concat(valores.Select(valor =>
    {
        if (valor == null || valor == DBNull.Value) return "-1:";
        string texto = valor is DateTime fecha ? fecha.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
            : Convert.ToString(valor, CultureInfo.InvariantCulture) ?? "";
        return texto.Length.ToString(CultureInfo.InvariantCulture) + ":" + texto;
    }));

    public static string Hexadecimal(string texto) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto))).ToLowerInvariant();
    public static string Base64(string texto) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(texto)));

    // La cuenta y sus asignaciones se verifican por separado: identidad y autorización tienen claves distintas.
    public static string Usuario(int idUsuario, string? nombreUsuario, string? password, string? nombre, string? apellido, bool activo)
        => Hexadecimal(Canonico(idUsuario, nombreUsuario, password, nombre, apellido, activo));

    public static string UsuarioRol(int idUsuario, int idRol)
        => Hexadecimal(Canonico(idUsuario, idRol));

}
