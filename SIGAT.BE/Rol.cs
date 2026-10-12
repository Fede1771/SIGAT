namespace SIGAT.BE
{
    // Un rol agrupa permisos y puede heredar otros roles, sin ciclos.
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        private readonly List<Rol> roles = new List<Rol>();
        // Devolvemos una copia para que las relaciones solo cambien mediante los métodos que validan la jerarquía.
        public List<Rol> Roles { get { return new List<Rol>(roles); } }
        public bool ContieneRol(Rol buscado)
        {
            return ReferenceEquals(this, buscado) || roles.Any(r => r.ContieneRol(buscado));
        }
        // Un rol puede heredar otro, pero no puede terminar conteniéndose a sí mismo: eso produciría una recursión sin fin.
        public void AgregarRol(Rol rol)
        {
            ArgumentNullException.ThrowIfNull(rol);
            if (rol.ContieneRol(this)) throw new InvalidOperationException("No se puede crear una jerarquía circular de roles.");
            if (!roles.Contains(rol)) roles.Add(rol);
        }
        public void QuitarRol(Rol rol) { roles.Remove(rol); }
        private readonly List<Permiso> permisos = new List<Permiso>();
        public List<Permiso> Permisos { get { return new List<Permiso>(permisos); } }

        public void AgregarPermiso(Permiso permiso)
        {
            ArgumentNullException.ThrowIfNull(permiso);
            foreach (Permiso actual in permisos)
                if (ReferenceEquals(actual, permiso)) return;
            permisos.Add(permiso);
        }

        public void QuitarPermiso(Permiso permiso)
        {
            foreach (Permiso actual in permisos)
            {
                if (ReferenceEquals(actual, permiso))
                {
                    permisos.Remove(actual);
                    return;
                }
            }
        }

        // Un permiso puede venir de una asignación directa o de un rol anidado. Ambas fuentes forman los permisos efectivos.
        public bool TienePermiso(string nombre)
        {
            foreach (Rol rol in roles)
                if (rol.TienePermiso(nombre)) return true;
            foreach (Permiso permiso in permisos)
                if (permiso.ContienePermiso(nombre)) return true;
            return false;
        }
    }
}
