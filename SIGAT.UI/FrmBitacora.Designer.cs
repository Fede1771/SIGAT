namespace SIGAT.UI
{
    partial class FrmBitacora
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            panelFiltros = new Panel();
            chkFechas = new CheckBox();
            dtpDesde = new DateTimePicker();
            dtpHasta = new DateTimePicker();
            lblUser = new Label();
            txtUser = new TextBox();
            lblAct = new Label();
            txtActividad = new TextBox();
            btnBuscar = new Button();
            dgv = new DataGridView();
            panelFiltros.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            SuspendLayout();
            // 
            // panelFiltros
            // 
            panelFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFiltros.BackColor = Color.Transparent;
            panelFiltros.Controls.Add(chkFechas);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(lblUser);
            panelFiltros.Controls.Add(txtUser);
            panelFiltros.Controls.Add(lblAct);
            panelFiltros.Controls.Add(txtActividad);
            panelFiltros.Controls.Add(btnBuscar);
            panelFiltros.Location = new Point(40, 30);
            panelFiltros.Name = "panelFiltros";
            panelFiltros.Size = new Size(899, 60);
            panelFiltros.TabIndex = 0;
            // 
            // chkFechas
            // 
            chkFechas.Font = new Font("Segoe UI", 9F);
            chkFechas.Location = new Point(10, 18);
            chkFechas.Name = "chkFechas";
            chkFechas.Size = new Size(70, 24);
            chkFechas.TabIndex = 0;
            chkFechas.Text = "Fechas:";
            // 
            // dtpDesde
            // 
            dtpDesde.Font = new Font("Segoe UI", 9F);
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Location = new Point(85, 16);
            dtpDesde.Name = "dtpDesde";
            dtpDesde.Size = new Size(110, 27);
            dtpDesde.TabIndex = 1;
            // 
            // dtpHasta
            // 
            dtpHasta.Font = new Font("Segoe UI", 9F);
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Location = new Point(205, 16);
            dtpHasta.Name = "dtpHasta";
            dtpHasta.Size = new Size(110, 27);
            dtpHasta.TabIndex = 2;
            // 
            // lblUser
            // 
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUser.Location = new Point(340, 19);
            lblUser.Name = "lblUser";
            lblUser.Size = new Size(67, 20);
            lblUser.TabIndex = 3;
            lblUser.Text = "Usuario:";
            // 
            // txtUser
            // 
            txtUser.Font = new Font("Segoe UI", 9F);
            txtUser.Location = new Point(400, 16);
            txtUser.Name = "txtUser";
            txtUser.Size = new Size(130, 27);
            txtUser.TabIndex = 4;
            // 
            // lblAct
            // 
            lblAct.AutoSize = true;
            lblAct.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblAct.Location = new Point(550, 19);
            lblAct.Name = "lblAct";
            lblAct.Size = new Size(79, 20);
            lblAct.TabIndex = 5;
            lblAct.Text = "Actividad:";
            // 
            // txtActividad
            // 
            txtActividad.Font = new Font("Segoe UI", 9F);
            txtActividad.Location = new Point(620, 16);
            txtActividad.Name = "txtActividad";
            txtActividad.Size = new Size(130, 27);
            txtActividad.TabIndex = 6;
            // 
            // btnBuscar
            // 
            btnBuscar.BackColor = Color.White;
            btnBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnBuscar.Location = new Point(770, 14);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(100, 30);
            btnBuscar.TabIndex = 7;
            btnBuscar.Text = "Buscar";
            btnBuscar.UseVisualStyleBackColor = false;
            btnBuscar.Click += BtnBuscar_Click;
            // 
            // dgv
            // 
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.ColumnHeadersHeight = 29;
            dgv.Location = new Point(40, 110);
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersWidth = 51;
            dgv.Size = new Size(1459, 300);
            dgv.TabIndex = 1;
            // 
            // FrmBitacora
            // 
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1539, 460);
            Controls.Add(panelFiltros);
            Controls.Add(dgv);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmBitacora";
            Text = "Consulta de Bitácora";
            WindowState = FormWindowState.Maximized;
            panelFiltros.ResumeLayout(false);
            panelFiltros.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelFiltros;
        private CheckBox chkFechas;
        private DateTimePicker dtpDesde;
        private DateTimePicker dtpHasta;
        private Label lblUser;
        private TextBox txtUser;
        private Label lblAct;
        private TextBox txtActividad;
        private Button btnBuscar;
        private DataGridView dgv;
    }
}
