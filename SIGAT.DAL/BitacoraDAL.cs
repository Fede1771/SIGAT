using System.Data;
using System.Data.SqlTypes;
using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BE.Auditoria;
using SIGAT.SERVICIOS;

namespace SIGAT.DAL;

public class BitacoraDAL
{
    private readonly string? cadena;
    private SqlConnection Conectar() => cadena == null ? ConexionBD.ObtenerConexion() : new SqlConnection(cadena);
    private readonly EncriptadorServicio encriptador = new();
    public BitacoraDAL(string? cadenaConexion = null) => cadena = cadenaConexion;

    // El detalle sigue cifrado con AES. La bitácora no depende del control de integridad para poder auditar incidentes.
    public void Insertar(Bitacora registro)
    {
        using var conexion = Conectar(); conexion.Open();
        using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable);
        registro.Fecha = new SqlDateTime(registro.Fecha).Value;
        string? cifrado = encriptador.Encriptar(registro.InformacionAsociada);
        using var comando = new SqlCommand(@"INSERT dbo.Bitacora(Fecha,Usuario,Actividad,InformacionAsociada)
            OUTPUT INSERTED.IdBitacora VALUES(@Fecha,@Usuario,@Actividad,@Info)", conexion, transaccion);
        comando.Parameters.Add("@Fecha", SqlDbType.DateTime).Value = registro.Fecha;
        comando.Parameters.Add("@Usuario", SqlDbType.VarChar, 50).Value = (object?)registro.Usuario ?? DBNull.Value;
        comando.Parameters.Add("@Actividad", SqlDbType.VarChar, 255).Value = (object?)registro.Actividad ?? DBNull.Value;
        comando.Parameters.Add("@Info", SqlDbType.NVarChar, -1).Value = (object?)cifrado ?? DBNull.Value;
        registro.IdBitacora = Convert.ToInt32(comando.ExecuteScalar());
        transaccion.Commit();

    }

    // Desciframos los detalles de auditoría al consultarlos; ya no se comprueba DVH/DVV sobre esta tabla.
    public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        => Buscar(desde, hasta, usuario, actividad, actividadExacta: false);

    // El desplegable reúne equivalencias conocidas mediante igualdad, sin confundir actividades con nombres parecidos.
    public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad, bool actividadExacta)
    {
        using var conexion = Conectar(); conexion.Open();
        var lista = new List<Bitacora>();
        // El usuario se busca como texto: sus caracteres no son comodines de SQL.
        string? usuarioLiteral = usuario?.Trim().Replace("~", "~~").Replace("%", "~%")
            .Replace("_", "~_").Replace("[", "~[");
        var variantes = actividadExacta ? ActividadesBitacora.VariantesParaFiltro(actividad) : Array.Empty<string>();
        string[] parametros = Enumerable.Range(0, variantes.Count).Select(i => "@Variante" + i).ToArray();
        // Solo los nombres de parámetros se incorporan al SQL; los valores siempre viajan parametrizados.
        string coincidencia = parametros.Length == 0 ? "Actividad=@Actividad" : "Actividad IN (" + string.Join(",", parametros) + ")";
        using var comando = new SqlCommand(@$"SELECT IdBitacora,Fecha,Usuario,Actividad,InformacionAsociada
            FROM dbo.Bitacora WHERE (@Desde IS NULL OR Fecha>=@Desde) AND (@Hasta IS NULL OR Fecha<=@Hasta)
            AND (@Usuario IS NULL OR Usuario LIKE '%'+@Usuario+'%' ESCAPE '~')
            AND (@Actividad IS NULL OR (@Exacta=1 AND {coincidencia})
                OR (@Exacta=0 AND Actividad LIKE '%'+@Actividad+'%')) ORDER BY Fecha DESC", conexion);
        comando.Parameters.Add("@Exacta", SqlDbType.Bit).Value = actividadExacta;
        for (int i = 0; i < variantes.Count; i++) comando.Parameters.Add(parametros[i], SqlDbType.VarChar, 255).Value = variantes[i];
        comando.Parameters.Add("@Desde", SqlDbType.DateTime).Value = (object?)desde ?? DBNull.Value;
        comando.Parameters.Add("@Hasta", SqlDbType.DateTime).Value = (object?)hasta ?? DBNull.Value;
        comando.Parameters.Add("@Usuario", SqlDbType.VarChar, -1).Value = string.IsNullOrEmpty(usuarioLiteral) ? DBNull.Value : usuarioLiteral;
        comando.Parameters.Add("@Actividad", SqlDbType.VarChar, 255).Value = string.IsNullOrEmpty(actividad) ? DBNull.Value : actividad;
        using var lector = comando.ExecuteReader();
        while (lector.Read()) lista.Add(new Bitacora
        {
            IdBitacora = lector.GetInt32(0), Fecha = lector.GetDateTime(1), Usuario = lector.GetString(2),
            Actividad = lector.GetString(3), InformacionAsociada = encriptador.Desencriptar(lector.IsDBNull(4) ? null : lector.GetString(4))
        });
        return lista;
    }

    // Consultamos los tipos registrados sin cargar ni descifrar todos los eventos.
    public List<string> ObtenerActividades()
    {
        using var conexion = Conectar(); conexion.Open();
        using var comando = new SqlCommand(@"SELECT DISTINCT Actividad FROM dbo.Bitacora
            WHERE Actividad IS NOT NULL AND LTRIM(RTRIM(Actividad))<>'' ORDER BY Actividad", conexion);
        using var lector = comando.ExecuteReader();
        var actividades = new List<string>();
        while (lector.Read()) actividades.Add(lector.GetString(0));
        return actividades;
    }

}
