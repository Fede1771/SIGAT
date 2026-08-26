using Microsoft.VisualStudio.TestTools.UnitTesting;
using SIGAT.SERVICIOS;
using System.Reflection;
using System.Linq;

namespace SIGAT.Tests
{
    [TestClass]
    public class TestsCompletos
    {
        // 1. Test de Arquitectura: ninguna capa inferior debe referenciar a UI
        [TestMethod]
        public void Arquitectura_CapasInferioresNoReferencianUI()
        {
            var asamblesBLL = Assembly.Load("SIGAT.BLL").GetReferencedAssemblies();
            var asamblesDAL = Assembly.Load("SIGAT.DAL").GetReferencedAssemblies();
            var asamblesSERV = Assembly.Load("SIGAT.SERVICIOS").GetReferencedAssemblies();
            var asamblesBE = Assembly.Load("SIGAT.BE").GetReferencedAssemblies();

            Assert.IsFalse(asamblesBLL.Any(a => a.Name == "SIGAT.UI"));
            Assert.IsFalse(asamblesDAL.Any(a => a.Name == "SIGAT.UI"));
            Assert.IsFalse(asamblesSERV.Any(a => a.Name == "SIGAT.UI"));
            Assert.IsFalse(asamblesBE.Any(a => a.Name == "SIGAT.UI"));
        }

        // 2. Test de Patrón Singleton: SesionServicio
        [TestMethod]
        public void SesionServicio_CumplePatronSingleton()
        {
            var instancia1 = SesionServicio.ObtenerInstancia();
            var instancia2 = SesionServicio.ObtenerInstancia();

            Assert.IsNotNull(instancia1);
            Assert.AreSame(instancia1, instancia2, "El patrón Singleton falló: se generaron múltiples instancias.");
        }

        // 3. Test de HashHelper (SHA-256)
        [TestMethod]
        public void HashHelper_ValidaConsistenciaYDiferencia()
        {
            string pass1 = "123456";
            string pass2 = "abcdef";

            string hash1_intento1 = HashHelper.ObtenerHashSHA256(pass1);
            string hash1_intento2 = HashHelper.ObtenerHashSHA256(pass1);
            string hash2 = HashHelper.ObtenerHashSHA256(pass2);

            Assert.AreEqual(hash1_intento1, hash1_intento2, "El mismo texto debe generar el mismo hash.");
            Assert.AreNotEqual(hash1_intento1, hash2, "Textos distintos deben generar hashes distintos.");
        }

        // 4. Test de Encriptado AES: lo que se encripta se puede desencriptar igual
        [TestMethod]
        public void EncriptadorServicio_EncriptaYDesencriptaCorrectamente()
        {
            var encriptador = new EncriptadorServicio();
            string original = "Dato sensible de prueba";

            string encriptado = encriptador.Encriptar(original);
            string desencriptado = encriptador.Desencriptar(encriptado);

            Assert.AreNotEqual(original, encriptado, "El texto encriptado no debería ser igual al original.");
            Assert.AreEqual(original, desencriptado, "El texto desencriptado debe coincidir con el original.");
        }
    }
}