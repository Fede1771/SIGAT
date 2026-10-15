-- Solo sobre una COPIA de pruebas, después de crear un respaldo comprobado desde SIGAT.
SET XACT_ABORT ON;
IF DB_NAME() NOT LIKE 'SIGAT[_]Roles[_]Pruebas[_]%'
    THROW 50001,'Esta demostración solo admite una base exclusiva de pruebas.',1;
DECLARE @Caso int=1;
DECLARE @Usuario int=(SELECT TOP(1) IdUsuario FROM dbo.Usuarios WHERE IdUsuario<>1 ORDER BY IdUsuario);
IF @Usuario IS NULL THROW 50001,'Cree primero una cuenta de prueba desde SIGAT.',1;
-- 1: modificar la cuenta sin actualizar DVH. 2: romper solo el DVH. 3: borrar una asignación (DVV).
IF @Caso=1 UPDATE dbo.Usuarios SET Apellido=N'ALTERADO EN SQL' WHERE IdUsuario=@Usuario;
ELSE IF @Caso=2 UPDATE dbo.Usuarios SET DigitoVerificador=REPLICATE('0',64) WHERE IdUsuario=@Usuario;
ELSE IF @Caso=3
BEGIN
    IF NOT EXISTS(SELECT 1 FROM dbo.UsuarioRol WHERE IdUsuario=@Usuario)
        THROW 50001,'Asigne un rol a la cuenta de prueba desde SIGAT antes de este caso.',1;
    DELETE dbo.UsuarioRol WHERE IdUsuario=@Usuario;
END
ELSE THROW 50001,'Caso desconocido.',1;
-- Verificar: debe bloquear las cuentas/asignaciones. Recuperar el .bak o aceptar explícitamente el estado con recálculo.
