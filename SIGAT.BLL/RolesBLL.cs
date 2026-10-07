using SIGAT.BE;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL
{
    public class RolesBLL
    {
        private readonly RolesDAL dal;
        public RolesBLL(RolesDAL? repositorio = null) { dal = repositorio ?? new RolesDAL(); }

        public EstadoRoles CargarParaGestion()
        {
            EstadoRoles estado = dal.Cargar();
            MatrizRoles.ValidarCatalogo(estado);
            ExigirGestion(estado);
            return estado;
        }

        public void CargarRolesUsuario(Usuario usuario)
        {
            EstadoRoles estado = dal.Cargar();
            MatrizRoles.ValidarCatalogo(estado);
            foreach (Rol anterior in usuario.Roles) usuario.QuitarRol(anterior);
            foreach (Usuario registrado in estado.Usuarios)
            {
                if (registrado.IdUsuario != usuario.IdUsuario) continue;
                foreach (Rol rol in registrado.Roles) usuario.AsignarRol(rol);
                return;
            }
        }

        public void Guardar(EstadoRoles estado)
        {
            ExigirGestion(dal.Cargar());
            Validar(estado);
            dal.Guardar(estado);
        }

        private static void ExigirGestion(EstadoRoles estado)
        {
            int? id = SesionServicio.ObtenerInstancia().UsuarioActual?.IdUsuario;
            foreach (Usuario usuario in estado.Usuarios)
            {
                if (usuario.IdUsuario == id && usuario.Activo && usuario.TienePermiso(MatrizRoles.Administracion)) return;
            }
            throw new InvalidOperationException("No tiene permiso para gestionar roles.");
        }

        public static void Validar(EstadoRoles estado)
        {
            MatrizRoles.ValidarCatalogo(estado);
            bool administradorDisponible = false;
            foreach (Usuario usuario in estado.Usuarios)
            {
                if (usuario.Activo && usuario.TienePermiso(MatrizRoles.Administracion)) administradorDisponible = true;
                foreach (Rol rol in usuario.Roles)
                {
                    bool existe = false;
                    foreach (Rol catalogado in estado.Roles)
                        if (ReferenceEquals(rol, catalogado)) existe = true;
                    if (!existe) throw new InvalidOperationException("El usuario tiene un rol fuera del catálogo.");
                }
            }
            if (!administradorDisponible)
                throw new InvalidOperationException("Debe quedar un administrador activo.");
        }
    }
}