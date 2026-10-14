using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmGestionUsuarios : Form, IIdiomaObserver
    {
        private readonly UsuarioBLL _usuarioBLL = new UsuarioBLL();
        // ID cero indica alta; un ID seleccionado indica edición. Así el mismo formulario resuelve las dos operaciones.
        private int _idSeleccionado;

        public FrmGestionUsuarios()
        {
            InitializeComponent();
            // El enlace usa las columnas del diseñador; nunca expone propiedades internas.
            dgvUsuarios.AutoGenerateColumns = false;
            if (EntornoDiseno.Activo) return;
            TemaVisual.Formulario(this);
            WindowState = FormWindowState.Maximized;
            Tag = "frmgestionusuarios_titulo";

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

        // Formateamos roles y estado para la lectura; la entidad conserva sus valores originales para editar y guardar.
        private void ConfigurarGrilla()
        {
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
                if (!usuario.Activo) e.CellStyle.ForeColor = TemaVisual.ColorActual(Color.DimGray, true);
                if (dgvUsuarios.Columns[e.ColumnIndex].Name == "Activo")
                {
                    e.Value = IdiomaManager.ObtenerInstancia().Traducir(Name,
                        usuario.Activo ? "chk_activo" : "estado_inactivo", usuario.Activo ? "Activo" : "Inactivo");
                    e.CellStyle.BackColor = TemaVisual.ColorActual(usuario.Activo ? TemaVisual.VerdeSuave : TemaVisual.Fondo);
                    e.CellStyle.ForeColor = TemaVisual.ColorActual(usuario.Activo ? TemaVisual.Verde : TemaVisual.Secundario, true);
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

        private void CargarGrilla()
        {
            dgvUsuarios.DataSource = _usuarioBLL.ObtenerTodos();
            dgvUsuarios.ClearSelection();
            dgvUsuarios.CurrentCell = null;
        }

        // Pasamos la cuenta al editor y vaciamos la clave. También aplicamos el bloqueo visual del administrador original.
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

        // La UI reúne los valores, la BLL aplica las reglas y el DAL persiste. Este evento decide entre alta y edición.
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

        // Volvemos al modo alta y descartamos la selección y la clave escrita para no reutilizar datos de otra cuenta.
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
