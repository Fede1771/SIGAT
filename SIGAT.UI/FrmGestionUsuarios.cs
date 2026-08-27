using System;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmGestionUsuarios : Form
    {
        private UsuarioBLL _bll = new UsuarioBLL();
        private DataGridView dgvUsuarios;
        private TextBox txtUsername, txtPass, txtNombre, txtApellido;
        private CheckBox chkActivo;
        private ComboBox cmbPerfil;
        private Button btnGuardar, btnEliminar, btnLimpiar;
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

            // Ancho reservado para el panel lateral de controles de la derecha
            int anchoPanelDerecho = 380;
            int margen = 40;

            // 1. Grilla a la izquierda (Ocupa todo el ancho flexible restante de forma limpia)
            dgvUsuarios = new DataGridView
            {
                Top = margen,
                Left = margen,
                Width = this.ClientSize.Width - (anchoPanelDerecho + (margen * 3)),
                Height = this.ClientSize.Height - 80,
                ReadOnly = true,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White
            };
            dgvUsuarios.CellClick += DgvUsuarios_CellClick;
            dgvUsuarios.DataBindingComplete += DgvUsuarios_DataBindingComplete;

            // 2. Panel lateral derecho anclado firmemente a la derecha
            var panelDerecho = new Panel
            {
                Top = margen,
                Width = anchoPanelDerecho,
                Height = this.ClientSize.Height - 80,
                Anchor = AnchorStyles.Top | AnchorStyles.Right | AnchorStyles.Bottom,
                BackColor = Color.Transparent
            };
            // Lo posicionamos exactamente al borde derecho restando su ancho y los márgenes
            panelDerecho.Left = this.ClientSize.Width - anchoPanelDerecho - margen;

            int topActual = 10;
            int salto = 62;

            var lbl1 = new Label { Text = "Usuario:", Top = topActual, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtUsername = new TextBox { Top = topActual + 22, Left = 10, Width = 350, Font = new Font("Segoe UI", 10), MaxLength = 50 };
            topActual += salto;

            var lbl2 = new Label { Text = "Clave (vacío no cambia):", Top = topActual, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtPass = new TextBox { Top = topActual + 22, Left = 10, Width = 350, UseSystemPasswordChar = true, Font = new Font("Segoe UI", 10), MaxLength = 100 };
            topActual += salto;

            var lbl3 = new Label { Text = "Nombre:", Top = topActual, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtNombre = new TextBox { Top = topActual + 22, Left = 10, Width = 350, Font = new Font("Segoe UI", 10), MaxLength = 80 };
            topActual += salto;

            var lbl4 = new Label { Text = "Apellido:", Top = topActual, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtApellido = new TextBox { Top = topActual + 22, Left = 10, Width = 350, Font = new Font("Segoe UI", 10), MaxLength = 80 };
            topActual += salto;

            var lbl5 = new Label { Text = "Perfil:", Top = topActual, Left = 10, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            cmbPerfil = new ComboBox { Top = topActual + 22, Left = 10, Width = 350, DropDownStyle = ComboBoxStyle.DropDownList, Font = new Font("Segoe UI", 10) };
            cmbPerfil.Items.Add(new Perfil { IdPerfil = 1, NombrePerfil = "Administrador" });
            cmbPerfil.Items.Add(new Perfil { IdPerfil = 2, NombrePerfil = "Operador" });
            topActual += salto + 10;

            chkActivo = new CheckBox { Text = "Activo", Top = topActual, Left = 10, Width = 100, Font = new Font("Segoe UI", 9, FontStyle.Bold), Checked = true };
            topActual += salto - 10;

            btnGuardar = new Button { Text = "Guardar", Top = topActual, Left = 10, Width = 110, Height = 35, Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White };
            btnGuardar.Click += BtnGuardar_Click;

            btnEliminar = new Button { Text = "Baja", Top = topActual, Left = 125, Width = 110, Height = 35, Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White };
            btnEliminar.Click += BtnEliminar_Click;

            btnLimpiar = new Button { Text = "Limpiar", Top = topActual, Left = 240, Width = 110, Height = 35, Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White };
            btnLimpiar.Click += (s, e) => Limpiar();

            panelDerecho.Controls.AddRange(new Control[] {
                lbl1, txtUsername,
                lbl2, txtPass,
                lbl3, txtNombre,
                lbl4, txtApellido,
                lbl5, cmbPerfil,
                chkActivo,
                btnGuardar, btnEliminar, btnLimpiar
            });

            this.Controls.AddRange(new Control[] { dgvUsuarios, panelDerecho });
            this.AcceptButton = btnGuardar;
        }

        private void CargarGrilla()
        {
            dgvUsuarios.DataSource = null;
            dgvUsuarios.DataSource = _bll.ObtenerTodos();
        }

        private void DgvUsuarios_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            if (dgvUsuarios.Columns["Password"] != null) dgvUsuarios.Columns["Password"].Visible = false;
            if (dgvUsuarios.Columns["IdPerfil"] != null) dgvUsuarios.Columns["IdPerfil"].Visible = false;
        }

        private void DgvUsuarios_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                var u = (Usuario)dgvUsuarios.Rows[e.RowIndex].DataBoundItem;
                _idSeleccionado = u.IdUsuario;
                txtUsername.Text = u.NombreUsuario;
                txtNombre.Text = u.Nombre;
                txtApellido.Text = u.Apellido;
                chkActivo.Checked = u.Activo;
                _passOriginal = u.Password;

                foreach (Perfil p in cmbPerfil.Items)
                {
                    if (p.IdPerfil == u.IdPerfil) cmbPerfil.SelectedItem = p;
                }
            }
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbPerfil.SelectedItem == null) throw new Exception("Seleccione perfil.");
                var u = new Usuario
                {
                    IdUsuario = _idSeleccionado,
                    NombreUsuario = txtUsername.Text,
                    Nombre = txtNombre.Text,
                    Apellido = txtApellido.Text,
                    Activo = chkActivo.Checked,
                    IdPerfil = ((Perfil)cmbPerfil.SelectedItem).IdPerfil,
                    Password = _idSeleccionado > 0 ? _passOriginal : ""
                };

                if (_idSeleccionado == 0) _bll.CrearUsuario(u, txtPass.Text);
                else _bll.ActualizarUsuario(u, txtPass.Text);

                MessageBox.Show("Operación exitosa.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarGrilla();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado > 0)
            {
                if (MessageBox.Show($"¿Desea dar de baja al usuario {txtUsername.Text}?", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    _bll.BajaLogicaUsuario(_idSeleccionado, txtUsername.Text);
                    MessageBox.Show("Usuario inhabilitado.", "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Limpiar();
                    CargarGrilla();
                }
            }
            else
            {
                MessageBox.Show("Seleccione un usuario de la grilla.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private void Limpiar()
        {
            _idSeleccionado = 0;
            _passOriginal = "";
            txtUsername.Clear(); txtPass.Clear(); txtNombre.Clear(); txtApellido.Clear();
            chkActivo.Checked = true; cmbPerfil.SelectedIndex = -1;
            dgvUsuarios.ClearSelection();
        }
    }
}