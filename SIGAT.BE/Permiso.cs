using System;
using System.Collections.Generic;

namespace SIGAT.BE
{
    // Composite: una acción simple y una familia comparten las mismas operaciones. Así consultamos permisos sin depender de su tipo concreto.
    public abstract class Permiso
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public abstract bool ContienePermiso(string nombre);
        public abstract void Agregar(Permiso permiso);
        public abstract void Quitar(Permiso permiso);
        public abstract List<Permiso> ObtenerHijos();
    }

    // Es la hoja del Composite: representa una acción y no admite otros permisos como hijos.
    public class PermisoSimple : Permiso
    {
        public override bool ContienePermiso(string nombre)
        {
            return string.Equals(Nombre, nombre, StringComparison.OrdinalIgnoreCase);
        }

        public override void Agregar(Permiso permiso)
        {
            throw new InvalidOperationException("Una patente no puede contener hijos.");
        }

        public override void Quitar(Permiso permiso)
        {
            throw new InvalidOperationException("Una patente no puede contener hijos.");
        }

        public override List<Permiso> ObtenerHijos()
        {
            return new List<Permiso>();
        }
    }

    // Es el componente compuesto: agrupa permisos y recorre sus hijos para resolver la consulta.
    public class PermisoCompuesto : Permiso
    {
        private readonly List<Permiso> hijos = new List<Permiso>();

        public override void Agregar(Permiso permiso)
        {
            ArgumentNullException.ThrowIfNull(permiso);
            // El candidato no debe contener al destino: cerraría un ciclo.
            if (ContieneReferencia(permiso, this))
                throw new InvalidOperationException("La asignación produciría un ciclo.");
            foreach (Permiso hijo in hijos)
            {
                if (ReferenceEquals(hijo, permiso)) return;
            }
            hijos.Add(permiso);
        }

        // Buscamos el mismo objeto dentro de la jerarquía. Esto permite rechazar ciclos antes de agregar un hijo.
        private static bool ContieneReferencia(Permiso origen, Permiso buscado)
        {
            if (ReferenceEquals(origen, buscado)) return true;
            foreach (Permiso hijo in origen.ObtenerHijos())
            {
                if (ContieneReferencia(hijo, buscado)) return true;
            }
            return false;
        }

        public override void Quitar(Permiso permiso)
        {
            foreach (Permiso hijo in hijos)
            {
                if (ReferenceEquals(hijo, permiso))
                {
                    hijos.Remove(hijo);
                    return;
                }
            }
        }

        public override List<Permiso> ObtenerHijos()
        {
            // La copia impide saltarse la validación de Agregar.
            return new List<Permiso>(hijos);
        }

        public override bool ContienePermiso(string nombre)
        {
            if (string.Equals(Nombre, nombre, StringComparison.OrdinalIgnoreCase))
                return true;
            foreach (Permiso hijo in hijos)
            {
                if (hijo.ContienePermiso(nombre)) return true;
            }
            return false;
        }
    }
}
