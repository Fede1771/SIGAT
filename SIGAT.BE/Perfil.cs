using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.BE
{
    public class Perfil
    {
        public int IdPerfil { get; set; }
        public string NombrePerfil { get; set; }

        public override string ToString()
        {
            return NombrePerfil; // Útil para mostrar en ComboBox de UI
        }
    }
}
