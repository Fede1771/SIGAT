# SIGAT

**Sistema de Gestión de Activos Tecnológicos** · Proyecto académico de escritorio

![C#](https://img.shields.io/badge/C%23-512BD4?style=flat-square&logo=dotnet&logoColor=white)
![.NET 10](https://img.shields.io/badge/.NET-10-512BD4?style=flat-square)
![Windows Forms](https://img.shields.io/badge/Interfaz-Windows%20Forms-0078D4?style=flat-square)
![SQL Server](https://img.shields.io/badge/Base%20de%20datos-SQL%20Server-CC2927?style=flat-square)

SIGAT administra cuentas, accesos, idiomas y eventos del sistema mediante una aplicación en C# con Windows Forms y SQL Server. El código está dividido en capas para separar la presentación, las reglas de negocio y la persistencia.

## Contenido

- [Estado del proyecto](#estado-del-proyecto)
- [Funcionalidades](#funcionalidades)
- [Tecnologías](#tecnologías)
- [Arquitectura](#arquitectura)
- [Patrones de diseño](#patrones-de-diseño)
- [Roles iniciales](#roles-iniciales)
- [Instalación y ejecución](#instalación-y-ejecución)
- [Actualización de una base existente](#actualización-de-una-base-existente)
- [Pruebas](#pruebas)
- [Guía de lectura del código](#guía-de-lectura-del-código)
- [Pendientes conocidos](#pendientes-conocidos)

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

```text
SIGAT/
├── SIGAT.slnx       Solución que se abre en Visual Studio
├── SIGAT.UI/        Formularios y archivos del diseñador
├── SIGAT.BLL/       Reglas de negocio
├── SIGAT.DAL/       Acceso a SQL Server
├── SIGAT.BE/        Entidades del dominio
├── SIGAT.SERVICIOS/ Servicios compartidos
├── SIGAT.Tests/     Pruebas automatizadas
└── Querys/          Instalación, actualización y mantenimiento SQL
```

En los formularios, `*.Designer.cs` contiene la creación y distribución de los controles; el archivo `.cs` contiene eventos, carga de datos y comportamiento. El modo de diseño evita ejecutar operaciones de sesión o consultas SQL al abrir una pantalla en el diseñador.

## Patrones de diseño

- **Composite:** los permisos simples son hojas y los compuestos agrupan otros permisos.
- **Observer:** los formularios suscriptos actualizan sus textos al cambiar el idioma.
- **Singleton:** mantiene una instancia de la sesión y del administrador de idiomas por proceso.
- **Factory Method:** las fábricas construyen los distintos eventos de bitácora.
- **Mapper:** adapta los datos del evento a las columnas persistidas en la bitácora.

Los roles reúnen permisos y pueden heredar otros roles. La jerarquía de permisos usa `Permiso` como componente común, `PermisoSimple` como hoja y `PermisoCompuesto` como agrupación. Ambas jerarquías rechazan ciclos para evitar recorridos infinitos.

## Roles iniciales

| Rol | Componentes iniciales |
|---|---|
| Administrador | Ver bitácora, Gestión de activos, Movimientos, Mantenimiento, Reportes y Administración del sistema. |
| Responsable de Inventario | Gestión de activos, Movimientos y Reportes. |
| Técnico de Mantenimiento | Mantenimiento y Consultar inventario. |
| Mesa de Ayuda | Movimientos y Consultar inventario. |
| Auditor | Ver bitácora, Consultar inventario y Reportes. |

Ver bitácora y Consultar inventario son permisos simples. Los otros cinco componentes son permisos compuestos.

La tabla describe la configuración inicial. Las asignaciones y las familias editables se guardan en SQL; no se restablecen automáticamente al cerrar sesión. El administrador original y la composición del rol Administrador están protegidos.

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

## Guía de lectura del código

Estos recorridos permiten explicar una funcionalidad desde la pantalla hasta su almacenamiento:

| Tema | Archivos y recorrido para estudiar |
|---|---|
| Inicio de sesión | [`Program.cs`](SIGAT.UI/Program.cs) verifica la integridad; [`FrmLogin.cs`](SIGAT.UI/FrmLogin.cs) solicita autenticación a [`UsuarioBLL.cs`](SIGAT.BLL/UsuarioBLL.cs), que consulta la cuenta, carga roles e inicia la sesión. |
| Usuarios | [`FrmGestionUsuarios.cs`](SIGAT.UI/FrmGestionUsuarios.cs) reúne los datos; [`UsuarioBLL.cs`](SIGAT.BLL/UsuarioBLL.cs) valida y calcula el hash de la clave; [`UsuarioDAL.cs`](SIGAT.DAL/UsuarioDAL.cs) guarda o desactiva la cuenta. |
| Roles y Composite | [`Permiso.cs`](SIGAT.BE/Permiso.cs), [`Rol.cs`](SIGAT.BE/Rol.cs) y [`Usuario.cs`](SIGAT.BE/Usuario.cs) resuelven los permisos efectivos. [`RolesBLL.cs`](SIGAT.BLL/RolesBLL.cs) valida y [`RolesDAL.cs`](SIGAT.DAL/RolesDAL.cs) persiste las relaciones en una transacción. |
| Idiomas y Observer | [`IdiomaBLL.cs`](SIGAT.BLL/IdiomaBLL.cs) carga las traducciones; [`IdiomaManager.cs`](SIGAT.SERVICIOS/Idiomas/IdiomaManager.cs) notifica a los formularios; [`TraductorFormularios.cs`](SIGAT.UI/TraductorFormularios.cs) actualiza los textos. |
| Bitácora | [`BitacoraFactory.cs`](SIGAT.BLL/Fabricas/BitacoraFactory.cs) construye eventos; [`BitacoraMapper.cs`](SIGAT.BLL/BitacoraMapper.cs) adapta sus datos; [`BitacoraDAL.cs`](SIGAT.DAL/BitacoraDAL.cs) cifra el detalle y guarda el registro con control de integridad. |
| DVH y DVV | [`VerificadorSHA256.cs`](SIGAT.SERVICIOS/VerificadorSHA256.cs) calcula las huellas; [`IntegridadDAL.cs`](SIGAT.DAL/IntegridadDAL.cs) compara filas y conjuntos con los valores guardados. [`FrmIntegridad.cs`](SIGAT.UI/FrmIntegridad.cs) presenta los problemas y las acciones de recuperación. |
| Temas visuales | [`TemaVisual.Modos.cs`](SIGAT.UI/TemaVisual.Modos.cs) conserva los colores originales, aplica el tema y guarda la preferencia local por cuenta. |

Para la defensa, conviene explicar **qué recibe cada capa, qué valida y qué devuelve**, y demostrar el resultado: asignar un rol, cerrar sesión y comprobar los accesos al volver a ingresar. Los comentarios señalan las decisiones del código; las pruebas comprueban comportamientos esperados.

### Datos y verificadores

- **Hash de contraseña:** permite comparar la clave ingresada con su huella guardada; no permite recuperar la contraseña original.
- **Cifrado AES:** permite recuperar el detalle de los eventos para consultarlos en la bitácora.
- **DVH:** comprueba los campos de un registro y se almacena en su columna `DigitoVerificador`.
- **DVV:** comprueba el conjunto ordenado de registros y se almacena por separado en `IntegridadTabla`.

El alcance actual de DVH y DVV es **Bitacora y Traduccion**. La columna de DVH visible en la bitácora muestra el valor almacenado. La comprobación completa se realiza desde Integridad y recuperación y antes del acceso al sistema.

## Pendientes conocidos

- Ajustar el espacio del panel Composición del rol: una prueba visual detecta desplazamiento vertical en determinados tamaños.
- Unificar las operaciones de usuarios y su registro de bitácora para evitar guardados parciales si falla la auditoría.
- Completar las validaciones de autorización de usuarios e idiomas en la capa de negocio.
- Hacer atómica la creación de idiomas y controles con sus traducciones.
- Resolver la recuperación cuando falta un registro de control en IntegridadTabla.
- Ajustar el límite superior del filtro de fechas de bitácora al redondeo de SQL datetime.

El control de integridad actual cubre Bitacora y Traduccion. SHA-256 se utiliza para verificadores y hashes de contraseñas; el cifrado AES del detalle de bitácora cumple una función diferente.

