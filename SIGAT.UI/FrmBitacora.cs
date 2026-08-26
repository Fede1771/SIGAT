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
            var panelFiltros = new Panel
            {
                Top = 30,
                Left = 40,
                Height = 60,
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                BackColor = Color.Transparent
            };

            chkFechas = new CheckBox { Text = "Fechas:", Top = 18, Left = 10, Width = 70, Font = new Font("Segoe UI", 9) };
            dtpDesde = new DateTimePicker { Format = DateTimePickerFormat.Short, Top = 16, Left = 85, Width = 110, Font = new Font("Segoe UI", 9) };
            dtpHasta = new DateTimePicker { Format = DateTimePickerFormat.Short, Top = 16, Left = 205, Width = 110, Font = new Font("Segoe UI", 9) };

            var lblUser = new Label { Text = "Usuario:", Top = 19, Left = 340, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtUser = new TextBox { Top = 16, Left = 400, Width = 130, Font = new Font("Segoe UI", 9) };

            var lblAct = new Label { Text = "Actividad:", Top = 19, Left = 550, AutoSize = true, Font = new Font("Segoe UI", 9, FontStyle.Bold) };
            txtActividad = new TextBox { Top = 16, Left = 620, Width = 130, Font = new Font("Segoe UI", 9) };

            btnBuscar = new Button { Text = "Buscar", Top = 14, Left = 770, Width = 100, Height = 30, Font = new Font("Segoe UI", 9, FontStyle.Bold), BackColor = Color.White };
            btnBuscar.Click += (s, e) => Buscar();

            panelFiltros.Controls.AddRange(new Control[] { chkFechas, dtpDesde, dtpHasta, lblUser, txtUser, lblAct, txtActividad, btnBuscar });

            // Grilla que se adapta de punta a punta usando el ancho real de la ventana (ClientSize)
            dgv = new DataGridView
            {
                Top = 110,
                Left = 40,
                Width = this.ClientSize.Width - 80,
                Height = this.ClientSize.Height - 150,
                ReadOnly = true,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                BackgroundColor = Color.White
            };

            this.Controls.AddRange(new Control[] { panelFiltros, dgv });
        }

        private void Buscar()
        {
            DateTime? desde = chkFechas.Checked ? dtpDesde.Value.Date : (DateTime?)null;
            DateTime? hasta = chkFechas.Checked ? dtpHasta.Value.Date.AddDays(1).AddTicks(-1) : (DateTime?)null;

            dgv.DataSource = _bll.Buscar(desde, hasta, txtUser.Text, txtActividad.Text);
        }
    }
}