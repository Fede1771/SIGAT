/* =========================================================================
   SIGAT - Limpieza de registros viejos de Bitácora
   Ejecutar los 3 pasos EN ORDEN, uno por uno. No saltear al Paso 3
   sin haber corrido antes el Paso 1 y el Paso 2.
   ========================================================================= */

USE [SIGAT]
GO

-- Paso 1: revisar cuántos registros se borrarían ANTES de ejecutar el DELETE
-- (cambiá el número de días según tu política de retención)
DECLARE @diasRetencion INT = 180;

SELECT COUNT(*) AS RegistrosAEliminar
FROM Bitacora
WHERE Fecha < DATEADD(DAY, -@diasRetencion, GETDATE());
GO

-- Paso 2: recomendado, copiar esos registros a una tabla histórica antes de borrarlos
DECLARE @diasRetencion2 INT = 180;

IF OBJECT_ID('dbo.Bitacora_Historico') IS NULL
BEGIN
    SELECT * INTO Bitacora_Historico FROM Bitacora WHERE 1 = 0; -- crea la tabla vacía con la misma estructura
END

INSERT INTO Bitacora_Historico
SELECT * FROM Bitacora
WHERE Fecha < DATEADD(DAY, -@diasRetencion2, GETDATE());
GO

-- Paso 3: recién ahora borrar los registros viejos de la tabla principal
DECLARE @diasRetencion3 INT = 180;

DELETE FROM Bitacora
WHERE Fecha < DATEADD(DAY, -@diasRetencion3, GETDATE());
GO
