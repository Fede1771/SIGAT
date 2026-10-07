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
            ExigirGestion(estado);
            return estado;
        }

        public void CargarRolesUsuario(Usuario usuario)
        {
            EstadoRoles estado = dal.Cargar();
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
                if (usuario.IdUsuario == id && usuario.Activo && usuario.TienePatente("Gestión de Roles")) return;
            }
            throw new InvalidOperationException("No tiene permiso para gestionar roles.");
        }

        public static void Validar(EstadoRoles estado)
        {
            bool administradorDisponible = false;
            foreach (Usuario usuario in estado.Usuarios)
            {
                if (usuario.Activo && usuario.TienePatente("Gestión de Roles")) administradorDisponible = true;
                foreach (Rol rol in usuario.Roles)
                {
                    if (!estado.Roles.Contains(rol)) throw new InvalidOperationException("El usuario tiene un rol fuera del catálogo.");
                    if (rol.IdUsuarioPersonal.HasValue && rol.IdUsuarioPersonal != usuario.IdUsuario)
                        throw new InvalidOperationException("Un rol personal solo puede asignarse a su propietario.");
                }
            }
            if (!administradorDisponible)
                throw new InvalidOperationException("Debe quedar un usuario activo con el permiso Gestión de Roles.");
            HashSet<int> ids = new HashSet<int>();
            foreach (Permiso permiso in estado.Permisos)
            {
                if (permiso.Id <= 0 || !ids.Add(permiso.Id)) throw new InvalidOperationException("Identificador de permiso inválido o duplicado.");
                if (string.IsNullOrWhiteSpace(permiso.Nombre) || permiso.Nombre.Length > 150)
                    throw new InvalidOperationException("Los nombres deben tener entre 1 y 150 caracteres.");
                foreach (Permiso hijo in permiso.ObtenerHijos())
                {
                    if (!estado.Permisos.Contains(hijo)) throw new InvalidOperationException("Falta un permiso del árbol en el catálogo.");
                }
            }
            foreach (Rol rol in estado.Roles)
            {
                if (!estado.Permisos.Contains(rol.Familia)) throw new InvalidOperationException("Falta la familia de un rol.");
            }
        }
    }
}
