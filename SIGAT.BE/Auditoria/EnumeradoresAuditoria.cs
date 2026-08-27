namespace SIGAT.BE.Auditoria
{
    // Define a qué parte del sistema pertenece el evento
    public enum BitacoraModulo
    {
        Seguridad,
        Usuarios,
        Sistema
    }

    // Define qué acción específica ocurrió
    public enum BitacoraAccion
    {
        LoginExitoso,
        LoginFallido,
        AltaUsuario,
        ModificacionUsuario,
        BajaUsuario,
        Logout,
        ErrorCritico
    }

    // Define la gravedad del evento
    public enum BitacoraNivel
    {
        Informacion,
        Advertencia,
        Critico
    }
}