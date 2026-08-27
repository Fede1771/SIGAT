using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIGAT.SERVICIOS;
using System.Reflection;

namespace SIGAT.Tests
{
    [TestClass]
    public class TestsCompletos
    {
        // 1. Test de Arquitectura: Ninguna capa de negocio o datos debe conocer a la Interfaz Gráfica (UI)
        [TestMethod]
        public void Arquitectura_CapasInferioresNoReferencianUI()
        {
            // Obtenemos los archivos compilados (.dll) de cada capa
            Assembly asambleaBLL = Assembly.Load("SIGAT.BLL");
            Assembly asambleaDAL = Assembly.Load("SIGAT.DAL");
            Assembly asambleaSERV = Assembly.Load("SIGAT.SERVICIOS");
            Assembly asambleaBE = Assembly.Load("SIGAT.BE");

            // Verificamos una por una que no tengan a SIGAT.UI entre sus referencias
            Assert.IsFalse(TieneReferencia(asambleaBLL, "SIGAT.UI"), "La capa BLL no debe referenciar a la UI.");
            Assert.IsFalse(TieneReferencia(asambleaDAL, "SIGAT.UI"), "La capa DAL no debe referenciar a la UI.");
            Assert.IsFalse(TieneReferencia(asambleaSERV, "SIGAT.UI"), "La capa SERVICIOS no debe referenciar a la UI.");
            Assert.IsFalse(TieneReferencia(asambleaBE, "SIGAT.UI"), "La capa BE no debe referenciar a la UI.");
        }

        // Método auxiliar clásico para buscar referencias sin usar LINQ
        private bool TieneReferencia(Assembly asamblea, string nombreReferencia)
        {
            AssemblyName[] referencias = asamblea.GetReferencedAssemblies();

            foreach (AssemblyName refActual in referencias)
            {
                if (refActual.Name == nombreReferencia)
                {
                    return true; // Encontró la referencia prohibida
                }
            }
            return false; // Está limpia, no la encontró
        }

        // 2. Test de Patrón Singleton: Asegura que exista una única sesión en todo el sistema
        [TestMethod]
        public void SesionServicio_CumplePatronSingleton()
        {
            // Pedimos la instancia dos veces simulando dos partes distintas del sistema
            SesionServicio instancia1 = SesionServicio.ObtenerInstancia();
            SesionServicio instancia2 = SesionServicio.ObtenerInstancia();

            // Primero validamos que realmente nos haya devuelto un objeto
            Assert.IsNotNull(instancia1, "La instancia de sesión no debería ser nula.");

            // AreSame verifica que ambas variables apunten exactamente al mismo espacio en la memoria
            Assert.AreSame(instancia1, instancia2, "El patrón Singleton falló: se crearon dos objetos distintos en memoria.");
        }

        // 3. Test de HashHelper (SHA-256): Verifica la encriptación de una sola vía (Irreversible)
        [TestMethod]
        public void HashHelper_ValidaConsistenciaYDiferencia()
        {
            string pass1 = "123456";
            string pass2 = "abcdef";

            string hash1_intento1 = HashHelper.ObtenerHashSHA256(pass1);
            string hash1_intento2 = HashHelper.ObtenerHashSHA256(pass1);
            string hash2 = HashHelper.ObtenerHashSHA256(pass2);

            // Regla 1: La misma contraseña siempre debe generar el mismo hash
            Assert.AreEqual(hash1_intento1, hash1_intento2, "El mismo texto debe generar el mismo hash.");

            // Regla 2: Contraseñas distintas deben generar hashes totalmente distintos (Evitar colisiones)
            Assert.AreNotEqual(hash1_intento1, hash2, "Textos distintos deben generar hashes distintos.");
        }

        // 4. Test de Encriptado AES: Verifica la criptografía de dos vías (Reversible)
        [TestMethod]
        public void EncriptadorServicio_EncriptaYDesencriptaCorrectamente()
        {
            EncriptadorServicio encriptador = new EncriptadorServicio();
            string textoOriginal = "Dato sensible de prueba";

            // Encriptamos
            string textoEncriptado = encriptador.Encriptar(textoOriginal);

            // Desencriptamos el resultado del paso anterior
            string textoDesencriptado = encriptador.Desencriptar(textoEncriptado);

            // Validamos que la encriptación haya ocultado el texto original
            Assert.AreNotEqual(textoOriginal, textoEncriptado, "El texto encriptado no debería ser igual al original.");

            // Validamos que el proceso inverso recupere el texto original intacto
            Assert.AreEqual(textoOriginal, textoDesencriptado, "El texto desencriptado debe coincidir exactamente con el original.");
        }
    }
}