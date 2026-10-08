-- Ejecutar sobre SIGAT después del script 13. Conserva las asignaciones existentes.
SET XACT_ABORT ON;
BEGIN TRANSACTION;
IF OBJECT_ID('dbo.RolHijo','U') IS NULL
    THROW 50001, 'Ejecute primero el script 13.', 1;
IF OBJECT_ID('dbo.PermisoHijo','U') IS NULL
BEGIN
    CREATE TABLE dbo.PermisoHijo (
        IdPadre int NOT NULL REFERENCES dbo.Permiso(Id),
        IdHijo int NOT NULL REFERENCES dbo.Permiso(Id),
        PRIMARY KEY (IdPadre,IdHijo),
        CHECK (IdPadre IN (3,4,5,6,7) AND IdPadre<>IdHijo)
    );
    UPDATE dbo.SeguridadVersion SET Version=Version+1 WHERE Id=1;
END;
COMMIT;
