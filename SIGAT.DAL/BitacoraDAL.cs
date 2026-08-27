using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.SERVICIOS;

namespace SIGAT.DAL
{
    public class BitacoraDAL
    {
        private EncriptadorServicio _encriptador = new EncriptadorServicio();

        public void Insertar(Bitacora nuevoRegistro)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"INSERT INTO Bitacora (Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador) 
                                    VALUES (@Fecha, @Usuario, @Actividad, @InformacionAsociada, @DigitoVerificador)";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Fecha", nuevoRegistro.Fecha);

                    comando.Parameters.AddWithValue("@Usuario", nuevoRegistro.Usuario != null ? nuevoRegistro.Usuario : DBNull.Value);
                    comando.Parameters.AddWithValue("@Actividad", nuevoRegistro.Actividad != null ? nuevoRegistro.Actividad : DBNull.Value);

                    // Agregamos "?" porque la información puede venir nula
                    string? infoEncriptada = _encriptador.Encriptar(nuevoRegistro.InformacionAsociada);
                    comando.Parameters.AddWithValue("@InformacionAsociada", infoEncriptada != null ? infoEncriptada : DBNull.Value);

                    string fechaFormateada = nuevoRegistro.Fecha.ToString("yyyyMMddHHmmss");
                    string cadenaParaDV = fechaFormateada + nuevoRegistro.Usuario + nuevoRegistro.Actividad + infoEncriptada;

                    nuevoRegistro.DigitoVerificador = _encriptador.CalcularDV(cadenaParaDV);
                    comando.Parameters.AddWithValue("@DigitoVerificador", nuevoRegistro.DigitoVerificador != null ? nuevoRegistro.DigitoVerificador : DBNull.Value);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string usuario, string actividad)
        {
            List<Bitacora> listaResultados = new List<Bitacora>();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"SELECT IdBitacora, Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador 
                                    FROM Bitacora 
                                    WHERE (@Desde IS NULL OR Fecha >= @Desde) 
                                    AND (@Hasta IS NULL OR Fecha <= @Hasta) 
                                    AND (@Usuario IS NULL OR Usuario LIKE '%' + @Usuario + '%') 
                                    AND (@Actividad IS NULL OR Actividad LIKE '%' + @Actividad + '%') 
                                    ORDER BY Fecha DESC";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@Desde", desde != null ? desde : DBNull.Value);
                    comando.Parameters.AddWithValue("@Hasta", hasta != null ? hasta : DBNull.Value);
                    comando.Parameters.AddWithValue("@Usuario", string.IsNullOrEmpty(usuario) ? DBNull.Value : usuario);
                    comando.Parameters.AddWithValue("@Actividad", string.IsNullOrEmpty(actividad) ? DBNull.Value : actividad);

                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            Bitacora registro = new Bitacora();

                            registro.IdBitacora = lector.GetInt32(0);
                            registro.Fecha = lector.GetDateTime(1);

                            registro.Usuario = lector.IsDBNull(2) ? null : lector.GetString(2);
                            registro.Actividad = lector.IsDBNull(3) ? null : lector.GetString(3);

                            // Agregamos "?" para indicar que aceptamos el nulo
                            string? infoEncriptada = lector.IsDBNull(4) ? null : lector.GetString(4);
                            registro.InformacionAsociada = _encriptador.Desencriptar(infoEncriptada);

                            registro.DigitoVerificador = lector.IsDBNull(5) ? null : lector.GetString(5);

                            listaResultados.Add(registro);
                        }
                    }
                }
            }
            return listaResultados;
        }

        public bool VerificarIntegridadBaseDatos()
        {
            bool integridadOk = true;

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = "SELECT Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador FROM Bitacora";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        while (lector.Read())
                        {
                            DateTime fecha = lector.GetDateTime(0);

                            // Transformamos estas variables a string?
                            string? usuario = lector.IsDBNull(1) ? null : lector.GetString(1);
                            string? actividad = lector.IsDBNull(2) ? null : lector.GetString(2);
                            string? infoEncriptada = lector.IsDBNull(3) ? null : lector.GetString(3);
                            string? dvGuardado = lector.IsDBNull(4) ? null : lector.GetString(4);

                            string fechaFormateada = fecha.ToString("yyyyMMddHHmmss");
                            string cadenaParaDV = fechaFormateada + usuario + actividad + infoEncriptada;

                            string dvCalculado = _encriptador.CalcularDV(cadenaParaDV);

                            if (dvGuardado != dvCalculado)
                            {
                                integridadOk = false;
                                break;
                            }
                        }
                    }
                }
            }
            return integridadOk;
        }
    }
}