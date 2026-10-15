namespace SIGAT.UI
{
    partial class FrmIntegridad
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridViewTextBoxColumn Tabla;
        private DataGridViewTextBoxColumn Registro;
        private DataGridViewTextBoxColumn Motivo;


        private TableLayoutPanel tablelayoutpanel1;
        private TableLayoutPanel tablelayoutpanel2;
        private Label label3;
        private Label label4;
        private Label estado;
        private DataGridView grilla;
        private TableLayoutPanel tablelayoutpanel7;
        private TextBox archivo;
        private Button btnElegirRespaldo;
        private CheckBox completas;
        private FlowLayoutPanel flowlayoutpanel8;
        private Button btnComprobar;
        private Button btnInicializar;
        private Button btnBackupVerificado;
        private Button btnRecuperar;
        private Button btnRecalcular;
        private Button btnArchivar;
        private Button btnContinuar;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        // Estructura visual editable desde el diseÃ±ador; el comportamiento queda en el .cs.
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            tablelayoutpanel1 = new TableLayoutPanel();
            tablelayoutpanel2 = new TableLayoutPanel();
            label3 = new Label();
            label4 = new Label();
            estado = new Label();
            grilla = new DataGridView();
            Tabla = new DataGridViewTextBoxColumn();
            Registro = new DataGridViewTextBoxColumn();
            Motivo = new DataGridViewTextBoxColumn();
            tablelayoutpanel7 = new TableLayoutPanel();
            archivo = new TextBox();
            btnElegirRespaldo = new Button();
            completas = new CheckBox();
            flowlayoutpanel8 = new FlowLayoutPanel();
            btnComprobar = new Button();
            btnInicializar = new Button();
            btnBackupVerificado = new Button();
            btnRecuperar = new Button();
            btnRecalcular = new Button();
            btnArchivar = new Button();
            btnContinuar = new Button();
            tablelayoutpanel1.SuspendLayout();
            tablelayoutpanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grilla).BeginInit();
            tablelayoutpanel7.SuspendLayout();
            flowlayoutpanel8.SuspendLayout();
            SuspendLayout();
            // 
            // tablelayoutpanel1
            // 
            tablelayoutpanel1.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel1.ColumnCount = 1;
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Controls.Add(tablelayoutpanel2, 0, 0);
            tablelayoutpanel1.Controls.Add(estado, 0, 1);
            tablelayoutpanel1.Controls.Add(grilla, 0, 2);
            tablelayoutpanel1.Controls.Add(tablelayoutpanel7, 0, 3);
            tablelayoutpanel1.Controls.Add(completas, 0, 4);
            tablelayoutpanel1.Controls.Add(flowlayoutpanel8, 0, 5);
            tablelayoutpanel1.Dock = DockStyle.Fill;
            tablelayoutpanel1.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel1.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel1.Location = new Point(0, 0);
            tablelayoutpanel1.Name = "tablelayoutpanel1";
            tablelayoutpanel1.Padding = new Padding(24);
            tablelayoutpanel1.RowCount = 6;
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.Size = new Size(1040, 670);
            tablelayoutpanel1.TabIndex = 0;
            // 
            // tablelayoutpanel2
            // 
            tablelayoutpanel2.AutoSize = true;
            tablelayoutpanel2.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel2.ColumnCount = 1;
            tablelayoutpanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel2.Controls.Add(label3, 0, 0);
            tablelayoutpanel2.Controls.Add(label4, 0, 1);
            tablelayoutpanel2.Dock = DockStyle.Top;
            tablelayoutpanel2.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel2.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel2.Location = new Point(24, 24);
            tablelayoutpanel2.Margin = new Padding(0, 0, 0, 22);
            tablelayoutpanel2.Name = "tablelayoutpanel2";
            tablelayoutpanel2.RowStyles.Add(new RowStyle());
            tablelayoutpanel2.RowStyles.Add(new RowStyle());
            tablelayoutpanel2.Size = new Size(992, 66);
            tablelayoutpanel2.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(245, 247, 251);
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(30, 45, 65);
            label3.Location = new Point(0, 0);
            label3.Margin = new Padding(0, 0, 0, 6);
            label3.Name = "label3";
            label3.Size = new Size(992, 41);
            label3.TabIndex = 0;
            label3.Text = "Integridad y recuperación";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(245, 247, 251);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 10F);
            label4.ForeColor = Color.FromArgb(94, 111, 133);
            label4.Location = new Point(0, 47);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(992, 19);
            label4.TabIndex = 1;
            label4.Text = "SHA-256: verificar Usuarios y UsuarioRol, comprobar un respaldo y recuperar las cuentas y asignaciones.";
            // 
            // estado
            // 
            estado.AutoSize = true;
            estado.BackColor = Color.FromArgb(245, 247, 251);
            estado.Dock = DockStyle.Fill;
            estado.Font = new Font("Segoe UI", 10F);
            estado.ForeColor = Color.FromArgb(30, 45, 65);
            estado.Location = new Point(24, 112);
            estado.Margin = new Padding(0, 0, 0, 14);
            estado.Name = "estado";
            estado.Padding = new Padding(12);
            estado.Size = new Size(992, 43);
            estado.TabIndex = 1;
            // 
            // grilla
            // 
            grilla.AllowUserToAddRows = false;
            grilla.AllowUserToDeleteRows = false;
            grilla.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 251, 254);
            grilla.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            grilla.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grilla.BackgroundColor = Color.White;
            grilla.BorderStyle = BorderStyle.None;
            grilla.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 10, 0);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 45, 65);
            grilla.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            grilla.ColumnHeadersHeight = 44;
            grilla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grilla.Columns.AddRange(new DataGridViewColumn[] { Tabla, Registro, Motivo });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.Padding = new Padding(10, 5, 10, 5);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 240, 252);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            grilla.DefaultCellStyle = dataGridViewCellStyle3;
            grilla.Dock = DockStyle.Fill;
            grilla.EnableHeadersVisualStyles = false;
            grilla.Font = new Font("Segoe UI", 10F);
            grilla.GridColor = Color.FromArgb(234, 239, 246);
            grilla.Location = new Point(27, 172);
            grilla.MultiSelect = false;
            grilla.Name = "grilla";
            grilla.ReadOnly = true;
            grilla.RowHeadersVisible = false;
            grilla.RowTemplate.Height = 40;
            grilla.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grilla.Size = new Size(986, 300);
            grilla.TabIndex = 2;
            // 
            // Tabla
            // 
            Tabla.DataPropertyName = "Tabla";
            Tabla.HeaderText = "Tabla";
            Tabla.Name = "Tabla";
            Tabla.ReadOnly = true;
            Tabla.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Registro
            // 
            Registro.DataPropertyName = "Registro";
            Registro.HeaderText = "Registro";
            Registro.Name = "Registro";
            Registro.ReadOnly = true;
            Registro.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Motivo
            // 
            Motivo.DataPropertyName = "Motivo";
            Motivo.HeaderText = "Motivo";
            Motivo.Name = "Motivo";
            Motivo.ReadOnly = true;
            Motivo.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // tablelayoutpanel7
            // 
            tablelayoutpanel7.AutoSize = true;
            tablelayoutpanel7.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel7.ColumnCount = 2;
            tablelayoutpanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel7.ColumnStyles.Add(new ColumnStyle());
            tablelayoutpanel7.Controls.Add(archivo, 0, 0);
            tablelayoutpanel7.Controls.Add(btnElegirRespaldo, 1, 0);
            tablelayoutpanel7.Dock = DockStyle.Top;
            tablelayoutpanel7.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel7.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel7.Location = new Point(27, 478);
            tablelayoutpanel7.Name = "tablelayoutpanel7";
            tablelayoutpanel7.Padding = new Padding(0, 14, 0, 8);
            tablelayoutpanel7.RowStyles.Add(new RowStyle());
            tablelayoutpanel7.Size = new Size(986, 70);
            tablelayoutpanel7.TabIndex = 3;
            // 
            // archivo
            // 
            archivo.BackColor = Color.FromArgb(255, 255, 255);
            archivo.Dock = DockStyle.Fill;
            archivo.Font = new Font("Segoe UI", 10F);
            archivo.ForeColor = Color.FromArgb(0, 0, 0);
            archivo.Location = new Point(3, 17);
            archivo.Name = "archivo";
            archivo.PlaceholderText = "Ruta del .bak en el equipo donde corre SQL Server";
            archivo.Size = new Size(850, 25);
            archivo.TabIndex = 0;
            // 
            // btnElegirRespaldo
            // 
            btnElegirRespaldo.AutoSize = true;
            btnElegirRespaldo.BackColor = Color.FromArgb(255, 255, 255);
            btnElegirRespaldo.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnElegirRespaldo.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnElegirRespaldo.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnElegirRespaldo.FlatStyle = FlatStyle.Flat;
            btnElegirRespaldo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnElegirRespaldo.ForeColor = Color.FromArgb(30, 45, 65);
            btnElegirRespaldo.Location = new Point(856, 14);
            btnElegirRespaldo.Margin = new Padding(0, 0, 8, 8);
            btnElegirRespaldo.MinimumSize = new Size(96, 40);
            btnElegirRespaldo.Name = "btnElegirRespaldo";
            btnElegirRespaldo.Size = new Size(122, 40);
            btnElegirRespaldo.TabIndex = 1;
            btnElegirRespaldo.Text = "Elegir respaldo";
            btnElegirRespaldo.UseVisualStyleBackColor = false;
            // 
            // completas
            // 
            completas.AutoSize = true;
            completas.BackColor = Color.FromArgb(245, 247, 251);
            completas.Font = new Font("Segoe UI", 10F);
            completas.ForeColor = Color.FromArgb(30, 45, 65);
            completas.Location = new Point(27, 554);
            completas.Name = "completas";
            completas.Size = new Size(556, 23);
            completas.TabIndex = 4;
            completas.Text = "Restaurar Usuarios y UsuarioRol completos (descarta sus cambios posteriores al respaldo)";
            completas.UseVisualStyleBackColor = false;
            // 
            // flowlayoutpanel8
            // 
            flowlayoutpanel8.AutoSize = true;
            flowlayoutpanel8.BackColor = Color.FromArgb(245, 247, 251);
            flowlayoutpanel8.Controls.Add(btnComprobar);
            flowlayoutpanel8.Controls.Add(btnInicializar);
            flowlayoutpanel8.Controls.Add(btnBackupVerificado);
            flowlayoutpanel8.Controls.Add(btnRecuperar);
            flowlayoutpanel8.Controls.Add(btnRecalcular);
            flowlayoutpanel8.Controls.Add(btnArchivar);
            flowlayoutpanel8.Controls.Add(btnContinuar);
            flowlayoutpanel8.Dock = DockStyle.Top;
            flowlayoutpanel8.Font = new Font("Segoe UI", 10F);
            flowlayoutpanel8.ForeColor = Color.FromArgb(30, 45, 65);
            flowlayoutpanel8.Location = new Point(27, 583);
            flowlayoutpanel8.Name = "flowlayoutpanel8";
            flowlayoutpanel8.Padding = new Padding(0, 12, 0, 0);
            flowlayoutpanel8.Size = new Size(986, 60);
            flowlayoutpanel8.TabIndex = 5;
            // 
            // btnComprobar
            // 
            btnComprobar.AutoSize = true;
            btnComprobar.BackColor = Color.FromArgb(255, 255, 255);
            btnComprobar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnComprobar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnComprobar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnComprobar.FlatStyle = FlatStyle.Flat;
            btnComprobar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnComprobar.ForeColor = Color.FromArgb(30, 45, 65);
            btnComprobar.Location = new Point(0, 12);
            btnComprobar.Margin = new Padding(0, 0, 8, 8);
            btnComprobar.MinimumSize = new Size(96, 40);
            btnComprobar.Name = "btnComprobar";
            btnComprobar.Size = new Size(96, 40);
            btnComprobar.TabIndex = 0;
            btnComprobar.Text = "Verificar";
            btnComprobar.UseVisualStyleBackColor = false;
            // 
            // btnInicializar
            // 
            btnInicializar.AutoSize = true;
            btnInicializar.BackColor = Color.FromArgb(255, 255, 255);
            btnInicializar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnInicializar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnInicializar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnInicializar.FlatStyle = FlatStyle.Flat;
            btnInicializar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnInicializar.ForeColor = Color.FromArgb(30, 45, 65);
            btnInicializar.Location = new Point(104, 12);
            btnInicializar.Margin = new Padding(0, 0, 8, 8);
            btnInicializar.MinimumSize = new Size(96, 40);
            btnInicializar.Name = "btnInicializar";
            btnInicializar.Size = new Size(176, 40);
            btnInicializar.TabIndex = 1;
            btnInicializar.Text = "Preparar control inicial";
            btnInicializar.UseVisualStyleBackColor = false;
            // 
            // btnBackupVerificado
            // 
            btnBackupVerificado.AutoSize = true;
            btnBackupVerificado.BackColor = Color.FromArgb(255, 255, 255);
            btnBackupVerificado.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnBackupVerificado.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnBackupVerificado.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnBackupVerificado.FlatStyle = FlatStyle.Flat;
            btnBackupVerificado.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBackupVerificado.ForeColor = Color.FromArgb(30, 45, 65);
            btnBackupVerificado.Location = new Point(288, 12);
            btnBackupVerificado.Margin = new Padding(0, 0, 8, 8);
            btnBackupVerificado.MinimumSize = new Size(96, 40);
            btnBackupVerificado.Name = "btnBackupVerificado";
            btnBackupVerificado.Size = new Size(213, 40);
            btnBackupVerificado.TabIndex = 2;
            btnBackupVerificado.Text = "Crear respaldo comprobado";
            btnBackupVerificado.UseVisualStyleBackColor = false;
            // 
            // btnRecuperar
            // 
            btnRecuperar.AutoSize = true;
            btnRecuperar.BackColor = Color.FromArgb(35, 101, 175);
            btnRecuperar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnRecuperar.FlatAppearance.BorderSize = 0;
            btnRecuperar.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnRecuperar.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnRecuperar.FlatStyle = FlatStyle.Flat;
            btnRecuperar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRecuperar.ForeColor = Color.FromArgb(255, 255, 255);
            btnRecuperar.Location = new Point(509, 12);
            btnRecuperar.Margin = new Padding(0, 0, 8, 8);
            btnRecuperar.MinimumSize = new Size(96, 40);
            btnRecuperar.Name = "btnRecuperar";
            btnRecuperar.Size = new Size(96, 40);
            btnRecuperar.TabIndex = 3;
            btnRecuperar.Text = "Recuperar";
            btnRecuperar.UseVisualStyleBackColor = false;
            // 
            // btnArchivar
            // 
            btnArchivar.AutoSize = true;
            btnArchivar.BackColor = Color.FromArgb(255, 255, 255);
            btnArchivar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnArchivar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnArchivar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnArchivar.FlatStyle = FlatStyle.Flat;
            btnArchivar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnArchivar.ForeColor = Color.FromArgb(30, 45, 65);
            btnArchivar.Location = new Point(613, 12);
            btnArchivar.Margin = new Padding(0, 0, 8, 8);
            btnArchivar.MinimumSize = new Size(96, 40);
            btnArchivar.Name = "btnArchivar";
            btnArchivar.Size = new Size(192, 40);
            btnArchivar.TabIndex = 4;
            btnArchivar.Text = "Archivar bitácora antigua";
            btnArchivar.UseVisualStyleBackColor = false;
            // 
            // btnRecalcular
            //
            btnRecalcular.AutoSize = true;
            btnRecalcular.BackColor = Color.FromArgb(255, 245, 219);
            btnRecalcular.ForeColor = Color.FromArgb(132, 84, 17);
            btnRecalcular.FlatStyle = FlatStyle.Flat;
            btnRecalcular.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnRecalcular.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnRecalcular.Margin = new Padding(0, 0, 8, 8);
            btnRecalcular.MinimumSize = new Size(96, 40);
            btnRecalcular.Name = "btnRecalcular";
            btnRecalcular.Text = "Recalcular dígitos (asumir pérdida)";
            btnRecalcular.UseVisualStyleBackColor = false;
            //
            // btnContinuar
            // 
            btnContinuar.AutoSize = true;
            btnContinuar.BackColor = Color.FromArgb(255, 255, 255);
            btnContinuar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnContinuar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnContinuar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnContinuar.FlatStyle = FlatStyle.Flat;
            btnContinuar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnContinuar.ForeColor = Color.FromArgb(30, 45, 65);
            btnContinuar.Location = new Point(813, 12);
            btnContinuar.Margin = new Padding(0, 0, 8, 8);
            btnContinuar.MinimumSize = new Size(96, 40);
            btnContinuar.Name = "btnContinuar";
            btnContinuar.Size = new Size(96, 40);
            btnContinuar.TabIndex = 5;
            btnContinuar.Text = "Cerrar";
            btnContinuar.UseVisualStyleBackColor = false;
            // 
            // FrmIntegridad
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1040, 670);
            Controls.Add(tablelayoutpanel1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MinimumSize = new Size(880, 620);
            Name = "FrmIntegridad";
            StartPosition = FormStartPosition.CenterParent;
            Text = "SIGAT · Integridad y recuperación";
            tablelayoutpanel1.ResumeLayout(false);
            tablelayoutpanel1.PerformLayout();
            tablelayoutpanel2.ResumeLayout(false);
            tablelayoutpanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)grilla).EndInit();
            tablelayoutpanel7.ResumeLayout(false);
            tablelayoutpanel7.PerformLayout();
            flowlayoutpanel8.ResumeLayout(false);
            flowlayoutpanel8.PerformLayout();
            ResumeLayout(false);
        }
    }
}
