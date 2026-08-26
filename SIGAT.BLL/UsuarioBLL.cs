using System.Collections.Generic;
using SIGAT.BE;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL
{
    public enum ResultadoLogin { Exito, CredencialesInvalidas, UsuarioInactivo, Requiere2FA }

    public class UsuarioBLL
    {
        private UsuarioDAL _dal = new UsuarioDAL();
        private BitacoraBLL _bitacora = new BitacoraBLL();

        public ResultadoLogin Autenticar(string nombreUsuario, string passwordPlana, out Usuario usuarioValidado)
        {
            usuarioValidado = _dal.ObtenerPorNombreUsuario(nombreUsuario);
            string hashCalculado = HashHelper.ObtenerHashSHA256(passwordPlana);

            if (usuarioValidado == null || usuarioValidado.Password != hashCalculado)
            {
                _bitacora.Registrar(nombreUsuario, "Login Fallido", "Intento de acceso con credenciales incorrectas.");
                return ResultadoLogin.CredencialesInvalidas;
            }

            if (!usuarioValidado.Activo)
            {
                _bitacora.Registrar(nombreUsuario, "Login Denegado", "El usuario intentó ingresar pero está inactivo.");
                return ResultadoLogin.UsuarioInactivo;
            }

            if (usuarioValidado.DosFactorActivo)
            {
                return ResultadoLogin.Requiere2FA;
            }

            SesionServicio.ObtenerInstancia().IniciarSesion(usuarioValidado);
            _bitacora.Registrar(usuarioValidado.NombreUsuario, "Login Exitoso", "Inicio de sesión sin 2FA.");
            return ResultadoLogin.Exito;
        }

        public List<Usuario> ObtenerTodos() => _dal.ObtenerTodos();

        public void CrearUsuario(Usuario u, string passwordPlana)
        {
            if (_dal.ObtenerPorNombreUsuario(u.NombreUsuario) != null)
                throw new System.Exception("El nombre de usuario ya existe.");

            u.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            _dal.Insertar(u);

            string usrAdmin = SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario ?? "Sistema";
            _bitacora.Registrar(usrAdmin, "Alta Usuario", $"Se creó el usuario {u.NombreUsuario}");
        }

        public void ActualizarUsuario(Usuario u, string passwordPlana)
        {
            if (!string.IsNullOrEmpty(passwordPlana))
            {
                u.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            }
            _dal.Actualizar(u);

            string usrAdmin = SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario ?? "Sistema";
            _bitacora.Registrar(usrAdmin, "Modificación Usuario", $"Se editó el usuario {u.NombreUsuario}");
        }

        public void BajaLogicaUsuario(int idUsuario, string nombreUsuario)
        {
            _dal.Eliminar(idUsuario);
            string usrAdmin = SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario ?? "Sistema";
            _bitacora.Registrar(usrAdmin, "Baja Lógica Usuario", $"Se inhabilitó al usuario {nombreUsuario}");
        }
    }
}