using SIGAT.BE.Integridad;
using SIGAT.BLL;

namespace SIGAT.UI;

public sealed partial class FrmIntegridad : Form
{
    private readonly IntegridadBLL servicio = new();
    private readonly bool soloRecuperacion;
    private InformeIntegridad informe;
    private bool administrador;
    private bool ocupada;

    public FrmIntegridad() : this(null, false) { }

    public FrmIntegridad(InformeIntegridad resultado, bool bloquearInicio = false)
    {
        informe = resultado;
        soloRecuperacion = bloquearInicio;
        InitializeComponent();
        if (EntornoDiseno.Activo) return;
        TemaVisual.Formulario(this);
        btnContinuar.Text = bloquearInicio ? "Continuar con SIGAT" : "Cerrar";
        btnElegirRespaldo.Click += (_, _) =>
        {
            using var dialogo = new OpenFileDialog { Filter = "Respaldo SQL (*.bak)|*.bak" };
            if (dialogo.ShowDialog(this) == DialogResult.OK) archivo.Text = dialogo.FileName;
        };
        btnComprobar.Click += async (_, _) => await Operar(() => { informe = servicio.Verificar(); return "Verificación finalizada."; });
        btnInicializar.Click += async (_, _) =>
        {
            if (MessageBox.Show(this, "Esta preparación establece la primera referencia de integridad. Confirme que los datos actuales son los que desea conservar. Se comprobarán los dígitos antiguos y se guardará un backup previo. No se permite reinicializar una base ya protegida.", "Preparación inicial", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await Operar(() => { string ruta = servicio.InicializarBaseConfiable(); informe = servicio.Verificar(); return "Referencia inicial y respaldo comprobado creados:\n" + ruta; }, true);
        };
        btnBackupVerificado.Click += async (_, _) =>
        {
            using var dialogo = new SaveFileDialog
            {
                Title = "Guardar respaldo comprobado",
                Filter = "Respaldo SQL (*.bak)|*.bak",
                DefaultExt = "bak", AddExtension = true,
                FileName = "SIGAT_confiable_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".bak"
            };
            if (dialogo.ShowDialog(this) != DialogResult.OK) return;
            string destino = dialogo.FileName;
            await Operar(() => "Respaldo comprobado creado:\n" + servicio.CrearRespaldoConfiable(destino), true);
        };
        btnRecuperar.Click += async (_, _) =>
        {
            string ruta = archivo.Text; bool total = completas.Checked;
            string texto = total ? "Se reemplazarán las tablas protegidas afectadas por sus datos del respaldo y se perderán sus cambios posteriores."
                : "Se intentará recuperar solo los registros afectados. Si el respaldo no permite conservar los demás cambios, se cancelará toda la operación.";
            if (MessageBox.Show(this, texto + "\n\nRespaldo: " + ruta, "Confirmar recuperación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await Operar(() => { informe = servicio.Recuperar(ruta, total); return "Datos recuperados y verificados correctamente."; });
        };
        btnContinuar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        btnArchivar.Click += async (_, _) =>
        {
            if (MessageBox.Show(this, "Se trasladarán al histórico los eventos anteriores a 180 días y se actualizará el DVV en la misma transacción. ¿Continuar?",
                "Archivar bitácora", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await Operar(() => { int cantidad = servicio.ArchivarBitacora(180); informe = servicio.Verificar(); return "Eventos archivados: " + cantidad; });
        };
        Shown += async (_, _) => await Operar(() => { administrador = servicio.PuedeAdministrar(); informe ??= servicio.Verificar(); return ""; });
        FormClosing += (_, e) => { if (ocupada) e.Cancel = true; };
    }

    // Task.Run mantiene la interfaz disponible mientras trabaja SQL. Deshabilitamos las acciones para evitar operaciones simultáneas.
    private async Task Operar(Func<string> accion, bool mostrarRuta = false)
    {
        UseWaitCursor = true;
        ocupada = true;
        foreach (var boton in new[] { btnInicializar, btnBackupVerificado, btnRecuperar, btnComprobar, btnContinuar, btnArchivar }) boton.Enabled = false;
        try
        {
            string mensaje = await Task.Run(accion);
            // Después del await volvemos al hilo de la interfaz para mostrar el resultado de la operación.
            if (mostrarRuta && mensaje.Contains('\n')) archivo.Text = mensaje[(mensaje.LastIndexOf('\n') + 1)..];
            if (!string.IsNullOrEmpty(mensaje)) MessageBox.Show(this, mensaje, "SIGAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se completó la operación", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally
        {
            // También si hubo un error, quitamos el estado ocupado y habilitamos solo las acciones permitidas por el informe.
            UseWaitCursor = false;
            ocupada = false;
            grilla.DataSource = informe?.Problemas.ToList();
            bool correcta = informe?.Correcta == true;
            estado.Text = correcta ? "Integridad correcta. Los datos protegidos pueden utilizarse." : "Se requiere atención. Los datos afectados están bloqueados hasta su recuperación.";
            estado.BackColor = TemaVisual.ColorActual(correcta ? TemaVisual.VerdeSuave : TemaVisual.AmbarSuave);
            estado.ForeColor = TemaVisual.ColorActual(correcta ? TemaVisual.Verde : TemaVisual.Ambar, true);
            btnComprobar.Enabled = true;
            btnInicializar.Visible = informe?.Configurada == false;
            btnInicializar.Enabled = administrador && informe?.Configurada == false;
            btnBackupVerificado.Enabled = administrador && correcta;
            btnArchivar.Enabled = administrador && correcta;
            btnRecuperar.Enabled = administrador && informe?.Configurada == true && !correcta;
            btnContinuar.Enabled = !soloRecuperacion || correcta;
        }
    }
}
