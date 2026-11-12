namespace SIGAT.UI;

partial class DlgHistorialUsuario
{
    private ComboBox cmbCuentas;
    private DataGridView dgvVersiones, dgvDetalle;
    private Label lblAyuda, lblVersiones, lblDetalle, lblRestauracion;
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
        lblVersiones = new Label();
        lblDetalle = new Label();
        lblRestauracion = new Label();
        btnRecargar = new Button();
        btnRestaurar = new Button();
        SuspendLayout();
        layout.SuspendLayout();

        layout.Name = "layoutPrincipal";
        layout.ColumnCount = 1;
        layout.RowCount = 8;
        layout.Padding = new Padding(16);
        layout.Dock = DockStyle.Fill;
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 45F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 55F));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        selector.AutoSize = true;
        selector.Dock = DockStyle.Top;
        selector.ColumnCount = 2;
        selector.TabIndex = 0;
        selector.Margin = new Padding(0, 0, 0, 8);
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        selector.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
        lblCuenta.AutoSize = true;
        lblCuenta.Anchor = AnchorStyles.Left;
        lblCuenta.Text = "Cuenta";
        lblCuenta.Tag = "cuenta";
        cmbCuentas.Name = "cmbCuentas";
        cmbCuentas.Dock = DockStyle.Fill;
        cmbCuentas.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbCuentas.TabIndex = 0;
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
        dgvVersiones.BorderStyle = BorderStyle.None;
        dgvVersiones.EnableHeadersVisualStyles = false;
        dgvVersiones.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(231, 240, 252), ForeColor = Color.FromArgb(30, 45, 65),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(6, 0, 6, 0),
            SelectionBackColor = Color.FromArgb(231, 240, 252), SelectionForeColor = Color.FromArgb(30, 45, 65)
        };
        dgvVersiones.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White, ForeColor = Color.FromArgb(30, 45, 65), Padding = new Padding(6, 1, 6, 1),
            SelectionBackColor = Color.FromArgb(231, 240, 252), SelectionForeColor = Color.FromArgb(30, 45, 65)
        };
        dgvVersiones.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 251, 254);
        dgvVersiones.GridColor = Color.FromArgb(223, 230, 239);
        dgvVersiones.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvVersiones.ColumnHeadersHeight = 28;
        dgvVersiones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvVersiones.RowTemplate.Height = 26;
        dgvVersiones.AllowUserToResizeRows = false;
        dgvVersiones.TabIndex = 2;
        dgvVersiones.Margin = Padding.Empty;
        dgvVersiones.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Version", HeaderText = "Versión", Tag = "version", FillWeight = 55, MinimumWidth = 90, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Fecha", HeaderText = "Fecha y hora", Tag = "fecha", FillWeight = 110, MinimumWidth = 165, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Autor", HeaderText = "Realizado por", Tag = "autor", FillWeight = 100, MinimumWidth = 130, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Operacion", HeaderText = "Operación", Tag = "operacion", FillWeight = 150, MinimumWidth = 190, SortMode = DataGridViewColumnSortMode.NotSortable });

        lblVersiones.Name = "lblVersiones";
        lblVersiones.Text = "Versiones registradas";
        lblVersiones.Tag = "titulo_versiones";
        lblVersiones.AutoSize = true;
        lblVersiones.Dock = DockStyle.Fill;
        lblVersiones.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblVersiones.Margin = new Padding(0, 0, 0, 4);
        lblDetalle.Name = "lblDetalle";
        lblDetalle.Text = "Detalle de la versión seleccionada";
        lblDetalle.Tag = "titulo_detalle";
        lblDetalle.AutoSize = true;
        lblDetalle.Dock = DockStyle.Fill;
        lblDetalle.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblDetalle.Margin = new Padding(0, 8, 0, 4);

        lblAyuda.Name = "lblAyuda";
        lblAyuda.AutoSize = true;
        lblAyuda.Dock = DockStyle.Fill;
        lblAyuda.ForeColor = Color.FromArgb(94, 111, 133);
        lblAyuda.Margin = new Padding(0, 0, 0, 4);

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
        dgvDetalle.BorderStyle = BorderStyle.None;
        dgvDetalle.EnableHeadersVisualStyles = false;
        dgvDetalle.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(231, 240, 252), ForeColor = Color.FromArgb(30, 45, 65),
            Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(6, 0, 6, 0),
            SelectionBackColor = Color.FromArgb(231, 240, 252), SelectionForeColor = Color.FromArgb(30, 45, 65)
        };
        dgvDetalle.DefaultCellStyle.Padding = new Padding(6, 1, 6, 1);
        dgvDetalle.DefaultCellStyle.BackColor = Color.White;
        dgvDetalle.DefaultCellStyle.ForeColor = Color.FromArgb(30, 45, 65);
        dgvDetalle.DefaultCellStyle.SelectionBackColor = Color.FromArgb(231, 240, 252);
        dgvDetalle.DefaultCellStyle.SelectionForeColor = Color.FromArgb(30, 45, 65);
        dgvDetalle.GridColor = Color.FromArgb(223, 230, 239);
        dgvDetalle.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        dgvDetalle.ColumnHeadersHeight = 28;
        dgvDetalle.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        dgvDetalle.AllowUserToResizeRows = false;
        dgvDetalle.MultiSelect = false;
        dgvDetalle.TabIndex = 5;
        dgvDetalle.Margin = Padding.Empty;
        dgvDetalle.Columns.AddRange(
            new DataGridViewTextBoxColumn { Name = "Campo", HeaderText = "Campo", Tag = "campo", FillWeight = 62, SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Anterior", HeaderText = "Versión anterior", Tag = "anterior", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Seleccionada", HeaderText = "Versión seleccionada", Tag = "seleccionada", SortMode = DataGridViewColumnSortMode.NotSortable },
            new DataGridViewTextBoxColumn { Name = "Vigente", HeaderText = "Estado actual", Tag = "vigente", SortMode = DataGridViewColumnSortMode.NotSortable });

        acciones.AutoSize = true;
        acciones.TabIndex = 7;
        acciones.Margin = new Padding(0, 8, 0, 0);
        acciones.Dock = DockStyle.Fill;
        acciones.FlowDirection = FlowDirection.RightToLeft;
        acciones.WrapContents = false;
        btnCerrar.AutoSize = true;
        btnCerrar.Name = "btnCerrar";
        btnCerrar.FlatStyle = FlatStyle.Flat;
        btnCerrar.BackColor = Color.White;
        btnCerrar.ForeColor = Color.FromArgb(30, 45, 65);
        btnCerrar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
        btnCerrar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
        btnCerrar.UseVisualStyleBackColor = false;
        btnCerrar.MinimumSize = new Size(88, 30);
        btnCerrar.Margin = new Padding(6, 0, 0, 0);
        btnCerrar.TabIndex = 2;
        btnCerrar.Text = "Cerrar";
        btnCerrar.Tag = "cerrar";
        btnCerrar.DialogResult = DialogResult.Cancel;
        btnRecargar.AutoSize = true;
        btnRecargar.Name = "btnRecargar";
        btnRecargar.FlatStyle = FlatStyle.Flat;
        btnRecargar.BackColor = Color.White;
        btnRecargar.ForeColor = Color.FromArgb(30, 45, 65);
        btnRecargar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
        btnRecargar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
        btnRecargar.UseVisualStyleBackColor = false;
        btnRecargar.MinimumSize = new Size(88, 30);
        btnRecargar.Margin = new Padding(6, 0, 0, 0);
        btnRecargar.TabIndex = 1;
        btnRecargar.Text = "Recargar";
        btnRecargar.Tag = "recargar";
        btnRestaurar.AutoSize = true;
        btnRestaurar.Name = "btnRestaurar";
        btnRestaurar.Text = "Restaurar versión";
        btnRestaurar.Tag = "restaurar";
        btnRestaurar.Enabled = false;
        btnRestaurar.FlatStyle = FlatStyle.Flat;
        btnRestaurar.BackColor = Color.FromArgb(35, 101, 175);
        btnRestaurar.ForeColor = Color.White;
        btnRestaurar.FlatAppearance.BorderSize = 0;
        btnRestaurar.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
        btnRestaurar.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
        btnRestaurar.UseVisualStyleBackColor = false;
        btnRestaurar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRestaurar.MinimumSize = new Size(136, 30);
        btnRestaurar.Margin = new Padding(6, 0, 0, 0);
        btnRestaurar.TabIndex = 0;
        acciones.Controls.AddRange(new Control[] { btnCerrar, btnRestaurar, btnRecargar });

        layout.Controls.Add(selector, 0, 0);
        layout.Controls.Add(lblVersiones, 0, 1);
        layout.Controls.Add(dgvVersiones, 0, 2);
        layout.Controls.Add(lblDetalle, 0, 3);
        layout.Controls.Add(lblAyuda, 0, 4);
        layout.Controls.Add(dgvDetalle, 0, 5);
        lblRestauracion.Name = "lblRestauracion";
        lblRestauracion.AutoSize = true;
        lblRestauracion.Dock = DockStyle.Fill;
        lblRestauracion.ForeColor = Color.FromArgb(94, 111, 133);
        lblRestauracion.Margin = new Padding(0, 8, 0, 0);
        layout.Controls.Add(lblRestauracion, 0, 6);
        layout.Controls.Add(acciones, 0, 7);
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
