using Microsoft.Data.SqlClient;
using SIGAT.BE.Idiomas;

namespace SIGAT.DAL
{
    public class IdiomaDAL
    {
        public List<Idioma> ObtenerIdiomas()
        {
            return ObtenerIdiomasPorEstado(true);
        }

        public List<Idioma> ObtenerTodosLosIdiomas()
        {
            List<Idioma> lista = new List<Idioma>();
            string consulta = "SELECT Id, Nombre, Codigo, NombreNativo, Activo FROM Idioma ORDER BY Nombre";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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

        public void CambiarEstadoIdioma(int idIdioma, bool activo)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand("UPDATE Idioma SET Activo = @Activo WHERE Id = @IdIdioma", conexion))
            {
                comando.Parameters.AddWithValue("@Activo", activo);
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public List<Traduccion> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            List<Traduccion> lista = new List<Traduccion>();
            string consulta = @"SELECT t.Id_Idioma, t.Id_Control, t.Texto, t.DigitoVerificador, c.Control, c.Form, t.Estado
                                FROM Traduccion t INNER JOIN Control c ON c.Id = t.Id_Control
                                WHERE t.Id_Idioma = @IdIdioma AND t.Texto IS NOT NULL";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Traduccion traduccion = new Traduccion();
                        traduccion.IdIdioma = lector.GetInt32(0);
                        traduccion.IdControl = lector.GetInt32(1);
                        traduccion.Texto = lector.GetString(2);
                        traduccion.DigitoVerificador = lector.IsDBNull(3) ? null : lector.GetString(3);
                        traduccion.ControlNombre = lector.GetString(4);
                        traduccion.FormNombre = lector.GetString(5);
                        traduccion.Estado = lector.GetString(6);
                        lista.Add(traduccion);
                    }
                }
            }
            return lista;
        }

        public List<Traduccion> ObtenerTraduccionesParaGestion(int idIdioma)
        {
            List<Traduccion> lista = new List<Traduccion>();
            string consulta = @"SELECT t.Id_Idioma, t.Id_Control, t.Texto, t.Estado, c.Control, c.Form,
                                       ISNULL(tBase.Texto, '') AS TextoBase
                                FROM Traduccion t
                                INNER JOIN Control c ON c.Id = t.Id_Control
                                LEFT JOIN Idioma idiomaBase ON idiomaBase.Codigo = 'es'
                                LEFT JOIN Traduccion tBase ON tBase.Id_Idioma = idiomaBase.Id AND tBase.Id_Control = t.Id_Control
                                WHERE t.Id_Idioma = @IdIdioma
                                ORDER BY c.Form, c.Control";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                conexion.Open();
                using (SqlDataReader lector = comando.ExecuteReader())
                {
                    while (lector.Read())
                    {
                        Traduccion traduccion = new Traduccion();
                        traduccion.IdIdioma = lector.GetInt32(0);
                        traduccion.IdControl = lector.GetInt32(1);
                        traduccion.Texto = lector.IsDBNull(2) ? "" : lector.GetString(2);
                        traduccion.Estado = lector.GetString(3);
                        traduccion.ControlNombre = lector.GetString(4);
                        traduccion.FormNombre = lector.GetString(5);
                        traduccion.TextoBase = lector.GetString(6);
                        lista.Add(traduccion);
                    }
                }
            }
            return lista;
        }

        public int ObtenerOCrearControl(string nombreControl, string nombreForm)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
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
            string consulta = @"INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador, Estado)
                                SELECT @IdIdioma, c.Id, NULL, NULL, 'Pendiente'
                                FROM Control c
                                WHERE NOT EXISTS
                                (SELECT 1 FROM Traduccion t WHERE t.Id_Idioma = @IdIdioma AND t.Id_Control = c.Id)";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void CrearPendientesParaTodosLosIdiomas(int idControl, int idIdiomaBase, string textoBase)
        {
            string consulta = @"INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador, Estado)
                                SELECT i.Id, @IdControl,
                                    CASE WHEN i.Id = @IdIdiomaBase THEN @TextoBase ELSE NULL END,
                                    NULL,
                                    CASE WHEN i.Id = @IdIdiomaBase THEN 'Completa' ELSE 'Pendiente' END
                                FROM Idioma i
                                WHERE NOT EXISTS
                                (SELECT 1 FROM Traduccion t WHERE t.Id_Idioma = i.Id AND t.Id_Control = @IdControl)";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdControl", idControl);
                comando.Parameters.AddWithValue("@IdIdiomaBase", idIdiomaBase);
                comando.Parameters.AddWithValue("@TextoBase", textoBase);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }

        public void ActualizarTraduccion(int idIdioma, int idControl, string texto, string estado, string digitoVerificador)
        {
            string consulta = @"UPDATE Traduccion SET Texto = @Texto, Estado = @Estado, DigitoVerificador = @DVH
                                WHERE Id_Idioma = @IdIdioma AND Id_Control = @IdControl";
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", idIdioma);
                comando.Parameters.AddWithValue("@IdControl", idControl);
                comando.Parameters.AddWithValue("@Texto", string.IsNullOrWhiteSpace(texto) ? DBNull.Value : texto);
                comando.Parameters.AddWithValue("@Estado", estado);
                comando.Parameters.AddWithValue("@DVH", string.IsNullOrWhiteSpace(digitoVerificador) ? DBNull.Value : digitoVerificador);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}
