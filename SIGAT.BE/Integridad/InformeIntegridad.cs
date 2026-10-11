namespace SIGAT.BE.Integridad;

public sealed record ProblemaIntegridad(string Tabla, string Registro, string Motivo);

public sealed class InformeIntegridad
{
    public bool Configurada { get; set; }
    public List<ProblemaIntegridad> Problemas { get; } = new();
    public bool Correcta => Configurada && Problemas.Count == 0;
}

public sealed class IntegridadException : InvalidOperationException
{
    public InformeIntegridad Informe { get; }
    public IntegridadException(InformeIntegridad informe)
        : base("Se detectó un problema de integridad. La operación fue bloqueada. " +
               string.Join("; ", informe.Problemas.Take(3).Select(p => $"{p.Tabla} [{p.Registro}]: {p.Motivo}")))
        => Informe = informe;
}
