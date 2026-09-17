/* =========================================================================
   SIGAT - Limpieza de usuarios
   No existía un script dedicado a esto; se arma nuevo a partir de la
   estructura real de Usuarios (SIGAT_DER.sql / DEFINITIVO).

   IMPORTANTE:
   - La tabla Usuarios tiene una columna [Activo] (bit). La forma correcta
     de "dar de baja" un usuario en este sistema NO es borrarlo, es
     desactivarlo (Activo = 0). De hecho ya existe un flujo para esto en
     la app: el control 'btn_eliminar' en FrmGestionUsuarios se traduce
     como "Baja" / "Deactivate", no como borrado físico.
   - Bitacora.Usuario es un varchar, NO tiene Foreign Key hacia Usuarios
     (ver SIGAT_DER.sql / DEFINITIVO). Por eso SÍ se puede borrar
     físicamente un usuario sin romper la bitácora, pero los registros
     históricos van a quedar con un nombre de usuario "huérfano" (ya no
     va a existir esa cuenta). Si te importa la trazabilidad, preferí
     desactivar en vez de borrar.
   - Usuarios SÍ tiene FK hacia Perfiles (FK_Usuarios_Perfiles). No hace
     falta ningún paso extra para borrar/desactivar un usuario por esa FK
     (el usuario es el lado "hijo"), pero si alguna vez querés borrar un
     Perfil, primero tenés que reasignar o borrar los usuarios que lo usan.
   - Nunca desactives ni borres el único usuario con perfil Administrador
     que quede activo: te quedarías sin forma de administrar el sistema.
   ========================================================================= */

USE [SIGAT]
GO

-- Paso 0: revisar el estado actual antes de tocar nada
SELECT IdUsuario, NombreUsuario, Nombre, Apellido, Activo, IdPerfil
FROM Usuarios
ORDER BY Activo DESC, NombreUsuario;
GO

-- Paso 0b: chequeo de seguridad -- cuántos administradores activos quedan
SELECT COUNT(*) AS AdminsActivos
FROM Usuarios u
JOIN Perfiles p ON p.IdPerfil = u.IdPerfil
WHERE p.NombrePerfil = 'Administrador' AND u.Activo = 1;
GO

/* -------------------------------------------------------------------------
   OPCIÓN A (recomendada): desactivar un usuario en vez de borrarlo
   ------------------------------------------------------------------------- */
DECLARE @usuarioABaja VARCHAR(50) = 'NombreUsuario_a_dar_de_baja'; -- <-- cambiar acá

UPDATE Usuarios
SET Activo = 0
WHERE NombreUsuario = @usuarioABaja;
GO

/* -------------------------------------------------------------------------
   OPCIÓN B: borrado físico definitivo de un usuario puntual
   Usar solo si estás seguro de que no lo necesitás ni siquiera como
   registro histórico (por ejemplo, cuentas de prueba como 'dasda1').
   ------------------------------------------------------------------------- */
DECLARE @usuarioABorrar VARCHAR(50) = 'NombreUsuario_a_borrar'; -- <-- cambiar acá

DELETE FROM Usuarios
WHERE NombreUsuario = @usuarioABorrar
  AND NombreUsuario NOT IN (
      SELECT NombreUsuario FROM Usuarios u
      JOIN Perfiles p ON p.IdPerfil = u.IdPerfil
      WHERE p.NombrePerfil = 'Administrador' AND u.Activo = 1
  ); -- la condición evita borrar por accidente al último admin activo
GO

/* -------------------------------------------------------------------------
   OPCIÓN C: limpieza masiva de cuentas de prueba/desarrollo
   Ejemplo de cómo borrar por patrón (ajustá el WHERE a tu caso real).
   Revisá SIEMPRE con un SELECT antes de correr el DELETE.
   ------------------------------------------------------------------------- */
-- SELECT * FROM Usuarios WHERE NombreUsuario LIKE 'test%' OR NombreUsuario LIKE 'prueba%';
-- DELETE FROM Usuarios WHERE NombreUsuario LIKE 'test%' OR NombreUsuario LIKE 'prueba%';
