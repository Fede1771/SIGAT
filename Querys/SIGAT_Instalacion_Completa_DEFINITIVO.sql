/* =========================================================================
   SIGAT - Instalación completa DEFINITIVA (estructura real + datos reales)
   Generado a partir de un export directo de la base SIGAT en producción
   (SSMS > Tareas > Generar scripts, esquema y datos, 15/9/2026).
   Pensado para levantar la app en una PC distinta desde cero.

   Incluye: Bitacora, Control, Idioma, Perfiles, Traduccion, Usuarios
   con todos sus datos actuales (4 idiomas: Español/Portugués/Inglés/Ruso,
   264 traducciones, 4 usuarios).
   ========================================================================= */

USE [master]
GO

IF DB_ID('SIGAT') IS NULL
BEGIN
    CREATE DATABASE [SIGAT];
END
GO

/* Si la PC destino tiene una versión de SQL Server anterior a 2022,
   bajá este nivel: 170/160 = SQL Server 2022, 150 = 2019, 140 = 2017 */
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
   A PARTIR DE ACÁ: tablas + datos reales, tal como están en producción
   ========================================================================= */

/****** Objeto: Table [dbo].[Bitacora] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Objeto: Table [dbo].[Control] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Control](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Control] [varchar](100) NOT NULL,
	[Form] [varchar](100) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Idioma] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Objeto: Table [dbo].[Perfiles] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Perfiles](
	[IdPerfil] [int] IDENTITY(1,1) NOT NULL,
	[NombrePerfil] [varchar](50) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[IdPerfil] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Objeto: Table [dbo].[Traduccion] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
GO
/****** Objeto: Table [dbo].[Usuarios] Fecha de script: 15/9/2026 20:54:47 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Bitacora] ON 

INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (1, CAST(N'2026-08-27T20:26:23.883' AS DateTime), N'admin', N'Login Exitoso', N'ur4qt+rRHyNH60p0mK1PWBebMaf8L8iCMjNAzvRfwfk=', N'GWQmdGyAZA2TS8uaCOJeTYClVp9F9bJZ4vNhXt1w76s=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (2, CAST(N'2026-08-27T20:49:18.367' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'TnCdzfmJXaOlF2dybEHY+/7y6ClX+RKUi9R9iSwlFZs=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (3, CAST(N'2026-08-27T20:58:58.753' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'CfJgKqFkv5nAuM0d+c8WtUUrSpaHbmU1t7llbcNAHus=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (4, CAST(N'2026-08-27T20:59:03.000' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'6Wb2awS2WkPis3/h1oAtLtbhYxj+4bWY3dri4bPl71E=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (5, CAST(N'2026-08-27T23:08:53.973' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'z0oU8zLRZ+HZb40m6nXoSc/qR6rOhgC4y2kkGnocF/g=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (6, CAST(N'2026-08-27T23:08:55.853' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'4dv5ua3l1EpnAOLV0Tp98/5XfBLnki9nMJPMmenH1dY=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (7, CAST(N'2026-08-28T08:18:39.990' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'FXW1ipJXoIXwnxufKfUr+9hqyJB5ccbn/6nKb3bDi2c=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (8, CAST(N'2026-08-28T08:19:38.813' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'WczNrCy6SV+1Hz1PweXy8K28qt584rLSK9WVoG5U7qc=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (9, CAST(N'2026-08-28T08:19:43.250' AS DateTime), N'Bruna26', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'QvA3D9OkbNPWOF3B9SJZN76cz3L/oOLfjXaCfPU8V4o=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (10, CAST(N'2026-08-28T08:20:56.393' AS DateTime), N'Bruna26', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'SlK8Hp+HSyr48163TjMHSqgThefWLzwZ+PDUyWGLNSs=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (11, CAST(N'2026-09-02T12:05:50.067' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'WasSXJ1LXLc7r1FOBV+HLsP+XqssxNdgSsY8c2pxxu4=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (12, CAST(N'2026-09-02T12:06:11.453' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'MkJ8CekttGnBhvjCP/Yiz4u3sax19s6npjh+eYgUeOg=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (13, CAST(N'2026-09-02T12:09:18.940' AS DateTime), N'fsfdsfsq', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj7026M936Ey84tU56xNX+/xWlgYSjVx3ZbPX4d4CzKraDIHxcHmcHRL1o7WWgrbnOw==', N'aT8LeLGzk3os4FzTomEhjI8yhoPhC2ysA0O7fM7n+Gc=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (14, CAST(N'2026-09-02T12:09:32.283' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'X9KmEpGWWvUKEMFEX32n0saGxZyBGYe9MJ6fpdJ0AAo=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (15, CAST(N'2026-09-02T12:15:12.607' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'l0L4/TZKuwq/nzrHhHvvHw7acGDRgVn0sGe5reU25Us=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (16, CAST(N'2026-09-02T12:15:43.193' AS DateTime), N'admin', N'AltaUsuario', N'YcxPstyILLJDyFsvp5zvLcZktEiRJSLioBc3O06VjwOnn0+srKx8AREfauUGJp4ujBHJ9IalPUnk61U/bP/A7/m7UUut1560oOvREtGeTR9hd+TB+UNlrTOW2TZI7Kty', N'CBv5Lz8fcTISezqTyjz3VBD9QgfCDP2/eMzgrj2Nnwo=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (17, CAST(N'2026-09-02T12:15:52.863' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'yGsT+gCYnHv9DumwwH1ysCisBLWgK+vAz44tTXYI6fQ=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (18, CAST(N'2026-09-02T12:15:58.317' AS DateTime), N'valen1', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'hx72DukeYTIGGAnc3SHl0mtYwSkkIpDWh/sRfAOB9ak=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (19, CAST(N'2026-09-02T12:16:12.773' AS DateTime), N'valen1', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'IxV/F71gHt0QHIC8JJzgUx+7KSzJVaZRU9OlcZjSzY0=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (20, CAST(N'2026-09-02T12:16:16.020' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'7quurhIfd+zCnsSGiL7RjqQuDZnnAtMDfO5FwckdMFg=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (21, CAST(N'2026-09-02T12:16:21.903' AS DateTime), N'admin', N'BajaUsuario', N'YcxPstyILLJDyFsvp5zvLcZktEiRJSLioBc3O06VjwOnn0+srKx8AREfauUGJp4ujBHJ9IalPUnk61U/bP/A75X+3fYcJz0RSqnXdazernBsFx+pzkgWLCh1MkHezK0iYUV8PDSYYfHdXcuOlqj9Bg==', N'mXdVeM3ezGgLNcZgITC/k0kZGv+y5J3fQoiyYKm7CI4=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (22, CAST(N'2026-09-02T12:16:25.240' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'wDwr+dfD+mMgSUlKQYylOWmrzRx7S1D3kZu2WBX0e+g=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (23, CAST(N'2026-09-02T12:16:28.760' AS DateTime), N'valen1', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj5oWqEgKUMRHMZfM1JY1o5KOjMbv/+SoHyzD8XsMQrbgGnkMo4NFybPrXt42jeCYn4KUP5cGk+xf3AOynBEFnzw=', N'aN/pyaxdRSt7LXuepul4cy1s4QpVASfOvEAz2b88wzw=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (24, CAST(N'2026-09-02T12:16:50.293' AS DateTime), N'fede1', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj7026M936Ey84tU56xNX+/xWlgYSjVx3ZbPX4d4CzKraDIHxcHmcHRL1o7WWgrbnOw==', N'mC/vrKH1moLaqDGxr1rD6TBfBxH4sKCijht5UNOH3+A=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (25, CAST(N'2026-09-02T12:18:43.747' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'blGibYusjbYtdm4X5gWn8LtR6yhEtlzbDodrFq8BjGs=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (26, CAST(N'2026-09-02T12:21:15.153' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'UMr5Y/OWnXarAIPRkeQhkZZY5zec20ntLAjPaDOLRE0=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (27, CAST(N'2026-09-02T12:21:33.227' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'yw2AtG6F+ljFpJaYaqFO41LOS07qkmvvpo2KqGbdwa8=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (28, CAST(N'2026-09-02T12:21:43.420' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'taBXynAydtyDJgfzXYssIpV/AzPIibRgdLGHdGz2nMk=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (29, CAST(N'2026-09-02T12:22:41.267' AS DateTime), N'admin', N'ModificacionUsuario', N'YcxPstyILLJDyFsvp5zvLcZktEiRJSLioBc3O06VjwOnn0+srKx8AREfauUGJp4ujBHJ9IalPUnk61U/bP/A7/I5ZOOnmeU/T+gZN/Wb0YPrEpYhH15K3Sw+MfjgrJTd', N'90hrzEJIWEbD61Ng0/MsFqc1puT4Jrl682bExjkWtEI=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (30, CAST(N'2026-09-02T12:22:44.887' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'NdBDBlAXB7ntwne66zDoA9I8LQryWPbSBQm42OLOc+8=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (31, CAST(N'2026-09-02T12:22:47.793' AS DateTime), N'valen1', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'x8brwxjRAkHIZkjqz536El1HRY9NrGJj1KuTMNFu2w0=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (32, CAST(N'2026-09-02T12:22:51.453' AS DateTime), N'valen1', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'TmLInkfmuD07IEccZ0fpS36Z3rTzbgya0o6+f976NZg=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (33, CAST(N'2026-09-02T12:24:37.783' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'+QQS1mIbCpBjTdaNFI8YA4esZfEk+vMB0n1t0HeU76A=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (34, CAST(N'2026-09-02T12:24:50.213' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'/bAtm1oZiDgKDE7AmGot0MaphasIeN0rVSGJlFuMTds=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (35, CAST(N'2026-09-02T12:25:01.257' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'9T6nFIFH91JprqCZ3V9C5Jhs0aviOzMnXGPaQkAdifw=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (36, CAST(N'2026-09-02T12:25:53.180' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'0yO3baRHWSis77SwMGtjfGx3TgQiknxbifqVK5Evqjk=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (37, CAST(N'2026-09-02T12:25:57.513' AS DateTime), N'valen1', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj7026M936Ey84tU56xNX+/xWlgYSjVx3ZbPX4d4CzKraDIHxcHmcHRL1o7WWgrbnOw==', N'vgXtPBwszZkjdcnc0r30rCUn2Gjt7P0xW6ry6+Ii8+c=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (38, CAST(N'2026-09-02T12:25:58.967' AS DateTime), N'valen1', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj7026M936Ey84tU56xNX+/xWlgYSjVx3ZbPX4d4CzKraDIHxcHmcHRL1o7WWgrbnOw==', N'fl5u3etoe1P4Xb8jamMcx6xrEa2YeBBh10XnvG+JAr0=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (39, CAST(N'2026-09-02T12:26:00.847' AS DateTime), N'valen1', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'QpLxEnDyi1It+YV67CadSqAjMNq0T2PUgzEIAOR4yP0=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (40, CAST(N'2026-09-02T18:48:39.050' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'K2jtltbMKi6S7aW6CHLTuMJ6Bmw3PCbkO+MsQ++4Kpc=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (41, CAST(N'2026-09-02T19:57:05.970' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'BpGqaFnQ4qwFf9G/adDlvVc/tHn4MtQhoCrXPV4q1jg=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (42, CAST(N'2026-09-04T19:17:18.220' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'CMKgXiogN3GZFZUgeC89spY2vldnC4HridFnGW0cuoo=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (43, CAST(N'2026-09-04T19:17:23.170' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'aDS1NrKpLhTL9BQAyMcq2hdztNkgr5UZKTzUlDuWZSE=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (44, CAST(N'2026-09-04T19:18:09.853' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'wwb27OSaKNgv9iOQeQrYyqXbEq1xRBkHKM12FnkK9sw=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (45, CAST(N'2026-09-04T19:23:49.683' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'pkTaswL95M3g8EajR3/v19O5HobFWgqqARzcK/dy8CI=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (46, CAST(N'2026-09-04T19:23:52.033' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'/pVFVy85YOaygAQzcZltsTuoWGwkyFirXpyKMuYJuIw=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (47, CAST(N'2026-09-14T19:35:03.123' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'ppTaeuyLYVnAAdqy4WRnPFJWDlqSeO7C7eyS9CIVvfU=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (48, CAST(N'2026-09-14T19:36:56.843' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'O/V1Ccf0HQxo92ryuIpa+7nmFASvQqfluJyM8cZQMwM=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (49, CAST(N'2026-09-14T19:39:39.343' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'dbLGH/mc8OCgf6WDLbcTxaqK75l1BOnZV6ninvFYu00=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (50, CAST(N'2026-09-14T19:46:48.247' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'PIUBb7ETmUDxAgFCvDG1vdvbDD6ew/Wl452XbruwHmo=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (51, CAST(N'2026-09-14T19:57:41.710' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'i2/we5liyMFJsJTsjwD8Wle8p9IupVhrPsvYQZzTdSQ=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (52, CAST(N'2026-09-14T20:27:55.223' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'/PAtDLGeuAQMYJ9TfNzNCX18C/ZB93DxIOgncbtr7pE=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (53, CAST(N'2026-09-14T20:32:35.650' AS DateTime), N'admin', N'LoginFallido', N'1kHEXwuiqStyuAuXrsxzx1CxXjqaDN97ctFJNaR5YOKmQjeZiPp5wZAjPr5GwNA1cSuH90VRaHV6XtzJY9xEj7026M936Ey84tU56xNX+/xWlgYSjVx3ZbPX4d4CzKraDIHxcHmcHRL1o7WWgrbnOw==', N'Y1L/wKm47kbgOuFDlzKzpzCaWPKH8D4Pr8efccXZ/2c=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (54, CAST(N'2026-09-14T20:32:37.947' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'F9/5sGJiAN1f6GUcxEnido4fNxatzZNzvie0r+/TQXE=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (55, CAST(N'2026-09-14T21:11:06.067' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'Bz3HY28GYRAHcOepNPRF1WlkRKJCmNvOkszQ3+yQDqk=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (56, CAST(N'2026-09-14T22:11:38.690' AS DateTime), N'admin', N'Logout', N'7GH+Asulvm/UNP9OEgiPb/fyPPaxikzHVLiSLi4qZp8=', N'7ZaG6MSSaBdTKsIaNZHD3QgGexvaNgJHkePJYGi5Qpc=')
INSERT [dbo].[Bitacora] ([IdBitacora], [Fecha], [Usuario], [Actividad], [InformacionAsociada], [DigitoVerificador]) VALUES (57, CAST(N'2026-09-14T22:11:41.820' AS DateTime), N'admin', N'LoginExitoso', N'1kHEXwuiqStyuAuXrsxzx9E3DC4VrJb6TRuInFCy0yCPT0pXQFxf6OuA6lzP3gpXU2jW7a5c3M6dYJjeP/6ouMlEItCJx5RegDbJoQYwCHAhgRLhL66pf0d3+oaWJ2Z6', N'5HJv7Zo4GnKtVAYFBay8RL4leHowqwyp/aveNkkqb2A=')
SET IDENTITY_INSERT [dbo].[Bitacora] OFF
GO
SET IDENTITY_INSERT [dbo].[Control] ON 

INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (33, N'1', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (35, N'2', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (34, N'3', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (46, N'4', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (38, N'btn_aplicar_idioma', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (49, N'btn_aplicar_idioma', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (9, N'btn_buscar', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (61, N'btn_buscar', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (39, N'btn_cambiar_estado', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (50, N'btn_cambiar_estado', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (24, N'btn_eliminar', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (23, N'btn_guardar', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (45, N'btn_guardar_traduccion', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (56, N'btn_guardar_traduccion', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (25, N'btn_limpiar', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (44, N'btn_nuevo_idioma', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (55, N'btn_nuevo_idioma', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (22, N'chk_activo', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (13, N'chk_fechas', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (58, N'chk_fechas', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (43, N'chk_idioma_activo', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (54, N'chk_idioma_activo', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (12, N'col_actividad', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (65, N'col_actividad', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (30, N'col_activo', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (29, N'col_apellido', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (10, N'col_fecha', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (63, N'col_fecha', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (14, N'col_id', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (62, N'col_id', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (26, N'col_idusuario', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (15, N'col_informacion', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (66, N'col_informacion', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (28, N'col_nombre', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (27, N'col_nombreusuario', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (31, N'col_perfil', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (11, N'col_usuario', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (64, N'col_usuario', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (6, N'frmbitacora_titulo', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (57, N'frmbitacora_titulo', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (36, N'frmgestionidiomas_titulo', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (47, N'frmgestionidiomas_titulo', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (16, N'frmgestionusuarios_titulo', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (8, N'lbl_actividad', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (60, N'lbl_actividad', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (20, N'lbl_apellido', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (18, N'lbl_clave', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (41, N'lbl_codigo_idioma', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (52, N'lbl_codigo_idioma', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (37, N'lbl_idioma_activo', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (48, N'lbl_idioma_activo', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (19, N'lbl_nombre', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (42, N'lbl_nombre_nativo', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (53, N'lbl_nombre_nativo', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (40, N'lbl_nuevo_idioma', N'FrmGestionIdiomas')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (51, N'lbl_nuevo_idioma', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (21, N'lbl_perfil', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (7, N'lbl_usuario', N'FrmBitacora')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (59, N'lbl_usuario', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (17, N'lbl_usuario_gu', N'FrmGestionUsuarios')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (3, N'menu_bitacora', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (5, N'menu_idioma', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (4, N'menu_logout', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (1, N'menu_sistema', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (2, N'menu_usuarios', N'FrmPrincipal')
INSERT [dbo].[Control] ([Id], [Control], [Form]) VALUES (32, N'titulo_ventana', N'FrmPrincipal')
SET IDENTITY_INSERT [dbo].[Control] OFF
GO
SET IDENTITY_INSERT [dbo].[Idioma] ON 

INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (1, N'Español', N'es', N'Español', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (2, N'Portugués', N'pt', N'Português', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (3, N'Inglés', N'en', N'English', 1)
INSERT [dbo].[Idioma] ([Id], [Nombre], [Codigo], [NombreNativo], [Activo]) VALUES (4, N'Ruso', N'ru', N'Русский', 1)
SET IDENTITY_INSERT [dbo].[Idioma] OFF
GO
SET IDENTITY_INSERT [dbo].[Perfiles] ON 

INSERT [dbo].[Perfiles] ([IdPerfil], [NombrePerfil]) VALUES (1, N'Administrador')
INSERT [dbo].[Perfiles] ([IdPerfil], [NombrePerfil]) VALUES (2, N'Operador')
SET IDENTITY_INSERT [dbo].[Perfiles] OFF
GO
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 1, N'Sistema', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 2, N'Gestión de Usuarios', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 3, N'Bitácora', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 4, N'Cerrar Sesión', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 5, N'Idioma', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 6, N'Consulta de Bitácora', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 7, N'Usuario:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 8, N'Actividad:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 9, N'Buscar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 10, N'Fecha', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 11, N'Usuario', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 12, N'Actividad', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 13, N'Fechas:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 14, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 15, N'Información', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 16, N'Gestión de Usuarios', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 17, N'Usuario:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 18, N'Clave (vacío no cambia):', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 19, N'Nombre:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 20, N'Apellido:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 21, N'Perfil:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 22, N'Activo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 23, N'Guardar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 24, N'Baja', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 25, N'Limpiar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 26, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 27, N'Usuario', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 28, N'Nombre', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 29, N'Apellido', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 30, N'Activo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 31, N'Perfil', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 32, N'FrmPrincipal', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 33, N'Español', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 34, N'Inglés', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 35, N'Portugués', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 36, N'Gestión de idiomas', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 37, N'Idioma:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 38, N'Aplicar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 39, N'Activar / desactivar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 40, N'Nombre:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 41, N'Código:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 42, N'Nombre nativo:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 43, N'Activo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 44, N'Crear idioma', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 45, N'Guardar traducción', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 46, N'Ruso', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 47, N'Gestión de idiomas', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 48, N'Idioma:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 49, N'Aplicar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 50, N'Activar / desactivar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 51, N'Nombre:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 52, N'Código:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 53, N'Nombre nativo:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 54, N'Activo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 55, N'Crear idioma', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 56, N'Guardar traducción', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 57, N'Consulta de Bitácora', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 58, N'Fechas:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 59, N'Usuario:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 60, N'Actividad:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 61, N'Поиск', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 62, N'IdBitacora', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 63, N'Fecha', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 64, N'Usuario', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 65, N'Actividad', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (1, 66, N'InformacionAsociada', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 1, N'Sistema', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 2, N'Gestão de Usuários', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 3, N'Registro', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 4, N'Encerrar Sessão', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 5, N'Idioma', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 6, N'Consulta de Registro', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 7, N'Usuário:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 8, N'Atividade:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 9, N'Buscar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 10, N'Data', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 11, N'Usuário', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 12, N'Atividade', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 13, N'Datas:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 14, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 15, N'Informação', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 16, N'Gestão de Usuários', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 17, N'Usuário:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 18, N'Senha (vazio não altera):', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 19, N'Nome:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 20, N'Sobrenome:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 21, N'Perfil:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 22, N'Ativo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 23, N'Salvar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 24, N'Baixa', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 25, N'Limpar', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 26, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 27, N'Usuário', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 28, N'Nome', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 29, N'Sobrenome', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 30, N'Ativo', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 31, N'Perfil', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 32, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 33, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 34, NULL, NULL, N'Pendiente')
GO
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 35, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 36, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 37, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 38, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 39, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 40, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 41, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 42, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 43, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 44, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 45, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 46, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 47, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 48, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 49, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 50, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 51, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 52, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 53, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 54, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 55, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 56, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 57, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 58, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 59, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 60, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 61, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 62, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 63, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 64, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 65, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (2, 66, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 1, N'System', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 2, N'User Management', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 3, N'Log', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 4, N'Log Out', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 5, N'Language', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 6, N'Log Search', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 7, N'User:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 8, N'Activity:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 9, N'Search', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 10, N'Date', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 11, N'User', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 12, N'Activity', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 13, N'Dates:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 14, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 15, N'Information', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 16, N'User Management', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 17, N'User:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 18, N'Password (blank = no change):', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 19, N'Name:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 20, N'Last name:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 21, N'Role:', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 22, N'Active', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 23, N'Save', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 24, N'Deactivate', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 25, N'Clear', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 26, N'Id', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 27, N'Username', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 28, N'First name', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 29, N'Last name', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 30, N'Active', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 31, N'Role', NULL, N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 32, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 33, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 34, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 35, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 36, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 37, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 38, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 39, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 40, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 41, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 42, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 43, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 44, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 45, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 46, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 47, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 48, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 49, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 50, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 51, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 52, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 53, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 54, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 55, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 56, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 57, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 58, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 59, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 60, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 61, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 62, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 63, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 64, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 65, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (3, 66, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 1, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 2, NULL, NULL, N'Pendiente')
GO
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 3, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 4, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 5, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 6, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 7, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 8, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 9, N'Поиск', N'69886a6e56c594db63de416605a13ec12c58633c0360c70f2e8e70aacc70f8aa', N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 10, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 11, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 12, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 13, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 14, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 15, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 16, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 17, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 18, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 19, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 20, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 21, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 22, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 23, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 24, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 25, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 26, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 27, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 28, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 29, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 30, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 31, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 32, N'SIGAT', N'ff523ab7bebc6501e26209b93c8c4225c2dba59d84cb486006d0a9a5abf49b6c', N'Completa')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 33, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 34, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 35, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 36, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 37, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 38, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 39, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 40, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 41, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 42, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 43, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 44, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 45, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 46, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 47, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 48, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 49, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 50, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 51, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 52, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 53, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 54, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 55, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 56, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 57, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 58, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 59, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 60, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 61, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 62, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 63, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 64, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 65, NULL, NULL, N'Pendiente')
INSERT [dbo].[Traduccion] ([Id_Idioma], [Id_Control], [Texto], [DigitoVerificador], [Estado]) VALUES (4, 66, NULL, NULL, N'Pendiente')
GO
SET IDENTITY_INSERT [dbo].[Usuarios] ON 

INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (1, N'admin', N'8c6976e5b5410415bde908bd4dee15dfb167a9c873fc4bb8a81f6f2ab448a918', N'Admin', N'Sistema', 1, 1)
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (2, N'Bruna26', N'3daa437813cde6b8426eb033604d2b18a68d0328e328fea527bc6f9d4aa594cc', N'Bruna', N'Burgos', 1, 2)
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (3, N'dasda1', N'03ac674216f3e15c761ee1a5e255f067953623c8b388b4459e13f978d7c846f4', N'dasdas', N'adsasd', 0, 2)
INSERT [dbo].[Usuarios] ([IdUsuario], [NombreUsuario], [Password], [Nombre], [Apellido], [Activo], [IdPerfil]) VALUES (4, N'valen1', N'ebb28927f9181fb3389744d3cba34d9d3943c6fd88e73c3e9e440daeb2423108', N'valen', N'planas', 1, 2)
SET IDENTITY_INSERT [dbo].[Usuarios] OFF
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ_Control] Fecha de script: 15/9/2026 20:54:47 ******/
ALTER TABLE [dbo].[Control] ADD  CONSTRAINT [UQ_Control] UNIQUE NONCLUSTERED 
(
	[Control] ASC,
	[Form] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ__Idioma__75E3EFCF2C5203BC] Fecha de script: 15/9/2026 20:54:47 ******/
ALTER TABLE [dbo].[Idioma] ADD UNIQUE NONCLUSTERED 
(
	[Nombre] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ_Idioma_Codigo] Fecha de script: 15/9/2026 20:54:47 ******/
ALTER TABLE [dbo].[Idioma] ADD  CONSTRAINT [UQ_Idioma_Codigo] UNIQUE NONCLUSTERED 
(
	[Codigo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ__Perfiles__9383318C71528306] Fecha de script: 15/9/2026 20:54:47 ******/
ALTER TABLE [dbo].[Perfiles] ADD UNIQUE NONCLUSTERED 
(
	[NombrePerfil] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
SET ANSI_PADDING ON
GO
/****** Objeto: Index [UQ__Usuarios__6B0F5AE02F37C5B0] Fecha de script: 15/9/2026 20:54:47 ******/
ALTER TABLE [dbo].[Usuarios] ADD UNIQUE NONCLUSTERED 
(
	[NombreUsuario] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Idioma] ADD  CONSTRAINT [DF_Idioma_Activo]  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Traduccion] ADD  CONSTRAINT [DF_Traduccion_Estado]  DEFAULT ('Completa') FOR [Estado]
GO
ALTER TABLE [dbo].[Usuarios] ADD  DEFAULT ((1)) FOR [Activo]
GO
ALTER TABLE [dbo].[Traduccion]  WITH CHECK ADD  CONSTRAINT [FK_Traduccion_Control] FOREIGN KEY([Id_Control])
REFERENCES [dbo].[Control] ([Id])
GO
ALTER TABLE [dbo].[Traduccion] CHECK CONSTRAINT [FK_Traduccion_Control]
GO
ALTER TABLE [dbo].[Traduccion]  WITH CHECK ADD  CONSTRAINT [FK_Traduccion_Idioma] FOREIGN KEY([Id_Idioma])
REFERENCES [dbo].[Idioma] ([Id])
GO
ALTER TABLE [dbo].[Traduccion] CHECK CONSTRAINT [FK_Traduccion_Idioma]
GO
ALTER TABLE [dbo].[Usuarios]  WITH CHECK ADD  CONSTRAINT [FK_Usuarios_Perfiles] FOREIGN KEY([IdPerfil])
REFERENCES [dbo].[Perfiles] ([IdPerfil])
GO
ALTER TABLE [dbo].[Usuarios] CHECK CONSTRAINT [FK_Usuarios_Perfiles]
GO

USE [master]
GO
ALTER DATABASE [SIGAT] SET READ_WRITE
GO

PRINT 'Base SIGAT instalada correctamente (estructura + datos reales de producción).';
GO
