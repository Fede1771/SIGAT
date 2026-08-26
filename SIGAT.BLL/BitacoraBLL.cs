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
            Bitacora b = new Bitacora
            {
                Fecha = DateTime.Now,
                Usuario = usuario,
                Actividad = actividad,
                InformacionAsociada = informacion
            };
            _dal.Insertar(b);
        }

        // NUEVO MÉTODO: El puente para que la UI le pida los datos a la DAL
        public List<Bitacora> Buscar(DateTime? desde, DateTime? hasta, string? usuario, string? actividad)
        {
            return _dal.Buscar(desde, hasta, usuario, actividad);
        }
    }
}