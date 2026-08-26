using System;

namespace SIGAT.BE
{
    public class Bitacora
    {
        public int IdBitacora { get; set; }
        public DateTime Fecha { get; set; }
        public string? Usuario { get; set; }
        public string? Actividad { get; set; }
        public string? InformacionAsociada { get; set; }
        public string? DigitoVerificador { get; set; }
    }
}