using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.BE.Idiomas
{
    public class Idioma
    {
        public int Id { get; set; }
        public string Nombre { get; set; }

        public override string ToString()
        {
            return Nombre;
        }
    }
}
