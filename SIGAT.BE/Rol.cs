using System.Collections.Generic;

namespace SIGAT.BE
{
    // Un rol contiene permisos; no es un permiso ni hereda de él.
    public class Rol
    {
        public int Id { get; set; }
        public PermisoCompuesto Familia { get; } = new PermisoCompuesto();
        public string Nombre
        {
            get { return Familia.Nombre; }
            set { Familia.Nombre = value; }
        }
        public List<Permiso> Permisos { get { return Familia.ObtenerHijos(); } }

        public void AgregarPermiso(Permiso p)
        {
            foreach (Permiso actual in Permisos)
            {
                if (ReferenceEquals(actual, p)) return;
            }
            Familia.Agregar(p);
        }

        public void QuitarPermiso(Permiso p)
        {
            foreach (Permiso actual in Permisos)
            {
                if (ReferenceEquals(actual, p))
                {
                    Familia.Quitar(actual);
                    return;
                }
            }
        }

        public bool TienePermiso(string nombre)
        {
            foreach (Permiso permiso in Permisos)
            {
                if (permiso.ContienePermiso(nombre)) return true;
            }
            return false;
        }
    }
}
