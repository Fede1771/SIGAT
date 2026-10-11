using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace SIGAT.SERVICIOS;

public static class VerificadorSHA256
{
    // Longitud + valor evita confundir campos que contienen separadores.
    public static string Canonico(params object?[] valores) => string.Concat(valores.Select(valor =>
    {
        if (valor == null || valor == DBNull.Value) return "-1:";
        string texto = valor is DateTime fecha ? fecha.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture)
            : Convert.ToString(valor, CultureInfo.InvariantCulture) ?? "";
        return texto.Length.ToString(CultureInfo.InvariantCulture) + ":" + texto;
    }));

    public static string Hexadecimal(string texto) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(texto))).ToLowerInvariant();
    public static string Base64(string texto) => Convert.ToBase64String(SHA256.HashData(Encoding.UTF8.GetBytes(texto)));

    public static string Traduccion(int idioma, int control, string? texto, string estado)
        => Hexadecimal(Canonico(idioma, control, texto, estado));

    public static string Bitacora(int id, DateTime fecha, string? usuario, string? actividad, string? cifrado)
        => Base64(Canonico(id, fecha, usuario, actividad, cifrado));
}
