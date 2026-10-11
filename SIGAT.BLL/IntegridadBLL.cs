using SIGAT.BE.Integridad;
using SIGAT.DAL;
using SIGAT.SERVICIOS;

namespace SIGAT.BLL;

public sealed class IntegridadBLL
{
    private readonly IntegridadDAL dal;
    public IntegridadBLL(IntegridadDAL? repositorio = null) => dal = repositorio ?? new IntegridadDAL();
    public InformeIntegridad Verificar() => dal.Verificar();
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
    public string CrearRespaldoConfiable() { ExigirAdministradorAplicacion(); return dal.CrearRespaldoConfiable(); }
    public int ArchivarBitacora(int dias) { ExigirAdministradorAplicacion(); return dal.ArchivarBitacora(dias); }
    public InformeIntegridad Recuperar(string ruta, bool tablasCompletas = false) { ExigirAdministradorAplicacion(); return dal.Recuperar(ruta, tablasCompletas); }
    public void ExigirIntegridad()
    {
        var informe = Verificar();
        if (!informe.Correcta) throw new IntegridadException(informe);
    }
    public static void RegistrarIncidente(InformeIntegridad informe)
        => IntegridadDAL.Registrar(string.Join("; ", informe.Problemas.Select(p => $"{p.Tabla} [{p.Registro}]: {p.Motivo}")));
}
