-- Ejecutar sobre SIGAT después de los scripts 10 y 12.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.AdministradorOriginal','U') IS NULL
BEGIN
    -- La instalación original crea la cuenta admin con ID 1. No adivinar otra cuenta.
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios u JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
                   WHERE u.IdUsuario=1 AND u.Activo=1 AND ur.IdRol=1)
        THROW 50001, 'No se encontró la cuenta original activa (ID 1) con rol Administrador. Revisar antes de migrar.', 1;
    CREATE TABLE dbo.AdministradorOriginal (
        Id int NOT NULL PRIMARY KEY CHECK (Id=1),
        IdUsuario int NOT NULL UNIQUE REFERENCES dbo.Usuarios(IdUsuario)
    );
    INSERT dbo.AdministradorOriginal VALUES (1,1);
END;
IF OBJECT_ID('dbo.RolHijo','U') IS NULL
    CREATE TABLE dbo.RolHijo (
        IdPadre int NOT NULL REFERENCES dbo.Rol(Id),
        IdHijo int NOT NULL REFERENCES dbo.Rol(Id),
        PRIMARY KEY (IdPadre,IdHijo), CHECK (IdPadre<>IdHijo)
    );
IF OBJECT_ID('dbo.CK_Rol_Matriz','C') IS NOT NULL ALTER TABLE dbo.Rol DROP CONSTRAINT CK_Rol_Matriz;
IF OBJECT_ID('dbo.CK_RolPermiso_Matriz','C') IS NOT NULL ALTER TABLE dbo.RolPermiso DROP CONSTRAINT CK_RolPermiso_Matriz;
UPDATE dbo.SeguridadVersion SET Version=Version+1 WHERE Id=1;
COMMIT;
GO
CREATE OR ALTER TRIGGER dbo.TR_Usuarios_AdministradorOriginal ON dbo.Usuarios
AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted d JOIN dbo.AdministradorOriginal a ON a.IdUsuario=d.IdUsuario
               LEFT JOIN inserted i ON i.IdUsuario=d.IdUsuario WHERE i.IdUsuario IS NULL OR i.Activo=0)
        THROW 50001, 'No se puede eliminar ni desactivar al administrador original.', 1;
END;
GO
