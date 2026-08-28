/* =========================================================================
   SIGAT - Script de instalación completa (estructura + datos)
   Generado a partir de la base actual. Pensado para levantar la app
   en otra PC sin depender de rutas de archivos ni de la instancia actual.
   ========================================================================= */

USE [master]
GO

-- Si la base ya existe, no la vuelve a crear (evita error al re-ejecutar)
IF DB_ID('SIGAT') IS NULL
BEGIN
    CREATE DATABASE [SIGAT];
END
GO

/* NOTA: si en la PC destino tenés una versión de SQL Server anterior a 2022,
   comentá o bajá el nivel de compatibilidad de la siguiente línea
   (170 = SQL Server 2022). Por ejemplo, 160 = SQL Server 2022 también,
   150 = SQL Server 2019, 140 = SQL Server 2017. */
ALTER DATABASE [SIGAT] SET COMPATIBILITY_LEVEL = 170
GO

IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
BEGIN
    EXEC [SIGAT].[dbo].[sp_fulltext_database] @action = 'enable'
END
GO

ALTER DATABASE [SIGAT] SET ANSI_NULLS OFF
GO
ALTER DATABASE [SIGAT] SET ANSI_PADDING OFF
GO
ALTER DATABASE [SIGAT] SET ANSI_WARNINGS OFF
GO
ALTER DATABASE [SIGAT] SET ARITHABORT OFF
GO
ALTER DATABASE [SIGAT] SET AUTO_CLOSE ON
GO
ALTER DATABASE [SIGAT] SET AUTO_SHRINK OFF
GO
ALTER DATABASE [SIGAT] SET AUTO_UPDATE_STATISTICS ON
GO
ALTER DATABASE [SIGAT] SET RECOVERY SIMPLE
GO
ALTER DATABASE [SIGAT] SET MULTI_USER
GO

USE [SIGAT]
GO

/* =========================================================================
   TABLAS
   ========================================================================= */

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.Bitacora') IS NULL
BEGIN
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
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.Perfiles') IS NULL
BEGIN
    CREATE TABLE [dbo].[Perfiles](
        [IdPerfil] [int] IDENTITY(1,1) NOT NULL,
        [NombrePerfil] [varchar](50) NOT NULL,
    PRIMARY KEY CLUSTERED
    (
        [IdPerfil] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
IF OBJECT_ID('dbo.Usuarios') IS NULL
BEGIN
    CREATE TABLE [dbo].[Usuarios](
        [IdUsuario] [int] IDENTITY(1,1) NOT NULL,
        [NombreUsuario] [varchar](50) NOT NULL,
        [Password] [varchar](256) NOT NULL,
        [Nombre] [varchar](100) NOT NULL,
        [Apellido] [varchar](100) NOT NULL,
        [Activo] [bit] NOT NULL,
        [IdPerfil] [int] NOT NULL,
    PRIMARY KEY CLUSTERED
    (
        [IdUsuario] ASC
    )WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
    ) ON [PRIMARY]
END
GO

/* =========================================================================
   DATOS
   ========================================================================= */

SET IDENTITY_INSERT [dbo].[Bitacora] ON
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (1, CAST(N'2026-08-27T20:26:23.883' AS DateTime), N'admin', N'Login Exitoso', N'ur4qt+rRHyNH60p0mK1PWBebMaf8L8iCMjNAzvRfwfk=', N'GWQmdGyAZA2TS8uaCOJeTYClVp9F9bJZ4vNhXt1w76s=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (2, CAST(N'2026-08-27T20:49:18.367' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'TnCdzfmJXaOlF2dybEHY+/7y6ClX+RKUi9R9iSwlFZs=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (3, CAST(N'2026-08-27T20:58:58.753' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'CfJgKqFkv5nAuM0d+c8WtUUrSpaHbmU1t7llbcNAHus=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (4, CAST(N'2026-08-27T20:59:03.000' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'6Wb2awS2WkPis3/h1oAtLtbhYxj+4bWY3dri4bPl71E=')
SET IDENTITY_INSERT [dbo].[Bitacora] OFF
GO

SET IDENTITY_INSERT [dbo].[Perfiles] ON
INSERT [dbo].[Perfiles] ([IdPerfil], [NombrePerfil]) VALUES (1, N'Administrador')
INSERT [dbo].[Perfiles] ([IdPerfil], [NombrePerfil]) VALUES (2, N'Operador')
SET IDENTITY_INSERT [dbo].[Perfiles] OFF
GO

SET IDENTITY_INSERT [dbo].[Usuarios] ON
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (1, N'admin', N'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Admin', N'Sistema', 1, 1)
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (2, N'Bruna26', N'3daa437813cde6b8426eb033604d2b18a68d0328e328fea527bc6f9d4aa594cc', N'Bruna', N'Burgos', 1, 2)
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (3, N'dasda1', N'03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', N'dasdas', N'adsasd', 0, 2)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO

/* =========================================================================
   CONSTRAINTS E ÍNDICES
   ========================================================================= */

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Perfiles_NombrePerfil')
BEGIN
    ALTER TABLE [dbo].[Perfiles] ADD CONSTRAINT UQ_Perfiles_NombrePerfil UNIQUE NONCLUSTERED ([NombrePerfil] ASC)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Usuarios_NombreUsuario')
BEGIN
    ALTER TABLE [dbo].[Usuarios] ADD CONSTRAINT UQ_Usuarios_NombreUsuario UNIQUE NONCLUSTERED ([NombreUsuario] ASC)
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_Usuarios_Activo')
BEGIN
    ALTER TABLE [dbo].[Usuarios] ADD CONSTRAINT DF_Usuarios_Activo DEFAULT ((1)) FOR [Activo]
END
GO

IF NOT EXISTS (SELECT 1 FROM sys.foreign_keys WHERE name = 'FK_Usuarios_Perfiles')
BEGIN
    ALTER TABLE [dbo].[Usuarios] WITH CHECK ADD CONSTRAINT [FK_Usuarios_Perfiles] FOREIGN KEY([IdPerfil])
    REFERENCES [dbo].[Perfiles] ([IdPerfil])

    ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Perfiles]
END
GO

USE [master]
GO
ALTER DATABASE [SIGAT] SET READ_WRITE
GO

PRINT 'Base SIGAT instalada correctamente.';
GO
