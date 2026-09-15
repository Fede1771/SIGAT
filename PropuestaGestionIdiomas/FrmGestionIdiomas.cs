using SIGAT.BE.Idiomas;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmGestionIdiomas : Form, IIdiomaObserver
    {
        private IdiomaBLL _idiomaBLL = new IdiomaBLL();

        public FrmGestionIdiomas()
        {
            InitializeComponent();

            Tag = "frmgestionidiomas_titulo";
            lblIdiomaActivo.Tag = "lbl_idioma_activo";
            btnAplicarIdioma.Tag = "btn_aplicar_idioma";
            lblNuevoIdioma.Tag = "lbl_nuevo_idioma";
            lblCodigoIdioma.Tag = "lbl_codigo_idioma";
            lblNombreNativo.Tag = "lbl_nombre_nativo";
            chkIdiomaActivo.Tag = "chk_idioma_activo";
            btnNuevoIdioma.Tag = "btn_nuevo_idioma";
            btnGuardarTraduccion.Tag = "btn_guardar_traduccion";
            btnCambiarEstado.Tag = "btn_cambiar_estado";

            chkIdiomaActivo.Checked = true;
            Load += FrmGestionIdiomas_Load;
            FormClosed += FrmGestionIdiomas_FormClosed;
        }

        private void FrmGestionIdiomas_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            CargarComboIdiomas();
            ActualizarIdioma();
            CargarTraducciones();
        }

        private void FrmGestionIdiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void CargarComboIdiomas()
        {
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DataSource = _idiomaBLL.ObtenerTodosLosIdiomas();
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTraducciones();
        }

        private void btnNuevoIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                int idIdiomaNuevo = _idiomaBLL.CrearIdioma(
                    txtNuevoIdioma.Text,
                    txtCodigoIdioma.Text,
                    txtNombreNativo.Text,
                    chkIdiomaActivo.Checked);

                MessageBox.Show("Idioma creado correctamente. Las traducciones quedaron pendientes.");

                txtNuevoIdioma.Clear();
                txtCodigoIdioma.Clear();
                txtNombreNativo.Clear();
                chkIdiomaActivo.Checked = true;

                CargarComboIdiomas();
                cmbIdiomas.SelectedValue = idIdiomaNuevo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAplicarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedValue == null) return;

                _idiomaBLL.CambiarIdioma((int)cmbIdiomas.SelectedValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedItem == null) return;

                Idioma idioma = (Idioma)cmbIdiomas.SelectedItem;
                bool nuevoEstado = !idioma.Activo;

                string accion = nuevoEstado ? "activar" : "desactivar";
                DialogResult respuesta = MessageBox.Show(
                    "¿Desea " + accion + " el idioma " + idioma.Nombre + "?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                _idiomaBLL.CambiarEstadoIdioma(idioma.Id, nuevoEstado);
                CargarComboIdiomas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarTraducciones()
        {
            if (cmbIdiomas.SelectedValue == null) return;
            if (!(cmbIdiomas.SelectedValue is int)) return;

            int idIdioma = (int)cmbIdiomas.SelectedValue;
            dgvTraducciones.DataSource = _idiomaBLL.ObtenerTraduccionesParaGestion(idIdioma);

            if (dgvTraducciones.Columns.Count == 0) return;

            dgvTraducciones.Columns["IdIdioma"].Visible = false;
            dgvTraducciones.Columns["IdControl"].Visible = false;
            dgvTraducciones.Columns["DigitoVerificador"].Visible = false;
            dgvTraducciones.Columns["FormNombre"].HeaderText = "Formulario";
            dgvTraducciones.Columns["ControlNombre"].HeaderText = "Clave";
            dgvTraducciones.Columns["TextoBase"].HeaderText = "Español";
            dgvTraducciones.Columns["Texto"].HeaderText = "Traducción";
            dgvTraducciones.Columns["Estado"].HeaderText = "Estado";

            dgvTraducciones.Columns["FormNombre"].ReadOnly = true;
            dgvTraducciones.Columns["ControlNombre"].ReadOnly = true;
            dgvTraducciones.Columns["TextoBase"].ReadOnly = true;
            dgvTraducciones.Columns["Estado"].ReadOnly = true;
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTraducciones.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una traducción.");
                    return;
                }

                dgvTraducciones.EndEdit();

                Traduccion traduccion = (Traduccion)dgvTraducciones.CurrentRow.DataBoundItem;

                _idiomaBLL.GuardarTraduccionPorControl(
                    traduccion.IdIdioma,
                    traduccion.IdControl,
                    traduccion.Texto);

                CargarTraducciones();
                MessageBox.Show("Traducción guardada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
        }
    }
}
