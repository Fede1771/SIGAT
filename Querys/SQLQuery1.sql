USE SIGAT;
GO

-- 1. Buscar y destruir el Constraint (el ancla) de DosFactorActivo
DECLARE @ConstraintName NVARCHAR(200);
SELECT @ConstraintName = d.name
FROM sys.default_constraints d
INNER JOIN sys.columns c ON d.parent_object_id = c.object_id AND d.parent_column_id = c.column_id
WHERE d.parent_object_id = OBJECT_ID('Usuarios') AND c.name = 'DosFactorActivo';

IF @ConstraintName IS NOT NULL
BEGIN
    EXEC('ALTER TABLE Usuarios DROP CONSTRAINT ' + @ConstraintName);
END
GO

-- 2. Ahora sí, eliminar la columna libremente
IF COL_LENGTH('dbo.Usuarios', 'DosFactorActivo') IS NOT NULL
BEGIN
    ALTER TABLE Usuarios DROP COLUMN DosFactorActivo;
END
GO

-- 3. Por las dudas, eliminamos también la SecretKey si quedó dando vueltas
IF COL_LENGTH('dbo.Usuarios', 'SecretKey2FA') IS NOT NULL
BEGIN
    ALTER TABLE Usuarios DROP COLUMN SecretKey2FA;
END
GO