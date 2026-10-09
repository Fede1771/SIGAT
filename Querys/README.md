# Scripts SQL de SIGAT

## PC nueva: instalar desde cero

1. Instalar SQL Server Express 2019 o posterior con instancia `SQLEXPRESS` y autenticación de Windows. También se necesita el entorno de .NET de la aplicación: el script prepara la base, no instala esos programas.
2. Conectarse a `.\SQLEXPRESS` desde SQL Server Management Studio. La cuenta que instala necesita permisos para crear bases. La aplicación se conecta con la cuenta de Windows que la ejecuta.
3. Abrir `01_Instalacion_Completa.sql`, cambiar `@ClaveAdmin` y ejecutar **todo el archivo**. Dejar `@Base = N'SIGAT'` para la conexión actual.
4. Ejecutar `04_Verificacion_Integridad.sql` y abrir SIGAT. Ingresar con `admin` y la contraseña elegida.

**El archivo 01 es autónomo:** instala las 13 tablas de la aplicación y sus relaciones, idiomas y traducciones, 5 roles iniciales, 7 permisos y 16 relaciones entre roles y permisos. Incluye administrador protegido, roles editables, roles anidados y componentes de familias. La bitácora empieza vacía y solo se crea la cuenta `admin`. Las traducciones pendientes del catálogo siguen pendientes; la aplicación registra nuevas claves al abrir formularios.

El instalador utiliza el nivel de compatibilidad del motor, sin exigir 170 a versiones anteriores. Se detiene si la base ya existe, incluso vacía. Si falla al crear la estructura, revierte tablas y datos, pero puede quedar una base vacía: revisar el error y esa base antes de repetir. Ejecutar todo el archivo permite detenerlo ante errores, también desde SSMS sin modo SQLCMD.

Si otra cuenta de Windows va a ejecutar SIGAT, un administrador del servidor debe crear su acceso a SQL Server y otorgarle permisos sobre la base.

## Base existente: actualizar

1. Cerrar SIGAT y ejecutar `05_Backup.sql`. Guardar la ruta que devuelve.
2. Ejecutar **todo** `02_Actualizar_Base_Existente.sql` en la instancia que contiene SIGAT.
3. Ejecutar `04_Verificacion_Integridad.sql` y `03_Consultar_Usuarios.sql`.
4. Asignar desde la aplicación los roles de las cuentas que hayan quedado sin rol.

El nuevo 02 reúne las migraciones anteriores **10 → 12 → 13 → 14** en una sola transacción. Conserva cuentas, contraseñas, bitácora, idiomas y traducciones. Al migrar el esquema viejo, conserva asignaciones por nombres de roles admitidos; un perfil `Operador` no se convierte automáticamente en un rol distinto. Requiere la cuenta original ID 1 activa con perfil/rol Administrador. Si una etapa falla, revierte toda la actualización.

Sobre el esquema actual conserva roles personalizados y asignaciones. Se puede repetir sin reiniciar el catálogo ni reponer permisos retirados; la versión interna de seguridad avanza al aplicar la etapa de protección.

## Archivos de uso habitual

| Archivo | Para qué sirve |
|---|---|
| `01_Instalacion_Completa.sql` | Instalar todo en una PC sin base SIGAT. |
| `02_Actualizar_Base_Existente.sql` | Actualizar una base al esquema del código actual. |
| `03_Consultar_Usuarios.sql` | Consultar cuentas y administradores activos. |
| `04_Verificacion_Integridad.sql` | Comprobar integridad, tablas y configuración. |
| `05_Backup.sql` | Crear un único backup fechado en la carpeta del servidor. |
| `06_Restore.sql` | Restaurar un backup sobre una base existente. |
| `07_Resetear_Password.sql` | Cambiar una contraseña con el hash UTF-8 de la aplicación. |
| `08_Reportes_Bitacora.sql` | Consultar eventos y estadísticas; editar fechas y usuario. |
| `09_Limpieza_Bitacora.sql` | Revisar y archivar eventos antiguos. |
| `10_Consultar_Roles.sql` | Consultar roles, asignaciones y permisos efectivos. |

Las tareas de mantenimiento se conservan separadas porque se ejecutan en momentos distintos. Backup, restore, cambio de contraseña y limpieza no se ejecutan automáticamente al instalar.

`06_Restore.sql` conserva el procedimiento anterior: usa `WITH REPLACE` y reemplaza los datos de una base existente. Editar la ruta del backup antes de usarlo. Al trasladar un backup a otra PC, consultar `RESTORE FILELISTONLY` y agregar `WITH MOVE` si las rutas de sus archivos no existen en el destino. La carpeta del backup pertenece al equipo donde corre SQL Server.

En la limpieza, `@Aplicar=0` muestra la cantidad sin archivar. Con `@Aplicar=1`, copia y elimina las mismas filas en una transacción. El histórico se crea sin `IDENTITY` para conservar IDs. Si existe un histórico creado por el script anterior con `IDENTITY`, se detiene para revisar su estructura, sin borrar eventos.

## Qué se unió o retiró

- El instalador anterior solo incluía las tablas básicas. El nuevo 01 incorpora los cambios de 09, 13 y 14 y deja instalada la configuración final.
- Los anteriores 10, 12, 13 y 14 se reemplazan para uso habitual por el nuevo 02.
- `09_Roles_Persistentes.sql` deja de ser un paso adicional de instalación.
- `7_T05_DDL_Idiomas.sql` y `8_T05_DML_Idiomas.sql` pertenecen a otra versión: les faltan columnas necesarias y usan otros IDs. El DDL intenta borrar tablas en un orden incompatible con sus claves foráneas. No ejecutar sobre el esquema actual.
- `03_Limpieza_Usuarios.sql` pasó a `03_Consultar_Usuarios.sql` porque solo consulta; la consulta de roles pasó de 11 a 10 y la limpieza de bitácora de 02 a 09.
- La verificación evita repetir `CHECKTABLE` después de `CHECKDB` y contempla todas las tablas actuales de seguridad.
- El backup evita ejecutar dos respaldos al seleccionar todo el archivo o reemplazar un archivo fijo. El reporte por fechas incluye el último día completo. El cambio de contraseña usa UTF-8 también para tildes y otros alfabetos.

Los originales de instalación y migración, junto con los T05, quedan en `Historico/` para consulta. **No son pasos para ejecutar en orden.** Si el mensaje del código menciona `Querys/14_Familias_Editables.sql`, ejecutar el nuevo 02 sobre la base existente: esa etapa ya está integrada. Los archivos de `tmp/usuarios-pruebas/` son auxiliares anteriores y no forman parte de la instalación.

## Validación realizada

Probado en SQL Server Express 2025 usando bases temporales exclusivas: instalación nueva, rechazo de reinstalación sobre una base existente, protección del administrador, actualización del esquema de perfiles, reversión completa ante un fallo intermedio, repetición de la actualización, conservación de cuentas/contraseñas/bitácora y roles personalizados, consultas, hash UTF-8 con tildes y caracteres chinos, y archivo de bitácora sin duplicaciones. `CHECKDB` no informó errores. Las bases temporales se eliminaron al finalizar; la base SIGAT actual no se migró.

El instalador requiere SQL Server 2019 o posterior, pero las pruebas de ejecución se hicieron en el motor disponible de esta PC. El backup pasó la comprobación de sintaxis sin generar un archivo. El restore se conserva para uso manual y no fue ejecutado.
