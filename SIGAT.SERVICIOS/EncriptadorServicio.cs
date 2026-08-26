using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace SIGAT.SERVICIOS
{
    public class EncriptadorServicio
    {
        private readonly byte[] _key = SHA256.HashData(Encoding.UTF8.GetBytes("ClaveSecretaSIGAT2026"));
        private readonly byte[] _iv = MD5.HashData(Encoding.UTF8.GetBytes("VectorSIGAT2026"));

        public string? Encriptar(string? textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano)) return null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(textoPlano);
                        }
                        return Convert.ToBase64String(msEncrypt.ToArray());
                    }
                }
            }
        }

        public string? Desencriptar(string? textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado)) return null;

            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = _key;
                aesAlg.IV = _iv;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);
                byte[] buffer = Convert.FromBase64String(textoEncriptado);

                using (MemoryStream msDecrypt = new MemoryStream(buffer))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            return srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
        }

        public string? CalcularDV(string? cadenaConcatenada)
        {
            if (string.IsNullOrEmpty(cadenaConcatenada)) return null;
            byte[] bytesIn = Encoding.UTF8.GetBytes(cadenaConcatenada);
            byte[] bytesOut = SHA256.HashData(bytesIn);
            return Convert.ToBase64String(bytesOut);
        }
    }
}