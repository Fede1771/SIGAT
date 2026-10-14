namespace SIGAT.UI;

partial class DlgFamiliaPermisos
{
    private System.ComponentModel.IContainer components;
    private Label lblAyuda;
    private CheckedListBox lista;
    private Button btnGuardar, btnCancelar;
    protected override void Dispose(bool disposing)
    {
        if (disposing) components?.Dispose();
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        components = new System.ComponentModel.Container();
        SuspendLayout();
        lblAyuda = new Label();
        lista = new CheckedListBox();
        btnGuardar = new Button();
        btnCancelar = new Button();
        Name = "DlgFamiliaPermisos";
        Text = "Componentes de la familia";
        ClientSize = new Size(460, 360);
        lblAyuda.Name = "lblAyuda";
        lblAyuda.Text = "Marcá los permisos que integran esta familia.\nEl cambio afecta a todos los roles que la utilizan.";
        lblAyuda.Location = new Point(16, 16);
        lblAyuda.Size = new Size(428, 45);
        lista.Name = "lista";
        lista.Location = new Point(16, 65);
        lista.Size = new Size(428, 230);
        lista.CheckOnClick = true;
        lista.DisplayMember = "Nombre";
        lista.BorderStyle = BorderStyle.None;
        btnGuardar.Name = "btnGuardar";
        btnGuardar.Text = "Guardar";
        btnGuardar.Location = new Point(274, 315);
        btnGuardar.Size = new Size(88, 36);
        btnGuardar.DialogResult = DialogResult.OK;
        btnGuardar.BackColor = Color.FromArgb(35, 101, 175);
        btnGuardar.ForeColor = Color.White;
        btnGuardar.FlatStyle = FlatStyle.Flat;
        btnGuardar.FlatAppearance.BorderSize = 0;
        btnGuardar.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
        btnGuardar.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
        btnGuardar.UseVisualStyleBackColor = false;
        btnGuardar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnGuardar.Cursor = Cursors.Hand;
        btnCancelar.Name = "btnCancelar";
        btnCancelar.Text = "Cancelar";
        btnCancelar.Location = new Point(364, 315);
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
        Controls.AddRange(new Control[] { lblAyuda, lista, btnGuardar, btnCancelar });
        AcceptButton = btnGuardar;
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
