USE [SIGAT];
GO
SET ANSI_NULLS ON;
SET ANSI_PADDING ON;
SET ANSI_WARNINGS ON;
SET ARITHABORT ON;
SET CONCAT_NULL_YIELDS_NULL ON;
SET QUOTED_IDENTIFIER ON;
SET NUMERIC_ROUNDABORT OFF;
SET XACT_ABORT ON;
BEGIN TRANSACTION;

-- Migración única: volver a ejecutarla no repone permisos que se hayan quitado.
IF OBJECT_ID(N'dbo.SeguridadVersion', N'U') IS NULL
BEGIN
    CREATE TABLE dbo.SeguridadVersion (
        Id int NOT NULL PRIMARY KEY CHECK (Id = 1),
        Version bigint NOT NULL
    );
    CREATE TABLE dbo.Permiso (
        Id int NOT NULL PRIMARY KEY,
        Nombre nvarchar(150) NOT NULL,
        EsCompuesto bit NOT NULL
    );
    CREATE TABLE dbo.PermisoHijo (
        IdPadre int NOT NULL REFERENCES dbo.Permiso(Id),
        IdHijo int NOT NULL REFERENCES dbo.Permiso(Id),
        PRIMARY KEY (IdPadre, IdHijo),
        CHECK (IdPadre <> IdHijo)
    );
    CREATE TABLE dbo.Rol (
        Id int NOT NULL PRIMARY KEY,
        IdFamilia int NOT NULL UNIQUE REFERENCES dbo.Permiso(Id),
        IdUsuarioPersonal int NULL REFERENCES dbo.Usuarios(IdUsuario)
    );
    CREATE UNIQUE INDEX UX_Rol_UsuarioPersonal ON dbo.Rol(IdUsuarioPersonal)
        WHERE IdUsuarioPersonal IS NOT NULL;
    CREATE TABLE dbo.UsuarioRol (
        IdUsuario int NOT NULL REFERENCES dbo.Usuarios(IdUsuario),
        IdRol int NOT NULL REFERENCES dbo.Rol(Id),
        PRIMARY KEY (IdUsuario, IdRol)
    );

    INSERT dbo.Permiso (Id, Nombre, EsCompuesto) VALUES
        (1, N'Bitácora', 0), (2, N'Control de Cambios', 0),
        (3, N'Gestión de Roles', 0), (4, N'Generar Copia de Seguridad', 0),
        (5, N'Gestión de Usuarios', 0), (6, N'Gestión de Idiomas', 0),
        (7, N'Administrador', 1), (8, N'Operador', 1), (9, N'Gestión', 1);
    INSERT dbo.Rol (Id, IdFamilia, IdUsuarioPersonal)
        VALUES (1, 7, NULL), (2, 8, NULL), (3, 9, NULL);
    INSERT dbo.PermisoHijo (IdPadre, IdHijo)
        VALUES (7, 1), (7, 2), (7, 3), (7, 4), (7, 5), (7, 6),
               (8, 1), (8, 6);

    -- Conserva los accesos anteriores una sola vez, también para cuentas inactivas.
    INSERT dbo.UsuarioRol (IdUsuario, IdRol)
    SELECT u.IdUsuario, CASE WHEN p.NombrePerfil = 'Administrador' THEN 1 ELSE 2 END
    FROM dbo.Usuarios u
    INNER JOIN dbo.Perfiles p ON p.IdPerfil = u.IdPerfil;

    INSERT dbo.SeguridadVersion (Id, Version) VALUES (1, 1);
END;
COMMIT;
GO
