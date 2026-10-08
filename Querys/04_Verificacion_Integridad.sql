/* =========================================================================
   SIGAT - Verificación de integridad de la base
   (Basado en 6_Verificacion_Integridad.sql original, actualizado para
   cubrir también Idioma, Control y Traduccion, que no existían cuando
   se escribió el script original.)
   ========================================================================= */

USE [SIGAT]
GO

-- Chequeo completo de integridad física y lógica de toda la base
DBCC CHECKDB ('SIGAT') WITH NO_INFOMSGS;
GO

-- Chequeo de cada tabla puntual (más rápido que toda la base)
DBCC CHECKTABLE ('Usuarios') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Bitacora') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Rol') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('UsuarioRol') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('RolPermiso') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Idioma') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Control') WITH NO_INFOMSGS;
DBCC CHECKTABLE ('Traduccion') WITH NO_INFOMSGS;
GO

-- Buscar traducciones "huérfanas": con un Id_Idioma o Id_Control que no existe
-- (tampoco debería pasar por el FK, es un chequeo de consistencia lógica)
SELECT t.*
FROM Traduccion t
LEFT JOIN Idioma i ON t.Id_Idioma = i.Id
WHERE i.Id IS NULL;
GO

SELECT t.*
FROM Traduccion t
LEFT JOIN Control c ON t.Id_Control = c.Id
WHERE c.Id IS NULL;
GO

-- Controles que no tienen traducción para alguno de los idiomas activos
-- (útil para detectar textos faltantes después de agregar un idioma o una pantalla nueva)
SELECT c.Id AS IdControl, c.Control, c.Form, i.Nombre AS IdiomaFaltante
FROM Control c
CROSS JOIN Idioma i
LEFT JOIN Traduccion t ON t.Id_Control = c.Id AND t.Id_Idioma = i.Id
WHERE i.Activo = 1 AND t.Id_Control IS NULL
ORDER BY c.Form, c.Control, i.Nombre;
GO

-- Verificar que todas las Foreign Keys de la base estén habilitadas y confiables
SELECT
    fk.name AS NombreFK,
    OBJECT_NAME(fk.parent_object_id) AS Tabla,
    fk.is_disabled AS Deshabilitada,
    fk.is_not_trusted AS NoConfiable
FROM sys.foreign_keys fk
WHERE fk.parent_object_id IN (
    OBJECT_ID('UsuarioRol'), OBJECT_ID('RolPermiso'), OBJECT_ID('Traduccion')
);
GO
