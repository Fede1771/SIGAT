namespace SIGAT.UI
{
    partial class FrmLogin
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
            this.picLogo = new PictureBox();
            this.lblUsuario = new Label();
            this.txtUsuario = new TextBox();
            this.lblPassword = new Label();
            this.txtPassword = new TextBox();
            this.btnLogin = new Button();
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).BeginInit();
            this.SuspendLayout();
            //
            // picLogo
            //
            this.picLogo.Location = new Point(90, 20);
            this.picLogo.Name = "picLogo";
            this.picLogo.Size = new Size(150, 150);
            this.picLogo.SizeMode = PictureBoxSizeMode.Zoom;
            this.picLogo.TabIndex = 0;
            this.picLogo.TabStop = false;
            //
            // lblUsuario
            //
            this.lblUsuario.AutoSize = true;
            this.lblUsuario.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblUsuario.Location = new Point(65, 180);
            this.lblUsuario.Name = "lblUsuario";
            this.lblUsuario.Size = new Size(58, 15);
            this.lblUsuario.TabIndex = 1;
            this.lblUsuario.Text = "Usuario:";
            //
            // txtUsuario
            //
            this.txtUsuario.Font = new Font("Segoe UI", 10F);
            this.txtUsuario.Location = new Point(65, 202);
            this.txtUsuario.Name = "txtUsuario";
            this.txtUsuario.Size = new Size(200, 25);
            this.txtUsuario.TabIndex = 2;
            //
            // lblPassword
            //
            this.lblPassword.AutoSize = true;
            this.lblPassword.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            this.lblPassword.Location = new Point(65, 240);
            this.lblPassword.Name = "lblPassword";
            this.lblPassword.Size = new Size(80, 15);
            this.lblPassword.TabIndex = 3;
            this.lblPassword.Text = "Contraseña:";
            //
            // txtPassword
            //
            this.txtPassword.Font = new Font("Segoe UI", 10F);
            this.txtPassword.Location = new Point(65, 262);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new Size(200, 25);
            this.txtPassword.TabIndex = 4;
            this.txtPassword.UseSystemPasswordChar = true;
            //
            // btnLogin
            //
            this.btnLogin.BackColor = Color.FromArgb(0, 120, 215);
            this.btnLogin.Cursor = Cursors.Hand;
            this.btnLogin.FlatAppearance.BorderSize = 0;
            this.btnLogin.FlatStyle = FlatStyle.Flat;
            this.btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            this.btnLogin.ForeColor = Color.White;
            this.btnLogin.Location = new Point(105, 315);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new Size(120, 40);
            this.btnLogin.TabIndex = 5;
            this.btnLogin.Text = "Ingresar";
            this.btnLogin.UseVisualStyleBackColor = false;
            this.btnLogin.Click += new EventHandler(this.BtnLogin_Click);
            //
            // FrmLogin
            //
            this.AcceptButton = this.btnLogin;
            this.AutoScaleMode = AutoScaleMode.Font;
            this.BackColor = Color.White;
            this.ClientSize = new Size(350, 430);
            this.Controls.Add(this.picLogo);
            this.Controls.Add(this.lblUsuario);
            this.Controls.Add(this.txtUsuario);
            this.Controls.Add(this.lblPassword);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.btnLogin);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "FrmLogin";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "SIGAT - Iniciar Sesión";
            ((System.ComponentModel.ISupportInitialize)(this.picLogo)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private PictureBox picLogo;
        private Label lblUsuario;
        private TextBox txtUsuario;
        private Label lblPassword;
        private TextBox txtPassword;
        private Button btnLogin;
    }
}
