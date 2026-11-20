using SIGAT.BE;
using SIGAT.BE.Integridad;
using SIGAT.BLL;
using SIGAT.SERVICIOS;

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
            ResultadoLogin resultado;
            try
            {
                do
                {
                    resultado = _usuarioBLL.Autenticar(txtUsuario.Text, txtPassword.Text, out usuarioValidado);
                    if (resultado != ResultadoLogin.RequiereRecuperacion) break;
                    try
                    {
                        if (Program.MostrarRecuperacion(this, new IntegridadBLL().Verificar()) != DialogResult.OK) return;
                    }
                    finally { SesionServicio.ObtenerInstancia().CerrarSesion(); }
                    // Después de reparar, validamos otra vez la clave, la integridad y los roles actuales.
                    // La sesión restringida nunca se convierte directamente en un acceso normal.
                } while (true);
            }
            catch (IntegridadException)
            {
                SesionServicio.ObtenerInstancia().CerrarSesion();
                Program.MostrarAvisoMantenimiento(this);
                return;
            }
            finally { txtPassword.Clear(); txtPassword.Focus(); }

            if (resultado == ResultadoLogin.Mantenimiento)
            {
                Program.MostrarAvisoMantenimiento(this);
            }
            else if (resultado == ResultadoLogin.CredencialesInvalidas)
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
