using SIGAT.BE;
using SIGAT.BE.Auditoria;

namespace SIGAT.BLL.Fabricas
{
    // Fábrica 1: Usada cuando el usuario entra bien
    public class LoginExitosoFactory : BitacoraFactory
    {
        protected override IBitacoraEvento CrearEvento()
        {
            Bitacora evento = new Bitacora();
            evento.Modulo = BitacoraModulo.Seguridad;
            evento.Accion = BitacoraAccion.LoginExitoso;
            evento.Nivel = BitacoraNivel.Informacion;
            return evento;
        }
    }

    // Fábrica 2: Usada cuando el usuario pone mal la clave
    public class LoginFallidoFactory : BitacoraFactory
    {
        protected override IBitacoraEvento CrearEvento()
        {
            Bitacora evento = new Bitacora();
            evento.Modulo = BitacoraModulo.Seguridad;
            evento.Accion = BitacoraAccion.LoginFallido;
            evento.Nivel = BitacoraNivel.Advertencia;
            return evento;
        }
    }

    // Fábrica 3: Usada cuando se crea, edita o elimina un usuario
    public class GestionUsuariosFactory : BitacoraFactory
    {
        private BitacoraAccion _accionEspecifica;

        // Usamos un constructor para pasarle la acción exacta (Alta, Baja, Modificacion)
        public GestionUsuariosFactory(BitacoraAccion accion)
        {
            _accionEspecifica = accion;
        }

        protected override IBitacoraEvento CrearEvento()
        {
            Bitacora evento = new Bitacora();
            evento.Modulo = BitacoraModulo.Usuarios;
            evento.Accion = _accionEspecifica;
            evento.Nivel = BitacoraNivel.Informacion;
            return evento;
        }
    }
}