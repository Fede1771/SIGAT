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
            // 1. Configuración de la ventana principal
            this.Text = "SIGAT - Iniciar Sesión";
            this.Size = new Size(350, 430);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.White; // Fondo blanco limpio y corporativo

            // 2. Configuración y carga del Logo
            PictureBox picLogo = new PictureBox();
            picLogo.Size = new Size(150, 150);
            picLogo.Location = new Point(90, 20); // Centrado horizontalmente
            picLogo.SizeMode = PictureBoxSizeMode.Zoom;

            // Carga segura de la imagen desde tu ruta
            try
            {
                picLogo.Image = Image.FromFile(@"C:\Users\feder\Documents\SIGAT\SIGAT\SIGAT LOGO.png");
            }
            catch
            {
                // Si la imagen no se encuentra, el programa no se cae, simplemente muestra el espacio en blanco
            }

            this.Controls.Add(picLogo);

            // Coordenada Y inicial para los controles de texto (debajo del logo)
            int topActual = 180;

            // 3. Campo de Usuario
            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.Location = new Point(65, topActual);
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.Controls.Add(lblUsuario);

            txtUsuario = new TextBox();
            txtUsuario.Location = new Point(65, topActual + 22);
            txtUsuario.Width = 200;
            txtUsuario.Font = new Font("Segoe UI", 10);
            this.Controls.Add(txtUsuario);

            // 4. Campo de Contraseña
            Label lblPassword = new Label();
            lblPassword.Text = "Contraseña:";
            lblPassword.Location = new Point(65, topActual + 60);
            lblPassword.AutoSize = true;
            lblPassword.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            this.Controls.Add(lblPassword);

            txtPassword = new TextBox();
            txtPassword.Location = new Point(65, topActual + 82);
            txtPassword.Width = 200;
            txtPassword.UseSystemPasswordChar = true;
            txtPassword.Font = new Font("Segoe UI", 10);
            this.Controls.Add(txtPassword);

            // 5. Botón de Ingreso Moderno
            btnLogin = new Button();
            btnLogin.Text = "Ingresar";
            btnLogin.Location = new Point(105, topActual + 135);
            btnLogin.Size = new Size(120, 40);
            btnLogin.Font = new Font("Segoe UI", 10, FontStyle.Bold);

            // Estética "Flat" (Plana) para el botón
            btnLogin.BackColor = Color.FromArgb(0, 120, 215); // Azul característico de Windows
            btnLogin.ForeColor = Color.White;
            btnLogin.FlatStyle = FlatStyle.Flat;
            btnLogin.FlatAppearance.BorderSize = 0;
            btnLogin.Cursor = Cursors.Hand;

            btnLogin.Click += new EventHandler(BtnLogin_Click);

            this.AcceptButton = btnLogin;
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