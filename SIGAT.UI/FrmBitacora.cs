using System;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public partial class FrmBitacora : Form
    {
        private BitacoraBLL _bll = new BitacoraBLL();
        private DataGridView dgv;
        private DateTimePicker dtpDesde, dtpHasta;
        private TextBox txtUser, txtActividad;
        private CheckBox chkFechas;
        private Button btnBuscar;

        public FrmBitacora()
        {
            ConfigurarUI();
            Buscar();
        }

        private void ConfigurarUI()
        {
            this.Text = "Consulta de Bitácora";
            this.FormBorderStyle = FormBorderStyle.None;
            this.WindowState = FormWindowState.Maximized;
            this.BackColor = Color.FromArgb(245, 247, 250);

            // Contenedor de filtros superior
            Panel panelFiltros = new Panel();
            panelFiltros.Top = 30;
            panelFiltros.Left = 40;
            panelFiltros.Height = 60;
            panelFiltros.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFiltros.BackColor = Color.Transparent;

            chkFechas = new CheckBox();
            chkFechas.Text = "Fechas:";
            chkFechas.Top = 18;
            chkFechas.Left = 10;
            chkFechas.Width = 70;
            chkFechas.Font = new Font("Segoe UI", 9);

            dtpDesde = new DateTimePicker();
            dtpDesde.Format = DateTimePickerFormat.Short;
            dtpDesde.Top = 16;
            dtpDesde.Left = 85;
            dtpDesde.Width = 110;
            dtpDesde.Font = new Font("Segoe UI", 9);

            dtpHasta = new DateTimePicker();
            dtpHasta.Format = DateTimePickerFormat.Short;
            dtpHasta.Top = 16;
            dtpHasta.Left = 205;
            dtpHasta.Width = 110;
            dtpHasta.Font = new Font("Segoe UI", 9);

            Label lblUser = new Label();
            lblUser.Text = "Usuario:";
            lblUser.Top = 19;
            lblUser.Left = 340;
            lblUser.AutoSize = true;
            lblUser.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtUser = new TextBox();
            txtUser.Top = 16;
            txtUser.Left = 400;
            txtUser.Width = 130;
            txtUser.Font = new Font("Segoe UI", 9);

            Label lblAct = new Label();
            lblAct.Text = "Actividad:";
            lblAct.Top = 19;
            lblAct.Left = 550;
            lblAct.AutoSize = true;
            lblAct.Font = new Font("Segoe UI", 9, FontStyle.Bold);

            txtActividad = new TextBox();
            txtActividad.Top = 16;
            txtActividad.Left = 620;
            txtActividad.Width = 130;
            txtActividad.Font = new Font("Segoe UI", 9);

            btnBuscar = new Button();
            btnBuscar.Text = "Buscar";
            btnBuscar.Top = 14;
            btnBuscar.Left = 770;
            btnBuscar.Width = 100;
            btnBuscar.Height = 30;
            btnBuscar.Font = new Font("Segoe UI", 9, FontStyle.Bold);
            btnBuscar.BackColor = Color.White;
            btnBuscar.Click += new EventHandler(BtnBuscar_Click);

            panelFiltros.Controls.Add(chkFechas);
            panelFiltros.Controls.Add(dtpDesde);
            panelFiltros.Controls.Add(dtpHasta);
            panelFiltros.Controls.Add(lblUser);
            panelFiltros.Controls.Add(txtUser);
            panelFiltros.Controls.Add(lblAct);
            panelFiltros.Controls.Add(txtActividad);
            panelFiltros.Controls.Add(btnBuscar);

            dgv = new DataGridView();
            dgv.Top = 110;
            dgv.Left = 40;
            dgv.Width = this.ClientSize.Width - 80;
            dgv.Height = this.ClientSize.Height - 150;
            dgv.ReadOnly = true;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            dgv.BackgroundColor = Color.White;

            this.Controls.Add(panelFiltros);
            this.Controls.Add(dgv);
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