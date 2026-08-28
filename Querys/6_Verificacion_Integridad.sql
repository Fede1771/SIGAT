/* =========================================================================
   SIGAT - Verificación de integridad de la base
   ========================================================================= */

USE [SIGAT]
GO

-- Chequeo completo de integridad física y lógica de toda la base
DBCC CHECKDB ('SIGAT') WITH NO_INFOMSGS;
GO

-- Chequeo de una tabla puntual (más rápido que toda la base)
DBCC CHECKTABLE ('Usuarios') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Bitacora') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Perfiles') WITH NO_INFOMSGS;
GO

-- Buscar usuarios "huérfanos": con un IdPerfil que no existe en Perfiles
-- (no debería pasar por el FK, pero sirve como chequeo de consistencia lógica)
SELECT u.*
FROM Usuarios u
LEFT JOIN Perfiles p ON u.IdPerfil = p.IdPerfil
WHERE p.IdPerfil IS NULL;
GO

-- Verificar que todas las Foreign Keys de la base estén habilitadas y confiables
SELECT
    fk.name AS NombreFK,
    OBJECT_NAME(fk.parent_object_id) AS Tabla,
    fk.is_disabled AS Deshabilitada,
    fk.is_not_trusted AS NoConfiable
FROM sys.foreign_keys fk
WHERE fk.parent_object_id IN (OBJECT_ID('Usuarios'));
GO
