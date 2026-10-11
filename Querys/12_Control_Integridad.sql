-- Estructura para SHA-256 por fila (DVH) y por conjunto (DVV).
-- La referencia inicial se prepara desde SIGAT con autorización del operador.
-- No recalcular sobre una base dañada: usar Integridad y recuperación.
USE [SIGAT];
IF OBJECT_ID('dbo.IntegridadTabla','U') IS NULL
    CREATE TABLE dbo.IntegridadTabla (
        Tabla nvarchar(30) NOT NULL PRIMARY KEY,
        HashVertical char(64) NOT NULL,
        Cantidad bigint NOT NULL,
        IdBase uniqueidentifier NOT NULL,
        Version int NOT NULL CHECK(Version=1)
    );
GO
