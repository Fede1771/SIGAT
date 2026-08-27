using SIGAT.BE;
using SIGAT.BLL;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace SIGAT.UI
{
    public partial class FrmLogin : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private TextBox txtUsuario;
        private TextBox txtPassword;
        private Button btnLogin;

        public FrmLogin()
        {
            this.Text = "SIGAT - Iniciar Sesión";
            this.Size = new Size(320, 250);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;

            this.Controls.Add(new Label { Text = "Usuario:", Location = new Point(50, 30), AutoSize = true });
            txtUsuario = new TextBox { Location = new Point(50, 50), Width = 200 };
            this.Controls.Add(txtUsuario);

            this.Controls.Add(new Label { Text = "Contraseña:", Location = new Point(50, 90), AutoSize = true });
            txtPassword = new TextBox { Location = new Point(50, 110), Width = 200, UseSystemPasswordChar = true };
            this.Controls.Add(txtPassword);

            btnLogin = new Button { Text = "Ingresar", Location = new Point(90, 150), Size = new Size(120, 35) };
            btnLogin.Click += BtnLogin_Click;
            this.AcceptButton = btnLogin;
            this.Controls.Add(btnLogin);
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            var resultado = _usuarioBLL.Autenticar(txtUsuario.Text, txtPassword.Text, out Usuario usuario);

            if (resultado == ResultadoLogin.CredencialesInvalidas)
            {
                MessageBox.Show("Usuario o contraseña incorrectos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else if (resultado == ResultadoLogin.UsuarioInactivo)
            {
                MessageBox.Show("El usuario está inactivo.", "Atención", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else if (resultado == ResultadoLogin.Exito)
            {
                this.Hide();
                new FrmPrincipal().ShowDialog();
                this.Close();
            }
        }
    }
}