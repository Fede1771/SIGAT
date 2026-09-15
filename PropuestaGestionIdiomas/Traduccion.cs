namespace SIGAT.BE.Idiomas
{
    public class Traduccion
    {
        public int IdIdioma { get; set; }
        public int IdControl { get; set; }
        public string Texto { get; set; }
        public string DigitoVerificador { get; set; }
        public string ControlNombre { get; set; }
        public string FormNombre { get; set; }
        public string Estado { get; set; }
        public string TextoBase { get; set; }
    }
}
