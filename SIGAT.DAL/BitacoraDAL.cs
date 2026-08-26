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

        public void Insertar(Bitacora b)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"INSERT INTO Bitacora (Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador) 
                                 VALUES (@Fecha, @Usuario, @Actividad, @InformacionAsociada, @DigitoVerificador)";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Fecha", b.Fecha);
                    cmd.Parameters.AddWithValue("@Usuario", b.Usuario ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Actividad", b.Actividad ?? (object)DBNull.Value);

                    string? infoEncriptada = _encriptador.Encriptar(b.InformacionAsociada);
                    cmd.Parameters.AddWithValue("@InformacionAsociada", (object?)infoEncriptada ?? DBNull.Value);

                    string cadenaParaDV = $"{b.Fecha:yyyyMMddHHmmss}{b.Usuario}{b.Actividad}{infoEncriptada}";
                    b.DigitoVerificador = _encriptador.CalcularDV(cadenaParaDV);
                    cmd.Parameters.AddWithValue("@DigitoVerificador", b.DigitoVerificador ?? (object)DBNull.Value);

                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        {
            var lista = new List<Bitacora>();
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"SELECT IdBitacora, Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador 
                                 FROM Bitacora 
                                 WHERE (@Desde IS NULL OR Fecha >= @Desde) 
                                 AND (@Hasta IS NULL OR Fecha <= @Hasta) 
                                 AND (@Usuario IS NULL OR Usuario LIKE '%' + @Usuario + '%') 
                                 AND (@Actividad IS NULL OR Actividad LIKE '%' + @Actividad + '%') 
                                 ORDER BY Fecha DESC";

                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@Desde", (object?)desde ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Hasta", (object?)hasta ?? DBNull.Value);
                    cmd.Parameters.AddWithValue("@Usuario", string.IsNullOrEmpty(usuario) ? DBNull.Value : (object)usuario);
                    cmd.Parameters.AddWithValue("@Actividad", string.IsNullOrEmpty(actividad) ? DBNull.Value : (object)actividad);

                    conexion.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string? infoEncriptada = reader.IsDBNull(4) ? null : reader.GetString(4);
                            lista.Add(new Bitacora
                            {
                                IdBitacora = reader.GetInt32(0),
                                Fecha = reader.GetDateTime(1),
                                Usuario = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Actividad = reader.IsDBNull(3) ? null : reader.GetString(3),
                                InformacionAsociada = _encriptador.Desencriptar(infoEncriptada),
                                DigitoVerificador = reader.IsDBNull(5) ? null : reader.GetString(5)
                            });
                        }
                    }
                }
            }
            return lista;
        }

        public bool VerificarIntegridadBaseDatos()
        {
            bool integridadOk = true;
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = "SELECT Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador FROM Bitacora";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var fecha = reader.GetDateTime(0);
                            var usuario = reader.IsDBNull(1) ? null : reader.GetString(1);
                            var actividad = reader.IsDBNull(2) ? null : reader.GetString(2);
                            var info = reader.IsDBNull(3) ? null : reader.GetString(3);
                            var dvGuardado = reader.IsDBNull(4) ? null : reader.GetString(4);

                            string cadenaParaDV = $"{fecha:yyyyMMddHHmmss}{usuario}{actividad}{info}";
                            string? dvCalculado = _encriptador.CalcularDV(cadenaParaDV);

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