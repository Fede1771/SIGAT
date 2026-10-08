USE [SIGAT];
GO
-- Componentes directos actuales; inicialmente hay 16 relaciones.
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
-- Requiere script 14. Incluye roles anidados y componentes de familias.
DECLARE @Usuario varchar(50)='valen1';
;WITH RolesEfectivos AS (
 SELECT ur.IdRol FROM dbo.UsuarioRol ur JOIN dbo.Usuarios u ON u.IdUsuario=ur.IdUsuario
 WHERE u.NombreUsuario=@Usuario AND u.Activo=1
 UNION ALL
 SELECT rh.IdHijo FROM RolesEfectivos re JOIN dbo.RolHijo rh ON rh.IdPadre=re.IdRol
), PermisosEfectivos AS (
 SELECT rp.IdPermiso FROM RolesEfectivos re JOIN dbo.RolPermiso rp ON rp.IdRol=re.IdRol
 UNION ALL
 SELECT ph.IdHijo FROM PermisosEfectivos pe JOIN dbo.PermisoHijo ph ON ph.IdPadre=pe.IdPermiso
)
SELECT DISTINCT p.Nombre AS Permiso,p.EsCompuesto
FROM PermisosEfectivos pe JOIN dbo.Permiso p ON p.Id=pe.IdPermiso
OPTION (MAXRECURSION 100);
GO
SELECT padre.Nombre AS RolPadre,hijo.Nombre AS RolAnidado
FROM dbo.RolHijo rh JOIN dbo.Rol padre ON padre.Id=rh.IdPadre JOIN dbo.Rol hijo ON hijo.Id=rh.IdHijo;
GO
