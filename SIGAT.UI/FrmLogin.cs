using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmLogin : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public FrmLogin()
        {
            InitializeComponent();

            CargarLogo();
            CrearUsuarioAdminInicial();
        }

        private void CargarLogo()
        {
            // Carga segura de la imagen desde tu ruta
            try
            {
                picLogo.Image = Image.FromFile(@"C:\Users\feder\Documents\SIGAT\SIGAT\SIGAT LOGO.png");
            }
            catch
            {
                // Si la imagen no se encuentra, el programa no se cae, simplemente muestra el espacio en blanco
            }
        }

        private void CrearUsuarioAdminInicial()
        {
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
