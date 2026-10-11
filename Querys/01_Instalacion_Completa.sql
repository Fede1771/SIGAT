/* SIGAT - INSTALACIÓN COMPLETA PARA UNA PC NUEVA
   Ejecutar TODO el archivo en SQL Server 2019 o posterior (SSMS o sqlcmd).
   Instalar SQL Server Express con instancia SQLEXPRESS para la conexión actual.
   Editar @ClaveAdmin antes de ejecutar. Usuario inicial: admin.
   Incluye usuarios, bitácora vacía, idiomas, traducciones, roles, permisos,
   familias editables y protección del administrador original.
   No requiere scripts adicionales. Se detiene si la base ya existe.
   Si falla, revierte tablas/datos; puede quedar una base vacía para revisar.
   No modificar el nivel de compatibilidad: se usa el predeterminado del motor.
*/
USE [master];
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @Base sysname = N'SIGAT';
DECLARE @ClaveAdmin nvarchar(100) = N'NuevaClave123'; -- Cambiar para esta instalación.
IF @@TRANCOUNT <> 0
    THROW 50001, 'Ejecute el instalador fuera de una transacción abierta.', 1;
IF TRY_CONVERT(int, SERVERPROPERTY('ProductMajorVersion')) < 15
    THROW 50001, 'Este instalador requiere SQL Server 2019 o posterior.', 1;
IF DB_ID(@Base) IS NOT NULL
    THROW 50001, 'La base ya existe. Use 02_Actualizar_Base_Existente.sql.', 1;
IF NULLIF(LTRIM(RTRIM(@ClaveAdmin)), N'') IS NULL
    THROW 50001, 'La contraseña inicial no puede estar vacía.', 1;
-- UTF-8 coincide con HashHelper.ObtenerHashSHA256, incluso con tildes.
DECLARE @HashAdmin varchar(64) = LOWER(CONVERT(varchar(64), HASHBYTES('SHA2_256',
    CONVERT(varchar(max), @ClaveAdmin COLLATE Latin1_General_100_BIN2_UTF8)), 2));
BEGIN TRY
    DECLARE @CrearBase nvarchar(max) = N'CREATE DATABASE ' + QUOTENAME(@Base);
    EXEC(@CrearBase);
    DECLARE @Opciones nvarchar(max) = N'ALTER DATABASE ' + QUOTENAME(@Base) + N' SET AUTO_CLOSE OFF; '
        + N'ALTER DATABASE ' + QUOTENAME(@Base) + N' SET RECOVERY SIMPLE;';
    EXEC(@Opciones);
    DECLARE @Sql nvarchar(max) = N'USE ' + QUOTENAME(@Base) + N'; ' +
N'SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
BEGIN TRANSACTION;
SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Bitacora](
	[IdBitacora] [int] IDENTITY(1,1) NOT NULL,
	[Fecha] [datetime] NOT NULL,
	[Usuario] [varchar](50) NOT NULL,
	[Actividad] [varchar](255) NOT NULL,
	[InformacionAsociada] [nvarchar](max) NULL,
	[DigitoVerificador] [nvarchar](250) NULL,
PRIMARY KEY CLUSTERED 
(
	[IdBitacora] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Control](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Control] [varchar](100) NOT NULL,
	[Form] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Idioma](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Nombre] [varchar](50) NOT NULL,
	[Codigo] [varchar](10) NOT NULL,
	[NombreNativo] [nvarchar](100) NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Traduccion](
	[Id_Idioma] [int] NOT NULL,
	[Id_Control] [int] NOT NULL,
	[Texto] [nvarchar](500) NULL,
	[DigitoVerificador] [char](64) NULL,
	[Estado] [varchar](20) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id_Idioma] ASC,
	[Id_Control] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]


SET ANSI_NULLS ON

SET QUOTED_IDENTIFIER ON

CREATE TABLE [dbo].[Usuarios](
	[IdUsuario] [int] IDENTITY(1,1) NOT NULL,
	[NombreUsuario] [varchar](50) NOT NULL,
	[Password] [varchar](256) NOT NULL,
	[Nombre] [varchar](100) NOT NULL,
	[Apellido] [varchar](100) NOT NULL,
	[Activo] [bit] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]

/* La tabla Bitacora se crea vacía a propósito: una instalación nueva no
   debe heredar el historial de auditoría de la PC de origen. */

SET IDENTITY_INSERT [dbo].[Control] ON 

INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (33, N''1'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (35, N''2'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (34, N''3'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (46, N''4'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (38, N''btn_aplicar_idioma'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (49, N''btn_aplicar_idioma'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (9, N''btn_buscar'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (61, N''btn_buscar'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (39, N''btn_cambiar_estado'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (50, N''btn_cambiar_estado'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (24, N''btn_eliminar'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (23, N''btn_guardar'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (45, N''btn_guardar_traduccion'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (56, N''btn_guardar_traduccion'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (25, N''btn_limpiar'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (44, N''btn_nuevo_idioma'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (55, N''btn_nuevo_idioma'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (22, N''chk_activo'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (13, N''chk_fechas'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (58, N''chk_fechas'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (43, N''chk_idioma_activo'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (54, N''chk_idioma_activo'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (12, N''col_actividad'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (65, N''col_actividad'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (30, N''col_activo'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (29, N''col_apellido'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (10, N''col_fecha'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (63, N''col_fecha'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (14, N''col_id'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (62, N''col_id'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (26, N''col_idusuario'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (15, N''col_informacion'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (66, N''col_informacion'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (28, N''col_nombre'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (27, N''col_nombreusuario'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (31, N''col_perfil'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (11, N''col_usuario'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (64, N''col_usuario'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (6, N''frmbitacora_titulo'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (57, N''frmbitacora_titulo'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (36, N''frmgestionidiomas_titulo'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (47, N''frmgestionidiomas_titulo'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (16, N''frmgestionusuarios_titulo'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (8, N''lbl_actividad'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (60, N''lbl_actividad'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (20, N''lbl_apellido'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (18, N''lbl_clave'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (41, N''lbl_codigo_idioma'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (52, N''lbl_codigo_idioma'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (37, N''lbl_idioma_activo'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (48, N''lbl_idioma_activo'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (19, N''lbl_nombre'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (42, N''lbl_nombre_nativo'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (53, N''lbl_nombre_nativo'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (40, N''lbl_nuevo_idioma'', N''FrmGestionIdiomas'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (51, N''lbl_nuevo_idioma'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (21, N''lbl_perfil'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (7, N''lbl_usuario'', N''FrmBitacora'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (59, N''lbl_usuario'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (17, N''lbl_usuario_gu'', N''FrmGestionUsuarios'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (3, N''menu_bitacora'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (5, N''menu_idioma'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (4, N''menu_logout'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (1, N''menu_sistema'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (2, N''menu_usuarios'', N''FrmPrincipal'')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (32, N''titulo_ventana'', N''FrmPrincipal'')
SET IDENTITY_INSERT [dbo].[Control] OFF

SET IDENTITY_INSERT [dbo].[Idioma] ON 

INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (1, N''Español'', N''es'', N''Español'', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (2, N''Portugués'', N''pt'', N''Português'', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (3, N''Inglés'', N''en'', N''English'', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (4, N''Ruso'', N''ru'', N''Русский'', 1)
SET IDENTITY_INSERT [dbo].[Idioma] OFF

INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 1, N''Sistema'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 2, N''Gestión de Usuarios'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 3, N''Bitácora'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 4, N''Cerrar Sesión'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 5, N''Idioma'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 6, N''Consulta de Bitácora'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 7, N''Usuario:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 8, N''Actividad:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 9, N''Buscar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 10, N''Fecha'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 11, N''Usuario'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 12, N''Actividad'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 13, N''Fechas:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 14, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 15, N''Información'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 16, N''Gestión de Usuarios'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 17, N''Usuario:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 18, N''Clave (vacío no cambia):'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 19, N''Nombre:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 20, N''Apellido:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 21, N''Perfil:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 22, N''Activo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 23, N''Guardar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 24, N''Baja'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 25, N''Limpiar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 26, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 27, N''Usuario'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 28, N''Nombre'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 29, N''Apellido'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 30, N''Activo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 31, N''Perfil'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 32, N''FrmPrincipal'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 33, N''Español'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 34, N''Inglés'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 35, N''Portugués'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 36, N''Gestión de idiomas'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 37, N''Idioma:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 38, N''Aplicar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 39, N''Activar / desactivar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 40, N''Nombre:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 41, N''Código:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 42, N''Nombre nativo:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 43, N''Activo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 44, N''Crear idioma'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 45, N''Guardar traducción'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 46, N''Ruso'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 47, N''Gestión de idiomas'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 48, N''Idioma:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 49, N''Aplicar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 50, N''Activar / desactivar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 51, N''Nombre:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 52, N''Código:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 53, N''Nombre nativo:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 54, N''Activo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 55, N''Crear idioma'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 56, N''Guardar traducción'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 57, N''Consulta de Bitácora'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 58, N''Fechas:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 59, N''Usuario:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 60, N''Actividad:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 61, N''Поиск'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 62, N''IdBitacora'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 63, N''Fecha'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 64, N''Usuario'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 65, N''Actividad'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 66, N''InformacionAsociada'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 1, N''Sistema'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 2, N''Gestão de Usuários'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 3, N''Registro'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 4, N''Encerrar Sessão'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 5, N''Idioma'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 6, N''Consulta de Registro'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 7, N''Usuário:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 8, N''Atividade:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 9, N''Buscar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 10, N''Data'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 11, N''Usuário'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 12, N''Atividade'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 13, N''Datas:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 14, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 15, N''Informação'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 16, N''Gestão de Usuários'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 17, N''Usuário:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 18, N''Senha (vazio não altera):'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 19, N''Nome:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 20, N''Sobrenome:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 21, N''Perfil:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 22, N''Ativo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 23, N''Salvar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 24, N''Baixa'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 25, N''Limpar'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 26, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 27, N''Usuário'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 28, N''Nome'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 29, N''Sobrenome'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 30, N''Ativo'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 31, N''Perfil'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 32, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 33, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 34, NULL, NULL, N''Pendiente'')

INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 35, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 36, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 37, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 38, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 39, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 40, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 41, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 42, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 43, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 44, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 45, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 46, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 47, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 48, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 49, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 50, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 51, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 52, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 53, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 54, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 55, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 56, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 57, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 58, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 59, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 60, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 61, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 62, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 63, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 64, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 65, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 66, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 1, N''System'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 2, N''User Management'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 3, N''Log'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 4, N''Log Out'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 5, N''Language'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 6, N''Log Search'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 7, N''User:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 8, N''Activity:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 9, N''Search'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 10, N''Date'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 11, N''User'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 12, N''Activity'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 13, N''Dates:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 14, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 15, N''Information'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 16, N''User Management'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 17, N''User:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 18, N''Password (blank = no change):'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 19, N''Name:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 20, N''Last name:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 21, N''Role:'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 22, N''Active'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 23, N''Save'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 24, N''Deactivate'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 25, N''Clear'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 26, N''Id'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 27, N''Username'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 28, N''First name'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 29, N''Last name'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 30, N''Active'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 31, N''Role'', NULL, N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 32, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 33, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 34, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 35, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 36, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 37, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 38, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 39, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 40, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 41, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 42, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 43, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 44, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 45, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 46, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 47, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 48, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 49, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 50, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 51, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 52, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 53, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 54, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 55, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 56, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 57, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 58, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 59, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 60, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 61, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 62, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 63, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 64, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 65, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 66, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 1, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 2, NULL, NULL, N''Pendiente'')

INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 3, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 4, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 5, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 6, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 7, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 8, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 9, N''Поиск'', N''69886a6e56c594db63de416605a13ec12c58633c0360c70f2e8e70aacc70f8aa'', N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 10, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 11, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 12, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 13, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 14, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 15, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 16, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 17, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 18, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 19, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 20, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 21, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 22, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 23, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 24, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 25, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 26, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 27, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 28, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 29, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 30, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 31, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 32, N''SIGAT'', N''ff523ab7bebc6501e26209b93c8c4225c2dba59d84cb486006d0a9a5abf49b6c'', N''Completa'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 33, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 34, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 35, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 36, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 37, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 38, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 39, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 40, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 41, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 42, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 43, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 44, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 45, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 46, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 47, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 48, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 49, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 50, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 51, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 52, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 53, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 54, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 55, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 56, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 57, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 58, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 59, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 60, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 61, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 62, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 63, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 64, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 65, NULL, NULL, N''Pendiente'')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 66, NULL, NULL, N''Pendiente'')

SET IDENTITY_INSERT [dbo].[Usuarios] ON 

INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo]) VALUES (1, N''admin'', @HashAdmin, N''Admin'', N''Sistema'', 1)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF

SET ANSI_PADDING ON


ALTER TABLE [dbo].[Control] ADD  CONSTRAINT [UQ_Control] UNIQUE NONCLUSTERED 
(
	[Control] ASC,
	[Form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

SET ANSI_PADDING ON


ALTER TABLE [dbo].[Idioma] ADD UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

SET ANSI_PADDING ON


ALTER TABLE [dbo].[Idioma] ADD  CONSTRAINT [UQ_Idioma_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

SET ANSI_PADDING ON


ALTER TABLE [dbo].[Usuarios] ADD UNIQUE NONCLUSTERED 
(
	[NombreUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]

ALTER TABLE [dbo].[Idioma] ADD  CONSTRAINT [DF_Idioma_Activo]  DEFAULT ((1)) FOR [Activo]

ALTER TABLE [dbo].[Traduccion] ADD  CONSTRAINT [DF_Traduccion_Estado]  DEFAULT (''Completa'') FOR [Estado]

ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Activo]

ALTER TABLE [dbo].[Traduccion]  WITH CHECK ADD  CONSTRAINT [FK_Traduccion_Control] FOREIGN KEY([Id_Control])
REFERENCES [dbo].[Control] ([Id])

ALTER TABLE [dbo].[Traduccion] CHECK CONSTRAINT [FK_Traduccion_Control]

ALTER TABLE [dbo].[Traduccion]  WITH CHECK ADD  CONSTRAINT [FK_Traduccion_Idioma] FOREIGN KEY([Id_Idioma])
REFERENCES [dbo].[Idioma] ([Id])

ALTER TABLE [dbo].[Traduccion] CHECK CONSTRAINT [FK_Traduccion_Idioma]

CREATE TABLE dbo.SeguridadVersion (
 Id int NOT NULL PRIMARY KEY CHECK (Id=1),
 Version bigint NOT NULL,
 Esquema int NOT NULL CHECK (Esquema=2)
);
CREATE TABLE dbo.Permiso (
 Id int NOT NULL PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL UNIQUE,
 EsCompuesto bit NOT NULL,
 CONSTRAINT CK_Permiso_Matriz CHECK (
 (Id=1 AND Nombre=N''Ver bitácora'' AND EsCompuesto=0) OR
 (Id=2 AND Nombre=N''Consultar inventario'' AND EsCompuesto=0) OR
 (Id=3 AND Nombre=N''Gestión de activos'' AND EsCompuesto=1) OR
 (Id=4 AND Nombre=N''Movimientos'' AND EsCompuesto=1) OR
 (Id=5 AND Nombre=N''Mantenimiento'' AND EsCompuesto=1) OR
 (Id=6 AND Nombre=N''Reportes'' AND EsCompuesto=1) OR
 (Id=7 AND Nombre=N''Administración del sistema'' AND EsCompuesto=1))
);
CREATE TABLE dbo.Rol (
 Id int NOT NULL PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL UNIQUE,
 CONSTRAINT CK_Rol_Matriz CHECK (
 (Id=1 AND Nombre=N''Administrador'') OR
 (Id=2 AND Nombre=N''Responsable de Inventario'') OR
 (Id=3 AND Nombre=N''Técnico de Mantenimiento'') OR
 (Id=4 AND Nombre=N''Mesa de Ayuda'') OR
 (Id=5 AND Nombre=N''Auditor''))
);
CREATE TABLE dbo.RolPermiso (
 IdRol int NOT NULL REFERENCES dbo.Rol(Id),
 IdPermiso int NOT NULL REFERENCES dbo.Permiso(Id),
 PRIMARY KEY (IdRol,IdPermiso),
 CONSTRAINT CK_RolPermiso_Matriz CHECK (
 (IdRol=1 AND IdPermiso IN (1,3,4,5,6,7)) OR
 (IdRol=2 AND IdPermiso IN (3,4,6)) OR
 (IdRol=3 AND IdPermiso IN (5,2)) OR
 (IdRol=4 AND IdPermiso IN (4,2)) OR
 (IdRol=5 AND IdPermiso IN (1,2,6)))
);
CREATE TABLE dbo.UsuarioRol (
 IdUsuario int NOT NULL REFERENCES dbo.Usuarios(IdUsuario),
 IdRol int NOT NULL REFERENCES dbo.Rol(Id),
 PRIMARY KEY (IdUsuario,IdRol)
);
-- No se crea PermisoHijo: la matriz actual no define hijos para las familias.
INSERT dbo.Permiso VALUES
 (1,N''Ver bitácora'',0),(2,N''Consultar inventario'',0),
 (3,N''Gestión de activos'',1),(4,N''Movimientos'',1),
 (5,N''Mantenimiento'',1),(6,N''Reportes'',1),
 (7,N''Administración del sistema'',1);
INSERT dbo.Rol VALUES
 (1,N''Administrador''),(2,N''Responsable de Inventario''),
 (3,N''Técnico de Mantenimiento''),(4,N''Mesa de Ayuda''),(5,N''Auditor'');
INSERT dbo.RolPermiso VALUES
 (1,1),(1,3),(1,4),(1,5),(1,6),(1,7),
 (2,3),(2,4),(2,6),(3,5),(3,2),(4,4),(4,2),(5,1),(5,2),(5,6);
ALTER TABLE dbo.Rol DROP CONSTRAINT CK_Rol_Matriz;
ALTER TABLE dbo.RolPermiso DROP CONSTRAINT CK_RolPermiso_Matriz;
CREATE TABLE dbo.AdministradorOriginal (
 Id int NOT NULL PRIMARY KEY CHECK (Id=1),
 IdUsuario int NOT NULL UNIQUE REFERENCES dbo.Usuarios(IdUsuario)
);
CREATE TABLE dbo.RolHijo (
 IdPadre int NOT NULL REFERENCES dbo.Rol(Id),
 IdHijo int NOT NULL REFERENCES dbo.Rol(Id),
 PRIMARY KEY (IdPadre,IdHijo), CHECK (IdPadre<>IdHijo)
);
CREATE TABLE dbo.PermisoHijo (
 IdPadre int NOT NULL REFERENCES dbo.Permiso(Id),
 IdHijo int NOT NULL REFERENCES dbo.Permiso(Id),
 PRIMARY KEY (IdPadre,IdHijo),
 CHECK (IdPadre IN (3,4,5,6,7) AND IdPadre<>IdHijo)
);
CREATE TABLE dbo.IntegridadTabla (
 Tabla nvarchar(30) NOT NULL PRIMARY KEY,HashVertical char(64) NOT NULL,Cantidad bigint NOT NULL,
 IdBase uniqueidentifier NOT NULL,Version int NOT NULL CHECK(Version=1)
);
INSERT dbo.SeguridadVersion (Id,Version,Esquema) VALUES (1,1,2);
INSERT dbo.UsuarioRol (IdUsuario,IdRol) VALUES (1,1);
INSERT dbo.AdministradorOriginal (Id,IdUsuario) VALUES (1,1);
CREATE TABLE dbo.UsuarioIdioma (
 IdUsuario int NOT NULL CONSTRAINT PK_UsuarioIdioma PRIMARY KEY,
 IdIdioma int NOT NULL,
 CONSTRAINT FK_UsuarioIdioma_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(IdUsuario),
 CONSTRAINT FK_UsuarioIdioma_Idioma FOREIGN KEY (IdIdioma) REFERENCES dbo.Idioma(Id)
);
EXEC(N''CREATE OR ALTER TRIGGER dbo.TR_Usuarios_AdministradorOriginal ON dbo.Usuarios
AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted d JOIN dbo.AdministradorOriginal a ON a.IdUsuario=d.IdUsuario
               LEFT JOIN inserted i ON i.IdUsuario=d.IdUsuario WHERE i.IdUsuario IS NULL OR i.Activo=0)
        THROW 50001, ''''No se puede eliminar ni desactivar al administrador original.'''', 1;
END;'');
COMMIT TRANSACTION;';
    EXEC sys.sp_executesql @Sql, N'@HashAdmin varchar(64)', @HashAdmin=@HashAdmin;
    PRINT N'Instalación completa finalizada. Ingrese con admin y la contraseña configurada en @ClaveAdmin.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
