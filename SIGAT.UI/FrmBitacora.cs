using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmBitacora : Form
    {
        private BitacoraBLL _bll = new BitacoraBLL();

        public FrmBitacora()
        {
            InitializeComponent();
            Buscar();
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            Buscar();
        }

        private void Buscar()
        {
            // Creamos las variables de fecha vacías
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            // Solo llenamos las fechas si el usuario tildó el CheckBox
            if (chkFechas.Checked == true)
            {
                fechaDesde = dtpDesde.Value.Date;
                fechaHasta = dtpHasta.Value.Date.AddDays(1).AddTicks(-1); // Abarca hasta el final del día
            }

            dgv.DataSource = _bll.Buscar(fechaDesde, fechaHasta, txtUser.Text, txtActividad.Text);
        }
    }
}
