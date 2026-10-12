# SIGAT

Aplicación de escritorio orientada a la gestión de activos tecnológicos, desarrollada en C# con Windows Forms y SQL Server.

El proyecto organiza sus responsabilidades en capas y utiliza patrones de diseño para administrar usuarios, accesos, idiomas y eventos del sistema.

## Estado del proyecto

Actualmente están implementados los módulos de usuarios, roles y permisos, idiomas, bitácora e integridad y recuperación.

Gestión de activos, Movimientos, Mantenimiento y Reportes forman parte del catálogo de permisos. Su presencia en el catálogo no implica que las pantallas y operaciones de esos módulos ya estén implementadas.

## Funcionalidades

- Alta y modificación de usuarios, cambio de contraseña, baja lógica y reactivación.
- Cuentas nuevas sin roles; los accesos se asignan desde la gestión de roles y permisos.
- Administrador original protegido contra baja, desactivación y retiro de su rol Administrador.
- Roles personalizados y anidados, con asignaciones persistentes en SQL.
- Agregado y retiro de componentes de roles; edición de los hijos de permisos compuestos.
- Validación de ciclos en las jerarquías de roles y permisos.
- Idiomas con preferencia por cuenta guardada en SQL e indicador del idioma activo.
- Modos claro y oscuro con preferencia local por usuario.
- Bitácora con detalle cifrado y consultas por fecha, usuario y actividad.
- Control SHA-256 de registros y conjuntos de Bitacora y Traduccion.
- Creación de respaldos comprobados y recuperación desde un respaldo de la misma base.

Los cambios de roles se aplican al volver a iniciar sesión. El rol Administrador conserva sus componentes originales.

## Tecnologías

- C# y .NET 10.
- Windows Forms.
- SQL Server Express 2019 o posterior.
- Microsoft.Data.SqlClient.
- MSTest.

La aplicación requiere Windows. La configuración actual utiliza la instancia local `.\SQLEXPRESS`, la base `SIGAT` y autenticación de Windows.

## Arquitectura

| Proyecto | Responsabilidad |
|---|---|
| SIGAT.BE | Entidades, relaciones e informes utilizados por las demás capas. |
| SIGAT.BLL | Reglas de negocio, validaciones y coordinación de operaciones. |
| SIGAT.DAL | Consultas SQL, transacciones y persistencia. |
| SIGAT.SERVICIOS | Sesión, administración del idioma, hashes y cifrado. |
| SIGAT.UI | Formularios, navegación, presentación y temas visuales. |
| SIGAT.Tests | Pruebas de lógica, interfaz e integración con SQL. |

El recorrido habitual de una operación es **UI → BLL → DAL**. Las entidades transportan los datos y los servicios aportan funciones compartidas.

## Patrones de diseño

- **Composite:** los permisos simples son hojas y los compuestos agrupan otros permisos.
- **Observer:** los formularios suscriptos actualizan sus textos al cambiar el idioma.
- **Singleton:** mantiene una instancia de la sesión y del administrador de idiomas por proceso.
- **Factory Method:** las fábricas construyen los distintos eventos de bitácora.
- **Mapper:** adapta los datos del evento a las columnas persistidas en la bitácora.

## Roles iniciales

| Rol | Componentes iniciales |
|---|---|
| Administrador | Ver bitácora, Gestión de activos, Movimientos, Mantenimiento, Reportes y Administración del sistema. |
| Responsable de Inventario | Gestión de activos, Movimientos y Reportes. |
| Técnico de Mantenimiento | Mantenimiento y Consultar inventario. |
| Mesa de Ayuda | Movimientos y Consultar inventario. |
| Auditor | Ver bitácora, Consultar inventario y Reportes. |

Ver bitácora y Consultar inventario son permisos simples. Los otros cinco componentes son permisos compuestos.

## Instalación y ejecución

1. Instalar el SDK de .NET 10 y SQL Server Express 2019 o posterior, con instancia `SQLEXPRESS`.
2. Conectarse a `.\SQLEXPRESS` con una cuenta autorizada para crear la base.
3. Abrir [01_Instalacion_Completa.sql](Querys/01_Instalacion_Completa.sql), configurar `@ClaveAdmin` y ejecutar el archivo completo. Se utiliza para una instalación nueva.
4. Ejecutar [04_Verificacion_Integridad.sql](Querys/04_Verificacion_Integridad.sql).
5. Abrir `SIGAT.slnx` en una versión de Visual Studio compatible con .NET 10 y establecer **SIGAT.UI** como proyecto de inicio.
6. Al primer inicio, preparar la referencia de integridad desde la pantalla de recuperación. Esta operación requiere permisos administrativos de SQL y genera respaldos.
7. Ingresar con `admin` y la contraseña configurada en el instalador.

También se puede compilar y ejecutar desde la carpeta principal:

```powershell
dotnet restore SIGAT.slnx
dotnet build SIGAT.slnx
dotnet run --project SIGAT.UI/SIGAT.UI.csproj
```

## Actualización de una base existente

Cerrar la aplicación, crear un respaldo con `Querys/05_Backup.sql` y ejecutar `Querys/02_Actualizar_Base_Existente.sql`.

Consultar [las instrucciones de SQL](Querys/README.md) para los requisitos, verificaciones y operaciones de mantenimiento. Los scripts de `Querys/Historico` se conservan como referencia y no son pasos de instalación.

## Pruebas

Para ejecutar las pruebas que no necesitan una conexión a SQL:

```powershell
dotnet test SIGAT.Tests/SIGAT.Tests.csproj --filter "TestCategory!=SQLIntegration"
```

Las pruebas de integración requieren bases exclusivas con el prefijo `SIGAT_Roles_Pruebas_` y las variables de conexión correspondientes:

- `SIGAT_ROLES_TEST_CONNECTION`: roles y usuarios.
- `SIGAT_IDIOMA_TEST_CONNECTION`: preferencias de idioma.
- `SIGAT_INTEGRIDAD_TEST_CONNECTION`: integridad, respaldos y recuperación.

Estas pruebas pueden modificar datos y restaurar bases auxiliares. Deben ejecutarse sobre bases de ensayo preparadas para cada conjunto de pruebas.

## Pendientes conocidos

- Ajustar el espacio del panel Composición del rol: una prueba visual detecta desplazamiento vertical en determinados tamaños.
- Unificar las operaciones de usuarios y su registro de bitácora para evitar guardados parciales si falla la auditoría.
- Completar las validaciones de autorización de usuarios e idiomas en la capa de negocio.
- Hacer atómica la creación de idiomas y controles con sus traducciones.
- Resolver la recuperación cuando falta un registro de control en IntegridadTabla.
- Ajustar el límite superior del filtro de fechas de bitácora al redondeo de SQL datetime.

El control de integridad actual cubre Bitacora y Traduccion. SHA-256 se utiliza para verificadores y hashes de contraseñas; el cifrado AES del detalle de bitácora cumple una función diferente.

