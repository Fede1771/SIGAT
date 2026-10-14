namespace SIGAT.UI
{
    partial class FrmBitacora
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridViewTextBoxColumn IdBitacora;
        private DataGridViewTextBoxColumn Fecha;
        private DataGridViewTextBoxColumn Usuario;
        private DataGridViewTextBoxColumn Actividad;
        private DataGridViewTextBoxColumn InformacionAsociada;
        private DataGridViewTextBoxColumn DigitoVerificador;


        private TableLayoutPanel tablelayoutpanel1;
        private TableLayoutPanel tablelayoutpanel2;
        private Label label3;
        private Label label4;
        private FlowLayoutPanel panelFiltros;
        private TableLayoutPanel tablelayoutpanel5;
        private CheckBox chkFechas;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private TableLayoutPanel tablelayoutpanel6;
        private Label lblUser;
        private TextBox txtUser;
        private TableLayoutPanel tablelayoutpanel7;
        private Label lblAct;
        private TextBox txtActividad;
        private Button btnBuscar;
        private Panel panel8;
        private DataGridView dgv;

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
            panelFiltros = new FlowLayoutPanel();
            tablelayoutpanel5 = new TableLayoutPanel();
            chkFechas = new CheckBox();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            tablelayoutpanel6 = new TableLayoutPanel();
            lblUser = new Label();
            txtUser = new TextBox();
            tablelayoutpanel7 = new TableLayoutPanel();
            lblAct = new Label();
            txtActividad = new TextBox();
            btnBuscar = new Button();
            panel8 = new Panel();
            dgv = new DataGridView();

            IdBitacora = new DataGridViewTextBoxColumn();
            Fecha = new DataGridViewTextBoxColumn();
            Usuario = new DataGridViewTextBoxColumn();
            Actividad = new DataGridViewTextBoxColumn();
            InformacionAsociada = new DataGridViewTextBoxColumn();
            DigitoVerificador = new DataGridViewTextBoxColumn();
            tablelayoutpanel1.SuspendLayout();
            tablelayoutpanel2.SuspendLayout();
            panelFiltros.SuspendLayout();
            tablelayoutpanel5.SuspendLayout();
            tablelayoutpanel6.SuspendLayout();
            tablelayoutpanel7.SuspendLayout();
            panel8.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // tablelayoutpanel1
            // 
            tablelayoutpanel1.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel1.ColumnCount = 1;
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Controls.Add(tablelayoutpanel2, 0, 0);
            tablelayoutpanel1.Controls.Add(panelFiltros, 0, 1);
            tablelayoutpanel1.Controls.Add(panel8, 0, 2);
            tablelayoutpanel1.Dock = DockStyle.Fill;
            tablelayoutpanel1.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel1.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel1.Location = new Point(0, 0);
            tablelayoutpanel1.Name = "tablelayoutpanel1";
            tablelayoutpanel1.Padding = new Padding(24);
            tablelayoutpanel1.RowCount = 3;
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Size = new Size(1140, 680);
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
            tablelayoutpanel2.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel2.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel2.Size = new Size(1092, 40);
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
            label3.Size = new Size(1092, 14);
            label3.TabIndex = 0;
            label3.Tag = "frmbitacora_titulo";
            label3.Text = "Bitácora";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(245, 247, 251);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 10F);
            label4.ForeColor = Color.FromArgb(94, 111, 133);
            label4.Location = new Point(0, 20);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(1092, 20);
            label4.TabIndex = 1;
            label4.Tag = "lbl_descripcion_bitacora";
            label4.Text = "Consultá la actividad del sistema y sus registros de auditoría.";
            // 
            // panelFiltros
            // 
            panelFiltros.AutoSize = true;
            panelFiltros.BackColor = Color.FromArgb(255, 255, 255);
            panelFiltros.Controls.Add(tablelayoutpanel5);
            panelFiltros.Controls.Add(tablelayoutpanel6);
            panelFiltros.Controls.Add(tablelayoutpanel7);
            panelFiltros.Controls.Add(btnBuscar);
            panelFiltros.Dock = DockStyle.Top;
            panelFiltros.Font = new Font("Segoe UI", 10F);
            panelFiltros.ForeColor = Color.FromArgb(30, 45, 65);
            panelFiltros.Location = new Point(24, 86);
            panelFiltros.Margin = new Padding(0, 0, 0, 18);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Padding = new Padding(18);
            panelFiltros.Size = new Size(1092, 106);
            panelFiltros.TabIndex = 1;
            // 
            // tablelayoutpanel5
            // 
            tablelayoutpanel5.AutoSize = true;
            tablelayoutpanel5.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel5.ColumnCount = 2;
            tablelayoutpanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            tablelayoutpanel5.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 136F));
            tablelayoutpanel5.Controls.Add(chkFechas, 0, 0);
            tablelayoutpanel5.Controls.Add(dtpDesde, 0, 1);
            tablelayoutpanel5.Controls.Add(dtpHasta, 1, 1);
            tablelayoutpanel5.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel5.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel5.Location = new Point(18, 18);
            tablelayoutpanel5.Margin = new Padding(0, 0, 16, 12);
            tablelayoutpanel5.Name = "tablelayoutpanel5";
            tablelayoutpanel5.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel5.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel5.Size = new Size(272, 40);
            tablelayoutpanel5.TabIndex = 0;
            // 
            // chkFechas
            // 
            chkFechas.AutoSize = true;
            chkFechas.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel5.SetColumnSpan(chkFechas, 2);
            chkFechas.Font = new Font("Segoe UI", 10F);
            chkFechas.ForeColor = Color.FromArgb(30, 45, 65);
            chkFechas.Location = new Point(0, 0);
            chkFechas.Margin = new Padding(0, 0, 0, 8);
            chkFechas.Name = "chkFechas";
            chkFechas.Size = new Size(130, 12);
            chkFechas.TabIndex = 0;
            chkFechas.Tag = "chk_fechas";
            chkFechas.Text = "Filtrar por fechas";
            chkFechas.UseVisualStyleBackColor = false;
            // 
            // dtpDesde
            // 
            dtpDesde.BackColor = Color.FromArgb(255, 255, 255);
            dtpDesde.Font = new Font("Segoe UI", 10F);
            dtpDesde.ForeColor = Color.FromArgb(0, 0, 0);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(0, 20);
            dtpDesde.Margin = new Padding(0);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(126, 25);
            dtpDesde.TabIndex = 1;
            // 
            // dtpHasta
            // 
            dtpHasta.BackColor = Color.FromArgb(255, 255, 255);
            dtpHasta.Font = new Font("Segoe UI", 10F);
            dtpHasta.ForeColor = Color.FromArgb(0, 0, 0);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(136, 20);
            dtpHasta.Margin = new Padding(0);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(126, 25);
            dtpHasta.TabIndex = 2;
            // 
            // tablelayoutpanel6
            // 
            tablelayoutpanel6.AutoSize = true;
            tablelayoutpanel6.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel6.ColumnCount = 1;
            tablelayoutpanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 178F));
            tablelayoutpanel6.Controls.Add(lblUser, 0, 0);
            tablelayoutpanel6.Controls.Add(txtUser, 0, 1);
            tablelayoutpanel6.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel6.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel6.Location = new Point(306, 18);
            tablelayoutpanel6.Margin = new Padding(0, 0, 16, 12);
            tablelayoutpanel6.Name = "tablelayoutpanel6";
            tablelayoutpanel6.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel6.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel6.Size = new Size(178, 40);
            tablelayoutpanel6.TabIndex = 1;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.BackColor = Color.FromArgb(255, 255, 255);
            lblUser.Dock = DockStyle.Fill;
            lblUser.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUser.ForeColor = Color.FromArgb(30, 45, 65);
            lblUser.Location = new Point(0, 0);
            lblUser.Margin = new Padding(0, 0, 0, 6);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(178, 14);
            lblUser.TabIndex = 0;
            lblUser.Tag = "lbl_usuario";
            lblUser.Text = "Usuario";
            // 
            // txtUser
            // 
            txtUser.BackColor = Color.FromArgb(255, 255, 255);
            txtUser.Dock = DockStyle.Fill;
            txtUser.Font = new Font("Segoe UI", 10F);
            txtUser.ForeColor = Color.FromArgb(0, 0, 0);
            txtUser.Location = new Point(3, 23);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(172, 25);
            txtUser.TabIndex = 1;
            // 
            // tablelayoutpanel7
            // 
            tablelayoutpanel7.AutoSize = true;
            tablelayoutpanel7.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel7.ColumnCount = 1;
            tablelayoutpanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 208F));
            tablelayoutpanel7.Controls.Add(lblAct, 0, 0);
            tablelayoutpanel7.Controls.Add(txtActividad, 0, 1);
            tablelayoutpanel7.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel7.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel7.Location = new Point(500, 18);
            tablelayoutpanel7.Margin = new Padding(0, 0, 16, 12);
            tablelayoutpanel7.Name = "tablelayoutpanel7";
            tablelayoutpanel7.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel7.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            tablelayoutpanel7.Size = new Size(208, 40);
            tablelayoutpanel7.TabIndex = 2;
            // 
            // lblAct
            // 
            lblAct.AutoSize = true;
            lblAct.BackColor = Color.FromArgb(255, 255, 255);
            lblAct.Dock = DockStyle.Fill;
            lblAct.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblAct.ForeColor = Color.FromArgb(30, 45, 65);
            lblAct.Location = new Point(0, 0);
            lblAct.Margin = new Padding(0, 0, 0, 6);
            lblAct.Name = "lblAct";
            lblAct.Size = new Size(208, 14);
            lblAct.TabIndex = 0;
            lblAct.Tag = "lbl_actividad";
            lblAct.Text = "Actividad";
            // 
            // txtActividad
            // 
            txtActividad.BackColor = Color.FromArgb(255, 255, 255);
            txtActividad.Dock = DockStyle.Fill;
            txtActividad.Font = new Font("Segoe UI", 10F);
            txtActividad.ForeColor = Color.FromArgb(0, 0, 0);
            txtActividad.Location = new Point(3, 23);
            txtActividad.Name = "txtActividad";
            txtActividad.Size = new Size(202, 25);
            txtActividad.TabIndex = 1;
            // 
            // btnBuscar
            // 
            btnBuscar.AutoSize = true;
            btnBuscar.BackColor = Color.FromArgb(35, 101, 175);
            btnBuscar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnBuscar.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnBuscar.ForeColor = Color.FromArgb(255, 255, 255);
            btnBuscar.Location = new Point(724, 38);
            btnBuscar.Margin = new Padding(0, 20, 0, 10);
            btnBuscar.MinimumSize = new Size(96, 40);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(96, 40);
            btnBuscar.TabIndex = 3;
            btnBuscar.Tag = "btn_buscar";
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            btnBuscar.Click += BtnBuscar_Click;
            // 
            // panel8
            // 
            panel8.BackColor = Color.FromArgb(255, 255, 255);
            panel8.Controls.Add(dgv);
            panel8.Dock = DockStyle.Fill;
            panel8.Font = new Font("Segoe UI", 10F);
            panel8.ForeColor = Color.FromArgb(30, 45, 65);
            panel8.Location = new Point(24, 210);
            panel8.Margin = new Padding(0);
            panel8.Name = "panel8";
            panel8.Size = new Size(1092, 446);
            panel8.TabIndex = 2;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 251, 254);
            dgv.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 10, 0);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgv.ColumnHeadersHeight = 44;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.Padding = new Padding(10, 5, 10, 5);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 240, 252);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle = dataGridViewCellStyle3;
            dgv.Dock = DockStyle.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.Font = new Font("Segoe UI", 10F);
            dgv.GridColor = Color.FromArgb(234, 239, 246);
            dgv.Location = new Point(0, 0);
            dgv.MultiSelect = false;
            IdBitacora.Name = "IdBitacora";
            IdBitacora.DataPropertyName = "IdBitacora";
            IdBitacora.HeaderText = "ID";
            IdBitacora.FillWeight = 45F;
            IdBitacora.MinimumWidth = 45;
            IdBitacora.ReadOnly = true;
            IdBitacora.Visible = true;
            IdBitacora.SortMode = DataGridViewColumnSortMode.NotSortable;
            IdBitacora.Tag = "col_id";
            Fecha.Name = "Fecha";
            Fecha.DataPropertyName = "Fecha";
            Fecha.HeaderText = "Fecha";
            Fecha.FillWeight = 140F;
            Fecha.MinimumWidth = 150;
            Fecha.ReadOnly = true;
            Fecha.Visible = true;
            Fecha.SortMode = DataGridViewColumnSortMode.NotSortable;
            Fecha.Tag = "col_fecha";
            Fecha.DefaultCellStyle.Format = "dd/MM/yyyy HH:mm";
            Usuario.Name = "Usuario";
            Usuario.DataPropertyName = "Usuario";
            Usuario.HeaderText = "Usuario";
            Usuario.FillWeight = 100F;
            Usuario.MinimumWidth = 80;
            Usuario.ReadOnly = true;
            Usuario.Visible = true;
            Usuario.SortMode = DataGridViewColumnSortMode.NotSortable;
            Usuario.Tag = "col_usuario";
            Actividad.Name = "Actividad";
            Actividad.DataPropertyName = "Actividad";
            Actividad.HeaderText = "Actividad";
            Actividad.FillWeight = 100F;
            Actividad.MinimumWidth = 90;
            Actividad.ReadOnly = true;
            Actividad.Visible = true;
            Actividad.SortMode = DataGridViewColumnSortMode.NotSortable;
            Actividad.Tag = "col_actividad";
            InformacionAsociada.Name = "InformacionAsociada";
            InformacionAsociada.DataPropertyName = "InformacionAsociada";
            InformacionAsociada.HeaderText = "Información";
            InformacionAsociada.FillWeight = 220F;
            InformacionAsociada.MinimumWidth = 120;
            InformacionAsociada.ReadOnly = true;
            InformacionAsociada.Visible = true;
            InformacionAsociada.SortMode = DataGridViewColumnSortMode.NotSortable;
            InformacionAsociada.Tag = "col_informacion";
            DigitoVerificador.Name = "DigitoVerificador";
            DigitoVerificador.DataPropertyName = "DigitoVerificador";
            DigitoVerificador.HeaderText = "Dígito verificador (DVH)";
            DigitoVerificador.FillWeight = 260F;
            DigitoVerificador.MinimumWidth = 340;
            DigitoVerificador.ReadOnly = true;
            DigitoVerificador.Visible = true;
            DigitoVerificador.SortMode = DataGridViewColumnSortMode.NotSortable;
            DigitoVerificador.Tag = "col_dvh";
            dgv.Columns.AddRange(new DataGridViewColumn[] { IdBitacora, Fecha, Usuario, Actividad, InformacionAsociada, DigitoVerificador });
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.RowTemplate.Height = 40;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new Size(1092, 446);
            dgv.TabIndex = 0;
            // 
            // FrmBitacora
            // 
            AcceptButton = btnBuscar;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1140, 680);
            Controls.Add(tablelayoutpanel1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MinimumSize = new Size(860, 540);
            Name = "FrmBitacora";
            Tag = "frmbitacora_titulo";
            Text = "Consulta de bitácora";
            tablelayoutpanel1.ResumeLayout(false);
            tablelayoutpanel1.PerformLayout();
            tablelayoutpanel2.ResumeLayout(false);
            tablelayoutpanel2.PerformLayout();
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            tablelayoutpanel5.ResumeLayout(false);
            tablelayoutpanel5.PerformLayout();
            tablelayoutpanel6.ResumeLayout(false);
            tablelayoutpanel6.PerformLayout();
            tablelayoutpanel7.ResumeLayout(false);
            tablelayoutpanel7.PerformLayout();
            panel8.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }
    }
}
