-- Ejecutar sobre una COPIA de pruebas, después de crear un respaldo comprobado desde SIGAT.
-- Seleccionar la base de pruebas en SSMS. No modificar estas protecciones para usar la base real.
SET XACT_ABORT ON;
IF DB_NAME() NOT LIKE 'SIGAT[_]Roles[_]Pruebas[_]%'
    THROW 50001,'Esta demostración solo admite una base exclusiva de pruebas.',1;
DECLARE @Caso int=1;
-- 1: cambiar un texto conservando su hash. 2: romper solo el hash. 3: borrar una fila.
IF @Caso=1 UPDATE dbo.Traduccion SET Texto=N'TEXTO ALTERADO PARA LA PRUEBA'
    WHERE Id_Idioma=1 AND Id_Control=1;
ELSE IF @Caso=2 UPDATE dbo.Traduccion SET DigitoVerificador=REPLICATE('0',64)
    WHERE Id_Idioma=1 AND Id_Control=1;
ELSE IF @Caso=3 DELETE dbo.Traduccion WHERE Id_Idioma=1 AND Id_Control=1;
ELSE THROW 50001,'Caso desconocido.',1;
-- Ahora verificar desde SIGAT y recuperar mediante el .bak comprobado. No recalcular a mano.
