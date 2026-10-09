/* Archivar registros antiguos de bitácora y retirarlos de la tabla principal.
   Primero ejecutar con @Aplicar=0 para ver la cantidad; cambiar a 1 para archivar.
   La copia y el borrado son una sola operación: si falla, se revierte todo.
*/
USE [SIGAT];
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @DiasRetencion int = 180;
DECLARE @Aplicar bit = 0;
IF @DiasRetencion < 1
    THROW 50001, 'La retención debe ser de al menos un día.', 1;
DECLARE @Limite datetime = DATEADD(DAY, -@DiasRetencion, GETDATE());
SELECT @Limite AS FechaLimite, COUNT(*) AS RegistrosAArchivar
FROM dbo.Bitacora WHERE Fecha < @Limite;
IF @Aplicar=0 RETURN;
BEGIN TRY
    BEGIN TRANSACTION;
    IF OBJECT_ID('dbo.Bitacora_Historico','U') IS NULL
        CREATE TABLE dbo.Bitacora_Historico (
            IdBitacora int NOT NULL PRIMARY KEY,
            Fecha datetime NOT NULL,
            Usuario varchar(50) NOT NULL,
            Actividad varchar(255) NOT NULL,
            InformacionAsociada nvarchar(max) NULL,
            DigitoVerificador nvarchar(250) NULL
        );
    -- El script anterior creaba IDENTITY por SELECT INTO. No borrar si está así.
    IF COLUMNPROPERTY(OBJECT_ID('dbo.Bitacora_Historico'),'IdBitacora','IsIdentity')=1
        THROW 50001, 'El histórico anterior tiene IDENTITY. Revisar su estructura antes de archivar.', 1;
    DELETE FROM dbo.Bitacora
    OUTPUT deleted.IdBitacora, deleted.Fecha, deleted.Usuario, deleted.Actividad,
           deleted.InformacionAsociada, deleted.DigitoVerificador
    INTO dbo.Bitacora_Historico
         (IdBitacora,Fecha,Usuario,Actividad,InformacionAsociada,DigitoVerificador)
    WHERE Fecha < @Limite;
    DECLARE @Archivados int = @@ROWCOUNT;
    COMMIT TRANSACTION;
    SELECT @Archivados AS RegistrosArchivados;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
