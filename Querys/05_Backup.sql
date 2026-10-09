-- Backup completo con nombre único. Ejecutar antes de migrar una base existente.
-- La carpeta debe existir y el servicio SQL Server necesita permiso de escritura.
USE [master];
SET NOCOUNT ON;
DECLARE @Carpeta nvarchar(2000) = CONVERT(nvarchar(2000),SERVERPROPERTY('InstanceDefaultBackupPath'));
-- Para otra carpeta, reemplazar la línea anterior por N'C:\Backups\'.
IF NULLIF(@Carpeta,N'') IS NULL
    THROW 50001, 'Configure @Carpeta con una carpeta de backups existente.', 1;
IF RIGHT(@Carpeta,1) NOT IN (N'\',N'/') SET @Carpeta += N'\';
DECLARE @Ruta nvarchar(4000) = @Carpeta + N'SIGAT_' +
    REPLACE(REPLACE(REPLACE(CONVERT(nvarchar(23),GETDATE(),121),N'-',N''),N':',N''),N' ',N'_') + N'.bak';
BACKUP DATABASE [SIGAT] TO DISK=@Ruta
WITH COPY_ONLY, NOINIT, CHECKSUM, NAME=N'SIGAT - Backup completo', STATS=10;
SELECT @Ruta AS ArchivoBackup;
GO
