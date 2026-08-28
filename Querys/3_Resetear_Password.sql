/* =========================================================================
   SIGAT - Resetear contraseña de un usuario

   OJO: tus contraseñas están guardadas como hash SHA-256 (64 caracteres hex).
   Si tu aplicación le agrega algo extra al hash (una "sal"/salt, un prefijo,
   etc.) antes de guardarlo, el hash generado acá con HASHBYTES puede NO
   coincidir con lo que espera la app al momento de loguearse. Si no estás
   seguro de cómo genera el hash tu código, probá primero con un usuario
   de prueba antes de tocar el admin.
   ========================================================================= */

USE [SIGAT]
GO

-- Ver el formato actual del hash guardado (para comparar longitudes/formato)
SELECT NombreUsuario, Password, LEN(Password) AS LargoHash
FROM Usuarios
WHERE NombreUsuario = 'admin';
GO

-- Opción A: si tu app usa SHA-256 simple (sin salt), esto genera el hash en hex minúscula
DECLARE @usuario VARCHAR(50) = 'admin';
DECLARE @nuevaPass VARCHAR(100) = 'NuevaClave123'; -- <-- cambiar acá
DECLARE @hash VARCHAR(256) = LOWER(CONVERT(VARCHAR(256), HASHBYTES('SHA2_256', @nuevaPass), 2));

UPDATE Usuarios
SET Password = @hash
WHERE NombreUsuario = @usuario;
GO

-- Opción B: si ya tenés el hash calculado desde afuera (por tu app/código), pegalo directo
-- UPDATE Usuarios SET Password = 'HASH_YA_CALCULADO_ACA' WHERE NombreUsuario = 'admin';
