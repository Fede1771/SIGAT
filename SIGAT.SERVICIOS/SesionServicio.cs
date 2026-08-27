using System;
using System.Collections.Generic;
using System.Text;
using SIGAT.BE;

namespace SIGAT.SERVICIOS
{
    public class SesionServicio
    {
        // 1. Variable estática que guarda la única instancia de la sesión
        private static SesionServicio? _instancia;

        // 2. Objeto de bloqueo para evitar que dos hilos creen la sesión al mismo tiempo
        private static readonly object _lock = new object();

        // 3. Propiedades del usuario logueado
        public Usuario? UsuarioActual { get; private set; }

        public bool EstaAutenticado
        {
            get
            {
                return UsuarioActual != null;
            }
        }

        public Perfil? PerfilActual
        {
            get
            {
                if (UsuarioActual != null)
                {
                    return UsuarioActual.Perfil;
                }
                else
                {
                    return null;
                }
            }
        }

        // 4. Constructor privado: Evita que alguien haga "new SesionServicio()" desde afuera
        private SesionServicio() { }

        // 5. Método global para obtener la sesión (Patrón Singleton)
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

        public void IniciarSesion(Usuario usuarioLogueado)
        {
            UsuarioActual = usuarioLogueado;
        }

        public void CerrarSesion()
        {
            UsuarioActual = null;
        }
    }
}