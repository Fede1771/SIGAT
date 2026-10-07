namespace SIGAT.BE
{
    // Rol queda fuera del Composite: contiene componentes, no es una familia.
    public class Rol
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
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

        public bool TienePermiso(string nombre)
        {
            foreach (Permiso permiso in permisos)
                if (permiso.ContienePermiso(nombre)) return true;
            return false;
        }
    }
}
