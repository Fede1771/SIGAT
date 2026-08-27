using System;

namespace SIGAT.BE.Auditoria
{
    public interface IBitacoraEvento
    {
        int Id { get; set; }
        int? IdUsuario { get; set; }
        string IdentificadorUsuario { get; set; }
        BitacoraModulo Modulo { get; set; }
        BitacoraAccion Accion { get; set; }
        BitacoraNivel Nivel { get; set; }
        string Descripcion { get; set; }
        string Equipo { get; set; }
        DateTime Fecha { get; set; }
    }
}