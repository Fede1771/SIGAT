using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.SERVICIOS.Idiomas
{
    public interface IIdiomaSubject
    {
        void Suscribir(IIdiomaObserver observador);
        void Desuscribir(IIdiomaObserver observador);
        void NotificarObservadores();
    }
}
