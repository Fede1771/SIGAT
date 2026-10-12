using SIGAT.BE.Auditoria;

namespace SIGAT.BLL.Fabricas
{
    public abstract class BitacoraFactory
    {
        // Factory Method: la clase hija define el tipo de evento; acá completamos los datos comunes.
        public IBitacoraEvento Crear(string identificadorUsuario, string descripcion)
        {
            // 1. Llama al método abstracto que implementarán las fábricas hijas
            IBitacoraEvento evento = CrearEvento();

            // 2. Llena los datos que son comunes para todos los eventos del sistema
            evento.IdentificadorUsuario = identificadorUsuario;
            evento.Descripcion = descripcion;

            // Obtenemos el nombre de la PC de forma automática
            evento.Equipo = Environment.MachineName;
            evento.Fecha = DateTime.Now;

            return evento;
        }

        // Método que cada fábrica específica (hija) está obligada a sobreescribir
        protected abstract IBitacoraEvento CrearEvento();
    }
}