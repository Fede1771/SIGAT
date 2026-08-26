using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.BE
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        public string NombreUsuario { get; set; }
        public string Password { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public bool Activo { get; set; }
        public bool DosFactorActivo { get; set; }
        public int IdPerfil { get; set; }
        public Perfil Perfil { get; set; }
    }
}
