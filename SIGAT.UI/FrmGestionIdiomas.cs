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

            this.Tag = "frmgestionidiomas_titulo";
            lblIdiomaActivo.Tag = "lbl_idioma_activo";
            btnAplicarIdioma.Tag = "btn_aplicar_idioma";
            lblNuevoIdioma.Tag = "lbl_nuevo_idioma";
            btnNuevoIdioma.Tag = "btn_nuevo_idioma";
            lblNombreForm.Tag = "lbl_nombre_form";
            lblNombreControl.Tag = "lbl_nombre_control";
            lblTexto.Tag = "lbl_texto";
            btnGuardarTraduccion.Tag = "btn_guardar_traduccion";

            this.Load += FrmGestionIdiomas_Load;
            this.FormClosed += FrmGestionIdiomas_FormClosed;
        }

        private void FrmGestionIdiomas_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            CargarComboIdiomas();
            ActualizarIdioma();
        }

        private void FrmGestionIdiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void CargarComboIdiomas()
        {
            cmbIdiomas.DataSource = _idiomaBLL.ObtenerIdiomas();
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
        }

        private void btnNuevoIdioma_Click(object sender, EventArgs e)
        {
            string nombre = txtNuevoIdioma.Text.Trim();
            if (string.IsNullOrEmpty(nombre))
            {
                MessageBox.Show("Ingrese el nombre del nuevo idioma.");
                return;
            }

            _idiomaBLL.CrearIdioma(nombre);
            txtNuevoIdioma.Clear();
            CargarComboIdiomas();
            MessageBox.Show("Idioma creado. Se copiaron las leyendas de Español como base.");
        }

        private void btnAplicarIdioma_Click(object sender, EventArgs e)
        {
            if (cmbIdiomas.SelectedValue == null)
            {
                return;
            }
            int idIdioma = (int)cmbIdiomas.SelectedValue;
            _idiomaBLL.CambiarIdioma(idIdioma);
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            if (cmbIdiomas.SelectedValue == null)
            {
                MessageBox.Show("Seleccione un idioma.");
                return;
            }

            int idIdioma = (int)cmbIdiomas.SelectedValue;

            _idiomaBLL.GuardarTraduccion(
                idIdioma,
                txtNombreControl.Text.Trim(),
                txtNombreForm.Text.Trim(),
                txtTexto.Text);

            MessageBox.Show("Traducción guardada correctamente.");
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
        }
    }
}