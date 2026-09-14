using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.BE.Idiomas
{
    public class Traduccion
    {
        public int IdIdioma { get; set; }
        public int IdControl { get; set; }
        public string Texto { get; set; }
        public string DigitoVerificador { get; set; }

        // Campos auxiliares, no son columnas de la tabla
        public string ControlNombre { get; set; }
        public string FormNombre { get; set; }
    }
}
