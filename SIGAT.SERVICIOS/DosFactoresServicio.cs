using System;
using System.Text;
using OtpNet;

namespace SIGAT.SERVICIOS
{
    public class DosFactoresServicio
    {
        // Generamos una clave única y repetible por usuario para no tocar la Base de Datos
        public string GenerarClaveSecreta(string nombreUsuario)
        {
            // Concatenamos el usuario con una semilla secreta del sistema
            string semilla = nombreUsuario.ToUpper() + "SIGAT2026SECRETKEY!!";
            byte[] bytes = Encoding.UTF8.GetBytes(semilla);
            return Base32Encoding.ToString(bytes);
        }

        // Genera la URI estándar que leen las apps como Authy o Google Authenticator
        public string GenerarUriAutenticador(string nombreUsuario)
        {
            string claveBase32 = GenerarClaveSecreta(nombreUsuario);
            return $"otpauth://totp/SIGAT:{nombreUsuario}?secret={claveBase32}&issuer=SIGAT";
        }

        // Valida que el código de 6 dígitos ingresado coincida con la hora actual
        public bool ValidarCodigo(string nombreUsuario, string codigoIngresado)
        {
            if (string.IsNullOrEmpty(codigoIngresado)) return false;

            string claveBase32 = GenerarClaveSecreta(nombreUsuario);
            byte[] secretBytes = Base32Encoding.ToBytes(claveBase32);

            var totp = new Totp(secretBytes);

            // VerificationWindow(2, 2) da un margen de error de +/- 1 minuto por si el celular está desincronizado
            return totp.VerifyTotp(codigoIngresado, out long timeStepMatched, new VerificationWindow(2, 2));
        }
    }
}