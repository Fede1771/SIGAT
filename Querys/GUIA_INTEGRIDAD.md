# Verificación y recuperación de integridad

El circuito usa SHA-256 por registro (DVH) y un SHA-256 del conjunto ordenado de registros (DVV). Protege **Bitacora y Traduccion**. Usuarios, roles, idiomas y controles no están incluidos en ese alcance; sus claves foráneas y validaciones siguen funcionando.

Los campos se serializan con longitud y valor, con una representación distinta para nulo y vacío. Se utiliza UTF-8 y fechas/números independientes del idioma del equipo. El DVH de Bitacora incluye ID, fecha, usuario, actividad e información cifrada; el de Traduccion incluye los dos IDs, texto y estado. El DVV incorpora tabla, cantidad, claves y hashes recalculados en orden de clave primaria.

## Preparación inicial

El instalador y la actualización crean IntegridadTabla vacía. Al abrir una base todavía no preparada, SIGAT bloquea el inicio normal y ofrece preparar la referencia inicial. La cuenta de Windows debe ser administradora SQL y poder crear/restaurar bases auxiliares. El operador confirma que reconoce los datos actuales como la referencia a conservar.

Antes de calcular los nuevos dígitos se crea un backup físico previo y se comprueban los hashes legados existentes. Las traducciones sin dígito del catálogo original se incorporan en esta preparación explícita. Se admite el caso conocido de fecha con milisegundo cero cuyo dígito legado coincide exactamente con un segundo anterior: es la compatibilidad con el redondeo de datetime antiguo. Se registra ese caso. Cualquier otra discrepancia legada cancela la preparación y revierte las escrituras.

No se permite volver a inicializar una base ya protegida. Una inconsistencia posterior se recupera desde un respaldo; nunca se soluciona aceptando automáticamente hashes nuevos sobre datos alterados.

## Uso normal

En la barra lateral del administrador, abrir **Integridad y recuperación**. Allí se puede verificar y crear un respaldo comprobado. Los archivos .bak se escriben en la carpeta de backups del servidor SQL; el cuadro muestra la ruta completa.

La verificación se ejecuta al iniciar SIGAT, antes de autenticar y al leer/escribir las tablas protegidas. Las escrituras legítimas actualizan los datos y sus verificadores dentro de la misma transacción. Un fallo identifica tabla y registro y bloquea la operación. La información cifrada de Bitacora se comprueba también antes de entregarla a la interfaz.

La acción **Archivar bitácora antigua** traslada los eventos anteriores a 180 días al histórico y actualiza el DVV de la tabla activa en la misma transacción. El script SQL antiguo de limpieza se detiene en una base protegida para evitar desbalancear el control. El histórico no forma parte de las dos tablas cubiertas por este verificador.

Un respaldo se considera comprobado después de verificar la base original, crear el .bak con CHECKSUM, restaurarlo en una base auxiliar y verificar allí DVH y DVV. La base auxiliar se elimina al terminar. El respaldo completo incluye otras tablas, pero la reparación desde la pantalla modifica únicamente las tablas protegidas afectadas.

## Recuperación

1. Seleccionar el .bak comprobado. La ruta corresponde al servidor SQL, que necesita permiso de lectura.
2. Elegir **Recuperar**. Primero se restaura y verifica el respaldo en una base auxiliar y se confirma que pertenece a la misma base mediante IdBase.
3. En la recuperación por registros se restauran los valores y hashes originales de las filas identificadas, o se reinsertan las filas borradas. No se confirman cambios si el resultado no coincide con la referencia de integridad o si el respaldo es insuficiente para conservar otras actualizaciones.
4. Para un daño amplio o un respaldo anterior, la opción explícita **Restaurar las tablas afectadas completas** reemplaza Bitacora y/o Traduccion por su contenido del respaldo. Descarta los cambios posteriores de esas tablas; requiere confirmación.
5. Solo se confirma la transacción después de que toda la verificación vuelva a pasar. Si el inicio estaba bloqueado, se puede continuar. Un incidente no capturado durante el uso solicita recuperación y reinicio de la aplicación.

Los incidentes y recuperaciones se anotan fuera de la Bitacora protegida, en `%LOCALAPPDATA%\SIGAT\logs\integridad.log`.

## Demostración para el profesor

Usar una base exclusiva con nombre que empiece con SIGAT_Roles_Pruebas_. Crear datos, preparar la integridad y guardar un respaldo comprobado **después** de los datos que se van a alterar. Se incluye `Pruebas/Forzar_Ruptura_Integridad.sql`, con protección para no ejecutar la ruptura sobre SIGAT.

Probar por separado texto cambiado sin cambiar el dígito, dígito cambiado sin cambiar texto y fila borrada. Al verificar o intentar utilizar los datos, el programa debe señalar el problema. Seleccionar el respaldo, recuperar, verificar nuevamente y confirmar que se puede volver a leer el contenido original.

Las pruebas automáticas también cubren cifrado ilegible, control vertical alterado, Unicode, cambio de cultura, redondeo de fecha y cancelación de recuperación por registros cuando el respaldo es anterior a cambios legítimos.

## Límites

SHA-256 permite detectar diferencias, no reconstruir contenido perdido. Se necesita un respaldo confiable y se conservan sus archivos para la demostración. Esta implementación no autentica criptográficamente al administrador SQL: quien pueda cambiar datos y todos sus verificadores también puede recomputarlos. Es un control de consistencia y detección de alteraciones respecto de la referencia guardada.

DBCC CHECKDB y el CHECKSUM de un backup comprueban aspectos de SQL Server; no reemplazan la comparación DVH/DVV. Los .bak anteriores a esta preparación no tienen el nuevo control y no se aceptan para recuperación automática.
