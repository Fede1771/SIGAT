using Microsoft.Data.SqlClient;
using SIGAT.BE.Idiomas;

namespace SIGAT.DAL
{
    public class IdiomaDAL
    {
        private readonly string? cadena;

        public IdiomaDAL(string? cadenaConexion = null) { cadena = cadenaConexion; }

        private SqlConnection Conectar()
            => cadena == null ? ConexionBD.ObtenerConexion() : new SqlConnection(cadena);

        // Sin una elección guardada devolvemos null; la BLL decide qué idioma de respaldo usar.
        public int? ObtenerIdiomaPreferido(int idUsuario)
        {
            using SqlConnection conexion = Conectar();
            using SqlCommand comando = new SqlCommand("SELECT IdIdioma FROM dbo.UsuarioIdioma WHERE IdUsuario=@Usuario", conexion);
            comando.Parameters.Add("@Usuario", System.Data.SqlDbType.Int).Value = idUsuario;
            conexion.Open();
            object? valor = comando.ExecuteScalar();
            return valor == null ? null : Convert.ToInt32(valor);
        }

        // Bloqueamos primero el idioma, igual que al desactivarlo, para ordenar las operaciones concurrentes.
        // La preferencia pertenece al ID de la cuenta aunque cambie su nombre.
        public void GuardarIdiomaPreferido(int idUsuario, int idIdioma)
        {
            using SqlConnection conexion = Conectar();
            conexion.Open();
            using SqlTransaction transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.Serializable);
            using SqlCommand comando = new SqlCommand(@"
                IF NOT EXISTS (SELECT 1 FROM dbo.Idioma WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Idioma AND Activo=1)
                    THROW 50001, 'El idioma no existe o está inactivo.', 1;
                IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WITH (HOLDLOCK) WHERE IdUsuario=@Usuario AND Activo=1)
                    THROW 50001, 'No se puede guardar el idioma de una cuenta inexistente o inactiva.', 1;
                UPDATE dbo.UsuarioIdioma WITH (UPDLOCK,HOLDLOCK) SET IdIdioma=@Idioma WHERE IdUsuario=@Usuario;
                IF @@ROWCOUNT=0 INSERT dbo.UsuarioIdioma (IdUsuario,IdIdioma) VALUES (@Usuario,@Idioma);", conexion, transaccion);
            comando.Parameters.Add("@Usuario", System.Data.SqlDbType.Int).Value = idUsuario;
            comando.Parameters.Add("@Idioma", System.Data.SqlDbType.Int).Value = idIdioma;
            comando.ExecuteNonQuery();
            transaccion.Commit();
        }

        public List<Idioma> ObtenerIdiomas()
        {
            return ObtenerIdiomasPorEstado(true);
        }

        public List<Idioma> ObtenerTodosLosIdiomas()
        {
            List<Idioma> lista = new List<Idioma>();
            string consulta = "SELECT Id, Nombre, Codigo, NombreNativo, Activo FROM Idioma ORDER BY Nombre";

            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(CrearIdioma(lector));
                    }
                }
            }
            return lista;
        }

        private List<Idioma> ObtenerIdiomasPorEstado(bool activo)
        {
            List<Idioma> lista = new List<Idioma>();
            string consulta = @"SELECT Id, Nombre, Codigo, NombreNativo, Activo
                                FROM Idioma WHERE Activo = @Activo ORDER BY Nombre";

            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@Activo", activo);
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        lista.Add(CrearIdioma(lector));
                    }
                }
            }
            return lista;
        }

        private Idioma CrearIdioma(SqlDataReader lector)
        {
            Idioma idioma = new Idioma();
            idioma.Id = lector.GetInt32(0);
            idioma.Nombre = lector.GetString(1);
            idioma.Codigo = lector.GetString(2);
            idioma.NombreNativo = lector.IsDBNull(3) ? "" : lector.GetString(3);
            idioma.Activo = lector.GetBoolean(4);
            return idioma;
        }

        public Idioma ObtenerIdiomaPorId(int idIdioma)
        {
            string consulta = @"SELECT Id, Nombre, Codigo, NombreNativo, Activo
                                FROM Idioma WHERE Id = @IdIdioma";

            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    if (lector.Read()) return CrearIdioma(lector);
                }
            }
            return null;
        }

        public int ObtenerIdIdiomaPorCodigo(string codigo)
        {
            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand("SELECT Id FROM Idioma WHERE Codigo = @Codigo", conexion))
            {
                comando.Parameters.AddWithValue("@Codigo", codigo);
                conexion.Open();
                object resultado = comando.ExecuteScalar();
                return resultado == null ? 0 : (int)resultado;
            }
        }

        public bool ExisteIdiomaPorNombre(string nombre)
        {
            return ExisteIdioma("Nombre", nombre);
        }

        public bool ExisteIdiomaPorCodigo(string codigo)
        {
            return ExisteIdioma("Codigo", codigo);
        }

        private bool ExisteIdioma(string columna, string valor)
        {
            string consulta = "SELECT COUNT(*) FROM Idioma WHERE " + columna + " = @Valor";
            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@Valor", valor);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public int InsertarIdioma(Idioma idioma)
        {
            string consulta = @"INSERT INTO Idioma (Nombre, Codigo, NombreNativo, Activo)
                                OUTPUT INSERTED.Id
                                VALUES (@Nombre, @Codigo, @NombreNativo, @Activo)";

            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@Nombre", idioma.Nombre);
                comando.Parameters.AddWithValue("@Codigo", idioma.Codigo);
                comando.Parameters.AddWithValue("@NombreNativo", string.IsNullOrWhiteSpace(idioma.NombreNativo) ? DBNull.Value : idioma.NombreNativo);
                comando.Parameters.AddWithValue("@Activo", idioma.Activo);
                conexion.Open();
                return (int)comando.ExecuteScalar();
            }
        }

        public bool TieneUsuariosAsignados(int idIdioma)
        {
            using var conexion = Conectar(); conexion.Open();
            using var comando = new SqlCommand(@"SELECT COUNT(*) FROM dbo.UsuarioIdioma ui
                JOIN dbo.Usuarios u ON u.IdUsuario=ui.IdUsuario WHERE ui.IdIdioma=@IdIdioma", conexion);
            comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
            return (int)comando.ExecuteScalar()! > 0;
        }

        // Repetimos la comprobación dentro de la transacción: una preferencia no puede guardarse mientras se desactiva su idioma.
        public void CambiarEstadoIdioma(int idIdioma, bool activo)
        {
            using var conexion = Conectar(); conexion.Open();
            using var transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.Serializable);
            using var consultar = new SqlCommand("SELECT Codigo FROM dbo.Idioma WITH (UPDLOCK,HOLDLOCK) WHERE Id=@Id", conexion, transaccion);
            consultar.Parameters.AddWithValue("@Id", idIdioma);
            string codigo = consultar.ExecuteScalar() as string ?? throw new InvalidOperationException("El idioma no existe.");
            if (!activo)
            {
                if (string.Equals(codigo, "es", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("No se puede desactivar Español porque es el idioma base.");
                using var asignados = new SqlCommand(@"SELECT COUNT(*) FROM dbo.UsuarioIdioma ui WITH (UPDLOCK,HOLDLOCK)
                    JOIN dbo.Usuarios u WITH (HOLDLOCK) ON u.IdUsuario=ui.IdUsuario WHERE ui.IdIdioma=@Id", conexion, transaccion);
                asignados.Parameters.AddWithValue("@Id", idIdioma);
                if ((int)asignados.ExecuteScalar()! > 0)
                    throw new InvalidOperationException("No se puede desactivar el idioma porque uno o más usuarios registrados lo tienen elegido.");
            }
            using var comando = new SqlCommand("UPDATE dbo.Idioma SET Activo=@Activo WHERE Id=@Id", conexion, transaccion);
            comando.Parameters.AddWithValue("@Activo", activo); comando.Parameters.AddWithValue("@Id", idIdioma);
            comando.ExecuteNonQuery(); transaccion.Commit();
        }

        public List<Traduccion> ObtenerTraduccionesPorIdioma(int idIdioma) => ConsultarTraducciones(idIdioma, false);
        public List<Traduccion> ObtenerTraduccionesParaGestion(int idIdioma) => ConsultarTraducciones(idIdioma, true);

        // Las traducciones se consultan independientemente de la integridad de las cuentas.
        private List<Traduccion> ConsultarTraducciones(int idIdioma, bool gestion)
        {
            using var conexion = Conectar(); conexion.Open();
            var lista = new List<Traduccion>();
            using var comando = new SqlCommand(@"SELECT t.Id_Idioma,t.Id_Control,t.Texto,t.Estado,c.Control,c.Form,
                ISNULL(tb.Texto,'') AS TextoBase FROM dbo.Traduccion t JOIN dbo.Control c ON c.Id=t.Id_Control
                LEFT JOIN dbo.Idioma es ON es.Codigo='es'
                LEFT JOIN dbo.Traduccion tb ON tb.Id_Idioma=es.Id AND tb.Id_Control=t.Id_Control
                WHERE t.Id_Idioma=@Idioma AND (@Gestion=1 OR t.Texto IS NOT NULL) ORDER BY c.Form,c.Control", conexion);
            comando.Parameters.AddWithValue("@Idioma", idIdioma);
            comando.Parameters.AddWithValue("@Gestion", gestion);
            using var lector = comando.ExecuteReader();
            while (lector.Read()) lista.Add(new Traduccion
            {
                IdIdioma=lector.GetInt32(0), IdControl=lector.GetInt32(1), Texto=lector.IsDBNull(2)?"":lector.GetString(2),
                Estado=lector.GetString(3), ControlNombre=lector.GetString(4), FormNombre=lector.GetString(5), TextoBase=lector.GetString(6)
            });
            return lista;
        }

        // La identidad de una etiqueta combina formulario y control: nombres iguales en pantallas distintas no se confunden.
        public int ObtenerOCrearControl(string nombreControl, string nombreForm)
        {
            using (SqlConnection conexion = Conectar())
            {
                conexion.Open();
                string buscar = "SELECT Id FROM Control WHERE Control = @Control AND Form = @Form";
                using (SqlCommand comandoBuscar = new SqlCommand(buscar, conexion))
                {
                    comandoBuscar.Parameters.AddWithValue("@Control", nombreControl);
                    comandoBuscar.Parameters.AddWithValue("@Form", nombreForm);
                    object encontrado = comandoBuscar.ExecuteScalar();
                    if (encontrado != null) return (int)encontrado;
                }

                string insertar = "INSERT INTO Control (Control, Form) OUTPUT INSERTED.Id VALUES (@Control, @Form)";
                using (SqlCommand comandoInsertar = new SqlCommand(insertar, conexion))
                {
                    comandoInsertar.Parameters.AddWithValue("@Control", nombreControl);
                    comandoInsertar.Parameters.AddWithValue("@Form", nombreForm);
                    return (int)comandoInsertar.ExecuteScalar();
                }
            }
        }

        public bool ExisteControl(string nombreControl, string nombreForm)
        {
            string consulta = "SELECT COUNT(*) FROM Control WHERE Control = @Control AND Form = @Form";
            using (SqlConnection conexion = Conectar())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@Control", nombreControl);
                comando.Parameters.AddWithValue("@Form", nombreForm);
                conexion.Open();
                return (int)comando.ExecuteScalar() > 0;
            }
        }

        public void CrearTraduccionesPendientes(int idIdioma)
        {
            using var conexion = Conectar(); conexion.Open();
            using var transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.Serializable);
            {
                using var cmd = new SqlCommand(@"INSERT dbo.Traduccion(Id_Idioma,Id_Control,Texto,Estado)
                    SELECT @Idioma,c.Id,NULL,'Pendiente' FROM dbo.Control c
                    WHERE NOT EXISTS(SELECT 1 FROM dbo.Traduccion t WHERE t.Id_Idioma=@Idioma AND t.Id_Control=c.Id)", conexion, transaccion);
                cmd.Parameters.AddWithValue("@Idioma",idIdioma);
                cmd.ExecuteNonQuery();
                transaccion.Commit();
            }
        }

        public void CrearPendientesParaTodosLosIdiomas(int idControl,int idIdiomaBase,string textoBase)
        {
            using var conexion = Conectar(); conexion.Open();
            using var transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.Serializable);
            {
                using var cmd = new SqlCommand(@"INSERT dbo.Traduccion(Id_Idioma,Id_Control,Texto,Estado)
                    SELECT i.Id,@Control,CASE WHEN i.Id=@Base THEN @Texto ELSE NULL END,
                    CASE WHEN i.Id=@Base THEN 'Completa' ELSE 'Pendiente' END FROM dbo.Idioma i
                    WHERE NOT EXISTS(SELECT 1 FROM dbo.Traduccion t WHERE t.Id_Idioma=i.Id AND t.Id_Control=@Control)", conexion, transaccion);
                cmd.Parameters.AddWithValue("@Control",idControl); cmd.Parameters.AddWithValue("@Base",idIdiomaBase); cmd.Parameters.AddWithValue("@Texto",textoBase);
                cmd.ExecuteNonQuery();
                transaccion.Commit();
            }
        }

        // La edición de textos ya no recalcula DVH ni DVV; queda disponible también durante una recuperación de cuentas.
        public void ActualizarTraduccion(int idIdioma,int idControl,string texto,string estado)
        {
            using var conexion = Conectar(); conexion.Open();
            using var transaccion = conexion.BeginTransaction(System.Data.IsolationLevel.Serializable);
            {
                string? normalizado=string.IsNullOrWhiteSpace(texto)?null:texto;
                using var cmd = new SqlCommand(@"UPDATE dbo.Traduccion SET Texto=@Texto,Estado=@Estado
                    WHERE Id_Idioma=@Idioma AND Id_Control=@Control",conexion,transaccion);
                cmd.Parameters.AddWithValue("@Texto",(object?)normalizado??DBNull.Value); cmd.Parameters.AddWithValue("@Estado",estado);
                cmd.Parameters.AddWithValue("@Idioma",idIdioma); cmd.Parameters.AddWithValue("@Control",idControl);
                int filas=cmd.ExecuteNonQuery();
                if(filas!=1) throw new InvalidOperationException("La traducción ya no existe.");
                transaccion.Commit();
            }
        }
    }
}
