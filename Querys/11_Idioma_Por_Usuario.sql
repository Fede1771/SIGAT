-- Guardar la elección de idioma de cada cuenta, entre sesiones y equipos.
-- Ejecutar una vez sobre una base actual; se puede repetir sin borrar preferencias.
USE [SIGAT];
SET XACT_ABORT ON;
IF OBJECT_ID('dbo.UsuarioIdioma','U') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioIdioma (
        IdUsuario int NOT NULL CONSTRAINT PK_UsuarioIdioma PRIMARY KEY,
        IdIdioma int NOT NULL,
        CONSTRAINT FK_UsuarioIdioma_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(IdUsuario),
        CONSTRAINT FK_UsuarioIdioma_Idioma FOREIGN KEY (IdIdioma) REFERENCES dbo.Idioma(Id)
    );
END;
GO
