-- Cambiar únicamente la contraseña de la cuenta indicada.
-- SQL Server 2019+: UTF-8 coincide con HashHelper, también para tildes y símbolos.
USE [SIGAT];
SET NOCOUNT ON;
DECLARE @Usuario varchar(50) = 'admin';
DECLARE @NuevaPass nvarchar(100) = N'NuevaClave123'; -- Editar antes de ejecutar.
IF NULLIF(LTRIM(RTRIM(@NuevaPass)),N'') IS NULL
    THROW 50001, 'La contraseña no puede estar vacía.', 1;
IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios WHERE NombreUsuario=@Usuario)
    THROW 50001, 'No existe la cuenta indicada.', 1;
DECLARE @Hash varchar(64) = LOWER(CONVERT(varchar(64),HASHBYTES('SHA2_256',
    CONVERT(varchar(max),@NuevaPass COLLATE Latin1_General_100_BIN2_UTF8)),2));
UPDATE dbo.Usuarios SET Password=@Hash WHERE NombreUsuario=@Usuario;
SELECT NombreUsuario,Activo FROM dbo.Usuarios WHERE NombreUsuario=@Usuario;
GO
