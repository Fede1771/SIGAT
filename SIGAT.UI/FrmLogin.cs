using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmLogin : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        public FrmLogin() : this(true) { }

        internal FrmLogin(bool inicializarAdministrador)
        {
            InitializeComponent();
            if (EntornoDiseno.Activo) return;
            TemaVisual.Formulario(this);
            if (inicializarAdministrador) CrearUsuarioAdminInicial();
        }

        // Solo intentamos el alta inicial si no hay cuentas; una instalación SQL normal ya contiene su administrador.
        private void CrearUsuarioAdminInicial()
        {
            try
            {
                if (_usuarioBLL.ObtenerTodos().Count == 0)
                {
                    Usuario adminInicial = new Usuario();
                    adminInicial.NombreUsuario = "admin";
                    adminInicial.Nombre = "Admin";
                    adminInicial.Apellido = "Sistema";
                    adminInicial.Activo = true;


                    _usuarioBLL.CrearAdministradorInicial(adminInicial, "admin");
                }
            }
            catch
            {}
        }

        // La BLL distingue credenciales incorrectas, cuenta inactiva e ingreso válido; aquí presentamos el resultado.
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
