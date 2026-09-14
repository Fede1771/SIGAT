USE SIGAT;
GO

CREATE TABLE Idioma
(
    Id      INT IDENTITY(1,1) PRIMARY KEY,
    Nombre  VARCHAR(50) NOT NULL UNIQUE
);
GO

CREATE TABLE Control
(
    Id       INT IDENTITY(1,1) PRIMARY KEY,
    Control  VARCHAR(100) NOT NULL,
    Form     VARCHAR(100) NOT NULL,
    CONSTRAINT UQ_Control UNIQUE (Control, Form)
);
GO

CREATE TABLE Traduccion
(
    Id_Idioma          INT NOT NULL,
    Id_Control         INT NOT NULL,
    Texto              NVARCHAR(500) NOT NULL,
    DigitoVerificador  CHAR(64) NULL,
    PRIMARY KEY (Id_Idioma, Id_Control),
    CONSTRAINT FK_Traduccion_Idioma FOREIGN KEY (Id_Idioma) REFERENCES Idioma(Id),
    CONSTRAINT FK_Traduccion_Control FOREIGN KEY (Id_Control) REFERENCES Control(Id)
);
GO