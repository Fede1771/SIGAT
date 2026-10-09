namespace SIGAT.UI
{
    partial class FrmBitacora
    {
        private System.ComponentModel.IContainer components;
        private Panel panelFiltros;
        private CheckBox chkFechas;
        private DateTimePicker dtpDesde, dtpHasta;
        private Label lblUser, lblAct;
        private TextBox txtUser, txtActividad;
        private Button btnBuscar;
        private DataGridView dgv;

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
            Name = "FrmBitacora";
            Text = "Consulta de bitácora";
            ClientSize = new Size(1140, 680);
            MinimumSize = new Size(860, 540);
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(24) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(TemaVisual.Encabezado("Bitácora", "Consultá la actividad del sistema y sus registros de auditoría.", "frmbitacora_titulo", "lbl_descripcion_bitacora"), 0, 0);

            var filters = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Padding = new Padding(18), BackColor = Color.White, Margin = Padding.Empty };
            var dates = new TableLayoutPanel { AutoSize = true, ColumnCount = 2, Width = 282, Margin = new Padding(0, 0, 16, 12) };
            dates.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136));
            dates.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136));
            chkFechas = new CheckBox { Name = "chkFechas", Text = "Filtrar por fechas", AutoSize = true, Margin = new Padding(0, 0, 0, 8) };
            dates.Controls.Add(chkFechas, 0, 0);
            dates.SetColumnSpan(chkFechas, 2);
            dtpDesde = new DateTimePicker { Name = "dtpDesde", Format = DateTimePickerFormat.Short, Width = 126, Margin = Padding.Empty };
            dtpHasta = new DateTimePicker { Name = "dtpHasta", Format = DateTimePickerFormat.Short, Width = 126, Margin = Padding.Empty };
            dates.Controls.Add(dtpDesde, 0, 1);
            dates.Controls.Add(dtpHasta, 1, 1);
            lblUser = TemaVisual.Etiqueta("Usuario");
            txtUser = new TextBox { Name = "txtUser", Width = 170, Dock = DockStyle.Fill };
            lblAct = TemaVisual.Etiqueta("Actividad");
            txtActividad = new TextBox { Name = "txtActividad", Width = 200, Dock = DockStyle.Fill };
            filters.Controls.Add(dates);
            filters.Controls.Add(CampoFiltro(lblUser, txtUser, 178));
            filters.Controls.Add(CampoFiltro(lblAct, txtActividad, 208));
            btnBuscar = TemaVisual.Boton("btnBuscar", "Buscar", true);
            btnBuscar.Margin = new Padding(0, 20, 0, 10);
            btnBuscar.Click += BtnBuscar_Click;
            filters.Controls.Add(btnBuscar);
            panelFiltros = filters;
            panelFiltros.Margin = new Padding(0, 0, 0, 18);
            layout.Controls.Add(panelFiltros, 0, 1);

            dgv = new DataGridView { Name = "dgv", Dock = DockStyle.Fill, AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill, ReadOnly = true };
            TemaVisual.Grilla(dgv);
            layout.Controls.Add(TemaVisual.Tarjeta(dgv, 0), 0, 2);
            Controls.Add(layout);
            AcceptButton = btnBuscar;
            ResumeLayout(true);
        }

        private static TableLayoutPanel CampoFiltro(Label label, TextBox input, int width)
        {
            var field = new TableLayoutPanel { AutoSize = true, Width = width, ColumnCount = 1, Margin = new Padding(0, 0, 16, 12) };
            field.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, width));
            field.Controls.Add(label);
            field.Controls.Add(input);
            return field;
        }
    }
}
