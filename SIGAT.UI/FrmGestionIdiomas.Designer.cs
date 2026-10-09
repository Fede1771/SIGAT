namespace SIGAT.UI
{
    partial class FrmGestionIdiomas
    {
        private System.ComponentModel.IContainer components;
        private Label lblIdiomaActivo, lblNuevoIdioma, lblCodigoIdioma, lblNombreNativo;
        private ComboBox cmbIdiomas;
        private TextBox txtNuevoIdioma, txtCodigoIdioma, txtNombreNativo;
        private CheckBox chkIdiomaActivo;
        private Button btnAplicarIdioma, btnCambiarEstado, btnNuevoIdioma, btnGuardarTraduccion, btnExportarTraducciones, btnImportarTraducciones;
        private DataGridView dgvTraducciones;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            SuspendLayout();
            TemaVisual.Formulario(this);
            AutoScaleDimensions = CurrentAutoScaleDimensions;
            Name = "FrmGestionIdiomas";
            Text = "Gestión de idiomas";
            ClientSize = new Size(1160, 740);
            MinimumSize = new Size(880, 600);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 3, Padding = new Padding(24) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 264));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            var heading = TemaVisual.Encabezado("Gestión de idiomas", "Administrá idiomas y completá las traducciones del sistema.", "frmgestionidiomas_titulo", "lbl_descripcion_idiomas");
            layout.Controls.Add(heading, 0, 0);
            layout.SetColumnSpan(heading, 2);

            var selection = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, BackColor = Color.White, Padding = new Padding(16), Margin = new Padding(0, 0, 0, 18) };
            var field = new TableLayoutPanel { AutoSize = true, ColumnCount = 1, Width = 220, Margin = new Padding(0, 0, 16, 0) };
            field.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220));
            lblIdiomaActivo = TemaVisual.Etiqueta("Idioma");
            cmbIdiomas = new ComboBox { Name = "cmbIdiomas", DropDownStyle = ComboBoxStyle.DropDownList, Dock = DockStyle.Fill };
            cmbIdiomas.SelectedIndexChanged += cmbIdiomas_SelectedIndexChanged;
            field.Controls.Add(lblIdiomaActivo);
            field.Controls.Add(cmbIdiomas);
            selection.Controls.Add(field);
            btnAplicarIdioma = TemaVisual.Boton("btnAplicarIdioma", "Aplicar", true);
            btnCambiarEstado = TemaVisual.Boton("btnCambiarEstado", "Activar / desactivar");
            btnAplicarIdioma.Margin = btnCambiarEstado.Margin = new Padding(0, 24, 8, 0);
            btnAplicarIdioma.Click += btnAplicarIdioma_Click;
            btnCambiarEstado.Click += btnCambiarEstado_Click;
            selection.Controls.Add(btnAplicarIdioma);
            selection.Controls.Add(btnCambiarEstado);
            layout.Controls.Add(selection, 0, 1);
            layout.SetColumnSpan(selection, 2);

            var editor = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Top, ColumnCount = 1, Padding = new Padding(18) };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            editor.Controls.Add(TemaVisual.Etiqueta("Nuevo idioma", "lbl_titulo_nuevo_idioma", true));
            lblNuevoIdioma = TemaVisual.Etiqueta("Nombre");
            txtNuevoIdioma = CampoIdioma("txtNuevoIdioma", 50);
            lblCodigoIdioma = TemaVisual.Etiqueta("Código");
            txtCodigoIdioma = CampoIdioma("txtCodigoIdioma", 10);
            lblNombreNativo = TemaVisual.Etiqueta("Nombre nativo");
            txtNombreNativo = CampoIdioma("txtNombreNativo", 100);
            chkIdiomaActivo = new CheckBox { Name = "chkIdiomaActivo", Text = "Activo", AutoSize = true, Checked = true, Margin = new Padding(0, 0, 0, 16) };
            btnNuevoIdioma = TemaVisual.Boton("btnNuevoIdioma", "Crear idioma", true);
            btnNuevoIdioma.Dock = DockStyle.Fill;
            btnNuevoIdioma.Click += btnNuevoIdioma_Click;
            foreach (Control control in new Control[] { lblNuevoIdioma, txtNuevoIdioma, lblCodigoIdioma, txtCodigoIdioma, lblNombreNativo, txtNombreNativo, chkIdiomaActivo, btnNuevoIdioma }) editor.Controls.Add(control);
            editor.Controls.Add(new Label { Text = "Las traducciones nuevas quedan pendientes hasta que las completes.", Tag = "lbl_ayuda_nuevo_idioma", AutoSize = true, Dock = DockStyle.Fill, ForeColor = TemaVisual.Secundario, Margin = new Padding(0, 10, 0, 0) });
            var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true, Margin = Padding.Empty };
            card.Controls.Add(editor);
            layout.Controls.Add(card, 1, 2);

            var translation = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Margin = new Padding(0, 0, 18, 0), BackColor = Color.White, Padding = new Padding(18) };
            translation.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            translation.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            translation.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            translation.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            translation.Controls.Add(TemaVisual.Etiqueta("Traducciones", "lbl_titulo_traducciones", true), 0, 0);
            dgvTraducciones = new DataGridView { Name = "dgvTraducciones", Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill };
            TemaVisual.Grilla(dgvTraducciones);
            dgvTraducciones.CellFormatting += FormatearEstadoTraduccion;
            dgvTraducciones.DataBindingComplete += (_, _) => ConfigurarColumnasTraducciones();
            translation.Controls.Add(dgvTraducciones, 0, 1);
            var actions = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Padding = new Padding(0, 14, 0, 0), Margin = Padding.Empty };
            btnGuardarTraduccion = TemaVisual.Boton("btnGuardarTraduccion", "Guardar traducción", true);
            btnExportarTraducciones = TemaVisual.Boton("btnExportarTraducciones", "Exportar traducciones");
            btnImportarTraducciones = TemaVisual.Boton("btnImportarTraducciones", "Importar traducciones");
            btnGuardarTraduccion.Click += btnGuardarTraduccion_Click;
            btnExportarTraducciones.Click += btnExportarTraducciones_Click;
            btnImportarTraducciones.Click += btnImportarTraducciones_Click;
            actions.Controls.AddRange(new Control[] { btnGuardarTraduccion, btnExportarTraducciones, btnImportarTraducciones });
            translation.Controls.Add(actions, 0, 2);
            layout.Controls.Add(translation, 0, 2);
            Controls.Add(layout);
            ResumeLayout(true);
        }

        private static TextBox CampoIdioma(string name, int maxLength)
            => new TextBox { Name = name, Dock = DockStyle.Fill, MaxLength = maxLength, Margin = new Padding(0, 0, 0, 16) };
    }
}
