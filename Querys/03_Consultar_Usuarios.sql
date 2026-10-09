-- La baja de cuentas debe hacerse desde Gestión de usuarios:
-- valida el último administrador y conserva las asignaciones y la auditoría.
USE [SIGAT];
GO
SELECT IdUsuario,NombreUsuario,Nombre,Apellido,Activo
FROM dbo.Usuarios ORDER BY Activo DESC,NombreUsuario;
SELECT COUNT(DISTINCT u.IdUsuario) AS AdministradoresActivos
FROM dbo.Usuarios u
JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
JOIN dbo.RolPermiso rp ON rp.IdRol=ur.IdRol
WHERE u.Activo=1 AND rp.IdPermiso=7;
GO
