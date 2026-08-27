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
            // El bloque "using" asegura que la conexión se cierre automáticamente al terminar
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"SELECT u.IdUsuario, u.NombreUsuario, u.Password, u.Nombre, u.Apellido, u.Activo, u.IdPerfil, p.NombrePerfil 
                                    FROM Usuarios u 
                                    INNER JOIN Perfiles p ON u.IdPerfil = p.IdPerfil 
                                    WHERE u.NombreUsuario = @User";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@User", nombreUsuario);
                    conexion.Open();

                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        // Si encuentra un registro, lo convertimos a objeto y lo devolvemos
                        if (lector.Read())
                        {
                            return MapearUsuario(lector);
                        }
                    }
                }
            }
            // Si no encontró nada, devuelve nulo
            return null;
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> listaDeUsuarios = new List<Usuario>();

            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"SELECT u.IdUsuario, u.NombreUsuario, u.Password, u.Nombre, u.Apellido, u.Activo, u.IdPerfil, p.NombrePerfil 
                                    FROM Usuarios u 
                                    INNER JOIN Perfiles p ON u.IdPerfil = p.IdPerfil";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    conexion.Open();
                    using (SqlDataReader lector = comando.ExecuteReader())
                    {
                        // Mientras haya filas en la base de datos, las agregamos a la lista
                        while (lector.Read())
                        {
                            Usuario usuarioEncontrado = MapearUsuario(lector);
                            listaDeUsuarios.Add(usuarioEncontrado);
                        }
                    }
                }
            }
            return listaDeUsuarios;
        }

        public void Insertar(Usuario nuevoUsuario)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"INSERT INTO Usuarios (NombreUsuario, Password, Nombre, Apellido, Activo, IdPerfil) 
                                    VALUES (@NombreUsuario, @Password, @Nombre, @Apellido, @Activo, @IdPerfil)";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    AsignarParametros(comando, nuevoUsuario);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Actualizar(Usuario usuarioEditado)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                string consulta = @"UPDATE Usuarios SET NombreUsuario = @NombreUsuario, Password = @Password, Nombre = @Nombre, 
                                    Apellido = @Apellido, Activo = @Activo, IdPerfil = @IdPerfil 
                                    WHERE IdUsuario = @IdUsuario";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdUsuario", usuarioEditado.IdUsuario);
                    AsignarParametros(comando, usuarioEditado);

                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        public void Eliminar(int idUsuario)
        {
            using (SqlConnection conexion = ConexionBD.ObtenerConexion())
            {
                // Baja Lógica: No borramos el registro, solo le ponemos Activo = 0 (Falso)
                string consulta = "UPDATE Usuarios SET Activo = 0 WHERE IdUsuario = @IdUsuario";

                using (SqlCommand comando = new SqlCommand(consulta, conexion))
                {
                    comando.Parameters.AddWithValue("@IdUsuario", idUsuario);
                    conexion.Open();
                    comando.ExecuteNonQuery();
                }
            }
        }

        // Método auxiliar para no repetir código en los INSERT y UPDATE
        private void AsignarParametros(SqlCommand comando, Usuario usuario)
        {
            comando.Parameters.AddWithValue("@NombreUsuario", usuario.NombreUsuario);
            comando.Parameters.AddWithValue("@Password", usuario.Password);
            comando.Parameters.AddWithValue("@Nombre", usuario.Nombre);
            comando.Parameters.AddWithValue("@Apellido", usuario.Apellido);
            comando.Parameters.AddWithValue("@Activo", usuario.Activo);
            comando.Parameters.AddWithValue("@IdPerfil", usuario.IdPerfil);
        }

        // Transforma los datos crudos de SQL Server a un objeto C#
        private Usuario MapearUsuario(SqlDataReader lector)
        {
            Usuario usuario = new Usuario();

            usuario.IdUsuario = lector.GetInt32(0);
            usuario.NombreUsuario = lector.GetString(1);
            usuario.Password = lector.GetString(2);
            usuario.Nombre = lector.GetString(3);
            usuario.Apellido = lector.GetString(4);
            usuario.Activo = lector.GetBoolean(5);
            usuario.IdPerfil = lector.GetInt32(6);

            // Armamos el objeto Perfil asociado
            usuario.Perfil = new Perfil();
            usuario.Perfil.IdPerfil = lector.GetInt32(6);
            usuario.Perfil.NombrePerfil = lector.GetString(7);

            return usuario;
        }
    }
}