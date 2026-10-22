# SIGAT

**Sistema de Gestión de Activos Tecnológicos**
Proyecto universitario de Ingeniería de Software · Universidad Abierta Interamericana

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square)
![Windows Forms](https://img.shields.io/badge/Windows%20Forms-0078D4?style=flat-square)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?style=flat-square)

SIGAT es una aplicación de escritorio desarrollada como proyecto académico para aplicar análisis, diseño orientado a objetos, arquitectura en capas, patrones de diseño y persistencia en SQL Server. Este repositorio contiene el código fuente, las pruebas automatizadas y los scripts de preparación de la base de datos.

## Datos académicos

| Dato | Información |
|---|---|
| Universidad | Universidad Abierta Interamericana (UAI) |
| Facultad | Tecnología Informática |
| Materia | Ingeniería de Software |
| Docente | Juan Ignacio Silva |
| Integrantes | Federico Agustin Burgos y Valentina Nicole Planas Stredel |
| Comisión | 3-J · turno noche · localización Centro |
| Período | Segundo cuatrimestre de 2026 |

## Contenido

- [Propósito y alcance](#propósito-y-alcance)
- [Funcionalidades implementadas](#funcionalidades-implementadas)
- [Arquitectura del proyecto](#arquitectura-del-proyecto)
- [Patrones de diseño](#patrones-de-diseño)
- [Roles y permisos](#roles-y-permisos)
- [Requisitos e instalación](#requisitos-e-instalación)
- [Integridad y recuperación](#integridad-y-recuperación)
- [Pruebas automatizadas](#pruebas-automatizadas)
- [Recorrido para la demostración](#recorrido-para-la-demostración)
- [Documentación y trabajo pendiente](#documentación-y-trabajo-pendiente)

## Propósito y alcance

El propósito general de SIGAT es centralizar la gestión de los activos tecnológicos de una organización: identificar los equipos, conocer su ubicación y responsable, y seguir sus movimientos, estado y mantenimiento durante su ciclo de vida.

**La versión actual implementa la base administrativa y técnica del sistema:** autenticación, gestión de cuentas, roles y permisos, múltiples idiomas, bitácora e integridad de los datos de seguridad. Los módulos operativos de inventario de activos, movimientos, mantenimiento y reportes quedan como desarrollo futuro. Sus nombres ya forman parte del catálogo de permisos, pero todavía no cuentan con pantallas ni operaciones de negocio.

El objetivo académico es poder explicar y demostrar cómo una operación atraviesa la interfaz, las reglas de negocio y el acceso a datos, y cómo se aplican los patrones de diseño en ese recorrido.

## Funcionalidades implementadas

| Módulo | Alcance actual |
|---|---|
| Autenticación | Ingreso mediante usuario y contraseña, rechazo de credenciales incorrectas y cuentas inactivas, sesión compartida y cierre de sesión. |
| Usuarios | Alta, modificación de datos y contraseña, baja lógica y reactivación. Las cuentas nuevas se crean sin roles. |
| Roles y permisos | Asignación y retiro de roles a usuarios, edición de componentes de roles y familias, creación de familias compuestas y anidamiento en árboles. Se rechazan ciclos. |
| Idiomas | Cambio de idioma durante la sesión, preferencia por cuenta, creación y activación de idiomas, edición e importación/exportación de traducciones. |
| Bitácora | Registro de accesos, cierre de sesión, altas, modificaciones y bajas de usuarios; intento de registro de errores inesperados y auditoría del recálculo de integridad. Consultas combinadas por fecha, usuario y actividad. |
| Integridad | DVH/DVV de Usuarios y UsuarioRol, verificación antes del acceso, respaldos comprobados, recuperación y recálculo administrativo con confirmación. |
| Presentación | Interfaz Windows Forms, modos claro y oscuro y formularios compatibles con el diseñador de Visual Studio. |

El administrador original está protegido contra baja, desactivación y retiro de su rol Administrador. Tampoco se permite desactivar español, el idioma activo de la sesión ni un idioma elegido por algún usuario registrado, aunque su cuenta esté inactiva.

Las traducciones se guardan en SQL por formulario y clave de control. Cuando falta una traducción, se conserva el texto de respaldo de la interfaz.

## Arquitectura del proyecto

La solución contiene cinco proyectos de aplicación y un proyecto de pruebas:

| Proyecto | Responsabilidad |
|---|---|
| [SIGAT.UI](SIGAT.UI) | Formularios, navegación, eventos, traducción de controles y temas visuales. |
| [SIGAT.BLL](SIGAT.BLL) | Reglas de negocio, validaciones y coordinación de las operaciones. |
| [SIGAT.DAL](SIGAT.DAL) | Consultas, transacciones y persistencia en SQL Server. |
| [SIGAT.BE](SIGAT.BE) | Entidades, relaciones, contratos de auditoría e informes de integridad. |
| [SIGAT.SERVICIOS](SIGAT.SERVICIOS) | Sesión, administración del idioma, hashes, cifrado y funciones compartidas. |
| [SIGAT.Tests](SIGAT.Tests) | Pruebas de lógica, interfaz, diseñador e integración con SQL Server. |

El recorrido habitual es **UI → BLL → DAL**. Las capas utilizan BE para representar datos y relaciones, y SERVICIOS para las funciones transversales. Las capas inferiores no dependen de la interfaz.

```text
SIGAT/
├── SIGAT.slnx          Solución para Visual Studio
├── SIGAT.UI/           Presentación
├── SIGAT.BLL/          Lógica de negocio
├── SIGAT.DAL/          Acceso a datos
├── SIGAT.BE/           Entidades y contratos
├── SIGAT.SERVICIOS/    Servicios compartidos
├── SIGAT.Tests/        Pruebas automatizadas
├── Querys/             Instalación y mantenimiento SQL
├── EA-Diagramas/       Modelo actual y diagramas
└── Documentacion/      Capturas, traducciones y notas de integridad
```

En cada formulario, `*.Designer.cs` contiene la creación y distribución de los controles; el archivo `.cs` contiene el comportamiento, los eventos y la carga de datos. [EntornoDiseno.cs](SIGAT.UI/EntornoDiseno.cs) evita ejecutar la lógica de sesión y consultas de los constructores al abrir los formularios en el diseñador.

## Patrones de diseño

| Patrón | Aplicación en SIGAT | Código para estudiar |
|---|---|---|
| **Composite** | Un permiso simple es una hoja; una familia agrupa permisos simples o compuestos. Los roles reúnen permisos y pueden incluir otros roles. | [Permiso.cs](SIGAT.BE/Permiso.cs), [Rol.cs](SIGAT.BE/Rol.cs), [FormGestionRoles.cs](SIGAT.UI/FormGestionRoles.cs). |
| **Observer** | Al cambiar el idioma, el administrador de idiomas notifica a los formularios suscriptos para actualizar sus textos. | [IdiomaManager.cs](SIGAT.SERVICIOS/Idiomas/IdiomaManager.cs), [IIdiomaObserver.cs](SIGAT.SERVICIOS/Idiomas/IIdiomaObserver.cs), [TraductorFormularios.cs](SIGAT.UI/TraductorFormularios.cs). |
| **Singleton** | La aplicación comparte una instancia de la sesión y una del administrador de idiomas por proceso. | [SesionServicio.cs](SIGAT.SERVICIOS/SesionServicio.cs), [IdiomaManager.cs](SIGAT.SERVICIOS/Idiomas/IdiomaManager.cs). |
| **Factory Method** | Las fábricas concretas definen los eventos de login y gestión de usuarios; la fábrica base completa sus datos comunes. | [BitacoraFactory.cs](SIGAT.BLL/Fabricas/BitacoraFactory.cs), [FabricasConcretas.cs](SIGAT.BLL/Fabricas/FabricasConcretas.cs). |
| **Mapper** | Adapta los datos del evento al formato persistido en la bitácora. | [BitacoraMapper.cs](SIGAT.BLL/BitacoraMapper.cs). |

## Roles y permisos

El catálogo inicial conserva cinco roles y siete permisos base:

| Rol | Permisos directos iniciales |
|---|---|
| Administrador | Ver bitácora, Gestión de activos, Movimientos, Mantenimiento, Reportes y Administración del sistema. |
| Responsable de Inventario | Gestión de activos, Movimientos y Reportes. |
| Técnico de Mantenimiento | Mantenimiento y Consultar inventario. |
| Mesa de Ayuda | Movimientos y Consultar inventario. |
| Auditor | Ver bitácora, Consultar inventario y Reportes. |

Ver bitácora y Consultar inventario son permisos simples; los otros cinco son compuestos. Las familias iniciales no incorporan subfamilias de ejemplo. Es posible crear familias personalizadas y editar sus hijos respetando la validación de ciclos.

Las asignaciones se conservan en SQL. La composición del rol Administrador está protegida. Los cambios se reflejan en los permisos de una cuenta al volver a iniciar sesión.

## Requisitos e instalación

Se necesita Windows, el SDK de **.NET 10**, SQL Server **2019 o posterior** y un cliente SQL como SQL Server Management Studio. Para trabajar con el diseñador se requiere Visual Studio con soporte para .NET 10 y desarrollo de escritorio Windows Forms.

La conexión definida en [ConexionBD.cs](SIGAT.DAL/ConexionBD.cs) utiliza:

| Parámetro | Valor |
|---|---|
| Servidor | `.\SQLEXPRESS` |
| Base | `SIGAT` |
| Autenticación | Windows |

### Primera instalación

1. Instalar SQL Server Express con instancia `SQLEXPRESS` y conectarse con una cuenta autorizada para crear bases.
2. Abrir [01_Instalacion_Completa.sql](Querys/01_Instalacion_Completa.sql), cambiar `@ClaveAdmin` y ejecutar **todo el archivo**. El instalador se detiene si la base ya existe.
3. Ejecutar [04_Verificacion_Integridad.sql](Querys/04_Verificacion_Integridad.sql) para revisar la estructura y configuración.
4. Abrir [SIGAT.slnx](SIGAT.slnx) y establecer **SIGAT.UI** como proyecto de inicio, o ejecutar los comandos siguientes desde la carpeta del repositorio.
5. En la primera apertura, pulsar **Preparar control inicial** en Integridad y recuperación. Esta operación requiere permisos administrativos de SQL y genera respaldos.
6. Cuando se indique integridad correcta, continuar e ingresar con `admin` y la contraseña elegida en `@ClaveAdmin`.

```powershell
dotnet restore SIGAT.slnx
dotnet build SIGAT.slnx
dotnet run --project SIGAT.UI/SIGAT.UI.csproj
```

### Base existente

Cerrar SIGAT, crear un respaldo con [05_Backup.sql](Querys/05_Backup.sql) y ejecutar completo [02_Actualizar_Base_Existente.sql](Querys/02_Actualizar_Base_Existente.sql). Después, revisar la base con el script 04.

Si se actualiza desde el control anterior de Bitacora/Traduccion, preparar la nueva referencia de Usuarios/UsuarioRol desde SIGAT y generar un **respaldo comprobado nuevo**. Los backups anteriores a ese cambio de esquema se conservan como respaldo histórico completo, no como fuente de recuperación selectiva del esquema nuevo.

Las instrucciones ampliadas están en [Querys/README.md](Querys/README.md). Los archivos de `Querys/Historico` son referencias de versiones anteriores y no constituyen pasos adicionales de instalación.

## Integridad y recuperación

El sistema utiliza mecanismos con objetivos diferentes:

| Mecanismo | Función |
|---|---|
| Hash SHA-256 de contraseña | Comparar la contraseña ingresada con su huella almacenada. |
| Cifrado AES | Guardar el detalle de la bitácora de forma reversible para poder consultarlo. |
| DVH | Comprobar los campos de cada cuenta o asignación de rol. |
| DVV | Comprobar el conjunto ordenado de registros y su cantidad, usando la referencia de `IntegridadTabla`. |

**Los DVH/DVV actuales protegen exclusivamente Usuarios y UsuarioRol.** Las escrituras autorizadas de esas tablas actualizan datos y verificadores en una misma transacción Serializable. La bitácora y las traducciones no participan de ese control.

- **Crear respaldo comprobado:** genera un `.bak`, lo restaura en una base auxiliar y verifica sus datos protegidos.
- **Elegir respaldo:** selecciona la ruta del archivo; la recuperación se inicia con el botón Recuperar.
- **Recuperar:** utiliza un respaldo comprobado de la misma base para reponer datos. La opción de restauración completa de seguridad repone Usuarios y UsuarioRol y descarta sus cambios posteriores al respaldo.
- **Recalcular dígitos (asumir pérdida):** acepta expresamente los registros actuales, incluidas posibles alteraciones o pérdidas. Recalcula verificadores y deja auditoría; no recupera información anterior.

El archivo debe ser accesible para SQL Server en el equipo donde se ejecuta el servicio. El log externo de integridad se encuentra en `%LOCALAPPDATA%\SIGAT\logs\integridad.log`.

Ver [Guía de integridad](Querys/GUIA_INTEGRIDAD.md) para preparar la referencia y demostrar detección y recuperación en una copia de ensayo.

## Pruebas automatizadas

Las pruebas utilizan **MSTest** y cubren Composite, sesión, idiomas, cifrado, presentación, diseñador y persistencia.

Para ejecutar las pruebas que no requieren SQL:

```powershell
dotnet test SIGAT.Tests/SIGAT.Tests.csproj --filter "TestCategory!=SQLIntegration"
```

Las pruebas `SQLIntegration` requieren bases exclusivas con prefijo `SIGAT_Roles_Pruebas_` y conexiones configuradas en las siguientes variables de entorno:

| Variable | Conjunto de pruebas |
|---|---|
| `SIGAT_ROLES_TEST_CONNECTION` | Roles, permisos y gestión de usuarios. |
| `SIGAT_IDIOMA_TEST_CONNECTION` | Preferencias de idioma y concurrencia con la desactivación. |
| `SIGAT_INTEGRIDAD_TEST_CONNECTION` | Inicialización, alteraciones, respaldos, recuperación y recálculo. |

Deben prepararse según las condiciones de cada prueba: algunas esperan cuentas de ensayo determinadas y la prueba de integridad comienza sin referencia inicializada. Estas pruebas modifican datos y generan respaldos; no se ejecutan sobre la base habitual.

Una vez configuradas las bases de ensayo y las variables:

```powershell
dotnet test SIGAT.Tests/SIGAT.Tests.csproj --filter "TestCategory=SQLIntegration"
```

## Recorrido para la demostración

1. **Usuarios y acceso:** crear una cuenta sin roles y explicar la baja lógica y la protección del administrador original.
2. **Composite:** asignar un rol a la cuenta, recorrer su árbol de permisos y demostrar el rechazo de un ciclo al editar familias.
3. **Observer:** cambiar el idioma y observar la actualización de las pantallas abiertas; volver a ingresar para comprobar la preferencia de la cuenta.
4. **Bitácora:** consultar los eventos de acceso y gestión de usuarios mediante filtros combinados.
5. **Integridad:** en una copia de ensayo con respaldo previo, modificar externamente una cuenta, verificar la detección y recuperar el dato original.

Para explicar el código, partir de [Program.cs](SIGAT.UI/Program.cs), continuar con el formulario del módulo y seguir la llamada a su BLL y DAL. En cada recorrido, identificar qué valida cada capa y dónde se confirma la transacción.

## Documentación y trabajo pendiente

La carpeta de entrega académica reúne las especificaciones de casos de uso y los diagramas de arquitectura, clases, secuencia y datos. Se presenta junto con el proyecto. El repositorio incluye el [modelo actualizado de Enterprise Architect](EA-Diagramas/Actual/SIGAT_Modelo_Actualizado.EAP), su [índice de diagramas](EA-Diagramas/Actual/index.html), la [guía del diseñador](GUIA_DISENADOR.txt) y las guías SQL enlazadas anteriormente.

Los modelos existentes requieren una revisión de correspondencia con la versión actual: deben reflejar roles y familias, Observer y el alcance vigente de DVH/DVV. La documentación de una versión anterior no sustituye el comportamiento implementado en el código.

Los principales pendientes de la entrega final son:

- **T06b — Control de cambios:** conservar versiones de una entidad, identificar quién y cuándo la modificó, consultar sus valores históricos y recomponer un estado anterior. La bitácora actual registra eventos, pero no implementa ese historial de versiones.
- **G06 — Diagramas de clases:** integrar los módulos implementados en un diagrama por capa y separar los aspectos técnicos y de negocio.
- **G07 — Modelo de datos:** actualizar tablas, claves y relaciones conforme al esquema final, incorporando la estructura de T06b cuando se implemente.
- **Auditoría y consistencia:** ampliar la cobertura de eventos y coordinar el guardado de operaciones con su auditoría; completar las autorizaciones en negocio y las transacciones de creación e importación de idiomas.
- **Alcance de negocio:** desarrollar inventario de activos, asignaciones, movimientos, mantenimiento y reportes según el propósito general de SIGAT.
