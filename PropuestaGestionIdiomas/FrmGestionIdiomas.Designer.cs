namespace SIGAT.UI
{
    partial class FrmGestionIdiomas
    {
        private System.ComponentModel.IContainer components = null;
        private Label lblIdiomaActivo;
        private ComboBox cmbIdiomas;
        private Button btnAplicarIdioma;
        private Button btnCambiarEstado;
        private Label lblNuevoIdioma;
        private TextBox txtNuevoIdioma;
        private Label lblCodigoIdioma;
        private TextBox txtCodigoIdioma;
        private Label lblNombreNativo;
        private TextBox txtNombreNativo;
        private CheckBox chkIdiomaActivo;
        private Button btnNuevoIdioma;
        private DataGridView dgvTraducciones;
        private Button btnGuardarTraduccion;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            lblIdiomaActivo = new Label();
            cmbIdiomas = new ComboBox();
            btnAplicarIdioma = new Button();
            btnCambiarEstado = new Button();
            lblNuevoIdioma = new Label();
            txtNuevoIdioma = new TextBox();
            lblCodigoIdioma = new Label();
            txtCodigoIdioma = new TextBox();
            lblNombreNativo = new Label();
            txtNombreNativo = new TextBox();
            chkIdiomaActivo = new CheckBox();
            btnNuevoIdioma = new Button();
            dgvTraducciones = new DataGridView();
            btnGuardarTraduccion = new Button();
            ((System.ComponentModel.ISupportInitialize)dgvTraducciones).BeginInit();
            SuspendLayout();

            lblIdiomaActivo.AutoSize = true;
            lblIdiomaActivo.Location = new Point(25, 25);
            lblIdiomaActivo.Text = "Idioma:";

            cmbIdiomas.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbIdiomas.Location = new Point(110, 22);
            cmbIdiomas.Size = new Size(220, 28);
            cmbIdiomas.SelectedIndexChanged += cmbIdiomas_SelectedIndexChanged;

            btnAplicarIdioma.Location = new Point(345, 20);
            btnAplicarIdioma.Size = new Size(115, 32);
            btnAplicarIdioma.Text = "Aplicar";
            btnAplicarIdioma.Click += btnAplicarIdioma_Click;

            btnCambiarEstado.Location = new Point(475, 20);
            btnCambiarEstado.Size = new Size(140, 32);
            btnCambiarEstado.Text = "Activar / desactivar";
            btnCambiarEstado.Click += btnCambiarEstado_Click;

            lblNuevoIdioma.AutoSize = true;
            lblNuevoIdioma.Location = new Point(25, 80);
            lblNuevoIdioma.Text = "Nombre:";

            txtNuevoIdioma.Location = new Point(110, 77);
            txtNuevoIdioma.Size = new Size(180, 27);

            lblCodigoIdioma.AutoSize = true;
            lblCodigoIdioma.Location = new Point(310, 80);
            lblCodigoIdioma.Text = "Código:";

            txtCodigoIdioma.Location = new Point(370, 77);
            txtCodigoIdioma.Size = new Size(70, 27);

            lblNombreNativo.AutoSize = true;
            lblNombreNativo.Location = new Point(25, 120);
            lblNombreNativo.Text = "Nombre nativo:";

            txtNombreNativo.Location = new Point(110, 117);
            txtNombreNativo.Size = new Size(180, 27);

            chkIdiomaActivo.AutoSize = true;
            chkIdiomaActivo.Location = new Point(310, 120);
            chkIdiomaActivo.Text = "Activo";

            btnNuevoIdioma.Location = new Point(475, 105);
            btnNuevoIdioma.Size = new Size(140, 35);
            btnNuevoIdioma.Text = "Crear idioma";
            btnNuevoIdioma.Click += btnNuevoIdioma_Click;

            dgvTraducciones.AllowUserToAddRows = false;
            dgvTraducciones.AllowUserToDeleteRows = false;
            dgvTraducciones.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgvTraducciones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dgvTraducciones.Location = new Point(25, 175);
            dgvTraducciones.MultiSelect = false;
            dgvTraducciones.Name = "dgvTraducciones";
            dgvTraducciones.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgvTraducciones.Size = new Size(760, 310);

            btnGuardarTraduccion.Location = new Point(635, 505);
            btnGuardarTraduccion.Size = new Size(150, 35);
            btnGuardarTraduccion.Text = "Guardar traducción";
            btnGuardarTraduccion.Click += btnGuardarTraduccion_Click;

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(815, 565);
            Controls.Add(lblIdiomaActivo);
            Controls.Add(cmbIdiomas);
            Controls.Add(btnAplicarIdioma);
            Controls.Add(btnCambiarEstado);
            Controls.Add(lblNuevoIdioma);
            Controls.Add(txtNuevoIdioma);
            Controls.Add(lblCodigoIdioma);
            Controls.Add(txtCodigoIdioma);
            Controls.Add(lblNombreNativo);
            Controls.Add(txtNombreNativo);
            Controls.Add(chkIdiomaActivo);
            Controls.Add(btnNuevoIdioma);
            Controls.Add(dgvTraducciones);
            Controls.Add(btnGuardarTraduccion);
            Name = "FrmGestionIdiomas";
            Text = "Gestión de idiomas";
            ((System.ComponentModel.ISupportInitialize)dgvTraducciones).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }
    }
}
