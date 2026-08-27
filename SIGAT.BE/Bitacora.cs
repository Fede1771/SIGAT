using System;
using SIGAT.BE.Auditoria;

namespace SIGAT.BE
{
    // Implementamos la interfaz para que las Fábricas (Factories) reconozcan este objeto
    public class Bitacora : IBitacoraEvento
    {
        // --- 1. Propiedades originales (Las que viajan a la Base de Datos) ---
        public int IdBitacora { get; set; }
        public DateTime Fecha { get; set; }
        public string? Usuario { get; set; }
        public string? Actividad { get; set; }
        public string? InformacionAsociada { get; set; }
        public string? DigitoVerificador { get; set; }

        // --- 2. Propiedades requeridas por la Interfaz IBitacoraEvento ---
        public int Id { get; set; }
        public int? IdUsuario { get; set; }
        public string IdentificadorUsuario { get; set; }
        public BitacoraModulo Modulo { get; set; }
        public BitacoraAccion Accion { get; set; }
        public BitacoraNivel Nivel { get; set; }
        public string Descripcion { get; set; }
        public string Equipo { get; set; }
    }
}