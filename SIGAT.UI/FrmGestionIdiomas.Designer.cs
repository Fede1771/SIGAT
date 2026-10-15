namespace SIGAT.UI
{
    partial class FrmGestionIdiomas
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridViewTextBoxColumn FormNombre;
        private DataGridViewTextBoxColumn ControlNombre;
        private DataGridViewTextBoxColumn TextoBase;
        private DataGridViewTextBoxColumn Texto;
        private DataGridViewTextBoxColumn Estado;
        private DataGridViewTextBoxColumn IdIdioma;
        private DataGridViewTextBoxColumn IdControl;


        private TableLayoutPanel tablelayoutpanel1;
        private TableLayoutPanel tablelayoutpanel2;
        private Label label3;
        private Label label4;
        private FlowLayoutPanel flowlayoutpanel5;
        private TableLayoutPanel tablelayoutpanel6;
        private Label lblIdiomaActivo;
        private ComboBox cmbIdiomas;
        private Button btnAplicarIdioma;
        private Button btnCambiarEstado;
        private Panel panel7;
        private TableLayoutPanel tablelayoutpanel8;
        private Label label9;
        private Label lblNuevoIdioma;
        private TextBox txtNuevoIdioma;
        private Label lblCodigoIdioma;
        private TextBox txtCodigoIdioma;
        private Label lblNombreNativo;
        private TextBox txtNombreNativo;
        private CheckBox chkIdiomaActivo;
        private Button btnNuevoIdioma;
        private Label label10;
        private TableLayoutPanel tablelayoutpanel11;
        private Label label12;
        private DataGridView dgvTraducciones;
        private FlowLayoutPanel flowlayoutpanel15;
        private Button btnGuardarTraduccion;
        private Button btnExportarTraducciones;
        private Button btnImportarTraducciones;

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
            flowlayoutpanel5 = new FlowLayoutPanel();
            tablelayoutpanel6 = new TableLayoutPanel();
            lblIdiomaActivo = new Label();
            cmbIdiomas = new ComboBox();
            btnAplicarIdioma = new Button();
            btnCambiarEstado = new Button();
            panel7 = new Panel();
            tablelayoutpanel8 = new TableLayoutPanel();
            label9 = new Label();
            lblNuevoIdioma = new Label();
            txtNuevoIdioma = new TextBox();
            lblCodigoIdioma = new Label();
            txtCodigoIdioma = new TextBox();
            lblNombreNativo = new Label();
            txtNombreNativo = new TextBox();
            chkIdiomaActivo = new CheckBox();
            btnNuevoIdioma = new Button();
            label10 = new Label();
            tablelayoutpanel11 = new TableLayoutPanel();
            label12 = new Label();
            dgvTraducciones = new DataGridView();
            FormNombre = new DataGridViewTextBoxColumn();
            ControlNombre = new DataGridViewTextBoxColumn();
            TextoBase = new DataGridViewTextBoxColumn();
            Texto = new DataGridViewTextBoxColumn();
            Estado = new DataGridViewTextBoxColumn();
            IdIdioma = new DataGridViewTextBoxColumn();
            IdControl = new DataGridViewTextBoxColumn();
            flowlayoutpanel15 = new FlowLayoutPanel();
            btnGuardarTraduccion = new Button();
            btnExportarTraducciones = new Button();
            btnImportarTraducciones = new Button();
            tablelayoutpanel1.SuspendLayout();
            tablelayoutpanel2.SuspendLayout();
            flowlayoutpanel5.SuspendLayout();
            tablelayoutpanel6.SuspendLayout();
            panel7.SuspendLayout();
            tablelayoutpanel8.SuspendLayout();
            tablelayoutpanel11.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTraducciones).BeginInit();
            flowlayoutpanel15.SuspendLayout();
            SuspendLayout();
            // 
            // tablelayoutpanel1
            // 
            tablelayoutpanel1.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel1.ColumnCount = 2;
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 264F));
            tablelayoutpanel1.Controls.Add(tablelayoutpanel2, 0, 0);
            tablelayoutpanel1.Controls.Add(flowlayoutpanel5, 0, 1);
            tablelayoutpanel1.Controls.Add(panel7, 1, 2);
            tablelayoutpanel1.Controls.Add(tablelayoutpanel11, 0, 2);
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
            tablelayoutpanel1.Size = new Size(1160, 740);
            tablelayoutpanel1.TabIndex = 0;
            // 
            // tablelayoutpanel2
            // 
            tablelayoutpanel2.AutoSize = true;
            tablelayoutpanel2.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel2.ColumnCount = 1;
            tablelayoutpanel1.SetColumnSpan(tablelayoutpanel2, 2);
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
            tablelayoutpanel2.Size = new Size(1112, 66);
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
            label3.Size = new Size(1112, 41);
            label3.TabIndex = 0;
            label3.Tag = "frmgestionidiomas_titulo";
            label3.Text = "Gestión de idiomas";
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
            label4.Size = new Size(1112, 19);
            label4.TabIndex = 1;
            label4.Tag = "lbl_descripcion_idiomas";
            label4.Text = "Administrá idiomas y completá las traducciones del sistema.";
            // 
            // flowlayoutpanel5
            // 
            flowlayoutpanel5.AutoSize = true;
            flowlayoutpanel5.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel1.SetColumnSpan(flowlayoutpanel5, 2);
            flowlayoutpanel5.Controls.Add(tablelayoutpanel6);
            flowlayoutpanel5.Controls.Add(btnAplicarIdioma);
            flowlayoutpanel5.Controls.Add(btnCambiarEstado);
            flowlayoutpanel5.Dock = DockStyle.Top;
            flowlayoutpanel5.Font = new Font("Segoe UI", 10F);
            flowlayoutpanel5.ForeColor = Color.FromArgb(30, 45, 65);
            flowlayoutpanel5.Location = new Point(24, 112);
            flowlayoutpanel5.Margin = new Padding(0, 0, 0, 18);
            flowlayoutpanel5.Name = "flowlayoutpanel5";
            flowlayoutpanel5.Padding = new Padding(16);
            flowlayoutpanel5.Size = new Size(1112, 96);
            flowlayoutpanel5.TabIndex = 1;
            // 
            // tablelayoutpanel6
            // 
            tablelayoutpanel6.AutoSize = true;
            tablelayoutpanel6.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel6.ColumnCount = 1;
            tablelayoutpanel6.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 220F));
            tablelayoutpanel6.Controls.Add(lblIdiomaActivo, 0, 0);
            tablelayoutpanel6.Controls.Add(cmbIdiomas, 0, 1);
            tablelayoutpanel6.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel6.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel6.Location = new Point(16, 16);
            tablelayoutpanel6.Margin = new Padding(0, 0, 16, 0);
            tablelayoutpanel6.Name = "tablelayoutpanel6";
            tablelayoutpanel6.RowStyles.Add(new RowStyle());
            tablelayoutpanel6.RowStyles.Add(new RowStyle());
            tablelayoutpanel6.Size = new Size(220, 58);
            tablelayoutpanel6.TabIndex = 0;
            // 
            // lblIdiomaActivo
            // 
            lblIdiomaActivo.AutoSize = true;
            lblIdiomaActivo.BackColor = Color.FromArgb(255, 255, 255);
            lblIdiomaActivo.Dock = DockStyle.Fill;
            lblIdiomaActivo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblIdiomaActivo.ForeColor = Color.FromArgb(30, 45, 65);
            lblIdiomaActivo.Location = new Point(0, 0);
            lblIdiomaActivo.Margin = new Padding(0, 0, 0, 6);
            lblIdiomaActivo.Name = "lblIdiomaActivo";
            lblIdiomaActivo.Size = new Size(220, 19);
            lblIdiomaActivo.TabIndex = 0;
            lblIdiomaActivo.Tag = "lbl_idioma_activo";
            lblIdiomaActivo.Text = "Idioma";
            // 
            // cmbIdiomas
            // 
            cmbIdiomas.BackColor = Color.FromArgb(255, 255, 255);
            cmbIdiomas.Dock = DockStyle.Fill;
            cmbIdiomas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIdiomas.Font = new Font("Segoe UI", 10F);
            cmbIdiomas.ForeColor = Color.FromArgb(0, 0, 0);
            cmbIdiomas.FormattingEnabled = true;
            cmbIdiomas.ItemHeight = 17;
            cmbIdiomas.Location = new Point(3, 28);
            cmbIdiomas.Name = "cmbIdiomas";
            cmbIdiomas.Size = new Size(214, 25);
            cmbIdiomas.TabIndex = 1;
            cmbIdiomas.SelectedIndexChanged += cmbIdiomas_SelectedIndexChanged;
            // 
            // btnAplicarIdioma
            // 
            btnAplicarIdioma.AutoSize = true;
            btnAplicarIdioma.BackColor = Color.FromArgb(35, 101, 175);
            btnAplicarIdioma.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnAplicarIdioma.FlatAppearance.BorderSize = 0;
            btnAplicarIdioma.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnAplicarIdioma.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnAplicarIdioma.FlatStyle = FlatStyle.Flat;
            btnAplicarIdioma.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAplicarIdioma.ForeColor = Color.FromArgb(255, 255, 255);
            btnAplicarIdioma.Location = new Point(252, 40);
            btnAplicarIdioma.Margin = new Padding(0, 24, 8, 0);
            btnAplicarIdioma.MinimumSize = new Size(96, 40);
            btnAplicarIdioma.Name = "btnAplicarIdioma";
            btnAplicarIdioma.Size = new Size(96, 40);
            btnAplicarIdioma.TabIndex = 1;
            btnAplicarIdioma.Tag = "btn_aplicar_idioma";
            btnAplicarIdioma.Text = "Aplicar";
            btnAplicarIdioma.UseVisualStyleBackColor = false;
            btnAplicarIdioma.Click += btnAplicarIdioma_Click;
            // 
            // btnCambiarEstado
            // 
            btnCambiarEstado.AutoSize = true;
            btnCambiarEstado.BackColor = Color.FromArgb(255, 255, 255);
            btnCambiarEstado.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnCambiarEstado.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnCambiarEstado.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnCambiarEstado.FlatStyle = FlatStyle.Flat;
            btnCambiarEstado.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnCambiarEstado.ForeColor = Color.FromArgb(30, 45, 65);
            btnCambiarEstado.Location = new Point(356, 40);
            btnCambiarEstado.Margin = new Padding(0, 24, 8, 0);
            btnCambiarEstado.MinimumSize = new Size(96, 40);
            btnCambiarEstado.Name = "btnCambiarEstado";
            btnCambiarEstado.Size = new Size(152, 40);
            btnCambiarEstado.TabIndex = 2;
            btnCambiarEstado.Tag = "btn_cambiar_estado";
            btnCambiarEstado.Text = "Activar / desactivar";
            btnCambiarEstado.UseVisualStyleBackColor = false;
            btnCambiarEstado.Click += btnCambiarEstado_Click;
            // 
            // panel7
            // 
            panel7.AutoScroll = true;
            panel7.BackColor = Color.FromArgb(255, 255, 255);
            panel7.Controls.Add(tablelayoutpanel8);
            panel7.Dock = DockStyle.Fill;
            panel7.Font = new Font("Segoe UI", 10F);
            panel7.ForeColor = Color.FromArgb(30, 45, 65);
            panel7.Location = new Point(872, 226);
            panel7.Margin = new Padding(0);
            panel7.Name = "panel7";
            panel7.Size = new Size(264, 490);
            panel7.TabIndex = 2;
            // 
            // tablelayoutpanel8
            // 
            tablelayoutpanel8.AutoSize = true;
            tablelayoutpanel8.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel8.ColumnCount = 1;
            tablelayoutpanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel8.Controls.Add(label9, 0, 0);
            tablelayoutpanel8.Controls.Add(lblNuevoIdioma, 0, 1);
            tablelayoutpanel8.Controls.Add(txtNuevoIdioma, 0, 2);
            tablelayoutpanel8.Controls.Add(lblCodigoIdioma, 0, 3);
            tablelayoutpanel8.Controls.Add(txtCodigoIdioma, 0, 4);
            tablelayoutpanel8.Controls.Add(lblNombreNativo, 0, 5);
            tablelayoutpanel8.Controls.Add(txtNombreNativo, 0, 6);
            tablelayoutpanel8.Controls.Add(chkIdiomaActivo, 0, 7);
            tablelayoutpanel8.Controls.Add(btnNuevoIdioma, 0, 8);
            tablelayoutpanel8.Controls.Add(label10, 0, 9);
            tablelayoutpanel8.Dock = DockStyle.Top;
            tablelayoutpanel8.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel8.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel8.Location = new Point(0, 0);
            tablelayoutpanel8.Name = "tablelayoutpanel8";
            tablelayoutpanel8.Padding = new Padding(18);
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.Size = new Size(264, 427);
            tablelayoutpanel8.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.FromArgb(255, 255, 255);
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label9.ForeColor = Color.FromArgb(30, 45, 65);
            label9.Location = new Point(18, 18);
            label9.Margin = new Padding(0, 0, 0, 14);
            label9.Name = "label9";
            label9.Size = new Size(228, 25);
            label9.TabIndex = 0;
            label9.Tag = "lbl_titulo_nuevo_idioma";
            label9.Text = "Nuevo idioma";
            // 
            // lblNuevoIdioma
            // 
            lblNuevoIdioma.AutoSize = true;
            lblNuevoIdioma.BackColor = Color.FromArgb(255, 255, 255);
            lblNuevoIdioma.Dock = DockStyle.Fill;
            lblNuevoIdioma.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNuevoIdioma.ForeColor = Color.FromArgb(30, 45, 65);
            lblNuevoIdioma.Location = new Point(18, 57);
            lblNuevoIdioma.Margin = new Padding(0, 0, 0, 6);
            lblNuevoIdioma.Name = "lblNuevoIdioma";
            lblNuevoIdioma.Size = new Size(228, 19);
            lblNuevoIdioma.TabIndex = 1;
            lblNuevoIdioma.Tag = "lbl_nuevo_idioma";
            lblNuevoIdioma.Text = "Nombre";
            // 
            // txtNuevoIdioma
            // 
            txtNuevoIdioma.BackColor = Color.FromArgb(255, 255, 255);
            txtNuevoIdioma.Dock = DockStyle.Fill;
            txtNuevoIdioma.Font = new Font("Segoe UI", 10F);
            txtNuevoIdioma.ForeColor = Color.FromArgb(0, 0, 0);
            txtNuevoIdioma.Location = new Point(18, 82);
            txtNuevoIdioma.Margin = new Padding(0, 0, 0, 16);
            txtNuevoIdioma.MaxLength = 50;
            txtNuevoIdioma.Name = "txtNuevoIdioma";
            txtNuevoIdioma.Size = new Size(228, 25);
            txtNuevoIdioma.TabIndex = 2;
            // 
            // lblCodigoIdioma
            // 
            lblCodigoIdioma.AutoSize = true;
            lblCodigoIdioma.BackColor = Color.FromArgb(255, 255, 255);
            lblCodigoIdioma.Dock = DockStyle.Fill;
            lblCodigoIdioma.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblCodigoIdioma.ForeColor = Color.FromArgb(30, 45, 65);
            lblCodigoIdioma.Location = new Point(18, 123);
            lblCodigoIdioma.Margin = new Padding(0, 0, 0, 6);
            lblCodigoIdioma.Name = "lblCodigoIdioma";
            lblCodigoIdioma.Size = new Size(228, 19);
            lblCodigoIdioma.TabIndex = 3;
            lblCodigoIdioma.Tag = "lbl_codigo_idioma";
            lblCodigoIdioma.Text = "Código";
            // 
            // txtCodigoIdioma
            // 
            txtCodigoIdioma.BackColor = Color.FromArgb(255, 255, 255);
            txtCodigoIdioma.Dock = DockStyle.Fill;
            txtCodigoIdioma.Font = new Font("Segoe UI", 10F);
            txtCodigoIdioma.ForeColor = Color.FromArgb(0, 0, 0);
            txtCodigoIdioma.Location = new Point(18, 148);
            txtCodigoIdioma.Margin = new Padding(0, 0, 0, 16);
            txtCodigoIdioma.MaxLength = 10;
            txtCodigoIdioma.Name = "txtCodigoIdioma";
            txtCodigoIdioma.Size = new Size(228, 25);
            txtCodigoIdioma.TabIndex = 4;
            // 
            // lblNombreNativo
            // 
            lblNombreNativo.AutoSize = true;
            lblNombreNativo.BackColor = Color.FromArgb(255, 255, 255);
            lblNombreNativo.Dock = DockStyle.Fill;
            lblNombreNativo.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNombreNativo.ForeColor = Color.FromArgb(30, 45, 65);
            lblNombreNativo.Location = new Point(18, 189);
            lblNombreNativo.Margin = new Padding(0, 0, 0, 6);
            lblNombreNativo.Name = "lblNombreNativo";
            lblNombreNativo.Size = new Size(228, 19);
            lblNombreNativo.TabIndex = 5;
            lblNombreNativo.Tag = "lbl_nombre_nativo";
            lblNombreNativo.Text = "Nombre nativo";
            // 
            // txtNombreNativo
            // 
            txtNombreNativo.BackColor = Color.FromArgb(255, 255, 255);
            txtNombreNativo.Dock = DockStyle.Fill;
            txtNombreNativo.Font = new Font("Segoe UI", 10F);
            txtNombreNativo.ForeColor = Color.FromArgb(0, 0, 0);
            txtNombreNativo.Location = new Point(18, 214);
            txtNombreNativo.Margin = new Padding(0, 0, 0, 16);
            txtNombreNativo.MaxLength = 100;
            txtNombreNativo.Name = "txtNombreNativo";
            txtNombreNativo.Size = new Size(228, 25);
            txtNombreNativo.TabIndex = 6;
            // 
            // chkIdiomaActivo
            // 
            chkIdiomaActivo.AutoSize = true;
            chkIdiomaActivo.BackColor = Color.FromArgb(255, 255, 255);
            chkIdiomaActivo.Checked = true;
            chkIdiomaActivo.CheckState = CheckState.Checked;
            chkIdiomaActivo.Font = new Font("Segoe UI", 10F);
            chkIdiomaActivo.ForeColor = Color.FromArgb(30, 45, 65);
            chkIdiomaActivo.Location = new Point(18, 255);
            chkIdiomaActivo.Margin = new Padding(0, 0, 0, 16);
            chkIdiomaActivo.Name = "chkIdiomaActivo";
            chkIdiomaActivo.Size = new Size(66, 23);
            chkIdiomaActivo.TabIndex = 7;
            chkIdiomaActivo.Tag = "chk_idioma_activo";
            chkIdiomaActivo.Text = "Activo";
            chkIdiomaActivo.UseVisualStyleBackColor = false;
            // 
            // btnNuevoIdioma
            // 
            btnNuevoIdioma.AutoSize = true;
            btnNuevoIdioma.BackColor = Color.FromArgb(35, 101, 175);
            btnNuevoIdioma.Dock = DockStyle.Fill;
            btnNuevoIdioma.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnNuevoIdioma.FlatAppearance.BorderSize = 0;
            btnNuevoIdioma.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnNuevoIdioma.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnNuevoIdioma.FlatStyle = FlatStyle.Flat;
            btnNuevoIdioma.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnNuevoIdioma.ForeColor = Color.FromArgb(255, 255, 255);
            btnNuevoIdioma.Location = new Point(18, 294);
            btnNuevoIdioma.Margin = new Padding(0, 0, 8, 8);
            btnNuevoIdioma.MinimumSize = new Size(96, 40);
            btnNuevoIdioma.Name = "btnNuevoIdioma";
            btnNuevoIdioma.Size = new Size(220, 40);
            btnNuevoIdioma.TabIndex = 8;
            btnNuevoIdioma.Tag = "btn_nuevo_idioma";
            btnNuevoIdioma.Text = "Crear idioma";
            btnNuevoIdioma.UseVisualStyleBackColor = false;
            btnNuevoIdioma.Click += btnNuevoIdioma_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.BackColor = Color.FromArgb(255, 255, 255);
            label10.Dock = DockStyle.Fill;
            label10.Font = new Font("Segoe UI", 10F);
            label10.ForeColor = Color.FromArgb(94, 111, 133);
            label10.Location = new Point(18, 352);
            label10.Margin = new Padding(0, 10, 0, 0);
            label10.Name = "label10";
            label10.Size = new Size(228, 57);
            label10.TabIndex = 9;
            label10.Tag = "lbl_ayuda_nuevo_idioma";
            label10.Text = "Las traducciones nuevas quedan pendientes hasta que las completes.";
            // 
            // tablelayoutpanel11
            // 
            tablelayoutpanel11.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel11.ColumnCount = 1;
            tablelayoutpanel11.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel11.Controls.Add(label12, 0, 0);
            tablelayoutpanel11.Controls.Add(dgvTraducciones, 0, 1);
            tablelayoutpanel11.Controls.Add(flowlayoutpanel15, 0, 2);
            tablelayoutpanel11.Dock = DockStyle.Fill;
            tablelayoutpanel11.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel11.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel11.Location = new Point(24, 226);
            tablelayoutpanel11.Margin = new Padding(0, 0, 18, 0);
            tablelayoutpanel11.Name = "tablelayoutpanel11";
            tablelayoutpanel11.Padding = new Padding(18);
            tablelayoutpanel11.RowCount = 3;
            tablelayoutpanel11.RowStyles.Add(new RowStyle());
            tablelayoutpanel11.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel11.RowStyles.Add(new RowStyle());
            tablelayoutpanel11.Size = new Size(830, 490);
            tablelayoutpanel11.TabIndex = 3;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.BackColor = Color.FromArgb(255, 255, 255);
            label12.Dock = DockStyle.Fill;
            label12.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label12.ForeColor = Color.FromArgb(30, 45, 65);
            label12.Location = new Point(18, 18);
            label12.Margin = new Padding(0, 0, 0, 14);
            label12.Name = "label12";
            label12.Size = new Size(794, 25);
            label12.TabIndex = 0;
            label12.Tag = "lbl_titulo_traducciones";
            label12.Text = "Traducciones";
            // 
            // dgvTraducciones
            // 
            dgvTraducciones.AllowUserToAddRows = false;
            dgvTraducciones.AllowUserToDeleteRows = false;
            dgvTraducciones.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 251, 254);
            dgvTraducciones.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvTraducciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTraducciones.BackgroundColor = Color.White;
            dgvTraducciones.BorderStyle = BorderStyle.None;
            dgvTraducciones.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 10, 0);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dgvTraducciones.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvTraducciones.ColumnHeadersHeight = 44;
            dgvTraducciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgvTraducciones.Columns.AddRange(new DataGridViewColumn[] { FormNombre, ControlNombre, TextoBase, Texto, Estado, IdIdioma, IdControl });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.Padding = new Padding(10, 5, 10, 5);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 240, 252);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvTraducciones.DefaultCellStyle = dataGridViewCellStyle3;
            dgvTraducciones.Dock = DockStyle.Fill;
            dgvTraducciones.EnableHeadersVisualStyles = false;
            dgvTraducciones.Font = new Font("Segoe UI", 10F);
            dgvTraducciones.GridColor = Color.FromArgb(234, 239, 246);
            dgvTraducciones.Location = new Point(21, 60);
            dgvTraducciones.MultiSelect = false;
            dgvTraducciones.Name = "dgvTraducciones";
            dgvTraducciones.RowHeadersVisible = false;
            dgvTraducciones.RowTemplate.Height = 40;
            dgvTraducciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTraducciones.Size = new Size(788, 347);
            dgvTraducciones.TabIndex = 1;
            dgvTraducciones.CellFormatting += FormatearEstadoTraduccion;
            dgvTraducciones.DataBindingComplete += Traducciones_DataBindingComplete;
            // 
            // FormNombre
            // 
            FormNombre.DataPropertyName = "FormNombre";
            FormNombre.FillWeight = 110F;
            FormNombre.HeaderText = "Formulario";
            FormNombre.MinimumWidth = 110;
            FormNombre.Name = "FormNombre";
            FormNombre.ReadOnly = true;
            FormNombre.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // ControlNombre
            // 
            ControlNombre.DataPropertyName = "ControlNombre";
            ControlNombre.FillWeight = 120F;
            ControlNombre.HeaderText = "Clave";
            ControlNombre.MinimumWidth = 110;
            ControlNombre.Name = "ControlNombre";
            ControlNombre.ReadOnly = true;
            ControlNombre.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // TextoBase
            // 
            TextoBase.DataPropertyName = "TextoBase";
            TextoBase.FillWeight = 160F;
            TextoBase.HeaderText = "Español";
            TextoBase.MinimumWidth = 110;
            TextoBase.Name = "TextoBase";
            TextoBase.ReadOnly = true;
            TextoBase.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Texto
            // 
            Texto.DataPropertyName = "Texto";
            Texto.FillWeight = 180F;
            Texto.HeaderText = "Traducción";
            Texto.MinimumWidth = 110;
            Texto.Name = "Texto";
            Texto.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Estado
            // 
            Estado.DataPropertyName = "Estado";
            Estado.FillWeight = 85F;
            Estado.HeaderText = "Estado";
            Estado.MinimumWidth = 95;
            Estado.Name = "Estado";
            Estado.ReadOnly = true;
            Estado.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // IdIdioma
            // 
            IdIdioma.DataPropertyName = "IdIdioma";
            IdIdioma.HeaderText = "IdIdioma";
            IdIdioma.Name = "IdIdioma";
            IdIdioma.ReadOnly = true;
            IdIdioma.SortMode = DataGridViewColumnSortMode.NotSortable;
            IdIdioma.Visible = false;
            // 
            // IdControl
            // 
            IdControl.DataPropertyName = "IdControl";
            IdControl.HeaderText = "IdControl";
            IdControl.Name = "IdControl";
            IdControl.ReadOnly = true;
            IdControl.SortMode = DataGridViewColumnSortMode.NotSortable;
            IdControl.Visible = false;
            // 
            // DigitoVerificador
            // 
            // 
            // flowlayoutpanel15
            // 
            flowlayoutpanel15.AutoSize = true;
            flowlayoutpanel15.BackColor = Color.FromArgb(255, 255, 255);
            flowlayoutpanel15.Controls.Add(btnGuardarTraduccion);
            flowlayoutpanel15.Controls.Add(btnExportarTraducciones);
            flowlayoutpanel15.Controls.Add(btnImportarTraducciones);
            flowlayoutpanel15.Dock = DockStyle.Top;
            flowlayoutpanel15.Font = new Font("Segoe UI", 10F);
            flowlayoutpanel15.ForeColor = Color.FromArgb(30, 45, 65);
            flowlayoutpanel15.Location = new Point(18, 410);
            flowlayoutpanel15.Margin = new Padding(0);
            flowlayoutpanel15.Name = "flowlayoutpanel15";
            flowlayoutpanel15.Padding = new Padding(0, 14, 0, 0);
            flowlayoutpanel15.Size = new Size(794, 62);
            flowlayoutpanel15.TabIndex = 2;
            // 
            // btnGuardarTraduccion
            // 
            btnGuardarTraduccion.AutoSize = true;
            btnGuardarTraduccion.BackColor = Color.FromArgb(35, 101, 175);
            btnGuardarTraduccion.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnGuardarTraduccion.FlatAppearance.BorderSize = 0;
            btnGuardarTraduccion.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnGuardarTraduccion.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnGuardarTraduccion.FlatStyle = FlatStyle.Flat;
            btnGuardarTraduccion.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGuardarTraduccion.ForeColor = Color.FromArgb(255, 255, 255);
            btnGuardarTraduccion.Location = new Point(0, 14);
            btnGuardarTraduccion.Margin = new Padding(0, 0, 8, 8);
            btnGuardarTraduccion.MinimumSize = new Size(96, 40);
            btnGuardarTraduccion.Name = "btnGuardarTraduccion";
            btnGuardarTraduccion.Size = new Size(149, 40);
            btnGuardarTraduccion.TabIndex = 0;
            btnGuardarTraduccion.Tag = "btn_guardar_traduccion";
            btnGuardarTraduccion.Text = "Guardar traducción";
            btnGuardarTraduccion.UseVisualStyleBackColor = false;
            btnGuardarTraduccion.Click += btnGuardarTraduccion_Click;
            // 
            // btnExportarTraducciones
            // 
            btnExportarTraducciones.AutoSize = true;
            btnExportarTraducciones.BackColor = Color.FromArgb(255, 255, 255);
            btnExportarTraducciones.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnExportarTraducciones.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnExportarTraducciones.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnExportarTraducciones.FlatStyle = FlatStyle.Flat;
            btnExportarTraducciones.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnExportarTraducciones.ForeColor = Color.FromArgb(30, 45, 65);
            btnExportarTraducciones.Location = new Point(157, 14);
            btnExportarTraducciones.Margin = new Padding(0, 0, 8, 8);
            btnExportarTraducciones.MinimumSize = new Size(96, 40);
            btnExportarTraducciones.Name = "btnExportarTraducciones";
            btnExportarTraducciones.Size = new Size(168, 40);
            btnExportarTraducciones.TabIndex = 1;
            btnExportarTraducciones.Tag = "btn_exportar_traducciones";
            btnExportarTraducciones.Text = "Exportar traducciones";
            btnExportarTraducciones.UseVisualStyleBackColor = false;
            btnExportarTraducciones.Click += btnExportarTraducciones_Click;
            // 
            // btnImportarTraducciones
            // 
            btnImportarTraducciones.AutoSize = true;
            btnImportarTraducciones.BackColor = Color.FromArgb(255, 255, 255);
            btnImportarTraducciones.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnImportarTraducciones.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnImportarTraducciones.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnImportarTraducciones.FlatStyle = FlatStyle.Flat;
            btnImportarTraducciones.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnImportarTraducciones.ForeColor = Color.FromArgb(30, 45, 65);
            btnImportarTraducciones.Location = new Point(333, 14);
            btnImportarTraducciones.Margin = new Padding(0, 0, 8, 8);
            btnImportarTraducciones.MinimumSize = new Size(96, 40);
            btnImportarTraducciones.Name = "btnImportarTraducciones";
            btnImportarTraducciones.Size = new Size(170, 40);
            btnImportarTraducciones.TabIndex = 2;
            btnImportarTraducciones.Tag = "btn_importar_traducciones";
            btnImportarTraducciones.Text = "Importar traducciones";
            btnImportarTraducciones.UseVisualStyleBackColor = false;
            btnImportarTraducciones.Click += btnImportarTraducciones_Click;
            // 
            // FrmGestionIdiomas
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1160, 740);
            Controls.Add(tablelayoutpanel1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MinimumSize = new Size(880, 600);
            Name = "FrmGestionIdiomas";
            Tag = "frmgestionidiomas_titulo";
            Text = "Gestión de idiomas";
            tablelayoutpanel1.ResumeLayout(false);
            tablelayoutpanel1.PerformLayout();
            tablelayoutpanel2.ResumeLayout(false);
            tablelayoutpanel2.PerformLayout();
            flowlayoutpanel5.ResumeLayout(false);
            flowlayoutpanel5.PerformLayout();
            tablelayoutpanel6.ResumeLayout(false);
            tablelayoutpanel6.PerformLayout();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            tablelayoutpanel8.ResumeLayout(false);
            tablelayoutpanel8.PerformLayout();
            tablelayoutpanel11.ResumeLayout(false);
            tablelayoutpanel11.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvTraducciones).EndInit();
            flowlayoutpanel15.ResumeLayout(false);
            flowlayoutpanel15.PerformLayout();
            ResumeLayout(false);
        }
    }
}
