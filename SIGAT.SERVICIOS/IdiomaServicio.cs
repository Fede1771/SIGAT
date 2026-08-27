using System;
using System.Globalization;
using System.Threading;

namespace SIGAT.SERVICIOS
{
    public class IdiomaServicio
    {
        private static IdiomaServicio? _instancia;

        // Delegado / Evento que avisa a los formularios cuando cambia el idioma
        public event Action? IdiomaCambiado;

        private IdiomaServicio() { }

        public static IdiomaServicio ObtenerInstancia()
        {
            if (_instancia == null)
            {
                _instancia = new IdiomaServicio();
            }
            return _instancia;
        }

        public void CambiarIdioma(string cultura)
        {
            // 1. Cambiamos la configuración regional del hilo de ejecución actual
            CultureInfo nuevaCultura = new CultureInfo(cultura);
            Thread.CurrentThread.CurrentUICulture = nuevaCultura;
            Thread.CurrentThread.CurrentCulture = nuevaCultura;

            // 2. Si hay formularios "escuchando" este evento, les avisamos que se actualicen
            if (IdiomaCambiado != null)
            {
                IdiomaCambiado.Invoke();
            }
        }
    }
}