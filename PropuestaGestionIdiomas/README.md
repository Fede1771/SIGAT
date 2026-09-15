# Reemplazos completos: gestión de idiomas

Estos archivos no modifican el proyecto activo. Copiá el contenido de cada archivo al archivo del mismo nombre dentro del proyecto.

Orden:
1. Ya ejecutaste `10_Mejora_Idiomas.sql`.
2. Reemplazá `SIGAT.BE/Idiomas/Idioma.cs` y `Traduccion.cs`.
3. Reemplazá `SIGAT.DAL/IdiomaDAL.cs`.
4. Reemplazá `SIGAT.BLL/IdiomaBLL.cs`.
5. Reemplazá los tres archivos de `SIGAT.UI`.
6. Compilá y probá crear `Ruso / ru / Русский`.

Los archivos fueron escritos para la base ya migrada: `Idioma` tiene Codigo, NombreNativo y Activo; `Traduccion` tiene Estado y permite Texto nulo.
