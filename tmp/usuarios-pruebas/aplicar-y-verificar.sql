USE SIGAT;
SELECT IdUsuario,NombreUsuario,Password,Nombre,Apellido,Activo INTO #CuentasAntes FROM dbo.Usuarios;
SELECT IdUsuario,IdRol INTO #RolesAntes FROM dbo.UsuarioRol;
GO
USE [SIGAT];
GO
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.RolPermiso','U') IS NULL
    THROW 50001, 'Instale primero la matriz de roles (script 10).', 1;

-- No toca cuentas, contraseñas ni asignaciones de roles.
IF COL_LENGTH('dbo.Usuarios','IdPerfil') IS NOT NULL
BEGIN
    DECLARE @Sql nvarchar(max)=N'';
    SELECT @Sql=@Sql+N'ALTER TABLE dbo.Usuarios DROP CONSTRAINT '+QUOTENAME(fk.name)+N';'
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fc ON fc.constraint_object_id=fk.object_id
    JOIN sys.columns c ON c.object_id=fc.parent_object_id AND c.column_id=fc.parent_column_id
    WHERE fk.parent_object_id=OBJECT_ID('dbo.Usuarios') AND c.name='IdPerfil';
    SELECT @Sql=@Sql+N'ALTER TABLE dbo.Usuarios DROP CONSTRAINT '+QUOTENAME(dc.name)+N';'
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id
    WHERE dc.parent_object_id=OBJECT_ID('dbo.Usuarios') AND c.name='IdPerfil';
    EXEC sp_executesql @Sql;
    ALTER TABLE dbo.Usuarios DROP COLUMN IdPerfil;
END;
DROP TABLE IF EXISTS dbo.Perfiles;
COMMIT;
GO

IF EXISTS (SELECT * FROM #CuentasAntes EXCEPT SELECT IdUsuario,NombreUsuario,Password,Nombre,Apellido,Activo FROM dbo.Usuarios)
 OR EXISTS (SELECT IdUsuario,NombreUsuario,Password,Nombre,Apellido,Activo FROM dbo.Usuarios EXCEPT SELECT * FROM #CuentasAntes)
 THROW 50002, 'La comparación de cuentas cambió.', 1;
IF EXISTS (SELECT * FROM #RolesAntes EXCEPT SELECT IdUsuario,IdRol FROM dbo.UsuarioRol)
 OR EXISTS (SELECT IdUsuario,IdRol FROM dbo.UsuarioRol EXCEPT SELECT * FROM #RolesAntes)
 THROW 50003, 'La comparación de roles cambió.', 1;
SELECT COUNT(*) AS CuentasConservadas FROM dbo.Usuarios;
SELECT COUNT(*) AS AsignacionesConservadas FROM dbo.UsuarioRol;
SELECT COL_LENGTH('dbo.Usuarios','IdPerfil') AS ColumnaPerfil, OBJECT_ID('dbo.Perfiles','U') AS TablaPerfiles;
GO