using System.Security.Cryptography;
using System.Text;

namespace SIGAT.SERVICIOS
{
    public class EncriptadorServicio
    {
        // AES permite recuperar el detalle de la bitácora. Esta implementación usa una clave y un vector fijos.
        private readonly byte[] _key = SHA256.HashData(Encoding.UTF8.GetBytes("ClaveSecretaSIGAT2026"));
        private readonly byte[] _iv = MD5.HashData(Encoding.UTF8.GetBytes("VectorSIGAT2026"));

        public string? Encriptar(string? textoPlano)
        {
            if (string.IsNullOrEmpty(textoPlano))
            {
                return null;
            }

            using (Aes algoritmoAes = Aes.Create())
            {
                algoritmoAes.Key = _key;
                algoritmoAes.IV = _iv;

                ICryptoTransform encriptador = algoritmoAes.CreateEncryptor(algoritmoAes.Key, algoritmoAes.IV);

                // Cadena de flujos: Memoria -> Criptografía -> Escritura de texto
                using (MemoryStream flujoMemoria = new MemoryStream())
                {
                    using (CryptoStream flujoCripto = new CryptoStream(flujoMemoria, encriptador, CryptoStreamMode.Write))
                    {
                        using (StreamWriter escritor = new StreamWriter(flujoCripto))
                        {
                            escritor.Write(textoPlano);
                        }

                        // Convertimos los bytes encriptados a texto en Base64 para guardarlo en SQL
                        return Convert.ToBase64String(flujoMemoria.ToArray());
                    }
                }
            }
        }

        public string? Desencriptar(string? textoEncriptado)
        {
            if (string.IsNullOrEmpty(textoEncriptado))
            {
                return null;
            }

            using (Aes algoritmoAes = Aes.Create())
            {
                algoritmoAes.Key = _key;
                algoritmoAes.IV = _iv;

                ICryptoTransform desencriptador = algoritmoAes.CreateDecryptor(algoritmoAes.Key, algoritmoAes.IV);

                // Convertimos el texto Base64 de la base de datos a bytes nuevamente
                byte[] buffer = Convert.FromBase64String(textoEncriptado);

                // Cadena de flujos inversa: Memoria -> Criptografía -> Lectura de texto
                using (MemoryStream flujoMemoria = new MemoryStream(buffer))
                {
                    using (CryptoStream flujoCripto = new CryptoStream(flujoMemoria, desencriptador, CryptoStreamMode.Read))
                    {
                        using (StreamReader lector = new StreamReader(flujoCripto))
                        {
                            return lector.ReadToEnd();
                        }
                    }
                }
            }
        }

        // Verificador del formato legado: calculamos SHA-256 sobre la cadena recibida para comprobar datos anteriores.
        public string? CalcularDV(string? cadenaConcatenada)
        {
            if (string.IsNullOrEmpty(cadenaConcatenada))
            {
                return null;
            }

            byte[] bytesEntrada = Encoding.UTF8.GetBytes(cadenaConcatenada);
            byte[] bytesSalida = SHA256.HashData(bytesEntrada);

            return Convert.ToBase64String(bytesSalida);
        }
    }
}