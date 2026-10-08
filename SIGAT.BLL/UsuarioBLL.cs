using SIGAT.BE;
using SIGAT.BE.Auditoria;
using SIGAT.BLL.Fabricas;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL
{
    public enum ResultadoLogin { Exito, CredencialesInvalidas, UsuarioInactivo }

    public class UsuarioBLL
    {
        private UsuarioDAL _dal = new UsuarioDAL();
        private BitacoraBLL _bitacora = new BitacoraBLL();

        // Método auxiliar para no repetir código
        private string ObtenerNombreUsuarioActual()
        {
            Usuario usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            if (usuarioLogueado != null)
            {
                return usuarioLogueado.NombreUsuario;
            }
            return "Sistema";
        }

        public ResultadoLogin Autenticar(string nombreUsuario, string passwordPlana, out Usuario usuarioValidado)
        {
            usuarioValidado = _dal.ObtenerPorNombreUsuario(nombreUsuario);
            string hashCalculado = HashHelper.ObtenerHashSHA256(passwordPlana);

            if (usuarioValidado == null || usuarioValidado.Password != hashCalculado)
            {
                // USAMOS LA FÁBRICA: Login Fallido
                BitacoraFactory fabrica = new LoginFallidoFactory();
                IBitacoraEvento evento = fabrica.Crear(nombreUsuario, "Usuario o contraseña incorrectos.");
                _bitacora.Registrar(evento);

                return ResultadoLogin.CredencialesInvalidas;
            }

            if (usuarioValidado.Activo == false)
            {
                // USAMOS LA FÁBRICA: Login Fallido (Variante inactivo)
                BitacoraFactory fabrica = new LoginFallidoFactory();
                IBitacoraEvento evento = fabrica.Crear(nombreUsuario, "El usuario intentó entrar pero está inactivo.");
                _bitacora.Registrar(evento);

                return ResultadoLogin.UsuarioInactivo;
            }

            new RolesBLL().CargarRolesUsuario(usuarioValidado);
            SesionServicio.ObtenerInstancia().IniciarSesion(usuarioValidado);

            // USAMOS LA FÁBRICA: Login Exitoso
            BitacoraFactory fabricaExito = new LoginExitosoFactory();
            IBitacoraEvento eventoExito = fabricaExito.Crear(usuarioValidado.NombreUsuario, "Ingreso al sistema.");
            _bitacora.Registrar(eventoExito);

            return ResultadoLogin.Exito;
        }

        public List<Usuario> ObtenerTodos()
        {
            List<Usuario> usuarios = _dal.ObtenerTodos();
            EstadoRoles estado = new RolesDAL().Cargar();
            foreach (Usuario usuario in usuarios)
                usuario.EsAdministradorOriginal = usuario.IdUsuario == estado.IdAdministradorOriginal;
            foreach (Usuario usuario in usuarios)
                foreach (Usuario asignado in estado.Usuarios)
                    if (usuario.IdUsuario == asignado.IdUsuario)
                        foreach (Rol rol in asignado.Roles) usuario.AsignarRol(rol);
            return usuarios;
        }

        public void CrearUsuario(Usuario nuevoUsuario, string passwordPlana)
        {
            ValidarDatos(nuevoUsuario);
            if (string.IsNullOrWhiteSpace(passwordPlana)) throw new ArgumentException("La contraseña es obligatoria al crear un usuario.");
            Usuario usuarioExistente = _dal.ObtenerPorNombreUsuario(nuevoUsuario.NombreUsuario);
            if (usuarioExistente != null)
            {
                throw new Exception("El nombre de usuario ya existe en el sistema.");
            }

            nuevoUsuario.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            _dal.Insertar(nuevoUsuario);

            // USAMOS LA FÁBRICA: Gestión de Usuarios (Pasando el Enum exacto)
            BitacoraFactory fabrica = new GestionUsuariosFactory(BitacoraAccion.AltaUsuario);
            IBitacoraEvento evento = fabrica.Crear(ObtenerNombreUsuarioActual(), "Se creó el usuario " + nuevoUsuario.NombreUsuario);
            _bitacora.Registrar(evento);
        }

        public void ActualizarUsuario(Usuario usuarioEditado, string passwordPlana)
        {
            ValidarDatos(usuarioEditado);
            Usuario? existente = _dal.ObtenerPorNombreUsuario(usuarioEditado.NombreUsuario);
            if (existente != null && existente.IdUsuario != usuarioEditado.IdUsuario)
                throw new ArgumentException("El nombre de usuario ya existe en el sistema.");
            usuarioEditado.Password = string.IsNullOrEmpty(passwordPlana) ? "" : HashHelper.ObtenerHashSHA256(passwordPlana);
            _dal.Actualizar(usuarioEditado);

            // USAMOS LA FÁBRICA: Gestión de Usuarios
            BitacoraFactory fabrica = new GestionUsuariosFactory(BitacoraAccion.ModificacionUsuario);
            IBitacoraEvento evento = fabrica.Crear(ObtenerNombreUsuarioActual(), "Se editó el usuario " + usuarioEditado.NombreUsuario);
            _bitacora.Registrar(evento);
        }

        public void BajaLogicaUsuario(int idUsuario, string nombreUsuario)
        {
            _dal.Eliminar(idUsuario);

            // USAMOS LA FÁBRICA: Gestión de Usuarios
            BitacoraFactory fabrica = new GestionUsuariosFactory(BitacoraAccion.BajaUsuario);
            IBitacoraEvento evento = fabrica.Crear(ObtenerNombreUsuarioActual(), "Se inhabilitó al usuario " + nombreUsuario);
            _bitacora.Registrar(evento);
        }

        public void CrearAdministradorInicial(Usuario usuario, string passwordPlana)
        {
            ValidarDatos(usuario);
            if (string.IsNullOrWhiteSpace(passwordPlana)) throw new ArgumentException("La contraseña es obligatoria.");
            usuario.Password = HashHelper.ObtenerHashSHA256(passwordPlana);
            _dal.Insertar(usuario, true);
        }

        public static void ValidarDatos(Usuario usuario)
        {
            usuario.NombreUsuario = (usuario.NombreUsuario ?? "").Trim();
            usuario.Nombre = (usuario.Nombre ?? "").Trim();
            usuario.Apellido = (usuario.Apellido ?? "").Trim();
            if (usuario.NombreUsuario.Length == 0 || usuario.NombreUsuario.Length > 50)
                throw new ArgumentException("El usuario debe tener entre 1 y 50 caracteres.");
            if (usuario.Nombre.Length == 0 || usuario.Nombre.Length > 100 || usuario.Apellido.Length == 0 || usuario.Apellido.Length > 100)
                throw new ArgumentException("Nombre y apellido son obligatorios y admiten hasta 100 caracteres.");
        }
    }
}
