namespace SIGAT.UI
{
    partial class FrmGestionIdiomas
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.lblIdiomaActivo = new Label();
            this.cmbIdiomas = new ComboBox();
            this.btnAplicarIdioma = new Button();
            this.lblNuevoIdioma = new Label();
            this.txtNuevoIdioma = new TextBox();
            this.btnNuevoIdioma = new Button();
            this.lblNombreForm = new Label();
            this.txtNombreForm = new TextBox();
            this.lblNombreControl = new Label();
            this.txtNombreControl = new TextBox();
            this.lblTexto = new Label();
            this.txtTexto = new TextBox();
            this.btnGuardarTraduccion = new Button();
            this.SuspendLayout();
            //
            // lblIdiomaActivo
            //
            this.lblIdiomaActivo.AutoSize = true;
            this.lblIdiomaActivo.Location = new Point(30, 30);
            this.lblIdiomaActivo.Name = "lblIdiomaActivo";
            this.lblIdiomaActivo.Size = new Size(90, 20);
            this.lblIdiomaActivo.Text = "Idioma activo:";
            //
            // cmbIdiomas
            //
            this.cmbIdiomas.DropDownStyle = ComboBoxStyle.DropDownList;
            this.cmbIdiomas.Location = new Point(140, 27);
            this.cmbIdiomas.Name = "cmbIdiomas";
            this.cmbIdiomas.Size = new Size(220, 28);
            //
            // btnAplicarIdioma
            //
            this.btnAplicarIdioma.Location = new Point(375, 25);
            this.btnAplicarIdioma.Name = "btnAplicarIdioma";
            this.btnAplicarIdioma.Size = new Size(150, 32);
            this.btnAplicarIdioma.Text = "Aplicar a todo SIGAT";
            this.btnAplicarIdioma.UseVisualStyleBackColor = true;
            this.btnAplicarIdioma.Click += new EventHandler(this.btnAplicarIdioma_Click);
            //
            // lblNuevoIdioma
            //
            this.lblNuevoIdioma.AutoSize = true;
            this.lblNuevoIdioma.Location = new Point(30, 90);
            this.lblNuevoIdioma.Name = "lblNuevoIdioma";
            this.lblNuevoIdioma.Size = new Size(110, 20);
            this.lblNuevoIdioma.Text = "Nuevo idioma:";
            //
            // txtNuevoIdioma
            //
            this.txtNuevoIdioma.Location = new Point(140, 87);
            this.txtNuevoIdioma.Name = "txtNuevoIdioma";
            this.txtNuevoIdioma.Size = new Size(220, 27);
            //
            // btnNuevoIdioma
            //
            this.btnNuevoIdioma.Location = new Point(375, 85);
            this.btnNuevoIdioma.Name = "btnNuevoIdioma";
            this.btnNuevoIdioma.Size = new Size(150, 32);
            this.btnNuevoIdioma.Text = "Crear (clona Español)";
            this.btnNuevoIdioma.UseVisualStyleBackColor = true;
            this.btnNuevoIdioma.Click += new EventHandler(this.btnNuevoIdioma_Click);
            //
            // lblNombreForm
            //
            this.lblNombreForm.AutoSize = true;
            this.lblNombreForm.Location = new Point(30, 160);
            this.lblNombreForm.Name = "lblNombreForm";
            this.lblNombreForm.Size = new Size(110, 20);
            this.lblNombreForm.Text = "Formulario (Form):";
            //
            // txtNombreForm
            //
            this.txtNombreForm.Location = new Point(30, 183);
            this.txtNombreForm.Name = "txtNombreForm";
            this.txtNombreForm.Size = new Size(220, 27);
            //
            // lblNombreControl
            //
            this.lblNombreControl.AutoSize = true;
            this.lblNombreControl.Location = new Point(270, 160);
            this.lblNombreControl.Name = "lblNombreControl";
            this.lblNombreControl.Size = new Size(130, 20);
            this.lblNombreControl.Text = "Control (Tag lógico):";
            //
            // txtNombreControl
            //
            this.txtNombreControl.Location = new Point(270, 183);
            this.txtNombreControl.Name = "txtNombreControl";
            this.txtNombreControl.Size = new Size(220, 27);
            //
            // lblTexto
            //
            this.lblTexto.AutoSize = true;
            this.lblTexto.Location = new Point(30, 220);
            this.lblTexto.Name = "lblTexto";
            this.lblTexto.Size = new Size(120, 20);
            this.lblTexto.Text = "Texto traducido:";
            //
            // txtTexto
            //
            this.txtTexto.Location = new Point(30, 243);
            this.txtTexto.Name = "txtTexto";
            this.txtTexto.Size = new Size(460, 27);
            //
            // btnGuardarTraduccion
            //
            this.btnGuardarTraduccion.Location = new Point(510, 241);
            this.btnGuardarTraduccion.Name = "btnGuardarTraduccion";
            this.btnGuardarTraduccion.Size = new Size(150, 32);
            this.btnGuardarTraduccion.Text = "Guardar traducción";
            this.btnGuardarTraduccion.UseVisualStyleBackColor = true;
            this.btnGuardarTraduccion.Click += new EventHandler(this.btnGuardarTraduccion_Click);
            //
            // FrmGestionIdiomas
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(820, 310);
            this.Controls.Add(this.lblIdiomaActivo);
            this.Controls.Add(this.cmbIdiomas);
            this.Controls.Add(this.btnAplicarIdioma);
            this.Controls.Add(this.lblNuevoIdioma);
            this.Controls.Add(this.txtNuevoIdioma);
            this.Controls.Add(this.btnNuevoIdioma);
            this.Controls.Add(this.lblNombreForm);
            this.Controls.Add(this.txtNombreForm);
            this.Controls.Add(this.lblNombreControl);
            this.Controls.Add(this.txtNombreControl);
            this.Controls.Add(this.lblTexto);
            this.Controls.Add(this.txtTexto);
            this.Controls.Add(this.btnGuardarTraduccion);
            this.Name = "FrmGestionIdiomas";
            this.Text = "FrmGestionIdiomas";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private Label lblIdiomaActivo;
        private ComboBox cmbIdiomas;
        private Button btnAplicarIdioma;
        private Label lblNuevoIdioma;
        private TextBox txtNuevoIdioma;
        private Button btnNuevoIdioma;
        private Label lblNombreForm;
        private TextBox txtNombreForm;
        private Label lblNombreControl;
        private TextBox txtNombreControl;
        private Label lblTexto;
        private TextBox txtTexto;
        private Button btnGuardarTraduccion;
    }
}