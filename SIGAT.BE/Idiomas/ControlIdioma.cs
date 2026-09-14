using System;
using System.Collections.Generic;
using System.Text;

namespace SIGAT.BE.Idiomas
{
    // Se llama "ControlIdioma" para no chocar con System.Windows.Forms.Control
    public class ControlIdioma
    {
        public int Id { get; set; }
        public string Control { get; set; }
        public string Form { get; set; }
    }
}
