using SIGAT.BE;
using SIGAT.BE.Auditoria;
using SIGAT.DAL;

namespace SIGAT.BLL
{
    public class BitacoraBLL
    {
        private BitacoraDAL _dal = new BitacoraDAL();

        // Recibimos el evento creado por la fábrica y lo adaptamos al formato que guarda la bitácora.
        public void Registrar(IBitacoraEvento evento)
        {
            // 1. Convertimos la interfaz genérica a nuestro objeto concreto
            Bitacora eventoConcreto = (Bitacora)evento;

            // El Mapper transforma los datos del evento en las columnas de la tabla Bitacora.
            BitacoraMapper.PrepararParaBaseDeDatos(eventoConcreto);

            // 3. Lo mandamos a guardar
            _dal.Insertar(eventoConcreto);
        }

        // --- MÉTODO CLÁSICO (Se mantiene por compatibilidad con la UI / Sobrecarga) ---
        public void Registrar(string usuario, string actividad, string informacion)
        {
            Bitacora registroClasico = new Bitacora();
            registroClasico.Fecha = DateTime.Now;
            registroClasico.Usuario = usuario;
            registroClasico.Actividad = actividad;
            registroClasico.InformacionAsociada = informacion;

            _dal.Insertar(registroClasico);
        }

        // Los filtros son opcionales; la DAL verifica la integridad antes de devolver los eventos para su consulta.
        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        {
            return _dal.Buscar(desde, hasta, usuario, actividad);
        }
    }
}
