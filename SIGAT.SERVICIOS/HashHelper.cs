using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;


namespace SIGAT.SERVICIOS
{
    public static class HashHelper
    {
        public static string ObtenerHashSHA256(string texto)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(texto));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
