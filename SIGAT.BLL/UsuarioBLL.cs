using System;
using System.Collections.Generic;
using SIGAT.BE;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL
{
    public enum ResultadoLogin { Exito, CredencialesInvalidas, UsuarioInactivo }

    public class UsuarioBLL
    {
        private UsuarioDAL _dal = new UsuarioDAL();
        private BitacoraBLL _bitacora = new BitacoraBLL();

        public ResultadoLogin Autenticar(string nombreUsuario, string passwordPlana, out Usuario usuarioValidado)
        {
            // 1. Buscamos los datos del usuario en la base de datos
            usuarioValidado = _dal.ObtenerPorNombreUsuario(nombreUsuario);

            // 2. Encriptamos la clave que escribió el usuario en la pantalla
            string hashCalculado = HashHelper.ObtenerHashSHA256(passwordPlana);

            // 3. Verificamos si el usuario no existe o si la clave encriptada no coincide
            if (usuarioValidado == null || usuarioValidado.Password != hashCalculado)
            {
                _bitacora.Registrar(nombreUsuario, "Login Fallido", "Usuario o contraseña incorrectos.");
                return ResultadoLogin.CredencialesInvalidas;
            }

            // 4. Verificamos si el usuario está dado de baja
            if (usuarioValidado.Activo == false)
            {
                _bitacora.Registrar(nombreUsuario, "Login Denegado", "El usuario está inactivo.");
                return ResultadoLogin.UsuarioInactivo;
            }

            // 5. Si pasó todas las validaciones, guardamos la sesión y entra
            SesionServicio.ObtenerInstancia().IniciarSesion(usuarioValidado);
            _bitacora.Registrar(usuarioValidado.NombreUsuario, "Login Exitoso", "Ingreso al sistema.");

            return ResultadoLogin.Exito;
        }

        public List<Usuario> ObtenerTodos()
        {
            return _dal.ObtenerTodos();
        }

        public void CrearUsuario(Usuario nuevoUsuario, string passwordPlana)
        {
            // Validamos que el nombre de usuario no esté repetido
            Usuario usuarioExistente = _dal.ObtenerPorNombreUsuario(nuevoUsuario.NombreUsuario);
            if (usuarioExistente != null)
            {
                throw new Exception("El nombre de usuario ya existe en el sistema.");
            }

            // Encriptamos la contraseña y lo guardamos
            nuevoUsuario.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            _dal.Insertar(nuevoUsuario);

            // Registramos la acción en la bitácora
            string usuarioAdmin = ObtenerNombreUsuarioActual();
            _bitacora.Registrar(usuarioAdmin, "Alta Usuario", "Se creó el usuario " + nuevoUsuario.NombreUsuario);
        }

        public void ActualizarUsuario(Usuario usuarioEditado, string passwordPlana)
        {
            // Solo encriptamos y cambiamos la clave si el administrador escribió una nueva
            if (passwordPlana != "")
            {
                usuarioEditado.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            }

            _dal.Actualizar(usuarioEditado);

            string usuarioAdmin = ObtenerNombreUsuarioActual();
            _bitacora.Registrar(usuarioAdmin, "Modificación Usuario", "Se editó el usuario " + usuarioEditado.NombreUsuario);
        }

        public void BajaLogicaUsuario(int idUsuario, string nombreUsuario)
        {
            _dal.Eliminar(idUsuario);

            string usuarioAdmin = ObtenerNombreUsuarioActual();
            _bitacora.Registrar(usuarioAdmin, "Baja Lógica", "Se inhabilitó al usuario " + nombreUsuario);
        }

        // Método auxiliar para saber quién está usando el sistema ahora mismo
        private string ObtenerNombreUsuarioActual()
        {
            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;

            if (usuarioLogueado != null)
            {
                return usuarioLogueado.NombreUsuario;
            }
            else
            {
                return "Sistema"; // Por si la sesión todavía no cargó
            }
        }
    }
}       