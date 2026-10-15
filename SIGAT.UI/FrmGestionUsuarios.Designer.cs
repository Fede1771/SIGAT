namespace SIGAT.UI
{
    partial class FrmGestionUsuarios
    {
        private System.ComponentModel.IContainer components = null;
        private DataGridViewTextBoxColumn IdUsuario;
        private DataGridViewTextBoxColumn NombreUsuario;
        private DataGridViewTextBoxColumn Nombre;
        private DataGridViewTextBoxColumn Apellido;
        private DataGridViewTextBoxColumn Activo;
        private DataGridViewTextBoxColumn Roles;
        private DataGridViewTextBoxColumn DigitoVerificador;

        private TableLayoutPanel tablelayoutpanel1;
        private TableLayoutPanel tablelayoutpanel2;
        private Label label3;
        private Label label4;
        private DataGridView dgvUsuarios;
        private Panel panel7;
        private TableLayoutPanel tablelayoutpanel8;
        private Label label9;
        private Label lblUsuario;
        private TextBox txtUsername;
        private Label lblClave;
        private TextBox txtPass;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private CheckBox chkActivo;
        private TableLayoutPanel tablelayoutpanel10;
        private Button btnGuardar;
        private Button btnEliminar;
        private Button btnLimpiar;
        private Label lblAyuda;


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
            dgvUsuarios = new DataGridView();
            IdUsuario = new DataGridViewTextBoxColumn();
            NombreUsuario = new DataGridViewTextBoxColumn();
            Nombre = new DataGridViewTextBoxColumn();
            Apellido = new DataGridViewTextBoxColumn();
            Activo = new DataGridViewTextBoxColumn();
            Roles = new DataGridViewTextBoxColumn();
            DigitoVerificador = new DataGridViewTextBoxColumn();
            panel7 = new Panel();
            tablelayoutpanel8 = new TableLayoutPanel();
            label9 = new Label();
            lblUsuario = new Label();
            txtUsername = new TextBox();
            lblClave = new Label();
            txtPass = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            chkActivo = new CheckBox();
            tablelayoutpanel10 = new TableLayoutPanel();
            btnGuardar = new Button();
            btnEliminar = new Button();
            btnLimpiar = new Button();
            lblAyuda = new Label();
            tablelayoutpanel1.SuspendLayout();
            tablelayoutpanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            panel7.SuspendLayout();
            tablelayoutpanel8.SuspendLayout();
            tablelayoutpanel10.SuspendLayout();
            SuspendLayout();
            // 
            // tablelayoutpanel1
            // 
            tablelayoutpanel1.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel1.ColumnCount = 2;
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360F));
            tablelayoutpanel1.Controls.Add(tablelayoutpanel2, 0, 0);
            tablelayoutpanel1.Controls.Add(dgvUsuarios, 0, 1);
            tablelayoutpanel1.Controls.Add(panel7, 1, 1);
            tablelayoutpanel1.Dock = DockStyle.Fill;
            tablelayoutpanel1.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel1.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel1.Location = new Point(0, 0);
            tablelayoutpanel1.Name = "tablelayoutpanel1";
            tablelayoutpanel1.Padding = new Padding(24);
            tablelayoutpanel1.RowCount = 2;
            tablelayoutpanel1.RowStyles.Add(new RowStyle());
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Size = new Size(1160, 620);
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
            tablelayoutpanel2.Dock = DockStyle.Fill;
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
            label3.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(30, 45, 65);
            label3.Location = new Point(0, 0);
            label3.Margin = new Padding(0, 0, 0, 6);
            label3.Name = "label3";
            label3.Size = new Size(293, 41);
            label3.TabIndex = 0;
            label3.Tag = "frmgestionusuarios_titulo";
            label3.Text = "Gestión de usuarios";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(245, 247, 251);
            label4.Font = new Font("Segoe UI", 10F);
            label4.ForeColor = Color.FromArgb(85, 100, 115);
            label4.Location = new Point(0, 47);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(513, 19);
            label4.TabIndex = 1;
            label4.Tag = "lbl_descripcion_usuarios";
            label4.Text = "Seleccioná una cuenta para editarla o completá el formulario para crear un usuario.";
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(249, 251, 254);
            dgvUsuarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.BorderStyle = BorderStyle.None;
            dgvUsuarios.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle2.Padding = new Padding(10, 0, 10, 0);
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(236, 241, 248);
            dataGridViewCellStyle2.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dgvUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            dgvUsuarios.ColumnHeadersHeight = 44;
            dgvUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            DigitoVerificador.Name = "DigitoVerificador";
            DigitoVerificador.DataPropertyName = "DigitoVerificador";
            DigitoVerificador.HeaderText = "Dígito verificador (DVH)";
            DigitoVerificador.Tag = "col_dvh";
            DigitoVerificador.MinimumWidth = 150;
            DigitoVerificador.FillWeight = 140F;
            DigitoVerificador.ReadOnly = true;
            DigitoVerificador.SortMode = DataGridViewColumnSortMode.NotSortable;
            dgvUsuarios.Columns.AddRange(new DataGridViewColumn[] { IdUsuario, NombreUsuario, Nombre, Apellido, Activo, Roles, DigitoVerificador });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.FromArgb(255, 255, 255);
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 10F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.Padding = new Padding(10, 5, 10, 5);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 240, 252);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(30, 45, 65);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            dgvUsuarios.DefaultCellStyle = dataGridViewCellStyle3;
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.EnableHeadersVisualStyles = false;
            dgvUsuarios.Font = new Font("Segoe UI", 10F);
            dgvUsuarios.GridColor = Color.FromArgb(234, 239, 246);
            dgvUsuarios.Location = new Point(24, 112);
            dgvUsuarios.Margin = new Padding(0, 0, 18, 0);
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersVisible = false;
            dgvUsuarios.RowTemplate.Height = 40;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Size = new Size(734, 484);
            dgvUsuarios.TabIndex = 1;
            // 
            // IdUsuario
            // 
            IdUsuario.DataPropertyName = "IdUsuario";
            IdUsuario.FillWeight = 45F;
            IdUsuario.HeaderText = "ID";
            IdUsuario.MinimumWidth = 45;
            IdUsuario.Name = "IdUsuario";
            IdUsuario.ReadOnly = true;
            IdUsuario.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // NombreUsuario
            // 
            NombreUsuario.DataPropertyName = "NombreUsuario";
            NombreUsuario.FillWeight = 130F;
            NombreUsuario.HeaderText = "Usuario";
            NombreUsuario.MinimumWidth = 100;
            NombreUsuario.Name = "NombreUsuario";
            NombreUsuario.ReadOnly = true;
            NombreUsuario.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Nombre
            // 
            Nombre.DataPropertyName = "Nombre";
            Nombre.FillWeight = 120F;
            Nombre.HeaderText = "Nombre";
            Nombre.MinimumWidth = 90;
            Nombre.Name = "Nombre";
            Nombre.ReadOnly = true;
            Nombre.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Apellido
            // 
            Apellido.DataPropertyName = "Apellido";
            Apellido.FillWeight = 120F;
            Apellido.HeaderText = "Apellido";
            Apellido.MinimumWidth = 90;
            Apellido.Name = "Apellido";
            Apellido.ReadOnly = true;
            Apellido.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Activo
            // 
            Activo.DataPropertyName = "Activo";
            Activo.FillWeight = 85F;
            Activo.HeaderText = "Activo";
            Activo.MinimumWidth = 85;
            Activo.Name = "Activo";
            Activo.ReadOnly = true;
            Activo.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // Roles
            // 
            Roles.FillWeight = 200F;
            Roles.HeaderText = "Roles asignados";
            Roles.MinimumWidth = 150;
            Roles.Name = "Roles";
            Roles.ReadOnly = true;
            Roles.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // panel7
            // 
            panel7.AutoScroll = true;
            panel7.BackColor = Color.FromArgb(255, 255, 255);
            panel7.Controls.Add(tablelayoutpanel8);
            panel7.Dock = DockStyle.Fill;
            panel7.Font = new Font("Segoe UI", 10F);
            panel7.ForeColor = Color.FromArgb(30, 45, 65);
            panel7.Location = new Point(776, 112);
            panel7.Margin = new Padding(0);
            panel7.Name = "panel7";
            panel7.Size = new Size(360, 484);
            panel7.TabIndex = 2;
            // 
            // tablelayoutpanel8
            // 
            tablelayoutpanel8.AutoSize = true;
            tablelayoutpanel8.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel8.ColumnCount = 1;
            tablelayoutpanel8.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel8.Controls.Add(label9, 0, 0);
            tablelayoutpanel8.Controls.Add(lblUsuario, 0, 1);
            tablelayoutpanel8.Controls.Add(txtUsername, 0, 2);
            tablelayoutpanel8.Controls.Add(lblClave, 0, 3);
            tablelayoutpanel8.Controls.Add(txtPass, 0, 4);
            tablelayoutpanel8.Controls.Add(lblNombre, 0, 5);
            tablelayoutpanel8.Controls.Add(txtNombre, 0, 6);
            tablelayoutpanel8.Controls.Add(lblApellido, 0, 7);
            tablelayoutpanel8.Controls.Add(txtApellido, 0, 8);
            tablelayoutpanel8.Controls.Add(chkActivo, 0, 9);
            tablelayoutpanel8.Controls.Add(tablelayoutpanel10, 0, 10);
            tablelayoutpanel8.Controls.Add(lblAyuda, 0, 11);
            tablelayoutpanel8.Dock = DockStyle.Top;
            tablelayoutpanel8.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel8.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel8.Location = new Point(0, 0);
            tablelayoutpanel8.Name = "tablelayoutpanel8";
            tablelayoutpanel8.Padding = new Padding(16);
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
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.RowStyles.Add(new RowStyle());
            tablelayoutpanel8.Size = new Size(343, 531);
            tablelayoutpanel8.TabIndex = 0;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.FromArgb(255, 255, 255);
            label9.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label9.ForeColor = Color.FromArgb(35, 55, 78);
            label9.Location = new Point(16, 16);
            label9.Margin = new Padding(0, 0, 0, 14);
            label9.Name = "label9";
            label9.Size = new Size(170, 25);
            label9.TabIndex = 0;
            label9.Tag = "lbl_datos_cuenta";
            label9.Text = "Datos de la cuenta";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.BackColor = Color.FromArgb(255, 255, 255);
            lblUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.FromArgb(30, 45, 65);
            lblUsuario.Location = new Point(16, 63);
            lblUsuario.Margin = new Padding(0, 8, 0, 5);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(64, 19);
            lblUsuario.TabIndex = 1;
            lblUsuario.Tag = "lbl_usuario_gu";
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsername
            // 
            txtUsername.BackColor = Color.FromArgb(255, 255, 255);
            txtUsername.Dock = DockStyle.Fill;
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.ForeColor = Color.FromArgb(0, 0, 0);
            txtUsername.Location = new Point(16, 87);
            txtUsername.Margin = new Padding(0, 0, 0, 8);
            txtUsername.MaxLength = 50;
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(311, 25);
            txtUsername.TabIndex = 2;
            // 
            // lblClave
            // 
            lblClave.AutoSize = true;
            lblClave.BackColor = Color.FromArgb(255, 255, 255);
            lblClave.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblClave.ForeColor = Color.FromArgb(30, 45, 65);
            lblClave.Location = new Point(16, 128);
            lblClave.Margin = new Padding(0, 8, 0, 5);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(174, 19);
            lblClave.TabIndex = 3;
            lblClave.Tag = "lbl_clave";
            lblClave.Text = "Clave (vacío no cambia):";
            // 
            // txtPass
            // 
            txtPass.BackColor = Color.FromArgb(255, 255, 255);
            txtPass.Dock = DockStyle.Fill;
            txtPass.Font = new Font("Segoe UI", 10F);
            txtPass.ForeColor = Color.FromArgb(0, 0, 0);
            txtPass.Location = new Point(16, 152);
            txtPass.Margin = new Padding(0, 0, 0, 8);
            txtPass.MaxLength = 100;
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(311, 25);
            txtPass.TabIndex = 4;
            txtPass.UseSystemPasswordChar = true;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.BackColor = Color.FromArgb(255, 255, 255);
            lblNombre.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblNombre.ForeColor = Color.FromArgb(30, 45, 65);
            lblNombre.Location = new Point(16, 193);
            lblNombre.Margin = new Padding(0, 8, 0, 5);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(69, 19);
            lblNombre.TabIndex = 5;
            lblNombre.Tag = "lbl_nombre";
            lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.BackColor = Color.FromArgb(255, 255, 255);
            txtNombre.Dock = DockStyle.Fill;
            txtNombre.Font = new Font("Segoe UI", 10F);
            txtNombre.ForeColor = Color.FromArgb(0, 0, 0);
            txtNombre.Location = new Point(16, 217);
            txtNombre.Margin = new Padding(0, 0, 0, 8);
            txtNombre.MaxLength = 100;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(311, 25);
            txtNombre.TabIndex = 6;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.BackColor = Color.FromArgb(255, 255, 255);
            lblApellido.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblApellido.ForeColor = Color.FromArgb(30, 45, 65);
            lblApellido.Location = new Point(16, 258);
            lblApellido.Margin = new Padding(0, 8, 0, 5);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(70, 19);
            lblApellido.TabIndex = 7;
            lblApellido.Tag = "lbl_apellido";
            lblApellido.Text = "Apellido:";
            // 
            // txtApellido
            // 
            txtApellido.BackColor = Color.FromArgb(255, 255, 255);
            txtApellido.Dock = DockStyle.Fill;
            txtApellido.Font = new Font("Segoe UI", 10F);
            txtApellido.ForeColor = Color.FromArgb(0, 0, 0);
            txtApellido.Location = new Point(16, 282);
            txtApellido.Margin = new Padding(0, 0, 0, 8);
            txtApellido.MaxLength = 100;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(311, 25);
            txtApellido.TabIndex = 8;
            // 
            // chkActivo
            // 
            chkActivo.AutoSize = true;
            chkActivo.BackColor = Color.FromArgb(255, 255, 255);
            chkActivo.Checked = true;
            chkActivo.CheckState = CheckState.Checked;
            chkActivo.Font = new Font("Segoe UI", 10F);
            chkActivo.ForeColor = Color.FromArgb(30, 45, 65);
            chkActivo.Location = new Point(16, 325);
            chkActivo.Margin = new Padding(0, 10, 0, 12);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(66, 23);
            chkActivo.TabIndex = 9;
            chkActivo.Tag = "chk_activo";
            chkActivo.Text = "Activo";
            chkActivo.UseVisualStyleBackColor = false;
            // 
            // tablelayoutpanel10
            // 
            tablelayoutpanel10.AutoSize = true;
            tablelayoutpanel10.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel10.ColumnCount = 3;
            tablelayoutpanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            tablelayoutpanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tablelayoutpanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33F));
            tablelayoutpanel10.Controls.Add(btnGuardar, 0, 0);
            tablelayoutpanel10.Controls.Add(btnEliminar, 1, 0);
            tablelayoutpanel10.Controls.Add(btnLimpiar, 2, 0);
            tablelayoutpanel10.Dock = DockStyle.Fill;
            tablelayoutpanel10.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel10.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel10.Location = new Point(16, 364);
            tablelayoutpanel10.Margin = new Padding(0, 4, 0, 12);
            tablelayoutpanel10.Name = "tablelayoutpanel10";
            tablelayoutpanel10.RowStyles.Add(new RowStyle());
            tablelayoutpanel10.Size = new Size(311, 40);
            tablelayoutpanel10.TabIndex = 10;
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.FromArgb(35, 101, 175);
            btnGuardar.Dock = DockStyle.Fill;
            btnGuardar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnGuardar.FlatAppearance.BorderSize = 0;
            btnGuardar.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnGuardar.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnGuardar.FlatStyle = FlatStyle.Flat;
            btnGuardar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.FromArgb(255, 255, 255);
            btnGuardar.Location = new Point(0, 0);
            btnGuardar.Margin = new Padding(0, 0, 6, 0);
            btnGuardar.MinimumSize = new Size(0, 40);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(99, 40);
            btnGuardar.TabIndex = 0;
            btnGuardar.Tag = "btn_guardar";
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += BtnGuardar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.FromArgb(255, 255, 255);
            btnEliminar.Dock = DockStyle.Fill;
            btnEliminar.Enabled = false;
            btnEliminar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnEliminar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnEliminar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnEliminar.FlatStyle = FlatStyle.Flat;
            btnEliminar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEliminar.ForeColor = Color.FromArgb(164, 48, 59);
            btnEliminar.Location = new Point(105, 0);
            btnEliminar.Margin = new Padding(0, 0, 6, 0);
            btnEliminar.MinimumSize = new Size(0, 40);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(96, 40);
            btnEliminar.TabIndex = 1;
            btnEliminar.Tag = "btn_eliminar";
            btnEliminar.Text = "Baja";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += BtnEliminar_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.FromArgb(255, 255, 255);
            btnLimpiar.Dock = DockStyle.Fill;
            btnLimpiar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnLimpiar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnLimpiar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnLimpiar.FlatStyle = FlatStyle.Flat;
            btnLimpiar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLimpiar.ForeColor = Color.FromArgb(30, 45, 65);
            btnLimpiar.Location = new Point(207, 0);
            btnLimpiar.Margin = new Padding(0, 0, 6, 0);
            btnLimpiar.MinimumSize = new Size(0, 40);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(98, 40);
            btnLimpiar.TabIndex = 2;
            btnLimpiar.Tag = "btn_limpiar";
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = false;
            btnLimpiar.Click += BtnLimpiar_Click;
            // 
            // lblAyuda
            // 
            lblAyuda.AutoSize = true;
            lblAyuda.BackColor = Color.FromArgb(245, 247, 250);
            lblAyuda.Dock = DockStyle.Fill;
            lblAyuda.Font = new Font("Segoe UI", 9F);
            lblAyuda.ForeColor = Color.FromArgb(85, 100, 115);
            lblAyuda.Location = new Point(19, 416);
            lblAyuda.MaximumSize = new Size(300, 0);
            lblAyuda.Name = "lblAyuda";
            lblAyuda.Padding = new Padding(12);
            lblAyuda.Size = new Size(300, 99);
            lblAyuda.TabIndex = 11;
            lblAyuda.Tag = "lbl_ayuda_roles";
            lblAyuda.Text = "Las cuentas nuevas se crean sin roles. Asigná sus accesos desde Gestión de roles y permisos.\n\nPara reactivar una cuenta, seleccionála, marcá Activo y guardá.";
            // 
            // FrmGestionUsuarios
            // 
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1160, 620);
            Controls.Add(tablelayoutpanel1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MinimumSize = new Size(940, 540);
            Name = "FrmGestionUsuarios";
            Tag = "frmgestionusuarios_titulo";
            Text = "Gestión de usuarios";
            tablelayoutpanel1.ResumeLayout(false);
            tablelayoutpanel1.PerformLayout();
            tablelayoutpanel2.ResumeLayout(false);
            tablelayoutpanel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            panel7.ResumeLayout(false);
            panel7.PerformLayout();
            tablelayoutpanel8.ResumeLayout(false);
            tablelayoutpanel8.PerformLayout();
            tablelayoutpanel10.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
