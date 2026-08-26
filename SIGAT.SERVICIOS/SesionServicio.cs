using System;
using System.Collections.Generic;
using System.Text;

using SIGAT.BE;

namespace SIGAT.SERVICIOS
{
    public class SesionServicio
    {
        private static SesionServicio _instancia;
        private static readonly object _lock = new object();

        public Usuario UsuarioActual { get; private set; }
        public bool EstaAutenticado => UsuarioActual != null;
        public Perfil PerfilActual => UsuarioActual?.Perfil;

        private SesionServicio() { }

        public static SesionServicio ObtenerInstancia()
        {
            lock (_lock)
            {
                if (_instancia == null)
                {
                    _instancia = new SesionServicio();
                }
                return _instancia;
            }
        }

        public void IniciarSesion(Usuario usuario)
        {
            UsuarioActual = usuario;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}
