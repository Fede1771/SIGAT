using SIGAT.BE.Idiomas;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.BLL
{
    // Esta clase conoce DAL y SERVICIOS a la vez, para que SERVICIOS
    // no tenga que depender de DAL (evita una referencia circular)
    public class IdiomaBLL
    {
        private const string IDIOMA_BASE = "Español";

        private IdiomaDAL _dal = new IdiomaDAL();

        public List<Idioma> ObtenerIdiomas()
        {
            return _dal.ObtenerIdiomas();
        }

        public void CambiarIdioma(int idIdioma)
        {
            Idioma idiomaElegido = null;
            foreach (Idioma i in _dal.ObtenerIdiomas())
            {
                if (i.Id == idIdioma)
                {
                    idiomaElegido = i;
                }
            }

            if (idiomaElegido == null)
            {
                throw new InvalidOperationException("El idioma solicitado no existe.");
            }

            List<Traduccion> traducciones = _dal.ObtenerTraduccionesPorIdioma(idIdioma);

            Dictionary<string, string> diccionario = new Dictionary<string, string>();
            foreach (Traduccion t in traducciones)
            {
                string clave = IdiomaManager.ClaveDe(t.FormNombre, t.ControlNombre);
                diccionario[clave] = t.Texto;
            }

            IdiomaManager.ObtenerInstancia().CargarIdioma(idiomaElegido, diccionario);
        }

        // Regla del enunciado: nuevo idioma = copia del Español
        public int CrearIdioma(string nombreNuevoIdioma)
        {
            if (string.IsNullOrWhiteSpace(nombreNuevoIdioma))
            {
                throw new ArgumentException("El nombre del idioma es obligatorio.");
            }

            int idIdiomaBase = _dal.ObtenerIdIdiomaPorNombre(IDIOMA_BASE);
            if (idIdiomaBase == 0)
            {
                throw new InvalidOperationException("No existe el idioma base (Español).");
            }

            Idioma nuevoIdioma = new Idioma();
            nuevoIdioma.Nombre = nombreNuevoIdioma;
            int idNuevoIdioma = _dal.InsertarIdioma(nuevoIdioma);

            ClonarTraducciones(idIdiomaBase, idNuevoIdioma);

            return idNuevoIdioma;
        }

        private void ClonarTraducciones(int idIdiomaBase, int idNuevoIdioma)
        {
            List<Traduccion> traduccionesEnEspañol = _dal.ObtenerTraduccionesPorIdioma(idIdiomaBase);

            foreach (Traduccion original in traduccionesEnEspañol)
            {
                Traduccion copia = new Traduccion();
                copia.IdIdioma = idNuevoIdioma;
                copia.IdControl = original.IdControl;
                copia.Texto = original.Texto;
                copia.DigitoVerificador = CalcularDVH(copia);

                _dal.InsertarOActualizarTraduccion(copia);
            }
        }

        public void GuardarTraduccion(int idIdioma, string nombreControl, string nombreForm, string texto)
        {
            int idControl = _dal.ObtenerOCrearControl(nombreControl, nombreForm);

            Traduccion traduccion = new Traduccion();
            traduccion.IdIdioma = idIdioma;
            traduccion.IdControl = idControl;
            traduccion.Texto = texto;
            traduccion.DigitoVerificador = CalcularDVH(traduccion);

            _dal.InsertarOActualizarTraduccion(traduccion);
        }

        // Hash SHA-256 sobre Idioma+Control+Texto: detecta ediciones directas en SQL
        private string CalcularDVH(Traduccion traduccion)
        {
            string textoAFirmar = traduccion.IdIdioma + "|" + traduccion.IdControl + "|" + traduccion.Texto;
            return HashHelper.ObtenerHashSHA256(textoAFirmar);
        }
    }
}