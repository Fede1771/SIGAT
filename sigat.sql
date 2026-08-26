USE SIGAT;
GO

-- 1. Tabla Perfiles (Roles)
CREATE TABLE Perfiles (
    IdPerfil INT IDENTITY(1,1) PRIMARY KEY,
    NombrePerfil VARCHAR(50) NOT NULL UNIQUE
);
GO

INSERT INTO Perfiles (NombrePerfil) VALUES ('Administrador'), ('Operador');
GO

-- 2. Relacionar Usuarios con Perfiles
ALTER TABLE Usuarios ADD IdPerfil INT NULL;
GO

ALTER TABLE Usuarios 
ADD CONSTRAINT FK_Usuarios_Perfiles FOREIGN KEY (IdPerfil) REFERENCES Perfiles(IdPerfil);
GO

UPDATE Usuarios SET IdPerfil = 1 WHERE IdPerfil IS NULL;
GO

ALTER TABLE Usuarios ALTER COLUMN IdPerfil INT NOT NULL;
GO

-- 3. Tabla Bitácora
CREATE TABLE Bitacora (
    IdBitacora INT IDENTITY(1,1) PRIMARY KEY,
    Fecha DATETIME NOT NULL,
    Usuario VARCHAR(50) NOT NULL,
    Actividad VARCHAR(255) NOT NULL,
    InformacionAsociada NVARCHAR(MAX) NULL
);
GO


ALTER TABLE Bitacora ADD DigitoVerificador NVARCHAR(250) NULL;


DELETE FROM Bitacora;

TRUNCATE TABLE Bitacora;