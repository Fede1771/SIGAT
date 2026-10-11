using SIGAT.BE.Integridad;
using SIGAT.BLL;

namespace SIGAT.UI;

public sealed class FrmIntegridad : Form
{
    private readonly IntegridadBLL servicio = new();
    private readonly DataGridView grilla = new();
    private readonly Label estado = new();
    private readonly TextBox archivo = new();
    private readonly Button inicializar, respaldo, recuperar, comprobar, continuar, archivar;
    private readonly CheckBox completas = new() { Text = "Restaurar las tablas afectadas completas (descarta sus cambios posteriores al respaldo)", AutoSize = true };
    private readonly bool soloRecuperacion;
    private InformeIntegridad informe;
    private bool administrador;
    private bool ocupada;

    public FrmIntegridad(InformeIntegridad resultado = null, bool bloquearInicio = false)
    {
        informe = resultado;
        soloRecuperacion = bloquearInicio;
        Name = "FrmIntegridad";
        Text = "SIGAT · Integridad y recuperación";
        TemaVisual.Formulario(this);
        ClientSize = new Size(1040, 670);
        MinimumSize = new Size(880, 620);
        StartPosition = FormStartPosition.CenterParent;
        var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 6, Padding = new Padding(24) };
        layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.Controls.Add(TemaVisual.Encabezado("Integridad y recuperación", "SHA-256: verificar los datos, comprobar un respaldo y recuperar los registros originales.", null, null), 0, 0);
        estado.AutoSize = true; estado.Dock = DockStyle.Fill; estado.Padding = new Padding(12); estado.Margin = new Padding(0, 0, 0, 14);
        layout.Controls.Add(estado, 0, 1);
        TemaVisual.Grilla(grilla); grilla.Dock = DockStyle.Fill; grilla.ReadOnly = true; grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        layout.Controls.Add(grilla, 0, 2);
        var fuente = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 2, Padding = new Padding(0, 14, 0, 8) };
        fuente.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100)); fuente.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        archivo.Dock = DockStyle.Fill; archivo.PlaceholderText = "Ruta del .bak en el equipo donde corre SQL Server";
        var elegir = TemaVisual.Boton("btnElegirRespaldo", "Elegir respaldo");
        elegir.Click += (_, _) => { using var dialogo = new OpenFileDialog { Filter = "Respaldo SQL (*.bak)|*.bak" }; if (dialogo.ShowDialog(this) == DialogResult.OK) archivo.Text = dialogo.FileName; };
        fuente.Controls.Add(archivo, 0, 0); fuente.Controls.Add(elegir, 1, 0); layout.Controls.Add(fuente, 0, 3);
        layout.Controls.Add(completas, 0, 4);
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Padding = new Padding(0, 12, 0, 0) };
        comprobar = TemaVisual.Boton("btnComprobar", "Verificar");
        inicializar = TemaVisual.Boton("btnInicializar", "Preparar control inicial");
        respaldo = TemaVisual.Boton("btnBackupVerificado", "Crear respaldo comprobado");
        recuperar = TemaVisual.Boton("btnRecuperar", "Recuperar", true);
        continuar = TemaVisual.Boton("btnContinuar", bloquearInicio ? "Continuar con SIGAT" : "Cerrar");
        archivar = TemaVisual.Boton("btnArchivar", "Archivar bitácora antigua");
        acciones.Controls.AddRange(new Control[] { comprobar, inicializar, respaldo, recuperar, archivar, continuar }); layout.Controls.Add(acciones, 0, 5);
        Controls.Add(layout);
        comprobar.Click += async (_, _) => await Operar(() => { informe = servicio.Verificar(); return "Verificación finalizada."; });
        inicializar.Click += async (_, _) =>
        {
            if (MessageBox.Show(this, "Esta preparación establece la primera referencia de integridad. Confirme que los datos actuales son los que desea conservar. Se comprobarán los dígitos antiguos y se guardará un backup previo. No se permite reinicializar una base ya protegida.", "Preparación inicial", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await Operar(() => { string ruta = servicio.InicializarBaseConfiable(); informe = servicio.Verificar(); return "Referencia inicial y respaldo comprobado creados:\n" + ruta; }, true);
        };
        respaldo.Click += async (_, _) => await Operar(() => "Respaldo comprobado creado:\n" + servicio.CrearRespaldoConfiable(), true);
        recuperar.Click += async (_, _) =>
        {
            string ruta = archivo.Text; bool total = completas.Checked;
            string texto = total ? "Se reemplazarán las tablas protegidas afectadas por sus datos del respaldo y se perderán sus cambios posteriores."
                : "Se intentará recuperar solo los registros afectados. Si el respaldo no permite conservar los demás cambios, se cancelará toda la operación.";
            if (MessageBox.Show(this, texto + "\n\nRespaldo: " + ruta, "Confirmar recuperación", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;
            await Operar(() => { informe = servicio.Recuperar(ruta, total); return "Datos recuperados y verificados correctamente."; });
        };
        continuar.Click += (_, _) => { DialogResult = DialogResult.OK; Close(); };
        archivar.Click += async (_, _) =>
        {
            if (MessageBox.Show(this, "Se trasladarán al histórico los eventos anteriores a 180 días y se actualizará el DVV en la misma transacción. ¿Continuar?",
                "Archivar bitácora", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            await Operar(() => { int cantidad = servicio.ArchivarBitacora(180); informe = servicio.Verificar(); return "Eventos archivados: " + cantidad; });
        };
        Shown += async (_, _) => await Operar(() => { administrador = servicio.PuedeAdministrar(); informe ??= servicio.Verificar(); return ""; });
        FormClosing += (_, e) => { if (ocupada) e.Cancel = true; };
    }

    private async Task Operar(Func<string> accion, bool mostrarRuta = false)
    {
        UseWaitCursor = true;
        ocupada = true;
        foreach (var boton in new[] { inicializar, respaldo, recuperar, comprobar, continuar, archivar }) boton.Enabled = false;
        try
        {
            string mensaje = await Task.Run(accion);
            if (mostrarRuta && mensaje.Contains('\n')) archivo.Text = mensaje[(mensaje.LastIndexOf('\n') + 1)..];
            if (!string.IsNullOrEmpty(mensaje)) MessageBox.Show(this, mensaje, "SIGAT", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, "No se completó la operación", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        finally
        {
            UseWaitCursor = false;
            ocupada = false;
            grilla.DataSource = informe?.Problemas.ToList();
            bool correcta = informe?.Correcta == true;
            estado.Text = correcta ? "Integridad correcta. Los datos protegidos pueden utilizarse." : "Se requiere atención. Los datos afectados están bloqueados hasta su recuperación.";
            estado.BackColor = correcta ? TemaVisual.VerdeSuave : TemaVisual.AmbarSuave;
            estado.ForeColor = correcta ? TemaVisual.Verde : TemaVisual.Ambar;
            comprobar.Enabled = true;
            inicializar.Visible = informe?.Configurada == false;
            inicializar.Enabled = administrador && informe?.Configurada == false;
            respaldo.Enabled = administrador && correcta;
            archivar.Enabled = administrador && correcta;
            recuperar.Enabled = administrador && informe?.Configurada == true && !correcta;
            continuar.Enabled = !soloRecuperacion || correcta;
        }
    }
}
