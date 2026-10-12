namespace SIGAT.BE
{
    public class Usuario
    {
        public bool EsAdministradorOriginal { get; set; }
        // Id e IdUsuario representan la misma cuenta; el alias conserva compatibilidad con los usos existentes.
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

        // El usuario obtiene sus accesos a través de los roles asignados. Basta con que uno de ellos otorgue el permiso.
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
        // Guardamos el hash de la clave. Para autenticar comparamos hashes, no recuperamos la contraseña original.
        public string Password { get; set; }
        public string Nombre { get; set; }  
        public string Apellido { get; set; }
        public bool Activo { get; set; }


    }
}
