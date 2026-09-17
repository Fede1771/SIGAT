/* =========================================================================
   SIGAT - Reportes de Bitácora
   ========================================================================= */

USE [SIGAT]
GO

-- Actividad de un usuario en un rango de fechas
SELECT *
FROM Bitacora
WHERE Usuario = 'admin'
  AND Fecha BETWEEN '2026-08-01' AND '2026-08-31'
ORDER BY Fecha DESC;
GO

-- Todos los eventos de un tipo de actividad (ej: logins)
SELECT *
FROM Bitacora
WHERE Actividad LIKE '%Login%'
ORDER BY Fecha DESC;
GO

-- Últimos 50 movimientos registrados, más recientes primero
SELECT TOP 50 *
FROM Bitacora
ORDER BY Fecha DESC;
GO

-- Cantidad de eventos por usuario y por tipo de actividad
SELECT Usuario, Actividad, COUNT(*) AS Cantidad
FROM Bitacora
GROUP BY Usuario, Actividad
ORDER BY Usuario, Cantidad DESC;
GO

-- Actividad por día (para ver picos de uso)
SELECT CAST(Fecha AS DATE) AS Dia, COUNT(*) AS Eventos
FROM Bitacora
GROUP BY CAST(Fecha AS DATE)
ORDER BY Dia DESC;
GO
