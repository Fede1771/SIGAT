USE [SIGAT];
GO
SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
-- Esquema 2 ya instalado: no reponer asignaciones retiradas.
DECLARE @Instalado bit = 0;
IF COL_LENGTH('dbo.SeguridadVersion','Esquema') IS NOT NULL
 EXEC sp_executesql N'SELECT @Valor=1 FROM dbo.SeguridadVersion WHERE Id=1 AND Esquema=2',
 N'@Valor bit OUTPUT', @Instalado OUTPUT;
IF @Instalado=1
BEGIN
 PRINT N'La matriz ya está instalada.';
 RETURN;
END;
IF OBJECT_ID('dbo.SeguridadVersion','U') IS NOT NULL
 THROW 50001, 'Para migrar el esquema anterior ejecute 10_Matriz_Roles_Exacta.sql.', 1;
BEGIN TRANSACTION;
EXEC(N'
CREATE TABLE dbo.SeguridadVersion (
 Id int NOT NULL PRIMARY KEY CHECK (Id=1),
 Version bigint NOT NULL,
 Esquema int NOT NULL CHECK (Esquema=2)
);
CREATE TABLE dbo.Permiso (
 Id int NOT NULL PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL UNIQUE,
 EsCompuesto bit NOT NULL,
 CONSTRAINT CK_Permiso_Matriz CHECK (
 (Id=1 AND Nombre=N''Ver bitácora'' AND EsCompuesto=0) OR
 (Id=2 AND Nombre=N''Consultar inventario'' AND EsCompuesto=0) OR
 (Id=3 AND Nombre=N''Gestión de activos'' AND EsCompuesto=1) OR
 (Id=4 AND Nombre=N''Movimientos'' AND EsCompuesto=1) OR
 (Id=5 AND Nombre=N''Mantenimiento'' AND EsCompuesto=1) OR
 (Id=6 AND Nombre=N''Reportes'' AND EsCompuesto=1) OR
 (Id=7 AND Nombre=N''Administración del sistema'' AND EsCompuesto=1))
);
CREATE TABLE dbo.Rol (
 Id int NOT NULL PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL UNIQUE,
 CONSTRAINT CK_Rol_Matriz CHECK (
 (Id=1 AND Nombre=N''Administrador'') OR
 (Id=2 AND Nombre=N''Responsable de Inventario'') OR
 (Id=3 AND Nombre=N''Técnico de Mantenimiento'') OR
 (Id=4 AND Nombre=N''Mesa de Ayuda'') OR
 (Id=5 AND Nombre=N''Auditor''))
);
CREATE TABLE dbo.RolPermiso (
 IdRol int NOT NULL REFERENCES dbo.Rol(Id),
 IdPermiso int NOT NULL REFERENCES dbo.Permiso(Id),
 PRIMARY KEY (IdRol,IdPermiso),
 CONSTRAINT CK_RolPermiso_Matriz CHECK (
 (IdRol=1 AND IdPermiso IN (1,3,4,5,6,7)) OR
 (IdRol=2 AND IdPermiso IN (3,4,6)) OR
 (IdRol=3 AND IdPermiso IN (5,2)) OR
 (IdRol=4 AND IdPermiso IN (4,2)) OR
 (IdRol=5 AND IdPermiso IN (1,2,6)))
);
CREATE TABLE dbo.UsuarioRol (
 IdUsuario int NOT NULL REFERENCES dbo.Usuarios(IdUsuario),
 IdRol int NOT NULL REFERENCES dbo.Rol(Id),
 PRIMARY KEY (IdUsuario,IdRol)
);
-- No se crea PermisoHijo: la matriz actual no define hijos para las familias.
INSERT dbo.Permiso VALUES
 (1,N''Ver bitácora'',0),(2,N''Consultar inventario'',0),
 (3,N''Gestión de activos'',1),(4,N''Movimientos'',1),
 (5,N''Mantenimiento'',1),(6,N''Reportes'',1),
 (7,N''Administración del sistema'',1);
INSERT dbo.Rol VALUES
 (1,N''Administrador''),(2,N''Responsable de Inventario''),
 (3,N''Técnico de Mantenimiento''),(4,N''Mesa de Ayuda''),(5,N''Auditor'');
INSERT dbo.RolPermiso VALUES
 (1,1),(1,3),(1,4),(1,5),(1,6),(1,7),
 (2,3),(2,4),(2,6),(3,5),(3,2),(4,4),(4,2),(5,1),(5,2),(5,6);
');
EXEC(N'INSERT dbo.SeguridadVersion VALUES (1,1,2);
 INSERT dbo.UsuarioRol (IdUsuario,IdRol)
 SELECT u.IdUsuario,1 FROM dbo.Usuarios u
 WHERE u.NombreUsuario=''admin'' AND u.Activo=1;');
COMMIT;
GO
