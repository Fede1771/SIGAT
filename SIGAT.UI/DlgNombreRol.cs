namespace SIGAT.UI;

public partial class DlgNombreRol : Form
{
    public DlgNombreRol()
    {
        InitializeComponent();
        if (!EntornoDiseno.Activo) TemaVisual.Formulario(this);
    }
    // Devolvemos el nombre ingresado; la pantalla que abre el diálogo comprueba que no esté vacío ni repetido.
    public string NombreRol => txtNombre.Text;
}
