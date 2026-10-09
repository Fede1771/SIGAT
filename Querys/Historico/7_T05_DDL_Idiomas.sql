/* =====================================================================
   T05 - GESTIÓN DE MÚLTIPLES IDIOMAS (SIGAT)
   Script DDL - SQL Server
   ===================================================================== */

USE SIGAT;
GO

-- ---------------------------------------------------------------------
-- Tabla: Idioma
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Idioma', 'U') IS NOT NULL DROP TABLE dbo.Idioma;
GO
CREATE TABLE dbo.Idioma
(
    Id      INT IDENTITY(1,1) NOT NULL,
    Nombre  VARCHAR(50)       NOT NULL,

    CONSTRAINT PK_Idioma PRIMARY KEY (Id),
    CONSTRAINT UQ_Idioma_Nombre UNIQUE (Nombre)
);
GO

-- ---------------------------------------------------------------------
-- Tabla: Control
-- Representa cada control traducible de cada pantalla del sistema.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Control', 'U') IS NOT NULL DROP TABLE dbo.Control;
GO
CREATE TABLE dbo.Control
(
    Id       INT IDENTITY(1,1) NOT NULL,
    Control  VARCHAR(100)      NOT NULL,   -- Nombre/Tag representativo (ej: "btn_login")
    Form     VARCHAR(100)      NOT NULL,   -- Pantalla a la que pertenece (ej: "frmConsultaBitacora")

    CONSTRAINT PK_Control PRIMARY KEY (Id),
    -- Un mismo nombre de control sólo puede existir una vez por formulario
    CONSTRAINT UQ_Control_Control_Form UNIQUE (Control, Form)
);
GO

-- ---------------------------------------------------------------------
-- Tabla: Traduccion (tabla intermedia asociativa Idioma <-> Control)
-- Incluye DigitoVerificador (DVH) para garantizar integridad.
-- ---------------------------------------------------------------------
IF OBJECT_ID('dbo.Traduccion', 'U') IS NOT NULL DROP TABLE dbo.Traduccion;
GO
CREATE TABLE dbo.Traduccion
(
    Id_Idioma          INT             NOT NULL,
    Id_Control         INT             NOT NULL,
    Texto              NVARCHAR(500)   NOT NULL,   -- NVARCHAR: soporta cirílico, chino, árabe, etc.
    DigitoVerificador  CHAR(64)        NULL,        -- SHA-256 en hexadecimal (Id_Idioma|Id_Control|Texto)

    CONSTRAINT PK_Traduccion PRIMARY KEY (Id_Idioma, Id_Control),

    CONSTRAINT FK_Traduccion_Idioma FOREIGN KEY (Id_Idioma)
        REFERENCES dbo.Idioma (Id)
        ON DELETE CASCADE,

    CONSTRAINT FK_Traduccion_Control FOREIGN KEY (Id_Control)
        REFERENCES dbo.Control (Id)
        ON DELETE CASCADE
);
GO

-- Índices de apoyo para las dos formas más frecuentes de consulta:
-- "dame todas las traducciones de este idioma" (carga de caché al cambiar idioma)
-- y "dame los controles de este formulario" (armado del ABM).
CREATE NONCLUSTERED INDEX IX_Traduccion_Id_Idioma ON dbo.Traduccion (Id_Idioma);
GO
CREATE NONCLUSTERED INDEX IX_Control_Form ON dbo.Control (Form);
GO
