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
BEGIN TRANSACTION;
DECLARE @Version bigint = 1;
CREATE TABLE #Asignaciones (IdUsuario int NOT NULL, NombreRol nvarchar(150) NOT NULL);
IF OBJECT_ID('dbo.SeguridadVersion','U') IS NOT NULL
 EXEC sp_executesql N'SELECT @Valor=Version+1 FROM dbo.SeguridadVersion WITH (UPDLOCK,HOLDLOCK) WHERE Id=1',
 N'@Valor bigint OUTPUT', @Version OUTPUT;

-- Solo conservar roles que coincidan por nombre, nunca por los IDs anteriores.
IF COL_LENGTH('dbo.Rol','IdFamilia') IS NOT NULL
 EXEC(N'INSERT #Asignaciones SELECT ur.IdUsuario,p.Nombre
 FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.Id=ur.IdRol
 JOIN dbo.Permiso p ON p.Id=r.IdFamilia WHERE r.IdUsuarioPersonal IS NULL');

-- Preservar administradores existentes para evitar perder el acceso.
INSERT #Asignaciones
 SELECT u.IdUsuario,N'Administrador' FROM dbo.Usuarios u
 JOIN dbo.Perfiles p ON p.IdPerfil=u.IdPerfil
 WHERE p.NombrePerfil='Administrador'
 AND NOT EXISTS (SELECT 1 FROM #Asignaciones a WHERE a.IdUsuario=u.IdUsuario AND a.NombreRol=N'Administrador');

IF NOT EXISTS (SELECT 1 FROM #Asignaciones a JOIN dbo.Usuarios u ON u.IdUsuario=a.IdUsuario
 WHERE u.Activo=1 AND a.NombreRol=N'Administrador')
 THROW 50001, 'Debe existir un administrador activo antes de migrar.', 1;

-- Informar qué asignaciones desaparecen, sin convertirlas en otro rol.
SELECT IdUsuario,NombreRol AS RolRetirado FROM #Asignaciones
WHERE NombreRol NOT IN (N'Administrador',N'Responsable de Inventario',
 N'Técnico de Mantenimiento',N'Mesa de Ayuda',N'Auditor');

DROP TABLE IF EXISTS dbo.UsuarioRol;
DROP TABLE IF EXISTS dbo.RolPermiso;
DROP TABLE IF EXISTS dbo.Rol;
DROP TABLE IF EXISTS dbo.PermisoHijo;
DROP TABLE IF EXISTS dbo.Permiso;
DROP TABLE IF EXISTS dbo.SeguridadVersion;

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
EXEC sp_executesql N'INSERT dbo.SeguridadVersion VALUES (1,@Valor,2)', N'@Valor bigint', @Version;
EXEC(N'INSERT dbo.UsuarioRol (IdUsuario,IdRol)
 SELECT DISTINCT a.IdUsuario,r.Id FROM #Asignaciones a
 JOIN dbo.Rol r ON r.Nombre=a.NombreRol');
COMMIT;
PRINT N'Matriz instalada: 5 roles, 7 permisos y 16 relaciones.';
GO

