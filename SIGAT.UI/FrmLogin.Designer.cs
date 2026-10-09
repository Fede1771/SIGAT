namespace SIGAT.UI
{
    partial class FrmLogin
    {
        private System.ComponentModel.IContainer components;
        private PictureBox picLogo;
        private Label lblUsuario, lblPassword;
        private TextBox txtUsuario, txtPassword;
        private Button btnLogin;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            var resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLogin));
            SuspendLayout();
            TemaVisual.Formulario(this);
            BackColor = Color.White;
            Name = "FrmLogin";
            Text = "SIGAT · Iniciar sesión";
            ClientSize = new Size(460, 610);
            AutoScaleDimensions = CurrentAutoScaleDimensions;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            StartPosition = FormStartPosition.CenterScreen;
            Padding = new Padding(32);

            var form = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 11, Padding = new Padding(28, 22, 28, 22), BackColor = Color.White };
            form.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            foreach (int height in new[] { 100, 38, 46, 26, 36, 12, 26, 36, 20, 46 })
                form.RowStyles.Add(new RowStyle(SizeType.Absolute, height));
            form.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            picLogo = new PictureBox { Name = "picLogo", Image = (Image)resources.GetObject("picLogo.Image"), SizeMode = PictureBoxSizeMode.Zoom, Size = new Size(112, 104), Anchor = AnchorStyles.None, TabStop = false };
            form.Controls.Add(picLogo, 0, 0);
            form.Controls.Add(new Label { Text = "Bienvenido a SIGAT", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Segoe UI", 19F, FontStyle.Bold), ForeColor = TemaVisual.Texto }, 0, 1);
            form.Controls.Add(new Label { Text = "Ingresá con tu cuenta para acceder al sistema.", Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopCenter, ForeColor = TemaVisual.Secundario }, 0, 2);
            lblUsuario = TemaVisual.Etiqueta("Usuario");
            txtUsuario = new TextBox { Name = "txtUsuario", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12F), TabIndex = 0, MaxLength = 50, Margin = Padding.Empty };
            lblPassword = TemaVisual.Etiqueta("Contraseña");
            txtPassword = new TextBox { Name = "txtPassword", Dock = DockStyle.Fill, Font = new Font("Segoe UI", 12F), TabIndex = 1, UseSystemPasswordChar = true, Margin = Padding.Empty };
            btnLogin = TemaVisual.Boton("btnLogin", "Ingresar", true);
            btnLogin.Dock = DockStyle.Fill;
            btnLogin.Margin = Padding.Empty;
            btnLogin.TabIndex = 2;
            btnLogin.Click += BtnLogin_Click;
            form.Controls.Add(lblUsuario, 0, 3);
            form.Controls.Add(txtUsuario, 0, 4);
            form.Controls.Add(lblPassword, 0, 6);
            form.Controls.Add(txtPassword, 0, 7);
            form.Controls.Add(btnLogin, 0, 9);
            Controls.Add(form);
            AcceptButton = btnLogin;
            ResumeLayout(true);
        }
    }
}
