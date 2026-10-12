using SIGAT.BE;

namespace SIGAT.BLL
{
    public static class BitacoraMapper
    {
        public static void PrepararParaBaseDeDatos(Bitacora evento)
        {
            // El identificador del evento se guarda en la columna Usuario.
            evento.Usuario = evento.IdentificadorUsuario;

            // La acción se define con un enum y se persiste como texto en Actividad.
            evento.Actividad = evento.Accion.ToString();

            // Agrupamos módulo, nivel, equipo y descripción en el detalle que luego cifra el DAL.
            evento.InformacionAsociada = "Módulo: " + evento.Modulo.ToString() +
                                         " | Nivel: " + evento.Nivel.ToString() +
                                         " | PC: " + evento.Equipo +
                                         " | Detalle: " + evento.Descripcion;
        }
    }
}