USE [SIGAT];
GO
-- Debe devolver 16 filas: los componentes directos exactos de cada rol.
SELECT r.Nombre AS Rol,
       CASE WHEN p.EsCompuesto=1 THEN N'Compuesto' ELSE N'Simple' END AS Tipo,
       p.Nombre AS Permiso
FROM dbo.Rol r
JOIN dbo.RolPermiso rp ON rp.IdRol=r.Id
JOIN dbo.Permiso p ON p.Id=rp.IdPermiso
ORDER BY r.Id,p.Id;
GO
-- Incluye cuentas sin rol para detectar cuáles requieren una asignación.
SELECT u.IdUsuario,u.NombreUsuario,u.Activo,r.Nombre AS Rol
FROM dbo.Usuarios u
LEFT JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
LEFT JOIN dbo.Rol r ON r.Id=ur.IdRol
ORDER BY u.NombreUsuario,r.Nombre;
GO
-- Permisos efectivos: unión de los componentes de los roles asignados.
DECLARE @Usuario varchar(50)='valen1';
SELECT DISTINCT p.Nombre AS Permiso,p.EsCompuesto
FROM dbo.Usuarios u
JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
JOIN dbo.RolPermiso rp ON rp.IdRol=ur.IdRol
JOIN dbo.Permiso p ON p.Id=rp.IdPermiso
WHERE u.NombreUsuario=@Usuario AND u.Activo=1;
GO
