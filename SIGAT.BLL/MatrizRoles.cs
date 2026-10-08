using SIGAT.BE;

namespace SIGAT.BLL
{
    // Única definición de la matriz en C#. Las familias no reciben hijos inventados.
    public static class MatrizRoles
    {
        public const string Administracion = "Administración del sistema";
        public const string Bitacora = "Ver bitácora";

        public static EstadoRoles CrearCatalogo()
        {
            EstadoRoles estado = new EstadoRoles();
            estado.Permisos.Add(new PermisoSimple { Id = 1, Nombre = Bitacora });
            estado.Permisos.Add(new PermisoSimple { Id = 2, Nombre = "Consultar inventario" });
            estado.Permisos.Add(new PermisoCompuesto { Id = 3, Nombre = "Gestión de activos" });
            estado.Permisos.Add(new PermisoCompuesto { Id = 4, Nombre = "Movimientos" });
            estado.Permisos.Add(new PermisoCompuesto { Id = 5, Nombre = "Mantenimiento" });
            estado.Permisos.Add(new PermisoCompuesto { Id = 6, Nombre = "Reportes" });
            estado.Permisos.Add(new PermisoCompuesto { Id = 7, Nombre = Administracion });
            AgregarRol(estado, 1, "Administrador", new[] { 1, 3, 4, 5, 6, 7 });
            AgregarRol(estado, 2, "Responsable de Inventario", new[] { 3, 4, 6 });
            AgregarRol(estado, 3, "Técnico de Mantenimiento", new[] { 5, 2 });
            AgregarRol(estado, 4, "Mesa de Ayuda", new[] { 4, 2 });
            AgregarRol(estado, 5, "Auditor", new[] { 1, 2, 6 });
            return estado;
        }

        private static void AgregarRol(EstadoRoles estado, int id, string nombre, int[] permisos)
        {
            Rol rol = new Rol { Id = id, Nombre = nombre };
            foreach (int idPermiso in permisos)
                foreach (Permiso permiso in estado.Permisos)
                    if (permiso.Id == idPermiso) rol.AgregarPermiso(permiso);
            estado.Roles.Add(rol);
        }

        public static void ValidarCatalogo(EstadoRoles estado)
        {
            EstadoRoles esperado = CrearCatalogo();
            if (estado.Permisos.Count != 7)
                throw new InvalidOperationException("El catálogo debe conservar los siete permisos del sistema.");
            HashSet<int> ids = new HashSet<int>();
            foreach (Permiso permiso in estado.Permisos)
            {
                if (!ids.Add(permiso.Id)) throw new InvalidOperationException("Permiso duplicado.");
                bool coincide = false;
                foreach (Permiso definido in esperado.Permisos)
                    if (permiso.Id == definido.Id && permiso.Nombre == definido.Nombre && permiso.GetType() == definido.GetType()) coincide = true;
                if (!coincide || permiso.ObtenerHijos().Count != 0)
                    throw new InvalidOperationException("El permiso o su jerarquía no coincide con la matriz.");
            }
            ids.Clear();
            var nombres = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (Rol rol in estado.Roles)
            {
                if (rol.Id <= 0 || !ids.Add(rol.Id)) throw new InvalidOperationException("Rol duplicado o inválido.");
                if (string.IsNullOrWhiteSpace(rol.Nombre) || rol.Nombre != rol.Nombre.Trim() || rol.Nombre.Length > 150 || !nombres.Add(rol.Nombre))
                    throw new InvalidOperationException("El nombre del rol es obligatorio, único y admite hasta 150 caracteres.");
                foreach (Permiso permiso in rol.Permisos)
                    if (!estado.Permisos.Contains(permiso)) throw new InvalidOperationException("Permiso fuera del catálogo.");
                foreach (Rol hijo in rol.Roles)
                    if (!estado.Roles.Contains(hijo)) throw new InvalidOperationException("Rol anidado fuera del catálogo.");
            }
            Rol? admin = estado.Roles.Find(r => r.Id == 1);
            Rol baseAdmin = esperado.Roles[0];
            if (admin == null || admin.Nombre != baseAdmin.Nombre || admin.Roles.Count != 0 ||
                !admin.Permisos.Select(p => p.Id).Order().SequenceEqual(baseAdmin.Permisos.Select(p => p.Id).Order()))
                throw new InvalidOperationException("El rol Administrador está protegido y debe conservar sus permisos originales.");
        }
    }
}