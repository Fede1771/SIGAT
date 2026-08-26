using System;
using System.Globalization;
using System.Threading;

namespace SIGAT.SERVICIOS
{
    public class IdiomaServicio
    {
        private static IdiomaServicio _instancia;

        // Este evento se dispara para avisarle a las pantallas que deben actualizar sus textos
        public event Action IdiomaCambiado;

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
            // Cambiamos la cultura del hilo actual (afecta a números, fechas y recursos)
            Thread.CurrentThread.CurrentUICulture = new CultureInfo(cultura);
            Thread.CurrentThread.CurrentCulture = new CultureInfo(cultura);

            // Avisamos a todos los que estén escuchando que el idioma cambió
            IdiomaCambiado?.Invoke();
        }
    }
}