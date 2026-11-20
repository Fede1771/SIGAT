# Integridad de cuentas y asignaciones

## Preparar una base instalada o actualizada

1. Cerrar SIGAT y crear un backup previo con `05_Backup.sql`.
2. En una base existente, ejecutar completo `02_Actualizar_Base_Existente.sql`. Para una base nueva, usar únicamente `01_Instalacion_Completa.sql`.
3. Abrir SIGAT e iniciar sesión con el administrador original. La cuenta de Windows también debe estar autorizada en SQL. Si aún no hay controles, la autenticación administrativa abre Integridad y recuperación en una sesión restringida.
4. Usar **Preparar referencia inicial**, comprobar los datos que se van a aceptar y confirmar. Se conserva un respaldo previo y se genera uno comprobado con los nuevos verificadores.
5. Guardar la ruta del nuevo `.bak`, verificar y continuar al login.

Esta preparación no se puede repetir sobre una base ya protegida. La actualización no convierte silenciosamente los datos actuales en una nueva referencia. Un respaldo anterior al cambio de esquema Bitacora/Traduccion → Usuarios/UsuarioRol debe conservarse como backup histórico completo, no como fuente de recuperación selectiva del esquema nuevo.

SQL Server necesita leer el `.bak` en el equipo donde se ejecuta el servicio. Para trasladarlo a otra PC, copiarlo a su carpeta Backup o a una carpeta accesible para la cuenta del servicio. El botón Elegir respaldo solo carga la ruta; no restaura por sí mismo.

## Qué comprueba el sistema

- **Usuarios:** ID, nombre de usuario, hash de contraseña, nombre, apellido y estado. El DVH se guarda en `Usuarios.DigitoVerificador`.
- **UsuarioRol:** pareja de IDs de usuario y rol. El DVH se guarda en `UsuarioRol.DigitoVerificador`.
- **IntegridadTabla:** dos controles DVV, cantidad de filas, identidad de la base y versión del formato.

El inicio y las lecturas críticas comprueban esos valores. Las altas, modificaciones, bajas y asignaciones actualizan los verificadores en la misma transacción Serializable que los datos. Un dato alterado externamente no se acepta al guardar otra operación.

Bitacora mantiene el detalle cifrado con AES. Bitacora y Traduccion pueden consultarse y escribirse aunque haya un incidente en las cuentas: no forman parte de DVH/DVV.

## Demostrar detección y recuperación

Realizar la demostración en una **copia de pruebas**, con cuentas y asignaciones conocidas.

1. Preparar la referencia y usar **Crear respaldo comprobado** antes de alterar datos.
2. Desde SQL, modificar el apellido de una cuenta de prueba sin cambiar `DigitoVerificador`, o borrar una asignación de `UsuarioRol`. El script [Forzar_Ruptura_Integridad.sql](Pruebas/Forzar_Ruptura_Integridad.sql) restringe la operación a bases de ensayo.
3. Confirmar la escritura en SQL; una celda pendiente de guardar no cambia la base.
4. Pulsar **Verificar**: debe aparecer la fila alterada y/o el incidente DVV del conjunto. Si se reinicia SIGAT, primero aparece el login: solo un administrador autenticado puede abrir la recuperación. Un usuario común ve **«En mantenimiento, estamos resolviendo el inconveniente»** y permanece sin sesión.
5. **Elegir respaldo**, seleccionar el `.bak` comprobado de esa misma base y pulsar **Recuperar** con la casilla de restauración completa desmarcada.
6. Confirmar. Si la recuperación puede conservar el resto de los datos y demostrar su coherencia, se confirma; de lo contrario, revierte la operación y explica que el respaldo no alcanza.
7. Verificar y continuar. Comprobar que el dato original y las asignaciones se hayan recuperado.

La restauración completa de seguridad repone **ambas tablas, Usuarios y UsuarioRol**, como un conjunto coherente desde el respaldo. Descarta sus cambios posteriores y puede retirar preferencias de las cuentas que ya no existan al regresar a ese estado. No reemplaza toda la base ni la bitácora: para reemplazar una base entera se utiliza SSMS o `06_Restore.sql`.

Si falta un registro DVV, utilizar la restauración completa desde un respaldo comprobado. La recuperación parcial se detiene con un mensaje; no inventa una referencia.

## Aceptar el estado actual y recalcular

**Recalcular dígitos (asumir pérdida)** es una alternativa administrativa, distinta de recuperar un respaldo:

1. Revisar las cuentas y sus roles actuales.
2. Pulsar el botón y leer la advertencia: se aceptarán las posibles alteraciones o pérdidas existentes.
3. Confirmar explícitamente. Se recalculan todos los DVH/DVV de seguridad bajo bloqueo exclusivo y transacción Serializable.
4. El resultado debe ser integridad correcta. Crear un nuevo respaldo comprobado.

El recálculo conserva el IdBase existente (o genera uno si no hay referencia), repone controles faltantes y deja auditoría en Bitacora y en `%LOCALAPPDATA%\SIGAT\logs\integridad.log`. Requiere un administrador autenticado de SIGAT y permisos administrativos de SQL. No se permite recuperar, recalcular ni preparar controles sin login. La sesión restringida no habilita los módulos normales; cancelar la recuperación la cierra y, tras reparar, se vuelven a validar credenciales y roles.

La acción **no recupera** una cuenta borrada ni detecta qué dato alterado era el correcto: acepta lo que hay. El botón se habilita solo para administración y con problemas de integridad. Verificar o iniciar sesión nunca ejecuta ese recálculo automáticamente.

## Comprobaciones de esta actualización

El **19/11/2026**, el nuevo flujo de acceso se verificó con **64 pruebas: 61 sin SQL y 3 de integración** en bases exclusivas de ensayo. Incluyen el aviso de mantenimiento, rechazo sin sesión, administrador con roles anidados, contraseña incorrecta, cuenta inactiva, sesión restringida, cancelación sin cambios, recuperación desde backup y preparación inicial después del login. También se comprobó que adulterar una asignación de rol o la contraseña administrativa no concede acceso de recuperación.

La validación inicial de integridad incluyó 40 pruebas automatizadas: 33 sin SQL y 7 de integración. Cubrieron diseñador, presentación, permisos anidados y protección del administrador, persistencia y concurrencia de idiomas, hash estable entre culturas, detección de alteraciones, bloqueo del login, backups comprobados, recuperación de cuentas y asignaciones, y recálculo administrativo auditado.

Se verificaron además la instalación nueva, la migración y dos repeticiones de la actualización en copias aisladas. Las huellas de cuentas, contraseñas, roles, familias, preferencias, traducciones, bitácora y controles permanecieron iguales. CHECKDB no informó errores. La recuperación completa y el recálculo se comprobaron también ante controles DVV faltantes; un `.bak` con alteración lógica fue rechazado.

Las pruebas de ejecución utilizaron SQL Server Express 2025 en esta PC. No se aplicó la migración a la base SIGAT habitual. La compilación no tiene errores; permanecen advertencias anteriores de nulabilidad y recomendaciones del analizador de pruebas.
