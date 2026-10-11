/* SIGAT - ACTUALIZAR UNA BASE EXISTENTE
   Reemplaza la secuencia 10 -> 12 -> 13 -> 14 del esquema anterior.
   Hacer un backup y cerrar SIGAT antes de ejecutar TODO el archivo.
   Conserva usuarios, contraseñas, bitácora, idiomas y traducciones.
   Conserva roles por nombre al migrar el esquema antiguo. Un perfil Operador
   no se convierte automáticamente en otro rol: asignarlo desde la aplicación.
   Requiere administrador original activo, ID 1, con rol/perfil Administrador.
   Sobre el esquema actual conserva roles personalizados y asignaciones.
   Todas las etapas se confirman juntas o se revierten si alguna falla.
*/
USE [master];
SET NOCOUNT ON;
SET XACT_ABORT ON;
DECLARE @Base sysname = N'SIGAT';
IF @@TRANCOUNT <> 0
    THROW 50001, 'Ejecute la actualización fuera de una transacción abierta.', 1;
IF DB_ID(@Base) IS NULL
    THROW 50001, 'La base no existe. Use 01_Instalacion_Completa.sql.', 1;
BEGIN TRY
    DECLARE @Sql nvarchar(max) = N'USE ' + QUOTENAME(@Base) + N'; ' +
N'SET ANSI_NULLS ON; SET QUOTED_IDENTIFIER ON;
BEGIN TRANSACTION;
EXEC(N''SET XACT_ABORT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
-- Esquema 2 ya instalado: no reponer asignaciones retiradas.
DECLARE @Instalado bit = 0;
IF COL_LENGTH(''''dbo.SeguridadVersion'''',''''Esquema'''') IS NOT NULL
 EXEC sp_executesql N''''SELECT @Valor=1 FROM dbo.SeguridadVersion WHERE Id=1 AND Esquema=2'''',
 N''''@Valor bit OUTPUT'''', @Instalado OUTPUT;
IF @Instalado=1
BEGIN
 PRINT N''''La matriz ya está instalada.'''';
 RETURN;
END;

DECLARE @Version bigint = 1;
CREATE TABLE #Asignaciones (IdUsuario int NOT NULL, NombreRol nvarchar(150) NOT NULL);
IF OBJECT_ID(''''dbo.SeguridadVersion'''',''''U'''') IS NOT NULL
 EXEC sp_executesql N''''SELECT @Valor=Version+1 FROM dbo.SeguridadVersion WITH (UPDLOCK,HOLDLOCK) WHERE Id=1'''',
 N''''@Valor bigint OUTPUT'''', @Version OUTPUT;

-- Solo conservar roles que coincidan por nombre, nunca por los IDs anteriores.
IF COL_LENGTH(''''dbo.Rol'''',''''IdFamilia'''') IS NOT NULL
 EXEC(N''''INSERT #Asignaciones SELECT ur.IdUsuario,p.Nombre
 FROM dbo.UsuarioRol ur JOIN dbo.Rol r ON r.Id=ur.IdRol
 JOIN dbo.Permiso p ON p.Id=r.IdFamilia WHERE r.IdUsuarioPersonal IS NULL'''');

-- Preservar administradores existentes para evitar perder el acceso.
IF COL_LENGTH(''''dbo.Usuarios'''',''''IdPerfil'''') IS NOT NULL
 EXEC(N''''INSERT #Asignaciones
 SELECT u.IdUsuario,N''''''''Administrador'''''''' FROM dbo.Usuarios u
 JOIN dbo.Perfiles p ON p.IdPerfil=u.IdPerfil
 WHERE p.NombrePerfil=''''''''Administrador''''''''
 AND NOT EXISTS (SELECT 1 FROM #Asignaciones a WHERE a.IdUsuario=u.IdUsuario AND a.NombreRol=N''''''''Administrador'''''''')'''');
IF NOT EXISTS (SELECT 1 FROM #Asignaciones a JOIN dbo.Usuarios u ON u.IdUsuario=a.IdUsuario
 WHERE u.Activo=1 AND a.NombreRol=N''''Administrador'''')
 THROW 50001, ''''Debe existir un administrador activo antes de migrar.'''', 1;

-- Informar qué asignaciones desaparecen, sin convertirlas en otro rol.
SELECT IdUsuario,NombreRol AS RolRetirado FROM #Asignaciones
WHERE NombreRol NOT IN (N''''Administrador'''',N''''Responsable de Inventario'''',
 N''''Técnico de Mantenimiento'''',N''''Mesa de Ayuda'''',N''''Auditor'''');

DROP TABLE IF EXISTS dbo.UsuarioRol;
DROP TABLE IF EXISTS dbo.RolPermiso;
DROP TABLE IF EXISTS dbo.Rol;
DROP TABLE IF EXISTS dbo.PermisoHijo;
DROP TABLE IF EXISTS dbo.Permiso;
DROP TABLE IF EXISTS dbo.SeguridadVersion;

EXEC(N''''
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
 (Id=1 AND Nombre=N''''''''Ver bitácora'''''''' AND EsCompuesto=0) OR
 (Id=2 AND Nombre=N''''''''Consultar inventario'''''''' AND EsCompuesto=0) OR
 (Id=3 AND Nombre=N''''''''Gestión de activos'''''''' AND EsCompuesto=1) OR
 (Id=4 AND Nombre=N''''''''Movimientos'''''''' AND EsCompuesto=1) OR
 (Id=5 AND Nombre=N''''''''Mantenimiento'''''''' AND EsCompuesto=1) OR
 (Id=6 AND Nombre=N''''''''Reportes'''''''' AND EsCompuesto=1) OR
 (Id=7 AND Nombre=N''''''''Administración del sistema'''''''' AND EsCompuesto=1))
);
CREATE TABLE dbo.Rol (
 Id int NOT NULL PRIMARY KEY,
 Nombre nvarchar(150) NOT NULL UNIQUE,
 CONSTRAINT CK_Rol_Matriz CHECK (
 (Id=1 AND Nombre=N''''''''Administrador'''''''') OR
 (Id=2 AND Nombre=N''''''''Responsable de Inventario'''''''') OR
 (Id=3 AND Nombre=N''''''''Técnico de Mantenimiento'''''''') OR
 (Id=4 AND Nombre=N''''''''Mesa de Ayuda'''''''') OR
 (Id=5 AND Nombre=N''''''''Auditor''''''''))
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
 (1,N''''''''Ver bitácora'''''''',0),(2,N''''''''Consultar inventario'''''''',0),
 (3,N''''''''Gestión de activos'''''''',1),(4,N''''''''Movimientos'''''''',1),
 (5,N''''''''Mantenimiento'''''''',1),(6,N''''''''Reportes'''''''',1),
 (7,N''''''''Administración del sistema'''''''',1);
INSERT dbo.Rol VALUES
 (1,N''''''''Administrador''''''''),(2,N''''''''Responsable de Inventario''''''''),
 (3,N''''''''Técnico de Mantenimiento''''''''),(4,N''''''''Mesa de Ayuda''''''''),(5,N''''''''Auditor'''''''');
INSERT dbo.RolPermiso VALUES
 (1,1),(1,3),(1,4),(1,5),(1,6),(1,7),
 (2,3),(2,4),(2,6),(3,5),(3,2),(4,4),(4,2),(5,1),(5,2),(5,6);
'''');
EXEC sp_executesql N''''INSERT dbo.SeguridadVersion VALUES (1,@Valor,2)'''', N''''@Valor bigint'''', @Version;
EXEC(N''''INSERT dbo.UsuarioRol (IdUsuario,IdRol)
 SELECT DISTINCT a.IdUsuario,r.Id FROM #Asignaciones a
 JOIN dbo.Rol r ON r.Nombre=a.NombreRol'''');

PRINT N''''Matriz instalada: 5 roles, 7 permisos y 16 relaciones.'''';'');
EXEC(N''SET XACT_ABORT ON;

IF OBJECT_ID(''''dbo.RolPermiso'''',''''U'''') IS NULL
    THROW 50001, ''''Instale primero la matriz de roles (script 10).'''', 1;

-- No toca cuentas, contraseñas ni asignaciones de roles.
IF COL_LENGTH(''''dbo.Usuarios'''',''''IdPerfil'''') IS NOT NULL
BEGIN
    DECLARE @Sql nvarchar(max)=N'''''''';
    SELECT @Sql=@Sql+N''''ALTER TABLE dbo.Usuarios DROP CONSTRAINT ''''+QUOTENAME(fk.name)+N'''';''''
    FROM sys.foreign_keys fk
    JOIN sys.foreign_key_columns fc ON fc.constraint_object_id=fk.object_id
    JOIN sys.columns c ON c.object_id=fc.parent_object_id AND c.column_id=fc.parent_column_id
    WHERE fk.parent_object_id=OBJECT_ID(''''dbo.Usuarios'''') AND c.name=''''IdPerfil'''';
    SELECT @Sql=@Sql+N''''ALTER TABLE dbo.Usuarios DROP CONSTRAINT ''''+QUOTENAME(dc.name)+N'''';''''
    FROM sys.default_constraints dc
    JOIN sys.columns c ON c.object_id=dc.parent_object_id AND c.column_id=dc.parent_column_id
    WHERE dc.parent_object_id=OBJECT_ID(''''dbo.Usuarios'''') AND c.name=''''IdPerfil'''';
    EXEC sp_executesql @Sql;
    ALTER TABLE dbo.Usuarios DROP COLUMN IdPerfil;
END;
DROP TABLE IF EXISTS dbo.Perfiles;'');
EXEC(N''-- Ejecutar sobre SIGAT después de los scripts 10 y 12.
SET XACT_ABORT ON;

IF OBJECT_ID(''''dbo.AdministradorOriginal'''',''''U'''') IS NULL
BEGIN
    -- La instalación original crea la cuenta admin con ID 1. No adivinar otra cuenta.
    IF NOT EXISTS (SELECT 1 FROM dbo.Usuarios u JOIN dbo.UsuarioRol ur ON ur.IdUsuario=u.IdUsuario
                   WHERE u.IdUsuario=1 AND u.Activo=1 AND ur.IdRol=1)
        THROW 50001, ''''No se encontró la cuenta original activa (ID 1) con rol Administrador. Revisar antes de migrar.'''', 1;
    CREATE TABLE dbo.AdministradorOriginal (
        Id int NOT NULL PRIMARY KEY CHECK (Id=1),
        IdUsuario int NOT NULL UNIQUE REFERENCES dbo.Usuarios(IdUsuario)
    );
    INSERT dbo.AdministradorOriginal VALUES (1,1);
END;
IF OBJECT_ID(''''dbo.RolHijo'''',''''U'''') IS NULL
    CREATE TABLE dbo.RolHijo (
        IdPadre int NOT NULL REFERENCES dbo.Rol(Id),
        IdHijo int NOT NULL REFERENCES dbo.Rol(Id),
        PRIMARY KEY (IdPadre,IdHijo), CHECK (IdPadre<>IdHijo)
    );
IF OBJECT_ID(''''dbo.CK_Rol_Matriz'''',''''C'''') IS NOT NULL ALTER TABLE dbo.Rol DROP CONSTRAINT CK_Rol_Matriz;
IF OBJECT_ID(''''dbo.CK_RolPermiso_Matriz'''',''''C'''') IS NOT NULL ALTER TABLE dbo.RolPermiso DROP CONSTRAINT CK_RolPermiso_Matriz;
UPDATE dbo.SeguridadVersion SET Version=Version+1 WHERE Id=1;'');
EXEC(N''CREATE OR ALTER TRIGGER dbo.TR_Usuarios_AdministradorOriginal ON dbo.Usuarios
AFTER UPDATE, DELETE AS
BEGIN
    SET NOCOUNT ON;
    IF EXISTS (SELECT 1 FROM deleted d JOIN dbo.AdministradorOriginal a ON a.IdUsuario=d.IdUsuario
               LEFT JOIN inserted i ON i.IdUsuario=d.IdUsuario WHERE i.IdUsuario IS NULL OR i.Activo=0)
        THROW 50001, ''''No se puede eliminar ni desactivar al administrador original.'''', 1;
END;'');
EXEC(N''-- Ejecutar sobre SIGAT después del script 13. Conserva las asignaciones existentes.
SET XACT_ABORT ON;

IF OBJECT_ID(''''dbo.RolHijo'''',''''U'''') IS NULL
    THROW 50001, ''''Ejecute primero el script 13.'''', 1;
IF OBJECT_ID(''''dbo.PermisoHijo'''',''''U'''') IS NULL
BEGIN
    CREATE TABLE dbo.PermisoHijo (
        IdPadre int NOT NULL REFERENCES dbo.Permiso(Id),
        IdHijo int NOT NULL REFERENCES dbo.Permiso(Id),
        PRIMARY KEY (IdPadre,IdHijo),
        CHECK (IdPadre IN (3,4,5,6,7) AND IdPadre<>IdHijo)
    );
    UPDATE dbo.SeguridadVersion SET Version=Version+1 WHERE Id=1;
END;'');
IF OBJECT_ID(''dbo.UsuarioIdioma'',''U'') IS NULL
BEGIN
    CREATE TABLE dbo.UsuarioIdioma (
        IdUsuario int NOT NULL CONSTRAINT PK_UsuarioIdioma PRIMARY KEY,
        IdIdioma int NOT NULL,
        CONSTRAINT FK_UsuarioIdioma_Usuario FOREIGN KEY (IdUsuario) REFERENCES dbo.Usuarios(IdUsuario),
        CONSTRAINT FK_UsuarioIdioma_Idioma FOREIGN KEY (IdIdioma) REFERENCES dbo.Idioma(Id)
    );
END;
IF OBJECT_ID(''dbo.IntegridadTabla'',''U'') IS NULL
    CREATE TABLE dbo.IntegridadTabla (
        Tabla nvarchar(30) NOT NULL PRIMARY KEY,HashVertical char(64) NOT NULL,Cantidad bigint NOT NULL,
        IdBase uniqueidentifier NOT NULL,Version int NOT NULL CHECK(Version=1)
    );
COMMIT TRANSACTION;';
    EXEC sys.sp_executesql @Sql;
    PRINT N'Actualización completa finalizada. Revise las cuentas sin rol con 03_Consultar_Usuarios.sql.';
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0 ROLLBACK TRANSACTION;
    THROW;
END CATCH;
GO
