SELECT Id, Nombre, Codigo, NombreNativo, Activo
FROM Idioma;

SELECT Estado, COUNT(*) AS Cantidad
FROM Traduccion
GROUP BY Estado;