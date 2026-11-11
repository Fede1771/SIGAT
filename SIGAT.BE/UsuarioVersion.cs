namespace SIGAT.BE;

public enum OperacionUsuarioVersion
{
    EstadoInicial, Alta, Edicion, Baja, Reactivacion, Roles, Restauracion, Recuperacion
}

public sealed record AutorCambioUsuario(int? IdUsuario, string Nombre);
public sealed record RolVersionUsuario(int Id, string Nombre);

public sealed record EstadoUsuarioVersion(string NombreUsuario, string Nombre, string Apellido,
    bool Activo, IReadOnlyList<RolVersionUsuario> Roles)
{
    public bool MismosRolesQue(EstadoUsuarioVersion otro) =>
        Roles.Select(rol => rol.Id).SequenceEqual(otro.Roles.Select(rol => rol.Id));

    public bool CoincideCon(EstadoUsuarioVersion otro) =>
        NombreUsuario == otro.NombreUsuario && Nombre == otro.Nombre && Apellido == otro.Apellido &&
        Activo == otro.Activo && MismosRolesQue(otro);
}

public sealed record UsuarioVersion(int IdUsuario, int Numero, DateTime FechaUtc, AutorCambioUsuario Autor,
    OperacionUsuarioVersion Operacion, EstadoUsuarioVersion Estado, bool Existe, bool CambioClave, int? VersionOrigen);

public sealed record CuentaConHistorial(int IdUsuario, string NombreUsuario, bool Existe);

public sealed record HistorialUsuario(long VersionSeguridad, EstadoUsuarioVersion? EstadoActual,
    IReadOnlyList<UsuarioVersion> Versiones);
