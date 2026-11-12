using Microsoft.Data.SqlClient;
using SIGAT.BE;
using SIGAT.BE.Idiomas;
using SIGAT.BLL;
using SIGAT.DAL;
using SIGAT.SERVICIOS.Idiomas;
using SIGAT.UI;
using System.Windows.Forms;

namespace SIGAT.Tests;

[TestClass]
[DoNotParallelize]
public class BitacoraFiltrosTests
{
    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void TodasLasActividadesCombinanUsuariosLiteralesYFechasSinMezclarEventos()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_BITACORA_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        if (!new SqlConnectionStringBuilder(cadena).InitialCatalog.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        string prefijo = "revision_" + Guid.NewGuid().ToString("N")[..10];
        string ana = prefijo + "_a%[1]~", luis = prefijo + "_aXYZ1~";
        var servicio = new BitacoraBLL(new BitacoraDAL(cadena));
        var dal = new BitacoraDAL(cadena);
        var dia = new DateTime(2025, 7, 20);
        var grupos = new (string Codigo, string[] Variantes)[]
        {
            ("LoginExitoso", new[] { "LoginExitoso", "Login Exitoso" }),
            ("LoginFallido", new[] { "LoginFallido" }),
            ("AltaUsuario", new[] { "AltaUsuario" }),
            ("ModificacionUsuario", new[] { "ModificacionUsuario" }),
            ("BajaUsuario", new[] { "BajaUsuario" }),
            ("Logout", new[] { "Logout" }),
            ("ErrorCritico", new[] { "ErrorCritico", "Error Crítico del Sistema" }),
            ("RecalculoIntegridad", new[] { "RecalculoIntegridad" }),
            ("Personalizada_100%", new[] { "Personalizada_100%" }),
            ("LoginExitosoExtendido", new[] { "LoginExitosoExtendido" })
        };
        var registros = new List<(string Grupo, Bitacora Evento)>();
        try
        {
            foreach (var grupo in grupos)
                foreach (string variante in grupo.Variantes)
                    foreach (var (usuario, fecha) in new[]
                    {
                        (ana, dia), (luis, dia.AddHours(12)),
                        (ana, dia.AddDays(1).AddMilliseconds(-3)), (ana, dia.AddDays(1))
                    })
                    {
                        var evento = new Bitacora { Usuario = usuario, Fecha = fecha, Actividad = variante, InformacionAsociada = "Ensayo de filtros" };
                        dal.Insertar(evento);
                        registros.Add((grupo.Codigo, evento));
                    }

            foreach (var grupo in grupos)
                foreach (string variante in grupo.Variantes)
                    foreach (string? usuario in new string?[] { ana, luis, prefijo, null })
                        foreach (bool fechas in new[] { false, true })
                        {
                            var esperados = registros.Where(r => r.Grupo == grupo.Codigo &&
                                (usuario == null || r.Evento.Usuario!.Contains(usuario, StringComparison.Ordinal)) &&
                                (!fechas || r.Evento.Fecha.Date == dia)).Select(r => r.Evento.IdBitacora).ToArray();
                            var encontrados = servicio.Buscar(fechas ? dia : null,
                                fechas ? dia.AddDays(1).AddMilliseconds(-3) : null, usuario, variante, actividadExacta: true);
                            CollectionAssert.AreEquivalent(esperados, encontrados.Select(r => r.IdBitacora).ToArray(),
                                $"Actividad: {variante}; usuario: {usuario}; fechas: {fechas}");
                        }
            var deAna = registros.Where(r => r.Evento.Usuario == ana).Select(r => r.Evento.IdBitacora).ToArray();
            CollectionAssert.AreEquivalent(deAna, servicio.Buscar(null, null, "  " + ana + "  ", null, true).Select(r => r.IdBitacora).ToArray());
            CollectionAssert.AreEquivalent(deAna, servicio.Buscar(null, null, "%[1]~", null, true).Select(r => r.IdBitacora).ToArray());
            Assert.HasCount(registros.Count, servicio.Buscar(null, null, "   ", null, true));
            Assert.IsEmpty(servicio.Buscar(dia.AddDays(1), dia, prefijo, null, true));
            foreach (var grupo in grupos)
                Assert.HasCount(1, servicio.ObtenerActividades().Where(a => a == grupo.Codigo).ToList());
            var guardados = servicio.Buscar(null, null, prefijo, null, true);
            CollectionAssert.AreEquivalent(registros.Select(r => r.Evento.IdBitacora).ToArray(), guardados.Select(r => r.IdBitacora).ToArray());
            Assert.IsTrue(guardados.All(r => r.InformacionAsociada == "Ensayo de filtros"));
        }
        finally
        {
            using var conexion = new SqlConnection(cadena); conexion.Open();
            using var limpiar = new SqlCommand("DELETE FROM dbo.Bitacora WHERE Usuario IN (@Ana,@Luis)", conexion);
            limpiar.Parameters.AddWithValue("@Ana", ana);
            limpiar.Parameters.AddWithValue("@Luis", luis);
            limpiar.ExecuteNonQuery();
        }
    }

    [TestMethod]
    [TestCategory("SQLIntegration")]
    public void DesplegableCombinaActividadExactaUsuarioYFechasSinModificarRegistros()
    {
        string? cadena = Environment.GetEnvironmentVariable("SIGAT_BITACORA_TEST_CONNECTION");
        if (string.IsNullOrWhiteSpace(cadena)) { Assert.Inconclusive("Requiere una base exclusiva de pruebas."); return; }
        string basePrueba = new SqlConnectionStringBuilder(cadena).InitialCatalog;
        if (!basePrueba.StartsWith("SIGAT_Roles_Pruebas_", StringComparison.Ordinal))
            throw new InvalidOperationException("Solo se admite una base exclusiva de pruebas.");
        string? baseAnterior = Environment.GetEnvironmentVariable("SIGAT_BASE_PRUEBAS");
        string prefijo = "filtro_" + Guid.NewGuid().ToString("N")[..12];
        string ana = prefijo + "_ana", luis = prefijo + "_luis";
        var servicio = new BitacoraBLL(new BitacoraDAL(cadena));
        var dia = new DateTime(2025, 4, 10);
        try
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", basePrueba);
            void Registrar(DateTime fecha, string usuario, string actividad) =>
                new BitacoraDAL(cadena).Insertar(new Bitacora
                {
                    Fecha = fecha, Usuario = usuario, Actividad = actividad, InformacionAsociada = "Detalle de ensayo"
                });
            Registrar(dia.AddHours(14), ana, "LoginExitoso");
            Registrar(dia.AddHours(15), ana, "AltaUsuario");
            Registrar(dia.AddHours(14), luis, "LoginExitoso");
            Registrar(dia.AddDays(1), ana, "LoginExitoso");
            Registrar(dia.AddDays(1).AddMilliseconds(-3), ana, "LoginExitoso");
            Registrar(dia.AddHours(16), ana, "LoginExitosoExtendido");
            Registrar(dia.AddDays(2), ana, "Personalizada_100%");
            Registrar(dia.AddHours(17), ana, "PersonalizadaX100ABC");
            // Reproduce los nombres históricos y actuales vistos juntos en la base real.
            Registrar(dia.AddYears(-1), ana, "Login Exitoso");
            Registrar(dia.AddHours(13), luis, "Login Exitoso");
            Registrar(dia.AddHours(12), ana, "Login Exitoso");
            var originales = servicio.Buscar(null, null, prefijo, null);
            Assert.HasCount(11, originales);
            Assert.HasCount(1, servicio.ObtenerActividades().Where(a => a == "LoginExitoso").ToList());
            Assert.IsFalse(servicio.ObtenerActividades().Contains("Login Exitoso"), "El desplegable debe ofrecer una sola opción para el mismo evento.");
            var actuales = servicio.Buscar(null, null, prefijo, "LoginExitoso", actividadExacta: true);
            var historicos = servicio.Buscar(null, null, prefijo, "Login Exitoso", actividadExacta: true);
            Assert.HasCount(7, actuales, "Incluye los ingresos antiguos y recientes de ambos usuarios, sin incluir LoginExitosoExtendido.");
            CollectionAssert.AreEquivalent(actuales.Select(e => e.IdBitacora).ToArray(), historicos.Select(e => e.IdBitacora).ToArray());
            Assert.IsTrue(servicio.Buscar(null, null, ana, "LoginExitoso").Any(e => e.Actividad == "LoginExitosoExtendido"),
                "La búsqueda parcial anterior debe conservarse para otros consumidores.");

            Exception? error = null;
            var hilo = new Thread(() =>
            {
                try
                {
                    using var formulario = new FrmBitacora { ShowInTaskbar = false, Opacity = 0 };
                    try
                    {
                        formulario.Show();
                        var actividad = (ComboBox)formulario.Controls.Find("cmbActividad", true).Single();
                        var usuario = (TextBox)formulario.Controls.Find("txtUser", true).Single();
                        var fechas = (CheckBox)formulario.Controls.Find("chkFechas", true).Single();
                        var desde = (DateTimePicker)formulario.Controls.Find("dtpDesde", true).Single();
                        var hasta = (DateTimePicker)formulario.Controls.Find("dtpHasta", true).Single();
                        var buscar = (Button)formulario.Controls.Find("btnBuscar", true).Single();
                        var grilla = (DataGridView)formulario.Controls.Find("dgv", true).Single();
                        List<Bitacora> Filas() => ((IEnumerable<Bitacora>)grilla.DataSource!).ToList();
                        Assert.AreEqual(ComboBoxStyle.DropDownList, actividad.DropDownStyle);
                        Assert.AreEqual("", actividad.SelectedValue);
                        Assert.AreEqual("Todas las actividades", actividad.Text);
                        actividad.SelectedValue = "LoginExitoso";
                        Assert.AreEqual("Ingreso al sistema", actividad.Text);
                        Assert.HasCount(1, actividad.Items.Cast<KeyValuePair<string, string>>().Where(o => o.Key == "LoginExitoso").ToList());
                        int opciones = actividad.Items.Count;
                        usuario.Text = ana;
                        fechas.Checked = true;
                        desde.Value = hasta.Value = dia;
                        buscar.PerformClick();
                        Assert.HasCount(3, Filas());
                        Assert.IsTrue(Filas().All(e => e.Usuario == ana &&
                            (e.Actividad == "LoginExitoso" || e.Actividad == "Login Exitoso") && e.Fecha.Date == dia));
                        Assert.IsTrue(Filas().Any(e => e.Fecha == dia.AddDays(1).AddMilliseconds(-3)),
                            "Incluye el último instante de SQL datetime y excluye la medianoche siguiente.");
                        actividad.SelectedValue = "";
                        buscar.PerformClick();
                        Assert.HasCount(6, Filas(), "Todas las actividades conserva usuario y fechas.");
                        fechas.Checked = false;
                        buscar.PerformClick();
                        Assert.HasCount(9, Filas());
                        actividad.SelectedValue = "Personalizada_100%";
                        buscar.PerformClick();
                        Assert.HasCount(1, Filas(), "Los caracteres de una actividad se buscan literalmente.");
                        Assert.AreEqual("Personalizada_100%", Filas()[0].Actividad);
                        actividad.SelectedValue = "LoginExitoso";
                        fechas.Checked = true;
                        usuario.Clear();
                        buscar.PerformClick();
                        Assert.HasCount(5, Filas(), "Vaciar el usuario conserva actividad y fechas, incluyendo ambos nombres del evento.");
                        fechas.Checked = false;
                        buscar.PerformClick();
                        Assert.HasCount(7, Filas(), "Sin usuario ni fechas aparecen todos los ingresos, antiguos y actuales.");
                        Assert.IsTrue(Filas().Any(e => e.Fecha.Year == dia.Year - 1));
                        fechas.Checked = true;
                        usuario.Text = "sin_resultados_" + prefijo;
                        buscar.PerformClick();
                        Assert.IsEmpty(Filas());
                        Assert.IsTrue(formulario.Controls.Find("lblVacio", true).Single().Visible);
                        Assert.AreEqual(opciones, actividad.Items.Count, "Las opciones no dependen de los resultados filtrados.");
                        IdiomaManager.ObtenerInstancia().CargarIdioma(new Idioma { Id = 1, Nombre = "Prueba", Codigo = "es" },
                            new Dictionary<string, string> { [formulario.Name + ".actividad_LoginExitoso"] = "Sign in" });
                        Assert.AreEqual("LoginExitoso", actividad.SelectedValue);
                        Assert.AreEqual("Sign in", actividad.Text);
                        Assert.AreEqual("sin_resultados_" + prefijo, usuario.Text);
                        Assert.IsTrue(fechas.Checked);
                    }
                    finally { formulario.Close(); }
                }
                catch (Exception ex) { error = ex; }
                finally { IdiomaManager.ObtenerInstancia().ReiniciarSesion(); }
            }) { IsBackground = true };
            hilo.SetApartmentState(ApartmentState.STA);
            hilo.Start();
            Assert.IsTrue(hilo.Join(TimeSpan.FromSeconds(25)), "La búsqueda debe responder sin bloquear la interfaz.");
            if (error != null) System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(error).Throw();
            CollectionAssert.AreEquivalent(originales.Select(e => e.IdBitacora).ToArray(),
                servicio.Buscar(null, null, prefijo, null).Select(e => e.IdBitacora).ToArray());
            Assert.IsTrue(servicio.Buscar(null, null, prefijo, null).All(e => e.InformacionAsociada == "Detalle de ensayo"));
            Assert.HasCount(3, servicio.Buscar(null, null, prefijo, null).Where(e => e.Actividad == "Login Exitoso").ToList(),
                "Los nombres originales de los eventos históricos deben conservarse intactos.");
        }
        finally
        {
            Environment.SetEnvironmentVariable("SIGAT_BASE_PRUEBAS", baseAnterior);
            using var conexion = new SqlConnection(cadena); conexion.Open();
            using var limpiar = new SqlCommand("DELETE FROM dbo.Bitacora WHERE Usuario IN (@Ana,@Luis)", conexion);
            limpiar.Parameters.AddWithValue("@Ana", ana);
            limpiar.Parameters.AddWithValue("@Luis", luis);
            limpiar.ExecuteNonQuery();
        }
    }
}
