using SIGAT.BE;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL
{
    public class RolesBLL
    {
        private readonly RolesDAL dal;
        public RolesBLL(RolesDAL? repositorio = null) { dal = repositorio ?? new RolesDAL(); }

        // La gestión exige un catálogo válido y una cuenta con acceso a Administración del sistema.
        public EstadoRoles CargarParaGestion()
        {
            EstadoRoles estado = dal.Cargar();
            MatrizRoles.ValidarCatalogo(estado);
            ExigirGestion(estado);
            return estado;
        }

        // Al iniciar sesión reemplazamos los roles en memoria por las asignaciones persistidas en SQL.
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

        // Volvemos a consultar la autorización en la base antes de guardar. Los permisos de la sesión podrían haber cambiado.
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

        // Controlamos que las asignaciones pertenezcan al catálogo y que el administrador original conserve su acceso.
        public static void Validar(EstadoRoles estado)
        {
            MatrizRoles.ValidarCatalogo(estado);
            if (estado.IdAdministradorOriginal > 0)
            {
                Usuario? original = estado.Usuarios.Find(u => u.IdUsuario == estado.IdAdministradorOriginal);
                if (original == null || !original.Activo || !original.Roles.Any(r => r.Id == 1))
                    throw new InvalidOperationException("El administrador original debe permanecer activo y conservar el rol Administrador.");
            }
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
