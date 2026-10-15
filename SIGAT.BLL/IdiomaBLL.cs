using SIGAT.BE.Idiomas;
using SIGAT.DAL;
using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.BLL
{
    public class IdiomaBLL
    {
        private readonly IdiomaDAL _dal;

        public IdiomaBLL(IdiomaDAL? dal = null) { _dal = dal ?? new IdiomaDAL(); }

        // Para elegir el idioma de la sesión ofrecemos solo los activos; la gestión también necesita ver los desactivados.
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
            CargarIdioma(idIdioma, guardarPreferencia: true);
        }

        // Leemos la preferencia de esta cuenta desde SQL. Si su idioma está inactivo, usamos otro sin borrar la elección guardada.
        public void RestablecerIdiomaUsuario()
        {
            var usuario = SesionServicio.ObtenerInstancia().UsuarioActual
                ?? throw new InvalidOperationException("Debe iniciar sesión para cargar su idioma.");
            var idiomas = _dal.ObtenerIdiomas();
            int? preferido = _dal.ObtenerIdiomaPreferido(usuario.IdUsuario);
            Idioma? idioma = idiomas.Find(i => i.Id == preferido)
                ?? idiomas.Find(i => string.Equals(i.Codigo, "es", StringComparison.OrdinalIgnoreCase))
                ?? idiomas.FirstOrDefault();
            if (idioma == null) throw new InvalidOperationException("No hay idiomas activos disponibles.");
            // El idioma de respaldo no reemplaza una preferencia temporalmente inactiva.
            CargarIdioma(idioma.Id, guardarPreferencia: false);
        }

        // Construimos un diccionario con claves de formulario y control. El Observer notifica a las pantallas para actualizar sus textos.
        private void CargarIdioma(int idIdioma, bool guardarPreferencia)
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

            if (guardarPreferencia)
            {
                // Solo una elección explícita cambia la preferencia; cargar un idioma de respaldo no la reemplaza.
                var usuario = SesionServicio.ObtenerInstancia().UsuarioActual
                    ?? throw new InvalidOperationException("Debe iniciar sesión para guardar su idioma.");
                _dal.GuardarIdiomaPreferido(usuario.IdUsuario, idIdioma);
            }
            IdiomaManager.ObtenerInstancia().CargarIdioma(idiomaElegido, diccionario);
        }

        // Validamos nombre y código antes de crear el idioma. Sus traducciones se generan inicialmente como pendientes.
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

            if (!activo)
            {
                if (IdiomaManager.ObtenerInstancia().IdiomaActual?.Id == idIdioma)
                    throw new InvalidOperationException("No se puede desactivar el idioma activo en la sesión actual.");
                if (_dal.TieneUsuariosAsignados(idIdioma))
                    throw new InvalidOperationException("No se puede desactivar el idioma porque uno o más usuarios registrados lo tienen elegido.");
            }
            _dal.CambiarEstadoIdioma(idIdioma, activo);
        }

        public List<Traduccion> ObtenerTraduccionesParaGestion(int idIdioma)
        {
            return _dal.ObtenerTraduccionesParaGestion(idIdioma);
        }

        // Un texto vacío queda pendiente; las traducciones ya no forman parte del control DVH/DVV.
        public void GuardarTraduccionPorControl(int idIdioma, int idControl, string texto)
        {
            string estado = string.IsNullOrWhiteSpace(texto) ? "Pendiente" : "Completa";
            _dal.ActualizarTraduccion(idIdioma, idControl, texto, estado);
        }

        // Cada etiqueta se identifica por formulario y clave. Las nuevas etiquetas reciben texto base y pendientes para los demás idiomas.
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
