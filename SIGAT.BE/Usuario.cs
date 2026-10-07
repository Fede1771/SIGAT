namespace SIGAT.BE
{
    public class Usuario
    {
        // Alias compatible con el modelo del TP y con el DAL existente.
        public int Id { get { return IdUsuario; } set { IdUsuario = value; } }
        private readonly List<Rol> roles = new List<Rol>();
        public List<Rol> Roles { get { return new List<Rol>(roles); } }

        public void AsignarRol(Rol r)
        {
            ArgumentNullException.ThrowIfNull(r);
            foreach (Rol actual in roles)
            {
                if (ReferenceEquals(actual, r)) return;
            }
            roles.Add(r);
        }

        public void QuitarRol(Rol r)
        {
            foreach (Rol actual in roles)
            {
                if (ReferenceEquals(actual, r))
                {
                    roles.Remove(actual);
                    return;
                }
            }
        }

        public bool TienePermiso(string nombre)
        {
            foreach (Rol rol in roles)
            {
                if (rol.TienePermiso(nombre)) return true;
            }
            return false;
        }

        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }  
        public string Apellido { get; set; }
        public bool Activo { get; set; }
        public int IdPerfil { get; set; }
        public Perfil Perfil { get; set; }
    }
}
