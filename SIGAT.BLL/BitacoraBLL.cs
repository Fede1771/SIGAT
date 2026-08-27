using System;
using System.Collections.Generic;
using SIGAT.BE;
using SIGAT.BE.Auditoria;
using SIGAT.DAL;

namespace SIGAT.BLL
{
    public class BitacoraBLL
    {
        private BitacoraDAL _dal = new BitacoraDAL();

        // --- NUEVO MÉTODO (Usa el Patrón Factory y el Mapper) ---
        public void Registrar(IBitacoraEvento evento)
        {
            // 1. Convertimos la interfaz genérica a nuestro objeto concreto
            Bitacora eventoConcreto = (Bitacora)evento;

            // 2. Pasamos el evento por el Mapper para adaptar las propiedades nuevas a la base de datos vieja
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

        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        {
            return _dal.Buscar(desde, hasta, usuario, actividad);
        }
    }
}