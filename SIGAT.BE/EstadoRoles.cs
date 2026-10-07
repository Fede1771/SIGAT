namespace SIGAT.BE
{
    // Una lectura consistente del catálogo y de todas las asignaciones.
    public class EstadoRoles
    {
        public long Version { get; set; }
        public List<Rol> Roles { get; } = new List<Rol>();
        public List<Permiso> Permisos { get; } = new List<Permiso>();
        public List<Usuario> Usuarios { get; } = new List<Usuario>();
    }
}
