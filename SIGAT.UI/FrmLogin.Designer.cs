namespace SIGAT.UI
{
    partial class FrmLogin
    {
        private System.ComponentModel.IContainer components;
        private TableLayoutPanel tablelayoutpanel1;
        private PictureBox picLogo;
        private Label label2;
        private Label label3;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        // Estructura visual editable desde el diseÃ±ador; el comportamiento queda en el .cs.
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            tablelayoutpanel1 = new TableLayoutPanel();
            picLogo = new PictureBox();
            label2 = new Label();
            label3 = new Label();
            lblUsuario = new Label();
            txtUsuario = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            btnLogin = new Button();
            tablelayoutpanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).BeginInit();
            SuspendLayout();
            // 
            // tablelayoutpanel1
            // 
            tablelayoutpanel1.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel1.ColumnCount = 1;
            tablelayoutpanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Controls.Add(picLogo, 0, 0);
            tablelayoutpanel1.Controls.Add(label2, 0, 1);
            tablelayoutpanel1.Controls.Add(label3, 0, 2);
            tablelayoutpanel1.Controls.Add(lblUsuario, 0, 3);
            tablelayoutpanel1.Controls.Add(txtUsuario, 0, 4);
            tablelayoutpanel1.Controls.Add(lblPassword, 0, 6);
            tablelayoutpanel1.Controls.Add(txtPassword, 0, 7);
            tablelayoutpanel1.Controls.Add(btnLogin, 0, 9);
            tablelayoutpanel1.Dock = DockStyle.Fill;
            tablelayoutpanel1.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel1.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel1.Location = new Point(32, 32);
            tablelayoutpanel1.Name = "tablelayoutpanel1";
            tablelayoutpanel1.Padding = new Padding(28, 22, 28, 22);
            tablelayoutpanel1.RowCount = 11;
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 12F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 26F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 36F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 20F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 46F));
            tablelayoutpanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel1.Size = new Size(396, 546);
            tablelayoutpanel1.TabIndex = 0;
            // 
            // picLogo
            // 
            picLogo.Anchor = AnchorStyles.None;
            picLogo.BackColor = Color.FromArgb(255, 255, 255);
            picLogo.Font = new Font("Segoe UI", 10F);
            picLogo.ForeColor = Color.FromArgb(30, 45, 65);
            picLogo.Image = (Image)resources.GetObject("picLogo.Image");
            picLogo.Location = new Point(142, 25);
            picLogo.Name = "picLogo";
            picLogo.Size = new Size(112, 94);
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            picLogo.TabIndex = 0;
            picLogo.TabStop = false;
            // 
            // label2
            // 
            label2.BackColor = Color.FromArgb(255, 255, 255);
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(30, 45, 65);
            label2.Location = new Point(31, 122);
            label2.Name = "label2";
            label2.Size = new Size(334, 38);
            label2.TabIndex = 1;
            label2.Text = "Bienvenido a SIGAT";
            label2.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            label3.BackColor = Color.FromArgb(255, 255, 255);
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 10F);
            label3.ForeColor = Color.FromArgb(94, 111, 133);
            label3.Location = new Point(31, 160);
            label3.Name = "label3";
            label3.Size = new Size(334, 46);
            label3.TabIndex = 2;
            label3.Text = "Ingresá con tu cuenta para acceder al sistema.";
            label3.TextAlign = ContentAlignment.TopCenter;
            // 
            // lblUsuario
            // 
            lblUsuario.AutoSize = true;
            lblUsuario.BackColor = Color.FromArgb(255, 255, 255);
            lblUsuario.Dock = DockStyle.Fill;
            lblUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.FromArgb(30, 45, 65);
            lblUsuario.Location = new Point(28, 206);
            lblUsuario.Margin = new Padding(0, 0, 0, 6);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(340, 20);
            lblUsuario.TabIndex = 3;
            lblUsuario.Text = "Usuario";
            // 
            // txtUsuario
            // 
            txtUsuario.BackColor = Color.FromArgb(255, 255, 255);
            txtUsuario.Dock = DockStyle.Fill;
            txtUsuario.Font = new Font("Segoe UI", 12F);
            txtUsuario.ForeColor = Color.FromArgb(0, 0, 0);
            txtUsuario.Location = new Point(28, 232);
            txtUsuario.Margin = new Padding(0);
            txtUsuario.MaxLength = 50;
            txtUsuario.Name = "txtUsuario";
            txtUsuario.Size = new Size(340, 29);
            txtUsuario.TabIndex = 0;
            // 
            // lblPassword
            // 
            lblPassword.AutoSize = true;
            lblPassword.BackColor = Color.FromArgb(255, 255, 255);
            lblPassword.Dock = DockStyle.Fill;
            lblPassword.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblPassword.ForeColor = Color.FromArgb(30, 45, 65);
            lblPassword.Location = new Point(28, 280);
            lblPassword.Margin = new Padding(0, 0, 0, 6);
            lblPassword.Name = "lblPassword";
            lblPassword.Size = new Size(340, 20);
            lblPassword.TabIndex = 4;
            lblPassword.Text = "Contraseña";
            // 
            // txtPassword
            // 
            txtPassword.BackColor = Color.FromArgb(255, 255, 255);
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Font = new Font("Segoe UI", 12F);
            txtPassword.ForeColor = Color.FromArgb(0, 0, 0);
            txtPassword.Location = new Point(28, 306);
            txtPassword.Margin = new Padding(0);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(340, 29);
            txtPassword.TabIndex = 1;
            txtPassword.UseSystemPasswordChar = true;
            // 
            // btnLogin
            // 
            btnLogin.AutoSize = true;
            btnLogin.BackColor = Color.FromArgb(35, 101, 175);
            btnLogin.Dock = DockStyle.Fill;
            btnLogin.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnLogin.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnLogin.ForeColor = Color.FromArgb(255, 255, 255);
            btnLogin.Location = new Point(28, 362);
            btnLogin.Margin = new Padding(0);
            btnLogin.MinimumSize = new Size(96, 40);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(340, 46);
            btnLogin.TabIndex = 2;
            btnLogin.Text = "Ingresar";
            btnLogin.UseVisualStyleBackColor = false;
            btnLogin.Click += BtnLogin_Click;
            // 
            // FrmLogin
            // 
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(255, 255, 255);
            ClientSize = new Size(460, 610);
            Controls.Add(tablelayoutpanel1);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "FrmLogin";
            Padding = new Padding(32);
            StartPosition = FormStartPosition.CenterScreen;
            Text = "SIGAT · Iniciar sesión";
            tablelayoutpanel1.ResumeLayout(false);
            tablelayoutpanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLogo).EndInit();
            ResumeLayout(false);
        }
    }
}
