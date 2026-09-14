using SIGAT.BE.Idiomas;

namespace SIGAT.SERVICIOS.Idiomas
{
    // Singleton + Subject del patron Observer
    public sealed class IdiomaManager : IIdiomaSubject
    {
        private static IdiomaManager _instancia;

        private List<IIdiomaObserver> _observadores = new List<IIdiomaObserver>();
        private Dictionary<string, string> _traducciones = new Dictionary<string, string>();

        public Idioma IdiomaActual { get; private set; }

        private IdiomaManager() { }

        public static IdiomaManager ObtenerInstancia()
        {
            if (_instancia == null)
            {
                _instancia = new IdiomaManager();
            }
            return _instancia;
        }

        public void Suscribir(IIdiomaObserver observador)
        {
            if (!_observadores.Contains(observador))
            {
                _observadores.Add(observador);
            }
        }

        public void Desuscribir(IIdiomaObserver observador)
        {
            _observadores.Remove(observador);
        }

        public void NotificarObservadores()
        {
            foreach (IIdiomaObserver observador in _observadores)
            {
                observador.ActualizarIdioma();
            }
        }

        // Lo llama IdiomaBLL despues de leer la base de datos
        public void CargarIdioma(Idioma idioma, Dictionary<string, string> traducciones)
        {
            IdiomaActual = idioma;
            _traducciones = traducciones;
            NotificarObservadores();
        }

        public string Traducir(string nombreForm, string nombreControl, string textoOriginal)
        {
            string clave = ClaveDe(nombreForm, nombreControl);

            if (_traducciones.ContainsKey(clave))
            {
                return _traducciones[clave];
            }
            return textoOriginal;
        }

        public static string ClaveDe(string nombreForm, string nombreControl)
        {
            return nombreForm + "." + nombreControl;
        }
    }
}