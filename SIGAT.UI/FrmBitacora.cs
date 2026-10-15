using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmBitacora : Form, IIdiomaObserver
    {
        private BitacoraBLL _bll = new BitacoraBLL();

        public FrmBitacora()
        {
            InitializeComponent();
            // El enlace usa las columnas del diseñador; nunca expone propiedades internas.
            dgv.AutoGenerateColumns = false;
            if (EntornoDiseno.Activo) return;
            TemaVisual.Formulario(this);


            // Después de cargar datos actualizamos los títulos con el idioma de la sesión.
            dgv.DataBindingComplete += Dgv_DataBindingComplete;
            this.Load += FrmBitacora_Load;
            this.FormClosed += FrmBitacora_FormClosed;
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            Buscar();
            ActualizarIdioma();
        }

        private void FrmBitacora_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (chkFechas.Checked == true)
            {
                fechaDesde = dtpDesde.Value.Date;
                fechaHasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1);
            }

            dgv.DataSource = _bll.Buscar(fechaDesde, fechaHasta, txtUser.Text, txtActividad.Text);
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Las columnas están en Designer; esta tabla ya no está protegida por DVH/DVV.
            TraducirColumnasVisibles();
        }

        private void TraducirColumnasVisibles()
        {
            foreach (DataGridViewColumn columna in dgv.Columns)
            {
                if (columna.Tag != null)
                {
                    columna.HeaderText = IdiomaManager.ObtenerInstancia()
                        .Traducir(this.Name, columna.Tag.ToString(), columna.HeaderText);
                }
            }
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
            TraducirColumnasVisibles();
        }
    }
}
