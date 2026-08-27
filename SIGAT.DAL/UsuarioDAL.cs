using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using SIGAT.BE;

namespace SIGAT.DAL
{
    public class UsuarioDAL
    {
        public Usuario ObtenerPorNombreUsuario(string nombreUsuario)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"SELECT u.IdUsuario, u.NombreUsuario, u.Password, u.Nombre, u.Apellido, u.Activo, u.IdPerfil, p.NombrePerfil 
                                 FROM Usuarios u INNER JOIN Perfiles p ON u.IdPerfil = p.IdPerfil 
                                 WHERE u.NombreUsuario = @User";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@User", nombreUsuario);
                    conexion.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) return MapearUsuario(reader);
                    }
                }
            }
            return null;
        }

        public List<Usuario> ObtenerTodos()
        {
            var lista = new List<Usuario>();
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"SELECT u.IdUsuario, u.NombreUsuario, u.Password, u.Nombre, u.Apellido, u.Activo, u.IdPerfil, p.NombrePerfil 
                                 FROM Usuarios u INNER JOIN Perfiles p ON u.IdPerfil = p.IdPerfil";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    conexion.Open();
                    using (var reader = cmd.ExecuteReader())
                    {
                        while (reader.Read()) lista.Add(MapearUsuario(reader));
                    }
                }
            }
            return lista;
        }

        public void Insertar(Usuario u)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"INSERT INTO Usuarios (NombreUsuario, Password, Nombre, Apellido, Activo, IdPerfil) 
                                 VALUES (@NombreUsuario, @Password, @Nombre, @Apellido, @Activo, @IdPerfil)";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    AsignarParametros(cmd, u);
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Usuario u)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = @"UPDATE Usuarios SET NombreUsuario=@NombreUsuario, Password=@Password, Nombre=@Nombre, 
                                 Apellido=@Apellido, Activo=@Activo, IdPerfil=@IdPerfil 
                                 WHERE IdUsuario=@IdUsuario";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", u.IdUsuario);
                    AsignarParametros(cmd, u);
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int idUsuario)
        {
            using (var conexion = ConexionBD.ObtenerConexion())
            {
                string query = "UPDATE Usuarios SET Activo = 0 WHERE IdUsuario = @IdUsuario";
                using (var cmd = new SqlCommand(query, conexion))
                {
                    cmd.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    conexion.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

        private void AsignarParametros(SqlCommand cmd, Usuario u)
        {
            cmd.Parameters.AddWithValue("@NombreUsuario", u.NombreUsuario);
            cmd.Parameters.AddWithValue("@Password", u.Password);
            cmd.Parameters.AddWithValue("@Nombre", u.Nombre);
            cmd.Parameters.AddWithValue("@Apellido", u.Apellido);
            cmd.Parameters.AddWithValue("@Activo", u.Activo);
            cmd.Parameters.AddWithValue("@IdPerfil", u.IdPerfil);
        }

        private Usuario MapearUsuario(SqlDataReader reader)
        {
            return new Usuario
            {
                IdUsuario = reader.GetInt32(0),
                NombreUsuario = reader.GetString(1),
                Password = reader.GetString(2),
                Nombre = reader.GetString(3),
                Apellido = reader.GetString(4),
                Activo = reader.GetBoolean(5),
                IdPerfil = reader.GetInt32(6),
                Perfil = new Perfil { IdPerfil = reader.GetInt32(6), NombrePerfil = reader.GetString(7) }
            };
        }
    }
}