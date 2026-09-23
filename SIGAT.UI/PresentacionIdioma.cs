using SIGAT.BE.Idiomas;

namespace SIGAT.UI
{
    public static class PresentacionIdioma
    {
        public static string Etiqueta(Idioma idioma, int? idActivo)
        {
            string codigo = (idioma.Codigo ?? "").Trim().ToLowerInvariant().Split('-', '_')[0];
            string nombre = !string.IsNullOrWhiteSpace(idioma.NombreNativo)
                ? idioma.NombreNativo
                : codigo switch
                {
                    "es" => "Español",
                    "en" => "English",
                    "pt" => "Português",
                    "ru" => "Русский",
                    _ => idioma.Nombre
                };

            if (idioma.Id != idActivo) return nombre;

            string simbolo = codigo switch
            {
                "es" => "*",
                "en" => ".",
                "pt" => "()",
                "ru" => "-",
                _ => "✓"
            };
            return nombre + " " + simbolo;
        }
    }
}
