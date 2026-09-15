using SIGAT.BE.Idiomas;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.BLL
{
    public class IdiomaBLL
    {
        private IdiomaDAL _dal = new IdiomaDAL();

        public List<Idioma> ObtenerIdiomas()
        {
            return _dal.ObtenerIdiomas();
        }

        public List<Idioma> ObtenerTodosLosIdiomas()
        {
            return _dal.ObtenerTodosLosIdiomas();
        }

        public void CambiarIdioma(int idIdioma)
        {
            Idioma idiomaElegido = _dal.ObtenerIdiomaPorId(idIdioma);

            if (idiomaElegido == null)
                throw new InvalidOperationException("El idioma solicitado no existe.");

            if (!idiomaElegido.Activo)
                throw new InvalidOperationException("El idioma está inactivo.");

            List<Traduccion> traducciones = _dal.ObtenerTraduccionesPorIdioma(idIdioma);
            Dictionary<string, string> diccionario = new Dictionary<string, string>();

            foreach (Traduccion traduccion in traducciones)
            {
                string clave = IdiomaManager.ClaveDe(traduccion.FormNombre, traduccion.ControlNombre);
                diccionario[clave] = traduccion.Texto;
            }

            IdiomaManager.ObtenerInstancia().CargarIdioma(idiomaElegido, diccionario);
        }

        public int CrearIdioma(string nombre, string codigo, string nombreNativo, bool activo)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del idioma es obligatorio.");

            if (string.IsNullOrWhiteSpace(codigo))
                throw new ArgumentException("El código del idioma es obligatorio.");

            nombre = nombre.Trim();
            codigo = codigo.Trim().ToLower();

            if (_dal.ExisteIdiomaPorNombre(nombre))
                throw new ArgumentException("Ya existe un idioma con ese nombre.");

            if (_dal.ExisteIdiomaPorCodigo(codigo))
                throw new ArgumentException("Ya existe un idioma con ese código.");

            Idioma idioma = new Idioma();
            idioma.Nombre = nombre;
            idioma.Codigo = codigo;
            idioma.NombreNativo = nombreNativo == null ? "" : nombreNativo.Trim();
            idioma.Activo = activo;

            int idIdiomaNuevo = _dal.InsertarIdioma(idioma);
            _dal.CrearTraduccionesPendientes(idIdiomaNuevo);
            return idIdiomaNuevo;
        }

        public void CambiarEstadoIdioma(int idIdioma, bool activo)
        {
            Idioma idioma = _dal.ObtenerIdiomaPorId(idIdioma);

            if (idioma == null)
                throw new InvalidOperationException("El idioma no existe.");

            if (idioma.Codigo == "es" && !activo)
                throw new InvalidOperationException("No se puede desactivar Español porque es el idioma base.");

            _dal.CambiarEstadoIdioma(idIdioma, activo);
        }

        public List<Traduccion> ObtenerTraduccionesParaGestion(int idIdioma)
        {
            return _dal.ObtenerTraduccionesParaGestion(idIdioma);
        }

        public void GuardarTraduccionPorControl(int idIdioma, int idControl, string texto)
        {
            string estado = string.IsNullOrWhiteSpace(texto) ? "Pendiente" : "Completa";
            string digitoVerificador = null;

            if (!string.IsNullOrWhiteSpace(texto))
            {
                digitoVerificador = HashHelper.ObtenerHashSHA256(idIdioma + "|" + idControl + "|" + texto);
            }

            _dal.ActualizarTraduccion(idIdioma, idControl, texto, estado, digitoVerificador);
        }

        public void RegistrarClaveNueva(string nombreForm, string nombreControl, string textoBase)
        {
            if (_dal.ExisteControl(nombreControl, nombreForm)) return;

            int idControl = _dal.ObtenerOCrearControl(nombreControl, nombreForm);
            int idEspanol = _dal.ObtenerIdIdiomaPorCodigo("es");

            if (idEspanol == 0)
                throw new InvalidOperationException("No existe el idioma base Español.");

            _dal.CrearPendientesParaTodosLosIdiomas(idControl, idEspanol, textoBase);
        }
    }
}
