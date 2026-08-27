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

            // Creamos y configuramos el Label de Usuario
            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.Location = new Point(50, 30);
            lblUsuario.AutoSize = true;
            this.Controls.Add(lblUsuario);

            // Creamos y configuramos el TextBox de Usuario
            txtUsuario = new TextBox();
            txtUsuario.Location = new Point(50, 50);
            txtUsuario.Width = 200;
            this.Controls.Add(txtUsuario);

            // Creamos y configuramos el Label de Contraseña
            Label lblPassword = new Label();
            lblPassword.Text = "Contraseña:";
            lblPassword.Location = new Point(50, 90);
            lblPassword.AutoSize = true;
            this.Controls.Add(lblPassword);

            // Creamos y configuramos el TextBox de Contraseña
            txtPassword = new TextBox();
            txtPassword.Location = new Point(50, 110);
            txtPassword.Width = 200;
            txtPassword.UseSystemPasswordChar = true;
            this.Controls.Add(txtPassword);

            // Creamos y configuramos el Botón de Ingreso
            btnLogin = new Button();
            btnLogin.Text = "Ingresar";
            btnLogin.Location = new Point(90, 150);
            btnLogin.Size = new Size(120, 35);
            btnLogin.Click += new EventHandler(BtnLogin_Click);

            this.AcceptButton = btnLogin; // Permite ingresar con la tecla Enter
            this.Controls.Add(btnLogin);

            // --- TRUCO DE INICIALIZACIÓN ---
            try
            {
                if (_usuarioBLL.ObtenerTodos().Count == 0)
                {
                    Usuario adminInicial = new Usuario();
                    adminInicial.NombreUsuario = "admin";
                    adminInicial.Nombre = "Admin";
                    adminInicial.Apellido = "Sistema";
                    adminInicial.Activo = true;
                    adminInicial.IdPerfil = 1;

                    _usuarioBLL.CrearUsuario(adminInicial, "admin");
                }
            }
            catch
            {
            }
            // ------------------------------------------------
        }

        private void BtnLogin_Click(object sender, EventArgs e)
        {
            Usuario usuarioValidado;
            ResultadoLogin resultado = _usuarioBLL.Autenticar(txtUsuario.Text, txtPassword.Text, out usuarioValidado);

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
                FrmPrincipal frmPrincipal = new FrmPrincipal();
                frmPrincipal.ShowDialog();
                this.Close();
            }
        }
    }
}