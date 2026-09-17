/* =========================================================================
   SIGAT - Restaurar la base (desde otra PC o luego de un problema)
   IMPORTANTE: esto sobreescribe la base SIGAT actual con la del backup.
   ========================================================================= */

USE [master]
GO

ALTER DATABASE [SIGAT] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
GO

RESTORE DATABASE [SIGAT]
FROM DISK = N'C:\Backups\SIGAT_full.bak'
WITH REPLACE, STATS = 10;
GO

ALTER DATABASE [SIGAT] SET MULTI_USER;
GO
