using SIGAT.BE;

namespace SIGAT.UI;

public partial class DlgFamiliaPermisos : Form
{
    public DlgFamiliaPermisos()
    {
        InitializeComponent();
        if (!EntornoDiseno.Activo) TemaVisual.Formulario(this);
    }

    // Solo cargamos datos: la lista y los botones ya existen en el diseñador.
    public void Cargar(PermisoCompuesto familia, IEnumerable<Permiso> catalogo)
    {
        Text = "Componentes de " + familia.Nombre;
        lista.Items.Clear();
        foreach (Permiso permiso in catalogo)
            if (!ReferenceEquals(permiso, familia))
                lista.Items.Add(permiso, familia.ObtenerHijos().Contains(permiso));
    }
    public IEnumerable<Permiso> Seleccionados => lista.CheckedItems.Cast<Permiso>();
}
