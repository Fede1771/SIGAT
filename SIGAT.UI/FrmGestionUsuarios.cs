using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmGestionUsuarios : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        // Variables de estado
        private int _idSeleccionado = 0;
        private string _passOriginal = "";

        public FrmGestionUsuarios()
        {
            InitializeComponent();

            CargarPerfiles();
            CargarGrilla();
        }

        private void CargarPerfiles()
        {
            Perfil perfilAdmin = new Perfil();
            perfilAdmin.IdPerfil = 1;
            perfilAdmin.NombrePerfil = "Administrador";

            Perfil perfilOperador = new Perfil();
            perfilOperador.IdPerfil = 2;
            perfilOperador.NombrePerfil = "Operador";

            cmbPerfil.Items.Add(perfilAdmin);
            cmbPerfil.Items.Add(perfilOperador);
        }

        private void CargarGrilla()
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = _usuarioBLL.ObtenerTodos();
        }

        private void DgvUsuarios_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Ocultamos columnas sensibles o técnicas que el usuario no necesita ver
            if (dgvUsuarios.Columns["Password"] != null)
            {
                dgvUsuarios.Columns["Password"].Visible = false;
            }

            if (dgvUsuarios.Columns["IdPerfil"] != null)
            {
                dgvUsuarios.Columns["IdPerfil"].Visible = false;
            }
        }

        private void DgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            // Verificamos que se haya hecho clic en una fila válida (evita errores si tocan los títulos)
            if (e.RowIndex >= 0)
            {
                Usuario usuarioFila = (Usuario)dgvUsuarios.Rows[e.RowIndex].DataBoundItem;

                _idSeleccionado = usuarioFila.IdUsuario;
                txtUsername.Text = usuarioFila.NombreUsuario;
                txtNombre.Text = usuarioFila.Nombre;
                txtApellido.Text = usuarioFila.Apellido;
                chkActivo.Checked = usuarioFila.Activo;
                _passOriginal = usuarioFila.Password;

                // Buscamos el perfil correcto en el combo box y lo seleccionamos
                foreach (Perfil perfilActual in cmbPerfil.Items)
                {
                    if (perfilActual.IdPerfil == usuarioFila.IdPerfil)
                    {
                        cmbPerfil.SelectedItem = perfilActual;
                    }
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPerfil.SelectedItem == null)
                {
                    throw new Exception("Debe seleccionar un perfil para el usuario.");
                }

                // Armamos el objeto con los datos del formulario
                Usuario usuarioParaGuardar = new Usuario();
                usuarioParaGuardar.IdUsuario = _idSeleccionado;
                usuarioParaGuardar.NombreUsuario = txtUsername.Text;
                usuarioParaGuardar.Nombre = txtNombre.Text;
                usuarioParaGuardar.Apellido = txtApellido.Text;
                usuarioParaGuardar.Activo = chkActivo.Checked;

                Perfil perfilSeleccionado = (Perfil)cmbPerfil.SelectedItem;
                usuarioParaGuardar.IdPerfil = perfilSeleccionado.IdPerfil;

                // Si es un usuario existente mantenemos la clave original si no escribió una nueva
                if (_idSeleccionado > 0)
                {
                    usuarioParaGuardar.Password = _passOriginal;
                }
                else
                {
                    usuarioParaGuardar.Password = "";
                }

                // Decidimos si es un Alta (Insert) o una Modificación (Update)
                if (_idSeleccionado == 0)
                {
                    _usuarioBLL.CrearUsuario(usuarioParaGuardar, txtPass.Text);
                }
                else
                {
                    _usuarioBLL.ActualizarUsuario(usuarioParaGuardar, txtPass.Text);
                }

                MessageBox.Show("Operación exitosa.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarGrilla();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado > 0)
            {
                DialogResult respuesta = MessageBox.Show("¿Desea dar de baja al usuario " + txtUsername.Text + "?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (respuesta == DialogResult.Yes)
                {
                    _usuarioBLL.BajaLogicaUsuario(_idSeleccionado, txtUsername.Text);
                    MessageBox.Show("Usuario inhabilitado correctamente.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar();
                    CargarGrilla();
                }
            }
            else
            {
                MessageBox.Show("Debe seleccionar un usuario de la grilla primero.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e)
        {
            Limpiar();
        }

        private void Limpiar()
        {
            _idSeleccionado = 0;
            _passOriginal = "";

            txtUsername.Clear();
            txtPass.Clear();
            txtNombre.Clear();
            txtApellido.Clear();

            chkActivo.Checked = true;
            cmbPerfil.SelectedIndex = -1;

            dgvUsuarios.ClearSelection();
        }
    }
}
