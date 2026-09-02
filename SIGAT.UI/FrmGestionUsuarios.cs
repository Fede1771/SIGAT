using System;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmGestionUsuarios : Form
    {
        private UsuarioBLL _usuarioBLL = new UsuarioBLL();

        // Controles de la pantalla
        private DataGridView dgvUsuarios;
        private TextBox txtUsername;
        private TextBox txtPass;
        private TextBox txtNombre;
        private TextBox txtApellido;
        private CheckBox chkActivo;
        private ComboBox cmbPerfil;
        private Button btnGuardar;
        private Button btnEliminar;
        private Button btnLimpiar;

        // Variables de estado
        private int _idSeleccionado = 0;
        private string _passOriginal = "";

        public FrmGestionUsuarios()
        {
            ConfigurarUI();
            CargarGrilla();
        }

        private void ConfigurarUI()
        {
            this.Text = "Gestión de Usuarios";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // 1. Usamos Padding para crear el margen de 40px en toda la ventana automáticamente
            this.Padding = new Padding(40);

            int anchoPanelDerecho = 380;

            // 2. Configuración del Panel Lateral (Derecha)
            Panel panelDerecho = new Panel();
            panelDerecho.Width = anchoPanelDerecho;
            panelDerecho.Dock = DockStyle.Right; // Se pega a la derecha respetando el margen
            panelDerecho.BackColor = Color.Transparent;

            // 3. Creamos un panel invisible para separar la grilla de los controles (40px)
            Panel separador = new Panel();
            separador.Width = 40;
            separador.Dock = DockStyle.Right; 
            separador.BackColor = Color.Transparent;

            // 4. Configuración de la Grilla (Izquierda)
            dgvUsuarios = new DataGridView();
            dgvUsuarios.Dock = DockStyle.Fill; // Ocupa automáticamente TODO el espacio sobrante
            dgvUsuarios.ReadOnly = true;
            dgvUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvUsuarios.MultiSelect = false;
            dgvUsuarios.AllowUserToAddRows = false;
            dgvUsuarios.AllowUserToDeleteRows = false;
            dgvUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvUsuarios.BackgroundColor = Color.White;

            dgvUsuarios.CellClick += new DataGridViewCellEventHandler(DgvUsuarios_CellClick);
            dgvUsuarios.DataBindingComplete += new DataGridViewBindingCompleteEventHandler(DgvUsuarios_DataBindingComplete);

            int topActual = 10;
            int salto = 62;

            // --- CAMPO: USUARIO ---
            Label lblUsuario = new Label();
            lblUsuario.Text = "Usuario:";
            lblUsuario.Top = topActual;
            lblUsuario.Left = 10;
            lblUsuario.AutoSize = true;
            lblUsuario.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtUsername = new TextBox();
            txtUsername.Top = topActual + 22;
            txtUsername.Left = 10;
            txtUsername.Width = 350;
            txtUsername.Font = new Font("Segoe UI", 10);
            txtUsername.MaxLength = 50;

            topActual = topActual + salto;

            // --- CAMPO: CLAVE ---
            Label lblClave = new Label();
            lblClave.Text = "Clave (vacío no cambia):";
            lblClave.Top = topActual;
            lblClave.Left = 10;
            lblClave.AutoSize = true;
            lblClave.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtPass = new TextBox();
            txtPass.Top = topActual + 22;
            txtPass.Left = 10;
            txtPass.Width = 350;
            txtPass.UseSystemPasswordChar = true;
            txtPass.Font = new Font("Segoe UI", 10);
            txtPass.MaxLength = 100;

            topActual = topActual + salto;

            // --- CAMPO: NOMBRE ---
            Label lblNombre = new Label();
            lblNombre.Text = "Nombre:";
            lblNombre.Top = topActual;
            lblNombre.Left = 10;
            lblNombre.AutoSize = true;
            lblNombre.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtNombre = new TextBox();
            txtNombre.Top = topActual + 22;
            txtNombre.Left = 10;
            txtNombre.Width = 350;
            txtNombre.Font = new Font("Segoe UI", 10);
            txtNombre.MaxLength = 80;

            topActual = topActual + salto;

            // --- CAMPO: APELLIDO ---
            Label lblApellido = new Label();
            lblApellido.Text = "Apellido:";
            lblApellido.Top = topActual;
            lblApellido.Left = 10;
            lblApellido.AutoSize = true;
            lblApellido.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtApellido = new TextBox();
            txtApellido.Top = topActual + 22;
            txtApellido.Left = 10;
            txtApellido.Width = 350;
            txtApellido.Font = new Font("Segoe UI", 10);
            txtApellido.MaxLength = 80;

            topActual = topActual + salto;

            // --- CAMPO: PERFIL ---
            Label lblPerfil = new Label();
            lblPerfil.Text = "Perfil:";
            lblPerfil.Top = topActual;
            lblPerfil.Left = 10;
            lblPerfil.AutoSize = true;
            lblPerfil.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            cmbPerfil = new ComboBox();
            cmbPerfil.Top = topActual + 22;
            cmbPerfil.Left = 10;
            cmbPerfil.Width = 350;
            cmbPerfil.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbPerfil.Font = new Font("Segoe UI", 10);

            Perfil perfilAdmin = new Perfil();
            perfilAdmin.IdPerfil = 1;
            perfilAdmin.NombrePerfil = "Administrador";

            Perfil perfilOperador = new Perfil();
            perfilOperador.IdPerfil = 2;
            perfilOperador.NombrePerfil = "Operador";

            cmbPerfil.Items.Add(perfilAdmin);
            cmbPerfil.Items.Add(perfilOperador);

            topActual = topActual + salto + 10;

            // --- CAMPO: ACTIVO ---
            chkActivo = new CheckBox();
            chkActivo.Text = "Activo";
            chkActivo.Top = topActual;
            chkActivo.Left = 10;
            chkActivo.Width = 100;
            chkActivo.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            chkActivo.Checked = true;

            topActual = topActual + salto - 10;

            // --- BOTONES ---
            btnGuardar = new Button();
            btnGuardar.Text = "Guardar";
            btnGuardar.Top = topActual;
            btnGuardar.Left = 10;
            btnGuardar.Width = 110;
            btnGuardar.Height = 35;
            btnGuardar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnGuardar.BackColor = Color.White;
            btnGuardar.Click += new EventHandler(BtnGuardar_Click);

            btnEliminar = new Button();
            btnEliminar.Text = "Baja";
            btnEliminar.Top = topActual;
            btnEliminar.Left = 125;
            btnEliminar.Width = 110;
            btnEliminar.Height = 35;
            btnEliminar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnEliminar.BackColor = Color.White;
            btnEliminar.Click += new EventHandler(BtnEliminar_Click);

            btnLimpiar = new Button();
            btnLimpiar.Text = "Limpiar";
            btnLimpiar.Top = topActual;
            btnLimpiar.Left = 240;
            btnLimpiar.Width = 110;
            btnLimpiar.Height = 35;
            btnLimpiar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnLimpiar.BackColor = Color.White;
            btnLimpiar.Click += new EventHandler(BtnLimpiar_Click);

            // Agregamos controles al panel derecho
            panelDerecho.Controls.Add(lblUsuario);
            panelDerecho.Controls.Add(txtUsername);
            panelDerecho.Controls.Add(lblClave);
            panelDerecho.Controls.Add(txtPass);
            panelDerecho.Controls.Add(lblNombre);
            panelDerecho.Controls.Add(txtNombre);
            panelDerecho.Controls.Add(lblApellido);
            panelDerecho.Controls.Add(txtApellido);
            panelDerecho.Controls.Add(lblPerfil);
            panelDerecho.Controls.Add(cmbPerfil);
            panelDerecho.Controls.Add(chkActivo);
            panelDerecho.Controls.Add(btnGuardar);
            panelDerecho.Controls.Add(btnEliminar);
            panelDerecho.Controls.Add(btnLimpiar);

            // IMPORTANTE: El orden en que se agregan al form define cómo actúa el Dock.
            this.Controls.Add(panelDerecho); // 1. Reclama la derecha
            this.Controls.Add(separador);    // 2. Reclama los 40px a la izquierda del panel
            this.Controls.Add(dgvUsuarios);  // 3. Llena todo el espacio que sobra a la izquierda

            this.AcceptButton = btnGuardar;
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