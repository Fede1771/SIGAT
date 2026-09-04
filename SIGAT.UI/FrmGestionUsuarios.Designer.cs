namespace SIGAT.UI
{
    partial class FrmGestionUsuarios
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
            panelDerecho = new Panel();
            lblUsuario = new Label();
            txtUsername = new TextBox();
            lblClave = new Label();
            txtPass = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblApellido = new Label();
            txtApellido = new TextBox();
            lblPerfil = new Label();
            cmbPerfil = new ComboBox();
            chkActivo = new CheckBox();
            btnGuardar = new Button();
            btnEliminar = new Button();
            btnLimpiar = new Button();
            separador = new Panel();
            dgvUsuarios = new DataGridView();
            panelDerecho.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).BeginInit();
            SuspendLayout();
            // 
            // panelDerecho
            // 
            panelDerecho.BackColor = Color.Transparent;
            panelDerecho.Controls.Add(lblUsuario);
            panelDerecho.Controls.Add(txtUsername);
            panelDerecho.Controls.Add(lblClave);
            panelDerecho.Controls.Add(txtPass);
            panelDerecho.Controls.Add(lblNombre);
            panelDerecho.Controls.Add(txtNombre);
            panelDerecho.Controls.Add(lblApellido);
            panelDerecho.Controls.Add(txtApellido);
            panelDerecho.Controls.Add(lblPerfil);
            panelDerecho.Controls.Add(cmbPerfil);
            panelDerecho.Controls.Add(chkActivo);
            panelDerecho.Controls.Add(btnGuardar);
            panelDerecho.Controls.Add(btnEliminar);
            panelDerecho.Controls.Add(btnLimpiar);
            panelDerecho.Dock = DockStyle.Right;
            panelDerecho.Location = new Point(703, 40);
            panelDerecho.Name = "panelDerecho";
            panelDerecho.Size = new Size(380, 520);
            panelDerecho.TabIndex = 0;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblUsuario.Location = new Point(10, 10);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(67, 20);
            lblUsuario.TabIndex = 0;
            lblUsuario.Text = "Usuario:";
            // 
            // txtUsername
            // 
            txtUsername.Font = new Font("Segoe UI", 10F);
            txtUsername.Location = new Point(10, 32);
            txtUsername.MaxLength = 50;
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(350, 30);
            txtUsername.TabIndex = 1;
            // 
            // lblClave
            // 
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblClave.Location = new Point(10, 72);
            lblClave.Name = "lblClave";
            lblClave.Size = new Size(178, 20);
            lblClave.TabIndex = 2;
            lblClave.Text = "Clave (vacío no cambia):";
            // 
            // txtPass
            // 
            txtPass.Font = new Font("Segoe UI", 10F);
            txtPass.Location = new Point(10, 94);
            txtPass.MaxLength = 100;
            txtPass.Name = "txtPass";
            txtPass.Size = new Size(350, 30);
            txtPass.TabIndex = 3;
            txtPass.UseSystemPasswordChar = true;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblNombre.Location = new Point(10, 134);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(71, 20);
            lblNombre.TabIndex = 4;
            lblNombre.Text = "Nombre:";
            // 
            // txtNombre
            // 
            txtNombre.Font = new Font("Segoe UI", 10F);
            txtNombre.Location = new Point(10, 156);
            txtNombre.MaxLength = 80;
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(350, 30);
            txtNombre.TabIndex = 5;
            // 
            // lblApellido
            // 
            lblApellido.AutoSize = true;
            lblApellido.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblApellido.Location = new Point(10, 196);
            lblApellido.Name = "lblApellido";
            lblApellido.Size = new Size(71, 20);
            lblApellido.TabIndex = 6;
            lblApellido.Text = "Apellido:";
            // 
            // txtApellido
            // 
            txtApellido.Font = new Font("Segoe UI", 10F);
            txtApellido.Location = new Point(10, 218);
            txtApellido.MaxLength = 80;
            txtApellido.Name = "txtApellido";
            txtApellido.Size = new Size(350, 30);
            txtApellido.TabIndex = 7;
            // 
            // lblPerfil
            // 
            lblPerfil.AutoSize = true;
            lblPerfil.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblPerfil.Location = new Point(10, 258);
            lblPerfil.Name = "lblPerfil";
            lblPerfil.Size = new Size(50, 20);
            lblPerfil.TabIndex = 8;
            lblPerfil.Text = "Perfil:";
            // 
            // cmbPerfil
            // 
            cmbPerfil.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPerfil.Font = new Font("Segoe UI", 10F);
            cmbPerfil.Location = new Point(10, 280);
            cmbPerfil.Name = "cmbPerfil";
            cmbPerfil.Size = new Size(350, 31);
            cmbPerfil.TabIndex = 9;
            // 
            // chkActivo
            // 
            chkActivo.Checked = true;
            chkActivo.CheckState = CheckState.Checked;
            chkActivo.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            chkActivo.Location = new Point(10, 330);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(100, 24);
            chkActivo.TabIndex = 10;
            chkActivo.Text = "Activo";
            // 
            // btnGuardar
            // 
            btnGuardar.BackColor = Color.White;
            btnGuardar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnGuardar.Location = new Point(10, 372);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(110, 35);
            btnGuardar.TabIndex = 11;
            btnGuardar.Text = "Guardar";
            btnGuardar.UseVisualStyleBackColor = false;
            btnGuardar.Click += BtnGuardar_Click;
            // 
            // btnEliminar
            // 
            btnEliminar.BackColor = Color.White;
            btnEliminar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnEliminar.Location = new Point(125, 372);
            btnEliminar.Name = "btnEliminar";
            btnEliminar.Size = new Size(110, 35);
            btnEliminar.TabIndex = 12;
            btnEliminar.Text = "Baja";
            btnEliminar.UseVisualStyleBackColor = false;
            btnEliminar.Click += BtnEliminar_Click;
            // 
            // btnLimpiar
            // 
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnLimpiar.Location = new Point(240, 372);
            btnLimpiar.Name = "btnLimpiar";
            btnLimpiar.Size = new Size(110, 35);
            btnLimpiar.TabIndex = 13;
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.UseVisualStyleBackColor = false;
            btnLimpiar.Click += BtnLimpiar_Click;
            // 
            // separador
            // 
            separador.BackColor = Color.Transparent;
            separador.Dock = DockStyle.Right;
            separador.Location = new Point(1083, 40);
            separador.Name = "separador";
            separador.Size = new Size(40, 520);
            separador.TabIndex = 1;
            // 
            // dgvUsuarios
            // 
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.BackgroundColor = Color.White;
            dgvUsuarios.ColumnHeadersHeight = 29;
            dgvUsuarios.Dock = DockStyle.Fill;
            dgvUsuarios.Location = new Point(40, 40);
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.Name = "dgvUsuarios";
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.RowHeadersWidth = 51;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.Size = new Size(1083, 520);
            dgvUsuarios.TabIndex = 2;
            dgvUsuarios.CellClick += DgvUsuarios_CellClick;
            dgvUsuarios.DataBindingComplete += DgvUsuarios_DataBindingComplete;
            // 
            // FrmGestionUsuarios
            // 
            AcceptButton = btnGuardar;
            AutoScaleDimensions = new SizeF(8F, 20F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 250);
            ClientSize = new Size(1163, 600);
            Controls.Add(panelDerecho);
            Controls.Add(separador);
            Controls.Add(dgvUsuarios);
            FormBorderStyle = FormBorderStyle.None;
            Name = "FrmGestionUsuarios";
            Padding = new Padding(40);
            Text = "Gestión de Usuarios";
            WindowState = FormWindowState.Maximized;
            panelDerecho.ResumeLayout(false);
            panelDerecho.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dgvUsuarios).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelDerecho;
        private Label lblUsuario;
        private TextBox txtUsername;
        private Label lblClave;
        private TextBox txtPass;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblApellido;
        private TextBox txtApellido;
        private Label lblPerfil;
        private ComboBox cmbPerfil;
        private CheckBox chkActivo;
        private Button btnGuardar;
        private Button btnEliminar;
        private Button btnLimpiar;
        private Panel separador;
        private DataGridView dgvUsuarios;
    }
}
