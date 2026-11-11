namespace SIGAT.UI;

partial class DlgHistorialUsuario
{
    private ComboBox cmbCuentas;
    private DataGridView dgvVersiones, dgvDetalle;
    private Label lblAyuda;
    private Button btnRecargar, btnRestaurar;

    private void InitializeComponent()
    {
        var layout = new TableLayoutPanel();
        var selector = new TableLayoutPanel();
        var lblCuenta = new Label();
        var acciones = new FlowLayoutPanel();
        var btnCerrar = new Button();
        cmbCuentas = new ComboBox();
        dgvVersiones = new DataGridView();
        dgvDetalle = new DataGridView();
        lblAyuda = new Label();
        btnRecargar = new Button();
        btnRestaurar = new Button();
        SuspendLayout();
        layout.SuspendLayout();

        layout.Name = "layoutPrincipal";
        layout.ColumnCount = 1;
        layout.RowCount = 5;
        layout.Padding = new Padding(16);
        layout.Dock = DockStyle.Fill;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        selector.AutoSize = true;
        selector.Dock = DockStyle.Top;
        selector.ColumnCount = 2;
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        lblCuenta.AutoSize = true;
        lblCuenta.Anchor = AnchorStyles.Left;
        lblCuenta.Text = "Cuenta";
        lblCuenta.Tag = "cuenta";
        cmbCuentas.Name = "cmbCuentas";
        cmbCuentas.Dock = DockStyle.Fill;
        cmbCuentas.DropDownStyle = ComboBoxStyle.DropDownList;
        selector.Controls.Add(lblCuenta, 0, 0);
        selector.Controls.Add(cmbCuentas, 1, 0);

        dgvVersiones.Name = "dgvVersiones";
        dgvVersiones.Dock = DockStyle.Fill;
        dgvVersiones.ReadOnly = true;
        dgvVersiones.AllowUserToAddRows = false;
        dgvVersiones.AllowUserToDeleteRows = false;
        dgvVersiones.RowHeadersVisible = false;
        dgvVersiones.MultiSelect = false;
        dgvVersiones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvVersiones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvVersiones.BackgroundColor = Color.White;
        dgvVersiones.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Version", HeaderText = "Versión", Tag = "version", FillWeight = 45, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha y hora", Tag = "fecha", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Autor", HeaderText = "Realizado por", Tag = "autor", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Operacion", HeaderText = "Operación", Tag = "operacion", SortMode = DataGridViewColumnSortMode.NotSortable });

        lblAyuda.AutoSize = true;
        lblAyuda.Dock = DockStyle.Fill;
        lblAyuda.Margin = new Padding(3, 8, 3, 4);

        dgvDetalle.Name = "dgvDetalle";
        dgvDetalle.Dock = DockStyle.Fill;
        dgvDetalle.ReadOnly = true;
        dgvDetalle.AllowUserToAddRows = false;
        dgvDetalle.AllowUserToDeleteRows = false;
        dgvDetalle.RowHeadersVisible = false;
        dgvDetalle.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvDetalle.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
        dgvDetalle.DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        dgvDetalle.BackgroundColor = Color.White;
        dgvDetalle.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Campo", HeaderText = "Campo", Tag = "campo", FillWeight = 62, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Anterior", HeaderText = "Versión anterior", Tag = "anterior", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Seleccionada", HeaderText = "Versión seleccionada", Tag = "seleccionada", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Vigente", HeaderText = "Estado actual", Tag = "vigente", SortMode = DataGridViewColumnSortMode.NotSortable });

        acciones.AutoSize = true;
        acciones.Dock = DockStyle.Fill;
        acciones.FlowDirection = FlowDirection.RightToLeft;
        acciones.WrapContents = false;
        btnCerrar.AutoSize = true;
        btnCerrar.Text = "Cerrar";
        btnCerrar.Tag = "cerrar";
        btnCerrar.DialogResult = DialogResult.Cancel;
        btnRecargar.AutoSize = true;
        btnRecargar.Text = "Recargar";
        btnRecargar.Tag = "recargar";
        btnRestaurar.AutoSize = true;
        btnRestaurar.Name = "btnRestaurar";
        btnRestaurar.Text = "Restaurar versión";
        btnRestaurar.Tag = "restaurar";
        btnRestaurar.Enabled = false;
        acciones.Controls.AddRange(new Control[] { btnCerrar, btnRestaurar, btnRecargar });

        layout.Controls.Add(selector, 0, 0);
        layout.Controls.Add(dgvVersiones, 0, 1);
        layout.Controls.Add(lblAyuda, 0, 2);
        layout.Controls.Add(dgvDetalle, 0, 3);
        layout.Controls.Add(acciones, 0, 4);
        Controls.Add(layout);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScaleDimensions = new SizeF(7F, 17F);
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(245, 247, 251);
        ForeColor = Color.FromArgb(30, 45, 65);
        ClientSize = new Size(920, 640);
        MinimumSize = new Size(700, 515);
        StartPosition = FormStartPosition.CenterParent;
        CancelButton = btnCerrar;
        MinimizeBox = false;
        ShowInTaskbar = false;
        Name = "DlgHistorialUsuario";
        Text = "Historial de usuarios";
        Tag = "titulo";
        layout.ResumeLayout(false);
        ResumeLayout(false);
    }
}
