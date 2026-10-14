namespace SIGAT.UI;

public partial class DlgNombreRol : Form
{
    public DlgNombreRol()
    {
        InitializeComponent();
        if (!EntornoDiseno.Activo) TemaVisual.Formulario(this);
    }
    public string NombreRol => txtNombre.Text;
}
