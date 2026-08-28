/* =========================================================================
   SIGAT - Backup de la base
   ========================================================================= */

USE [SIGAT]
GO

-- Backup completo. Cambiá la ruta según donde quieras guardarlo.
BACKUP DATABASE [SIGAT]
TO DISK = N'C:\Backups\SIGAT_full.bak'
WITH FORMAT, INIT, NAME = N'SIGAT-Backup completo', SKIP, STATS = 10;
GO

-- (Opcional) Backup con fecha en el nombre del archivo, para no pisar el anterior
DECLARE @ruta NVARCHAR(500);
SET @ruta = N'C:\Backups\SIGAT_' + FORMAT(GETDATE(), 'yyyyMMdd_HHmmss') + N'.bak';
BACKUP DATABASE [SIGAT] TO DISK = @ruta WITH FORMAT, INIT, STATS = 10;
GO
