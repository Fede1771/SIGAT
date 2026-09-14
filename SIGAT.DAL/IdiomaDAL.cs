using Microsoft.Data.SqlClient;
using SIGAT.BE.Idiomas;

namespace SIGAT.DAL
{
    public class IdiomaDAL
    {
        public List<Idioma> ObtenerIdiomas()
        {
            List<Idioma> lista = new List<Idioma>();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = "SELECT Id, Nombre FROM Idioma ORDER BY Id";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Idioma idioma = new Idioma();
                            idioma.Id = lector.GetInt32(0);
                            idioma.Nombre = lector.GetString(1);
                            lista.Add(idioma);
                        }
                    }
                }
            }
            return lista;
        }

        public int ObtenerIdIdiomaPorNombre(string nombre)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = "SELECT Id FROM Idioma WHERE Nombre = @Nombre";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", nombre);
                    conexion.Open();
                    object resultado = comando.ExecuteScalar();

                    if (resultado == null)
                    {
                        return 0;
                    }
                    return (int)resultado;
                }
            }
        }

        public int InsertarIdioma(Idioma idioma)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = "INSERT INTO Idioma (Nombre) OUTPUT INSERTED.Id VALUES (@Nombre)";
                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Nombre", idioma.Nombre);
                    conexion.Open();
                    return (int)comando.ExecuteScalar();
                }
            }
        }

        // Trae las traducciones de un idioma, con el JOIN a Control ya resuelto
        public List<Traduccion> ObtenerTraduccionesPorIdioma(int idIdioma)
        {
            List<Traduccion> lista = new List<Traduccion>();

            string consulta = @"SELECT t.Id_Idioma, t.Id_Control, t.Texto, t.DigitoVerificador,
                                        c.Control, c.Form
                                 FROM Traduccion t
                                 INNER JOIN Control c ON c.Id = t.Id_Control
                                 WHERE t.Id_Idioma = @IdIdioma";

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
                    if (encontrado != null)
                    {
                        return (int)encontrado;
                    }
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

        // Update si ya existe, Insert si no (upsert)
        public void InsertarOActualizarTraduccion(Traduccion traduccion)
        {
            string consulta = @"IF EXISTS (SELECT 1 FROM Traduccion WHERE Id_Idioma = @IdIdioma AND Id_Control = @IdControl)
                                    UPDATE Traduccion SET Texto = @Texto, DigitoVerificador = @DVH
                                    WHERE Id_Idioma = @IdIdioma AND Id_Control = @IdControl
                                 ELSE
                                    INSERT INTO Traduccion (Id_Idioma, Id_Control, Texto, DigitoVerificador)
                                    VALUES (@IdIdioma, @IdControl, @Texto, @DVH)";

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            using (SqlCommand comando = new SqlCommand(consulta, conexion))
            {
                comando.Parameters.AddWithValue("@IdIdioma", traduccion.IdIdioma);
                comando.Parameters.AddWithValue("@IdControl", traduccion.IdControl);
                comando.Parameters.AddWithValue("@Texto", traduccion.Texto);
                comando.Parameters.AddWithValue("@DVH", (object)traduccion.DigitoVerificador ?? DBNull.Value);
                conexion.Open();
                comando.ExecuteNonQuery();
            }
        }
    }
}