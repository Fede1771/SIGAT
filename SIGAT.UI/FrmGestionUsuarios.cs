using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmGestionUsuarios : Form, IIdiomaObserver
    {
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        private int _idSeleccionado;

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            Tag = "frmgestionusuarios_titulo";
            lblUsuario.Tag = "lbl_usuario_gu";
            lblClave.Tag = "lbl_clave";
            lblNombre.Tag = "lbl_nombre";
            lblApellido.Tag = "lbl_apellido";
            chkActivo.Tag = "chk_activo";
            btnGuardar.Tag = "btn_guardar";
            btnEliminar.Tag = "btn_eliminar";
            btnLimpiar.Tag = "btn_limpiar";
            lblAyuda.Tag = "lbl_ayuda_roles";
            ConfigurarGrilla();
            Load += FrmGestionUsuarios_Load;
            FormClosed += (s, e) => IdiomaManager.ObtenerInstancia().Desuscribir(this);
            Limpiar();
        }

        private void FrmGestionUsuarios_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            try { CargarGrilla(); ActualizarIdioma(); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Gestión de usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void ConfigurarGrilla()
        {
            // Columnas explícitas: no se generan Id duplicado, Password ni colecciones.
            dgvUsuarios.AutoGenerateColumns = false;
            AgregarColumna("IdUsuario", "ID", "col_idusuario", 45, 45);
            AgregarColumna("NombreUsuario", "Usuario", "col_nombreusuario", 130, 100);
            AgregarColumna("Nombre", "Nombre", "col_nombre", 120, 90);
            AgregarColumna("Apellido", "Apellido", "col_apellido", 120, 90);
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "Activo", DataPropertyName = "Activo", HeaderText = "Activo", Tag = "col_activo",
                FillWeight = 85, MinimumWidth = 85, SortMode = DataGridViewColumnSortMode.NotSortable
            });
            AgregarColumna("Roles", "Roles asignados", "col_roles_asignados", 200, 150);
            dgvUsuarios.Columns["Roles"].DataPropertyName = "";
            dgvUsuarios.CellFormatting += (s, e) =>
            {
                if (e.RowIndex < 0 || dgvUsuarios.Rows[e.RowIndex].DataBoundItem is not Usuario usuario) return;
                if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Roles")
                {
                    string nombres = "";
                    foreach (Rol rol in usuario.Roles)
                    {
                        if (nombres != "") nombres += ", ";
                        nombres += rol.Nombre;
                    }
                    e.Value = nombres == "" ? IdiomaManager.ObtenerInstancia().Traducir(Name, "sin_roles", "Sin roles asignados") : nombres;
                    e.FormattingApplied = true;
                }
                if (!usuario.Activo) e.CellStyle.ForeColor = Color.DimGray;
                if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Activo")
                {
                    e.Value = IdiomaManager.ObtenerInstancia().Traducir(Name,
                        usuario.Activo ? "chk_activo" : "estado_inactivo", usuario.Activo ? "Activo" : "Inactivo");
                    e.CellStyle.BackColor = usuario.Activo ? TemaVisual.VerdeSuave : TemaVisual.Fondo;
                    e.CellStyle.ForeColor = usuario.Activo ? TemaVisual.Verde : TemaVisual.Secundario;
                    e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
                    e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
                    e.CellStyle.Font = TemaVisual.FuenteEstado;
                    e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                    e.FormattingApplied = true;
                }
            };
            dgvUsuarios.CellClick += (s, e) => { if (e.RowIndex >= 0) SeleccionarFila(); };
            dgvUsuarios.KeyUp += (s, e) =>
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Enter) SeleccionarFila();
            };
        }

        private void AgregarColumna(string propiedad, string titulo, string clave, float ancho, int minimo)
        {
            dgvUsuarios.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = propiedad, DataPropertyName = propiedad, HeaderText = titulo, Tag = clave,
                FillWeight = ancho, MinimumWidth = minimo, SortMode = DataGridViewColumnSortMode.NotSortable
            });
        }

        private void CargarGrilla()
        {
            dgvUsuarios.DataSource = _usuarioBLL.ObtenerTodos();
            dgvUsuarios.ClearSelection();
            dgvUsuarios.CurrentCell = null;
        }

        private void SeleccionarFila()
        {
            if (dgvUsuarios.CurrentRow?.DataBoundItem is not Usuario usuario) return;
            _idSeleccionado = usuario.IdUsuario;
            txtUsername.Text = usuario.NombreUsuario;
            txtNombre.Text = usuario.Nombre;
            txtApellido.Text = usuario.Apellido;
            chkActivo.Checked = usuario.Activo;
            // Nunca reutilizar una clave que se escribió para otra cuenta.
            txtPass.Clear();
            btnEliminar.Enabled = usuario.Activo && !usuario.EsAdministradorOriginal;
            chkActivo.Enabled = !usuario.EsAdministradorOriginal;
        }

        private void BtnGuardar_Click(object sender, EventArgs e)
        {
            try
            {
                Usuario usuario = new Usuario
                {
                    IdUsuario = _idSeleccionado, NombreUsuario = txtUsername.Text,
                    Nombre = txtNombre.Text, Apellido = txtApellido.Text, Activo = chkActivo.Checked
                };
                if (_idSeleccionado == 0) _usuarioBLL.CrearUsuario(usuario, txtPass.Text);
                else _usuarioBLL.ActualizarUsuario(usuario, txtPass.Text);
                MessageBox.Show("Usuario guardado correctamente.", "Gestión de usuarios", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Limpiar();
                CargarGrilla();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Gestión de usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnEliminar_Click(object sender, EventArgs e)
        {
            if (_idSeleccionado == 0) return;
            if (MessageBox.Show("¿Desea dar de baja al usuario " + txtUsername.Text + "?", "Confirmación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                _usuarioBLL.BajaLogicaUsuario(_idSeleccionado, txtUsername.Text);
                Limpiar();
                CargarGrilla();
                MessageBox.Show("Usuario inhabilitado correctamente.", "Gestión de usuarios");
            }
            catch (Exception ex) { MessageBox.Show(ex.Message, "Gestión de usuarios", MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void BtnLimpiar_Click(object sender, EventArgs e) { Limpiar(); }

        private void Limpiar()
        {
            _idSeleccionado = 0;
            txtUsername.Clear();
            txtPass.Clear();
            txtNombre.Clear();
            txtApellido.Clear();
            chkActivo.Checked = true;
            chkActivo.Enabled = true;
            btnEliminar.Enabled = false;
            dgvUsuarios.ClearSelection();
            dgvUsuarios.CurrentCell = null;
            txtUsername.Focus();
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
            dgvUsuarios.Invalidate();
        }
    }
}
