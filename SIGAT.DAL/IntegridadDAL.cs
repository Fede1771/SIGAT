using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using SIGAT.BE.Integridad;
using SIGAT.SERVICIOS;

namespace SIGAT.DAL;

public sealed class IntegridadDAL
{
    private readonly string? cadena;
    private static readonly object LogLock = new();
    private static readonly string[] Tablas = { "Bitacora", "Traduccion" };
    private static readonly Dictionary<string, string[]> Columnas = new()
    {
        ["Bitacora"] = new[] { "IdBitacora", "Fecha", "Usuario", "Actividad", "InformacionAsociada", "DigitoVerificador" },
        ["Traduccion"] = new[] { "Id_Idioma", "Id_Control", "Texto", "Estado", "DigitoVerificador" }
    };
    private sealed record Fila(string Tabla, object?[] Valores)
    {
        public string Clave => Tabla == "Bitacora" ? Convert.ToString(Valores[0], CultureInfo.InvariantCulture)!
            : $"{Valores[0]}|{Valores[1]}";
        public string? Guardado => Valores[^1] as string;
        public string Calculado => Tabla == "Bitacora"
            ? VerificadorSHA256.Bitacora((int)Valores[0]!, (DateTime)Valores[1]!, Valores[2] as string, Valores[3] as string, Valores[4] as string)
            : VerificadorSHA256.Traduccion((int)Valores[0]!, (int)Valores[1]!, Valores[2] as string, (string)Valores[3]!);
    }
    private sealed record Control(string Hash, long Cantidad, Guid Base, int Version);
    private sealed class Estado
    {
        public Dictionary<string, List<Fila>> Filas { get; } = new();
        public Dictionary<string, Control> Controles { get; } = new();
        public InformeIntegridad Informe { get; } = new();
    }

    public IntegridadDAL(string? cadenaConexion = null) => cadena = cadenaConexion;
    private SqlConnection Conectar() => cadena == null ? ConexionBD.ObtenerConexion() : new SqlConnection(cadena);
    private static string Id(string nombre) => "[" + nombre.Replace("]", "]]") + "]";
    private static string Literal(string valor) => "N'" + valor.Replace("'", "''") + "'";
    private static SqlCommand Comando(SqlConnection c, SqlTransaction? t, string sql) => new(sql, c, t) { CommandTimeout = 120 };
    private static void Ejecutar(SqlConnection c, SqlTransaction? t, string sql)
    {
        using var cmd = Comando(c, t, sql);
        cmd.ExecuteNonQuery();
    }
    private static void Bloquear(SqlConnection c, SqlTransaction t, bool escritura)
    {
        using var cmd = Comando(c, t, @"DECLARE @r int; EXEC @r=sys.sp_getapplock @Resource=N'SIGAT.Integridad',
            @LockMode=@Modo,@LockOwner='Transaction',@LockTimeout=10000;
            IF @r<0 THROW 50001,'No se pudo obtener el bloqueo de integridad.',1;");
        cmd.Parameters.AddWithValue("@Modo", escritura ? "Exclusive" : "Shared");
        cmd.ExecuteNonQuery();
    }
    private static bool TieneEstructura(SqlConnection c, SqlTransaction? t)
    {
        using var cmd = Comando(c, t, "SELECT CASE WHEN OBJECT_ID('dbo.IntegridadTabla','U') IS NULL THEN 0 ELSE 1 END");
        return (int)cmd.ExecuteScalar()! == 1;
    }
    private static List<Fila> LeerFilas(SqlConnection c, SqlTransaction t, string tabla)
    {
        var result = new List<Fila>();
        string orden = tabla == "Bitacora" ? "IdBitacora" : "Id_Idioma,Id_Control";
        using var cmd = Comando(c, t, $"SELECT {string.Join(",", Columnas[tabla].Select(Id))} FROM dbo.{Id(tabla)} WITH (HOLDLOCK) ORDER BY {orden}");
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            var values = new object?[reader.FieldCount];
            for (int i = 0; i < values.Length; i++) values[i] = reader.IsDBNull(i) ? null : reader.GetValue(i);
            result.Add(new Fila(tabla, values));
        }
        return result;
    }
    private static string Vertical(string tabla, List<Fila> filas) => VerificadorSHA256.Hexadecimal(
        VerificadorSHA256.Canonico(tabla, filas.Count) + string.Concat(filas.Select(f => VerificadorSHA256.Canonico(f.Clave, f.Calculado))));

    private static Estado Examinar(SqlConnection c, SqlTransaction t)
    {
        var estado = new Estado();
        if (!TieneEstructura(c, t))
        {
            estado.Informe.Problemas.Add(new("Configuración", "", "Falta instalar e inicializar el control de integridad."));
            return estado;
        }
        using (var cmd = Comando(c, t, "SELECT Tabla,HashVertical,Cantidad,IdBase,Version FROM dbo.IntegridadTabla WITH (HOLDLOCK)"))
        using (var reader = cmd.ExecuteReader())
            while (reader.Read()) estado.Controles.Add(reader.GetString(0), new(reader.GetString(1), reader.GetInt64(2), reader.GetGuid(3), reader.GetInt32(4)));
        estado.Informe.Configurada = estado.Controles.Count != 0;
        foreach (string tabla in Tablas)
        {
            var filas = LeerFilas(c, t, tabla);
            estado.Filas[tabla] = filas;
            foreach (var fila in filas)
            {
                if (!string.Equals(fila.Guardado, fila.Calculado, StringComparison.Ordinal))
                    estado.Informe.Problemas.Add(new(tabla, fila.Clave, fila.Guardado == null ? "Falta el dígito por registro (DVH)." : "El SHA-256 del registro no coincide."));
                if (tabla == "Bitacora" && fila.Valores[4] is string cifrado)
                {
                    try { new EncriptadorServicio().Desencriptar(cifrado); }
                    catch (Exception ex) when (ex is FormatException or System.Security.Cryptography.CryptographicException)
                    { estado.Informe.Problemas.Add(new(tabla, fila.Clave, "La información cifrada no se puede leer.")); }
                }
            }
            if (!estado.Controles.TryGetValue(tabla, out var control))
                estado.Informe.Problemas.Add(new(tabla, "Tabla", "Falta el control del conjunto (DVV)."));
            else if (control.Version != 1 || control.Cantidad != filas.Count || control.Hash != Vertical(tabla, filas))
                estado.Informe.Problemas.Add(new(tabla, "Tabla", "El conjunto de registros no coincide con el DVV guardado."));
        }
        if (estado.Controles.Count != 2 || estado.Controles.Values.Select(v => v.Base).Distinct().Count() != 1)
            estado.Informe.Problemas.Add(new("Configuración", "", "Los controles de integridad están incompletos o no corresponden a la misma base."));
        return estado;
    }

    public InformeIntegridad Verificar()
    {
        using var c = Conectar(); c.Open();
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, false);
        var estado = Examinar(c, t);
        t.Commit();
        return estado.Informe;
    }

    public T LeerVerificado<T>(Func<SqlConnection, SqlTransaction, T> leer)
    {
        using var c = Conectar(); c.Open();
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, false);
        Exigir(Examinar(c, t).Informe);
        T resultado = leer(c, t);
        t.Commit();
        return resultado;
    }

    public T Cambiar<T>(string tabla, Func<SqlConnection, SqlTransaction, T> cambiar)
    {
        if (!Tablas.Contains(tabla)) throw new ArgumentException("Tabla no protegida.");
        using var c = Conectar(); c.Open();
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, true);
        Exigir(Examinar(c, t).Informe);
        T resultado = cambiar(c, t);
        // Solo las filas nuevas/actualizadas por el comando autorizado reciben el nuevo DVH.
        foreach (var fila in LeerFilas(c, t, tabla))
            if (fila.Guardado == null) GuardarHash(c, t, fila);
        ActualizarVertical(c, t, tabla);
        Exigir(Examinar(c, t).Informe);
        t.Commit();
        return resultado;
    }

    private static void Exigir(InformeIntegridad informe)
    {
        if (!informe.Correcta) throw new IntegridadException(informe);
    }
    private static string Condicion(string tabla) => tabla == "Bitacora" ? "IdBitacora=@p0" : "Id_Idioma=@p0 AND Id_Control=@p1";
    private static void GuardarHash(SqlConnection c, SqlTransaction t, Fila fila)
    {
        using var cmd = Comando(c, t, $"UPDATE dbo.{Id(fila.Tabla)} SET DigitoVerificador=@Hash WHERE {Condicion(fila.Tabla)}");
        cmd.Parameters.AddWithValue("@Hash", fila.Calculado);
        cmd.Parameters.AddWithValue("@p0", fila.Valores[0]);
        if (fila.Tabla == "Traduccion") cmd.Parameters.AddWithValue("@p1", fila.Valores[1]);
        cmd.ExecuteNonQuery();
    }
    private static void ActualizarVertical(SqlConnection c, SqlTransaction t, string tabla)
    {
        var filas = LeerFilas(c, t, tabla);
        using var cmd = Comando(c, t, "UPDATE dbo.IntegridadTabla SET HashVertical=@Hash,Cantidad=@Cantidad WHERE Tabla=@Tabla");
        cmd.Parameters.AddWithValue("@Hash", Vertical(tabla, filas));
        cmd.Parameters.AddWithValue("@Cantidad", (long)filas.Count);
        cmd.Parameters.AddWithValue("@Tabla", tabla);
        if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("Falta el control de integridad de " + tabla);
    }

    public bool PuedeAdministrar()
    {
        using var c = Conectar(); c.Open();
        return EsAdministrador(c);
    }

    public int ArchivarBitacora(int dias)
    {
        if (dias < 1) throw new ArgumentException("La retención debe ser de al menos un día.");
        using (var c = Conectar()) { c.Open(); ExigirAdministrador(c); }
        return Cambiar("Bitacora", (c, t) =>
        {
            using var cmd = Comando(c, t, @"IF OBJECT_ID('dbo.Bitacora_Historico','U') IS NULL
                CREATE TABLE dbo.Bitacora_Historico(IdBitacora int NOT NULL PRIMARY KEY,Fecha datetime NOT NULL,
                    Usuario varchar(50) NOT NULL,Actividad varchar(255) NOT NULL,
                    InformacionAsociada nvarchar(max) NULL,DigitoVerificador nvarchar(250) NULL);
                IF COLUMNPROPERTY(OBJECT_ID('dbo.Bitacora_Historico'),'IdBitacora','IsIdentity')=1
                    THROW 50001,'El histórico anterior tiene IDENTITY. Revisar su estructura antes de archivar.',1;
                DELETE dbo.Bitacora OUTPUT deleted.IdBitacora,deleted.Fecha,deleted.Usuario,deleted.Actividad,
                    deleted.InformacionAsociada,deleted.DigitoVerificador
                    INTO dbo.Bitacora_Historico(IdBitacora,Fecha,Usuario,Actividad,InformacionAsociada,DigitoVerificador)
                WHERE Fecha<@Limite;");
            cmd.Parameters.Add("@Limite", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-dias);
            return cmd.ExecuteNonQuery();
        });
    }
    private static bool EsAdministrador(SqlConnection c)
    {
        using var cmd = Comando(c, null, @"SELECT CASE WHEN IS_SRVROLEMEMBER('sysadmin')=1 OR
            (IS_MEMBER('db_owner')=1 AND (IS_SRVROLEMEMBER('dbcreator')=1 OR HAS_PERMS_BY_NAME(NULL,'SERVER','CREATE ANY DATABASE')=1)) THEN 1 ELSE 0 END");
        return (int)cmd.ExecuteScalar()! == 1;
    }
    private static void ExigirAdministrador(SqlConnection c)
    {
        if (!EsAdministrador(c)) throw new UnauthorizedAccessException("La recuperación requiere una cuenta SQL administradora de la base con permiso de crear/restaurar bases auxiliares.");
    }

    public string InicializarBaseConfiable()
    {
        using var c = Conectar(); c.Open(); ExigirAdministrador(c);
        if (TieneEstructura(c, null))
        {
            using var existentes = Comando(c, null, "SELECT COUNT(*) FROM dbo.IntegridadTabla");
            if ((int)existentes.ExecuteScalar()! != 0)
                throw new InvalidOperationException("La integridad ya está inicializada. Utilice recuperación, no recálculo.");
        }
        string previo = CrearArchivoBackup(c, "antes_integridad");
        using var t = c.BeginTransaction(IsolationLevel.Serializable);
        Bloquear(c, t, true);
        Ejecutar(c, t, @"IF OBJECT_ID('dbo.IntegridadTabla','U') IS NULL CREATE TABLE dbo.IntegridadTabla (
            Tabla nvarchar(30) NOT NULL PRIMARY KEY,HashVertical char(64) NOT NULL,Cantidad bigint NOT NULL,
            IdBase uniqueidentifier NOT NULL,Version int NOT NULL CHECK(Version=1));");
        using (var cmd = Comando(c, t, "SELECT COUNT(*) FROM dbo.IntegridadTabla"))
            if ((int)cmd.ExecuteScalar()! != 0) throw new InvalidOperationException("La integridad ya está inicializada. Utilice recuperación, no recálculo.");
        Guid identidad = Guid.NewGuid();
        foreach (string tabla in Tablas)
        {
            var filas = LeerFilas(c, t, tabla);
            foreach (var fila in filas)
            {
                ValidarLegado(fila);
                GuardarHash(c, t, fila);
            }
            using var cmd = Comando(c, t, "INSERT dbo.IntegridadTabla(Tabla,HashVertical,Cantidad,IdBase,Version) VALUES(@Tabla,@Hash,@Cantidad,@Base,1)");
            cmd.Parameters.AddWithValue("@Tabla", tabla);
            cmd.Parameters.AddWithValue("@Hash", Vertical(tabla, filas));
            cmd.Parameters.AddWithValue("@Cantidad", (long)filas.Count);
            cmd.Parameters.AddWithValue("@Base", identidad);
            cmd.ExecuteNonQuery();
        }
        Exigir(Examinar(c, t).Informe);
        t.Commit();
        Registrar("Inicialización autorizada del esquema SHA-256/DVH/DVV. Respaldo previo: " + previo);
        return CrearRespaldoConfiable();
    }

    private static void ValidarLegado(Fila fila)
    {
        if (fila.Tabla == "Traduccion")
        {
            if (fila.Guardado == null) return; // Catálogo original y pendientes: confirmación explícita del operador.
            string legado = HashHelper.ObtenerHashSHA256($"{fila.Valores[0]}|{fila.Valores[1]}|{fila.Valores[2]}");
            if (fila.Guardado == legado) return;
        }
        else
        {
            DateTime fecha = (DateTime)fila.Valores[1]!;
            string Calcular(DateTime f) => new EncriptadorServicio().CalcularDV(f.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture) +
                fila.Valores[2] + fila.Valores[3] + fila.Valores[4])!;
            if (fila.Guardado == Calcular(fecha)) return;
            // Compatibilidad limitada con el redondeo del datetime original: solo un segundo y milisegundo cero.
            if (fecha.Millisecond == 0 && fila.Guardado == Calcular(fecha.AddSeconds(-1)))
            {
                Registrar("Compatibilidad reconocida para Bitacora [" + fila.Clave + "]: DV legado coincide con la fecha anterior al redondeo SQL.");
                return;
            }
        }
        var informe = new InformeIntegridad { Configurada = true };
        informe.Problemas.Add(new(fila.Tabla, fila.Clave, "El dígito legado no coincide. No se permite inicializar aceptando datos alterados."));
        throw new IntegridadException(informe);
    }

    public string CrearRespaldoConfiable()
    {
        using var c = Conectar(); c.Open(); ExigirAdministrador(c);
        Exigir(Verificar());
        string ruta = CrearArchivoBackup(c, "confiable");
        ConBaseAuxiliar(ruta, auxiliar => { Exigir(auxiliar.Informe); return 0; });
        Registrar("Respaldo comprobado: " + ruta);
        return ruta;
    }
    private static string CrearArchivoBackup(SqlConnection c, string sufijo)
    {
        using var carpetaCmd = Comando(c, null, "SELECT CONVERT(nvarchar(2000),SERVERPROPERTY('InstanceDefaultBackupPath'))");
        string carpeta = Convert.ToString(carpetaCmd.ExecuteScalar()) ?? "";
        if (string.IsNullOrWhiteSpace(carpeta)) throw new InvalidOperationException("SQL Server no informó una carpeta de backups.");
        string ruta = Path.Combine(carpeta, c.Database + "_" + sufijo + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + "_" + Guid.NewGuid().ToString("N") + ".bak");
        using var cmd = Comando(c, null, $"BACKUP DATABASE {Id(c.Database)} TO DISK=@Ruta WITH COPY_ONLY,NOINIT,CHECKSUM");
        cmd.Parameters.AddWithValue("@Ruta", ruta);
        cmd.ExecuteNonQuery();
        return ruta;
    }

    private T ConBaseAuxiliar<T>(string ruta, Func<Estado, T> accion)
    {
        using var conexion = Conectar(); conexion.Open(); ExigirAdministrador(conexion);
        var builder = new SqlConnectionStringBuilder(conexion.ConnectionString) { InitialCatalog = "master" };
        using var master = new SqlConnection(builder.ConnectionString); master.Open();
        string nombre = "SIGAT_Recuperacion_" + Guid.NewGuid().ToString("N");
        var archivos = new List<(string Nombre, string Tipo)>();
        using (var cmd = Comando(master, null, "RESTORE VERIFYONLY FROM DISK=@Ruta WITH CHECKSUM; RESTORE FILELISTONLY FROM DISK=@Ruta"))
        {
            cmd.Parameters.AddWithValue("@Ruta", ruta);
            using var reader = cmd.ExecuteReader();
            do
            {
                if (reader.FieldCount == 0) continue;
                while (reader.Read()) archivos.Add((reader.GetString(reader.GetOrdinal("LogicalName")), reader.GetString(reader.GetOrdinal("Type"))));
            } while (reader.NextResult());
        }
        string datos, logs;
        using (var cmd = Comando(master, null, "SELECT CONVERT(nvarchar(2000),SERVERPROPERTY('InstanceDefaultDataPath')),CONVERT(nvarchar(2000),SERVERPROPERTY('InstanceDefaultLogPath'))"))
        using (var reader = cmd.ExecuteReader())
        {
            reader.Read(); datos = reader.GetString(0); logs = reader.GetString(1);
        }
        if (archivos.Count == 0 || archivos.Any(a => a.Tipo != "D" && a.Tipo != "L"))
            throw new InvalidOperationException("El respaldo no tiene un formato de archivos soportado para SIGAT.");
        string moves = string.Join(",", archivos.Select((a, n) => "MOVE " + Literal(a.Nombre) + " TO " +
            Literal(Path.Combine(a.Tipo == "L" ? logs : datos, nombre + "_" + n + (a.Tipo == "L" ? ".ldf" : ".mdf")))));
        try
        {
            using (var cmd = Comando(master, null, $"RESTORE DATABASE {Id(nombre)} FROM DISK=@Ruta WITH {moves},CHECKSUM,RECOVERY"))
            { cmd.Parameters.AddWithValue("@Ruta", ruta); cmd.ExecuteNonQuery(); }
            builder.InitialCatalog = nombre;
            using var aux = new SqlConnection(builder.ConnectionString); aux.Open();
            using var t = aux.BeginTransaction(IsolationLevel.Serializable);
            var estado = Examinar(aux, t);
            t.Commit();
            return accion(estado);
        }
        finally
        {
            SqlConnection.ClearAllPools();
            using var cmd = Comando(master, null, $"IF DB_ID(@Nombre) IS NOT NULL BEGIN ALTER DATABASE {Id(nombre)} SET SINGLE_USER WITH ROLLBACK IMMEDIATE; DROP DATABASE {Id(nombre)}; END");
            cmd.Parameters.AddWithValue("@Nombre", nombre);
            cmd.ExecuteNonQuery();
        }
    }

    public InformeIntegridad Recuperar(string ruta, bool restaurarTablasCompletas = false)
    {
        if (string.IsNullOrWhiteSpace(ruta)) throw new ArgumentException("Seleccione un respaldo comprobado.");
        return ConBaseAuxiliar(ruta, origen =>
        {
            Exigir(origen.Informe);
            using var c = Conectar(); c.Open(); ExigirAdministrador(c);
            using var t = c.BeginTransaction(IsolationLevel.Serializable);
            Bloquear(c, t, true);
            var actual = Examinar(c, t);
            if (!actual.Informe.Configurada) throw new InvalidOperationException("La base debe tener controles de integridad inicializados para recuperar registros.");
            if (actual.Controles.Values.Any(v => v.Base != origen.Controles.Values.First().Base))
                throw new InvalidOperationException("El respaldo pertenece a otra base. No se modificó ningún registro.");
            if (actual.Informe.Correcta) { t.Commit(); return actual.Informe; }
            foreach (string tabla in Tablas)
            {
                if (restaurarTablasCompletas && actual.Informe.Problemas.Any(p => p.Tabla == tabla))
                {
                    Ejecutar(c, t, $"DELETE FROM dbo.{Id(tabla)}");
                    foreach (var fila in origen.Filas[tabla]) RestaurarFila(c, t, fila, true);
                    using var control = Comando(c, t, "UPDATE dbo.IntegridadTabla SET HashVertical=@Hash,Cantidad=@Cantidad,Version=1 WHERE Tabla=@Tabla");
                    control.Parameters.AddWithValue("@Hash", origen.Controles[tabla].Hash);
                    control.Parameters.AddWithValue("@Cantidad", origen.Controles[tabla].Cantidad);
                    control.Parameters.AddWithValue("@Tabla", tabla);
                    control.ExecuteNonQuery();
                    continue;
                }
                var backup = origen.Filas[tabla].ToDictionary(f => f.Clave);
                var clavesActuales = actual.Filas[tabla].Select(f => f.Clave).ToHashSet();
                var afectadas = actual.Informe.Problemas.Where(p => p.Tabla == tabla && p.Registro != "Tabla").Select(p => p.Registro).Distinct();
                foreach (string clave in afectadas)
                {
                    if (!backup.TryGetValue(clave, out var fila)) throw new InvalidOperationException($"El respaldo no contiene {tabla} [{clave}]. Se necesita un respaldo más reciente.");
                    RestaurarFila(c, t, fila, false);
                }
                if (actual.Informe.Problemas.Any(p => p.Tabla == tabla && p.Registro == "Tabla")
                    && actual.Controles[tabla].Cantidad > clavesActuales.Count)
                    foreach (var fila in origen.Filas[tabla].Where(f => !clavesActuales.Contains(f.Clave))) RestaurarFila(c, t, fila, true);
                var reparadas = LeerFilas(c, t, tabla);
                string hash = Vertical(tabla, reparadas);
                // Se admite el control actual intacto o el del respaldo si todos los datos coinciden con él.
                if (hash == actual.Controles[tabla].Hash && reparadas.Count == actual.Controles[tabla].Cantidad) continue;
                bool filasDaniadas = actual.Informe.Problemas.Any(p => p.Tabla == tabla && p.Registro != "Tabla")
                    || actual.Controles[tabla].Cantidad > clavesActuales.Count;
                if (filasDaniadas)
                    throw new InvalidOperationException("El respaldo es anterior a otros cambios de " + tabla + ". La recuperación por registros se canceló sin confirmar modificaciones.");
                if (hash != origen.Controles[tabla].Hash || reparadas.Count != origen.Controles[tabla].Cantidad)
                    throw new InvalidOperationException("El respaldo no alcanza para recuperar " + tabla + " sin perder otros cambios. No se confirmó ninguna modificación.");
                using var cmd = Comando(c, t, "UPDATE dbo.IntegridadTabla SET HashVertical=@Hash,Cantidad=@Cantidad,Version=1 WHERE Tabla=@Tabla");
                cmd.Parameters.AddWithValue("@Hash", hash); cmd.Parameters.AddWithValue("@Cantidad", (long)reparadas.Count); cmd.Parameters.AddWithValue("@Tabla", tabla);
                cmd.ExecuteNonQuery();
            }
            var final = Examinar(c, t);
            Exigir(final.Informe);
            t.Commit();
            Registrar("Recuperación verificada " + (restaurarTablasCompletas ? "de tablas completas" : "por registros") + " desde " + ruta + ". Incidentes: " + string.Join("; ", actual.Informe.Problemas));
            return final.Informe;
        });
    }
    private static void RestaurarFila(SqlConnection c, SqlTransaction t, Fila fila, bool insertar)
    {
        var cols = Columnas[fila.Tabla];
        int claves = fila.Tabla == "Bitacora" ? 1 : 2;
        string sql = insertar
            ? $"INSERT dbo.{Id(fila.Tabla)} ({string.Join(",", cols.Select(Id))}) VALUES ({string.Join(",", cols.Select((_, n) => "@p" + n))})"
            : $"UPDATE dbo.{Id(fila.Tabla)} SET {string.Join(",", cols.Skip(claves).Select((col, n) => Id(col) + "=@p" + (n + claves)))} WHERE {Condicion(fila.Tabla)}";
        bool identity = insertar && fila.Tabla == "Bitacora";
        if (identity) Ejecutar(c, t, "SET IDENTITY_INSERT dbo.Bitacora ON");
        try
        {
            using var cmd = Comando(c, t, sql);
            for (int n = 0; n < cols.Length; n++) cmd.Parameters.AddWithValue("@p" + n, fila.Valores[n] ?? DBNull.Value);
            if (cmd.ExecuteNonQuery() != 1) throw new InvalidOperationException("No se pudo recuperar " + fila.Clave);
        }
        finally { if (identity) Ejecutar(c, t, "SET IDENTITY_INSERT dbo.Bitacora OFF"); }
    }

    public static void Registrar(string mensaje)
    {
        lock (LogLock)
        {
            string dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIGAT", "logs");
            Directory.CreateDirectory(dir);
            File.AppendAllText(Path.Combine(dir, "integridad.log"), DateTime.UtcNow.ToString("O") + " " + mensaje + Environment.NewLine);
        }
    }
}
