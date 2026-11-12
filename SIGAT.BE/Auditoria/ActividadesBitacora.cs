namespace SIGAT.BE.Auditoria;

// Compatibilidad de lectura con los nombres que usaban los primeros registros.
// Los eventos guardados no se renombran ni se modifica su información de auditoría.
public static class ActividadesBitacora
{
    private static readonly IReadOnlyDictionary<string, string> Equivalencias =
        new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            [nameof(BitacoraAccion.LoginExitoso)] = nameof(BitacoraAccion.LoginExitoso),
            ["Login Exitoso"] = nameof(BitacoraAccion.LoginExitoso),
            [nameof(BitacoraAccion.ErrorCritico)] = nameof(BitacoraAccion.ErrorCritico),
            ["Error Crítico del Sistema"] = nameof(BitacoraAccion.ErrorCritico)
        };

    public static string CodigoCanonico(string? actividad) => actividad != null &&
        Equivalencias.TryGetValue(actividad, out var codigo) ? codigo : actividad ?? "";

    public static IReadOnlyList<string> VariantesParaFiltro(string? actividad)
    {
        string codigo = CodigoCanonico(actividad);
        if (codigo.Length == 0) return Array.Empty<string>();
        return Equivalencias.Where(par => par.Value == codigo).Select(par => par.Key)
            .Append(codigo).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
    }
}
