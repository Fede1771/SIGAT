-- Solo lectura. Ejecutar después del instalador o de la actualización.
USE [SIGAT];
SET NOCOUNT ON;
DBCC CHECKDB ('SIGAT') WITH NO_INFOMSGS;
-- CHECKDB ya incluye CHECKTABLE: no es necesario repetirlo para cada tabla.
DECLARE @Requeridas TABLE (Nombre sysname);
INSERT @Requeridas VALUES
 ('Usuarios'),('Bitacora'),('Idioma'),('Control'),('Traduccion'),
 ('SeguridadVersion'),('Permiso'),('Rol'),('RolPermiso'),('UsuarioRol'),
 ('AdministradorOriginal'),('RolHijo'),('PermisoHijo'),('UsuarioIdioma');
SELECT Nombre AS TablaFaltante FROM @Requeridas
WHERE OBJECT_ID(N'dbo.' + Nombre,'U') IS NULL;
IF EXISTS (SELECT 1 FROM @Requeridas WHERE OBJECT_ID(N'dbo.' + Nombre,'U') IS NULL)
    THROW 50001, 'Faltan tablas. Ejecute la instalación o actualización completa.', 1;
SELECT Version,Esquema FROM dbo.SeguridadVersion WHERE Id=1;
SELECT a.IdUsuario,u.NombreUsuario,u.Activo,
       CASE WHEN ur.IdUsuario IS NULL THEN 0 ELSE 1 END AS TieneRolAdministrador
FROM dbo.AdministradorOriginal a
LEFT JOIN dbo.Usuarios u ON u.IdUsuario=a.IdUsuario
LEFT JOIN dbo.UsuarioRol ur ON ur.IdUsuario=a.IdUsuario AND ur.IdRol=1;
SELECT name AS TriggerProteccion, is_disabled AS Deshabilitado
FROM sys.triggers WHERE name='TR_Usuarios_AdministradorOriginal';
SELECT name AS RestriccionAntigua FROM sys.check_constraints
WHERE name IN ('CK_Rol_Matriz','CK_RolPermiso_Matriz');
SELECT COL_LENGTH('dbo.Usuarios','IdPerfil') AS ColumnaPerfilAntigua;
SELECT fk.name AS NombreFK, OBJECT_NAME(fk.parent_object_id) AS Tabla,
       fk.is_disabled AS Deshabilitada, fk.is_not_trusted AS NoConfiable
FROM sys.foreign_keys fk ORDER BY Tabla,NombreFK;
SELECT c.Id AS IdControl,c.Control,c.Form,i.Nombre AS IdiomaFaltante
FROM dbo.Control c CROSS JOIN dbo.Idioma i
LEFT JOIN dbo.Traduccion t ON t.Id_Control=c.Id AND t.Id_Idioma=i.Id
WHERE i.Activo=1 AND t.Id_Control IS NULL
ORDER BY c.Form,c.Control,i.Nombre;
-- Pendiente es un estado permitido; no significa corrupción de la base.
SELECT i.Nombre,t.Estado,COUNT(*) AS Cantidad
FROM dbo.Traduccion t JOIN dbo.Idioma i ON i.Id=t.Id_Idioma
GROUP BY i.Nombre,t.Estado ORDER BY i.Nombre,t.Estado;
GO
