namespace SIGAT.UI;

partial class DlgNombreRol
{
    private System.ComponentModel.IContainer components;
    private Label lblNombre;
    private TextBox txtNombre;
    private Button btnCrear, btnCancelar;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        lblNombre = new Label();
        txtNombre = new TextBox();
        btnCrear = new Button();
        btnCancelar = new Button();
        Name = "DlgNombreRol";
        Text = "Crear rol";
        ClientSize = new Size(400, 135);
        lblNombre.Name = "lblNombre";
        lblNombre.Text = "Nombre del rol";
        lblNombre.Location = new Point(16, 16);
        lblNombre.AutoSize = true;
        txtNombre.Name = "txtNombre";
        txtNombre.Location = new Point(16, 40);
        txtNombre.Size = new Size(368, 26);
        txtNombre.MaxLength = 150;
        btnCrear.Name = "btnCrear";
        btnCrear.Text = "Crear";
        btnCrear.Location = new Point(204, 88);
        btnCrear.Size = new Size(88, 36);
        btnCrear.DialogResult = DialogResult.OK;
        btnCrear.BackColor = Color.FromArgb(35, 101, 175);
        btnCrear.ForeColor = Color.White;
        btnCrear.FlatStyle = FlatStyle.Flat;
        btnCrear.FlatAppearance.BorderSize = 0;
        btnCrear.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
        btnCrear.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
        btnCrear.UseVisualStyleBackColor = false;
        btnCrear.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCrear.Cursor = Cursors.Hand;
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Text = "Cancelar";
        btnCancelar.Location = new Point(294, 88);
        btnCancelar.Size = new Size(88, 36);
        btnCancelar.DialogResult = DialogResult.Cancel;
        btnCancelar.BackColor = Color.White;
        btnCancelar.ForeColor = Color.FromArgb(30, 45, 65);
        btnCancelar.FlatStyle = FlatStyle.Flat;
        btnCancelar.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
        btnCancelar.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
        btnCancelar.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
        btnCancelar.UseVisualStyleBackColor = false;
        btnCancelar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnCancelar.Cursor = Cursors.Hand;
        Controls.AddRange(new Control[] { lblNombre, txtNombre, btnCrear, btnCancelar });
        AcceptButton = btnCrear;
        CancelButton = btnCancelar;
        Font = new Font("Segoe UI", 10F);
        BackColor = Color.FromArgb(245, 247, 251);
        ForeColor = Color.FromArgb(30, 45, 65);
        AutoScaleMode = AutoScaleMode.Font;
        AutoScaleDimensions = new SizeF(7F, 17F);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MinimizeBox = false;
        MaximizeBox = false;
        ResumeLayout(false);
        PerformLayout();
    }
}
