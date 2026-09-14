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

            this.Tag = "frmbitacora_titulo";
            chkFechas.Tag = "chk_fechas";
            lblUser.Tag = "lbl_usuario";
            lblAct.Tag = "lbl_actividad";
            btnBuscar.Tag = "btn_buscar";

            // La grilla arma sus columnas solas al recibir datos, hay que esperar a que termine para poder taguearlas
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
            foreach (DataGridViewColumn columna in dgv.Columns)
            {
                if (columna.Name == "IdBitacora")
                {
                    columna.Tag = "col_id";
                }
                else if (columna.Name == "Fecha")
                {
                    columna.Tag = "col_fecha";
                }
                else if (columna.Name == "Usuario")
                {
                    columna.Tag = "col_usuario";
                }
                else if (columna.Name == "Actividad")
                {
                    columna.Tag = "col_actividad";
                }
                else if (columna.Name == "InformacionAsociada")
                {
                    columna.Tag = "col_informacion";
                }
                else
                {
                    columna.Visible = false;
                }
            }

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