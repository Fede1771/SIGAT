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
    private bool ajustandoGrupos;

    public FrmIntegridad() : this(null, false) { }

    public FrmIntegridad(InformeIntegridad resultado, bool bloquearInicio = false)
    {
        informe = resultado;
        soloRecuperacion = bloquearInicio;
        InitializeComponent();
        if (EntornoDiseno.Activo) return;
        TemaVisual.Formulario(this);
        flowlayoutpanel8.SizeChanged += (_, _) => AjustarGruposAcciones();
        AjustarGruposAcciones();
        btnContinuar.Text = bloquearInicio ? "Continuar con SIGAT" : "Cerrar";
        chkOpcionesAvanzadas.CheckedChanged += (_, _) =>
        {
            panelAvanzado.Visible = chkOpcionesAvanzadas.Checked;
            // La recuperación completa debe ser una elección explícita y visible.
            if (!chkOpcionesAvanzadas.Checked) completas.Checked = false;
            AjustarGruposAcciones();
        };
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
        btnRecalcular.Click += async (_, _) =>
        {
            if (MessageBox.Show(this,
                "Se aceptarán los registros actuales de Usuarios y UsuarioRol, incluidas posibles alteraciones o pérdidas. " +
                "Se recalcularán todos sus DVH y DVV. Esta acción NO recupera datos y quedará auditada. ¿Desea continuar?",
                "Asumir estado actual y recalcular", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await Operar(() => { informe = servicio.RecalcularIntegridad(); return "Estado actual aceptado y dígitos verificadores recalculados."; });
        };
        btnRecuperar.Click += async (_, _) =>
        {
            string ruta = archivo.Text; bool total = completas.Checked;
            string texto = total ? "Se reemplazarán Usuarios y UsuarioRol por sus datos del respaldo y se perderán sus cambios posteriores."
                : "Se intentará recuperar solo los registros afectados. Si el respaldo no permite conservar los demás cambios, se cancelará toda la operación.";
            if (MessageBox.Show(this, texto + "\n\nRespaldo: " + ruta, "Confirmar recuperación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await Operar(() => { informe = servicio.Recuperar(ruta, total); return "Datos recuperados y verificados correctamente."; });
        };
        btnContinuar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        btnArchivar.Click += async (_, _) =>
        {
            if (MessageBox.Show(this, "Se trasladarán al histórico los eventos anteriores a 180 días en una transacción. ¿Continuar?",
                "Archivar bitácora", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await Operar(() => { int cantidad = servicio.ArchivarBitacora(180); informe = servicio.Verificar(); return "Eventos archivados: " + cantidad; });
        };
        Shown += async (_, _) => await Operar(() => { administrador = servicio.PuedeAdministrar(); informe ??= servicio.Verificar(); return ""; });
        FormClosing += (_, e) => { if (ocupada) e.Cancel = true; };
    }

    private void AjustarGruposAcciones()
    {
        if (ajustandoGrupos || flowlayoutpanel8.ClientSize.Width <= 0) return;
        ajustandoGrupos = true;
        try
        {
            // La consecuencia de recuperar tablas completas debe leerse también en ventanas angostas.
            int anchoCasilla = Math.Max(1, completas.Parent.ClientSize.Width - completas.Parent.Padding.Horizontal - completas.Margin.Horizontal);
            int marca = (int)Math.Ceiling(24 * DeviceDpi / 96f);
            int altoTexto = TextRenderer.MeasureText(completas.Text, completas.Font,
                new Size(Math.Max(1, anchoCasilla - marca), int.MaxValue), TextFormatFlags.WordBreak).Height;
            completas.Size = new Size(anchoCasilla, Math.Max(completas.Font.Height + 4, altoTexto + 4));
            foreach (var grupo in flowlayoutpanel8.Controls.OfType<TableLayoutPanel>()
                .Concat(panelAvanzado.Controls.OfType<TableLayoutPanel>()))
            {
                int ancho = Math.Max(1, flowlayoutpanel8.ClientSize.Width - flowlayoutpanel8.Padding.Horizontal - grupo.Margin.Horizontal);
                grupo.MaximumSize = new Size(ancho, 0);
                foreach (var acciones in grupo.Controls.OfType<FlowLayoutPanel>()) acciones.MaximumSize = new Size(ancho, 0);
            }
        }
        finally { ajustandoGrupos = false; }
    }

    // Task.Run mantiene la interfaz disponible mientras trabaja SQL. Deshabilitamos las acciones para evitar operaciones simultáneas.
    private async Task Operar(Func<string> accion, bool mostrarRuta = false)
    {
        UseWaitCursor = true;
        ocupada = true;
        chkOpcionesAvanzadas.Enabled = archivo.Enabled = btnElegirRespaldo.Enabled = completas.Enabled = false;
        foreach (var boton in new[] { btnInicializar, btnBackupVerificado, btnRecuperar, btnComprobar, btnContinuar, btnArchivar, btnRecalcular }) boton.Enabled = false;
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
            estado.Text = correcta ? "Integridad correcta. Los datos protegidos pueden utilizarse." :
                informe?.Configurada == false ? "Falta preparar el control inicial. El acceso permanece bloqueado." :
                $"Se detectaron {informe?.Problemas.Count ?? 0} diferencias. El acceso permanece bloqueado.";
            label4.Text = correcta ? "Podés verificar nuevamente o crear un respaldo." :
                informe?.Configurada == false ? "Prepará la referencia inicial de integridad para continuar." :
                "Elegí un respaldo y pulsá Recuperar. Se comprobarán los datos antes de habilitar el acceso.";
            chkOpcionesAvanzadas.Enabled = archivo.Enabled = btnElegirRespaldo.Enabled = completas.Enabled = true;
            estado.BackColor = TemaVisual.ColorActual(correcta ? TemaVisual.VerdeSuave : TemaVisual.AmbarSuave);
            estado.ForeColor = TemaVisual.ColorActual(correcta ? TemaVisual.Verde : TemaVisual.Ambar, true);
            btnComprobar.Enabled = true;
            btnInicializar.Visible = informe?.Configurada == false;
            btnInicializar.Enabled = administrador && informe?.Configurada == false;
            btnBackupVerificado.Enabled = administrador && correcta;
            btnArchivar.Enabled = administrador && correcta;
            btnRecalcular.Enabled = administrador && !correcta;
            btnRecuperar.Enabled = administrador && informe?.Configurada == true && !correcta;
            btnContinuar.Enabled = !soloRecuperacion || correcta;
        }
    }
}
