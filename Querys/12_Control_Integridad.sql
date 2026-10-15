-- Estructura de integridad para Usuarios y UsuarioRol. No calcula ni acepta datos automáticamente.
-- Cerrar SIGAT; respaldar la base; ejecutar y preparar luego la referencia desde Integridad y recuperación.
USE [SIGAT];
SET XACT_ABORT ON;
BEGIN TRY
    SET TRANSACTION ISOLATION LEVEL SERIALIZABLE;
    BEGIN TRANSACTION;
    IF COL_LENGTH('dbo.Usuarios','DigitoVerificador') IS NULL
        ALTER TABLE dbo.Usuarios ADD DigitoVerificador char(64) NULL;
    IF COL_LENGTH('dbo.UsuarioRol','DigitoVerificador') IS NULL
        ALTER TABLE dbo.UsuarioRol ADD DigitoVerificador char(64) NULL;
    IF OBJECT_ID('dbo.IntegridadTabla','U') IS NULL
        CREATE TABLE dbo.IntegridadTabla (
            Tabla nvarchar(30) NOT NULL PRIMARY KEY, HashVertical char(64) NOT NULL,
            Cantidad bigint NOT NULL, IdBase uniqueidentifier NOT NULL, Version int NOT NULL CHECK(Version=1));
    DELETE dbo.IntegridadTabla WHERE Tabla IN ('Bitacora','Traduccion');
    COMMIT;
END TRY
BEGIN CATCH
    IF XACT_STATE()<>0 ROLLBACK;
    THROW;
END CATCH;
GO
