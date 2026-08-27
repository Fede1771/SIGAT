using System;
using System.Collections.Generic;
using SIGAT.BE;
using SIGAT.DAL;

namespace SIGAT.BLL
{
    public class BitacoraBLL
    {
        private BitacoraDAL _dal = new BitacoraDAL();

        public void Registrar(string usuario, string actividad, string informacion)
        {
            // Creamos el objeto y le cargamos los datos uno por uno
            Bitacora nuevoRegistro = new Bitacora();
            nuevoRegistro.Fecha = DateTime.Now;
            nuevoRegistro.Usuario = usuario;
            nuevoRegistro.Actividad = actividad;
            nuevoRegistro.InformacionAsociada = informacion;

            // Lo enviamos a la capa de datos para guardar en la tabla
            _dal.Insertar(nuevoRegistro);
        }

        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        {
            // Actuamos como puente: recibimos los filtros de la pantalla y se los pasamos a la base de datos
            return _dal.Buscar(desde, hasta, usuario, actividad);
        }
    }
}