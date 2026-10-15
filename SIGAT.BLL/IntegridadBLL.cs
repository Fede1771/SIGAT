using SIGAT.BE.Integridad;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL;

// La BLL controla la autorización para recuperar; el DAL realiza las comprobaciones y las operaciones sobre SQL.
public sealed class IntegridadBLL
{
    private readonly IntegridadDAL dal;
    private readonly BitacoraBLL bitacora;
    public IntegridadBLL(IntegridadDAL? repositorio = null, BitacoraBLL? auditoria = null)
    {
        dal = repositorio ?? new IntegridadDAL();
        bitacora = auditoria ?? new BitacoraBLL();
    }
    public InformeIntegridad Verificar() => dal.Verificar();
    // Para administrar la recuperación se necesitan permisos en SIGAT y permisos administrativos en SQL Server.
    public bool PuedeAdministrar() => EsAdministradorAplicacion() && dal.PuedeAdministrar();
    private static bool EsAdministradorAplicacion()
    {
        var usuario = SesionServicio.ObtenerInstancia().UsuarioActual;
        // Antes del login, la autorización depende del administrador SQL de Windows.
        return usuario == null || usuario.TienePermiso(MatrizRoles.Administracion);
    }
    private static void ExigirAdministradorAplicacion()
    {
        if (!EsAdministradorAplicacion()) throw new UnauthorizedAccessException("Esta cuenta de SIGAT no puede administrar la recuperación.");
    }
    public string InicializarBaseConfiable() { ExigirAdministradorAplicacion(); return dal.InicializarBaseConfiable(); }
    public string CrearRespaldoConfiable(string? destino = null) { ExigirAdministradorAplicacion(); return dal.CrearRespaldoConfiable(destino); }
    public int ArchivarBitacora(int dias) { ExigirAdministradorAplicacion(); return dal.ArchivarBitacora(dias); }
    public InformeIntegridad Recuperar(string ruta, bool tablasCompletas = false) { ExigirAdministradorAplicacion(); return dal.Recuperar(ruta, tablasCompletas); }
    // Aceptar el estado actual es una decisión administrativa explícita y queda registrada en SQL y en el log externo.
    public InformeIntegridad RecalcularIntegridad()
    {
        ExigirAdministradorAplicacion();
        var informe = dal.RecalcularIntegridad();
        string responsable = SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario ?? "SQL:" + Environment.UserName;
        bitacora.Registrar(responsable, "RecalculoIntegridad",
            "El administrador aceptó el estado actual, incluidas posibles alteraciones o pérdidas, y recalculó DVH/DVV de Usuarios y UsuarioRol.");
        return informe;
    }

    // Las operaciones normales se bloquean si hay diferencias; nunca recalculan automáticamente para permitir el acceso.
    public void ExigirIntegridad()
    {
        var informe = Verificar();
        if (!informe.Correcta) throw new IntegridadException(informe);
    }
    public static void RegistrarIncidente(InformeIntegridad informe)
        => IntegridadDAL.Registrar(string.Join("; ", informe.Problemas.Select(p => $"{p.Tabla} [{p.Registro}]: {p.Motivo}")));
}
