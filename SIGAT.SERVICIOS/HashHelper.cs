using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace SIGAT.SERVICIOS
{
    public static class HashHelper
    {
        // Función de Hash (Una vía): Transforma la clave en una cadena irreversible
        public static string ObtenerHashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                // 1. Convertimos el texto a un arreglo de bytes
                byte[] bytesOriginales = Encoding.UTF8.GetBytes(texto);

                // 2. Calculamos el hash criptográfico
                byte[] bytesHasheados = sha256.ComputeHash(bytesOriginales);

                // 3. Convertimos los bytes devueltos a un texto legible en formato Hexadecimal (x2)
                StringBuilder constructorTexto = new StringBuilder();
                foreach (byte b in bytesHasheados)
                {
                    constructorTexto.Append(b.ToString("x2"));
                }

                return constructorTexto.ToString();
            }
        }
    }
}