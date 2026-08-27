using SIGAT.BE;

namespace SIGAT.BLL
{
    public static class BitacoraMapper
    {
        public static void PrepararParaBaseDeDatos(Bitacora evento)
        {
            // 1. Pasamos el Identificador que armó la fábrica a tu columna "Usuario"
            evento.Usuario = evento.IdentificadorUsuario;

            // 2. Convertimos el Enum de la acción a texto para tu columna "Actividad"
            evento.Actividad = evento.Accion.ToString();

            // 3. Juntamos toda la información de auditoría avanzada en tu columna "InformacionAsociada"
            evento.InformacionAsociada = "Módulo: " + evento.Modulo.ToString() +
                                         " | Nivel: " + evento.Nivel.ToString() +
                                         " | PC: " + evento.Equipo +
                                         " | Detalle: " + evento.Descripcion;
        }
    }
}