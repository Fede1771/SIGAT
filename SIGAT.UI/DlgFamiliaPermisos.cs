using SIGAT.BE;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI;

public partial class DlgFamiliaPermisos : Form
{
    private readonly HashSet<object> seleccionOpciones = new();
    private static string T(string clave, string respaldo) => IdiomaManager.ObtenerInstancia().Traducir("FormGestionRoles", "roles_v2_" + clave, respaldo);
    private sealed record Opcion(object Componente, string Texto);
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
    // El diálogo devuelve la selección sin guardar en SQL; la pantalla de roles valida y persiste al aceptar.
    public IEnumerable<Permiso> Seleccionados => lista.CheckedItems.Cast<Permiso>();

    public IEnumerable<object> SeleccionadosObjetos => seleccionOpciones.ToArray();

    public void CargarResumen(string titulo, string resumen)
    {
        Text = titulo;
        Controls.Clear();
        ClientSize = new Size(700, 480);
        MinimumSize = new Size(480, 360);
        var raiz = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 1, RowCount = 2 };
        raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var texto = new TextBox { Name = "resumenCambios", ReadOnly = true, Multiline = true,
            ScrollBars = ScrollBars.Both, WordWrap = false, Dock = DockStyle.Fill, Text = resumen };
        var acciones = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, FlowDirection = FlowDirection.RightToLeft };
        btnGuardar.Text = T("guardar", "Guardar cambios");
        btnCancelar.Text = T("seguirEditando", "Seguir editando");
        btnGuardar.AutoSize = btnCancelar.AutoSize = true;
        btnGuardar.Anchor = btnCancelar.Anchor = AnchorStyles.None;
        acciones.Controls.Add(btnCancelar); acciones.Controls.Add(btnGuardar);
        raiz.Controls.Add(texto, 0, 0); raiz.Controls.Add(acciones, 0, 1); Controls.Add(raiz);
        Shown += (_, _) => TemaVisual.Aplicar(this);
    }

    // Selector reutilizado para familias y roles: no crea otra pantalla ni
    // modifica el catálogo hasta aceptar. La vista previa permite revisar la herencia.
    public void CargarOpciones(IEnumerable<object> opciones, string titulo)
    {
        Text = titulo;
        btnGuardar.Text = T("agregarSeleccionados", "Agregar seleccionados");
        btnCancelar.Text = T("cancelar", "Cancelar");
        ClientSize = new Size(720, 500);
        MinimumSize = new Size(520, 420);
        Controls.Clear();
        var raiz = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(16), ColumnCount = 2, RowCount = 4 };
        raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        lblAyuda.Text = T("selectorAyuda", "Marcá los componentes para la copia de trabajo. Se excluyen los ya asignados y los que formarían ciclos. Los datos se persistirán con Guardar cambios en la pantalla principal.");
        lblAyuda.AutoSize = true; lblAyuda.Dock = DockStyle.Fill;
        raiz.Controls.Add(lblAyuda, 0, 0); raiz.SetColumnSpan(lblAyuda, 2);
        var buscar = new TextBox { Name = "buscarComponentes", Dock = DockStyle.Top, PlaceholderText = T("buscarComponente", "Buscar componente...") };
        raiz.Controls.Add(buscar, 0, 1);
        var preview = new TreeView { Name = "vistaPrevia", Dock = DockStyle.Fill, BorderStyle = BorderStyle.None, ShowNodeToolTips = true };
        raiz.Controls.Add(preview, 1, 1); raiz.SetRowSpan(preview, 2);
        lista.Dock = DockStyle.Fill; lista.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        lista.IntegralHeight = false; lista.DisplayMember = "Texto";
        raiz.Controls.Add(lista, 0, 2);
        var botones = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, FlowDirection = FlowDirection.RightToLeft };
        btnGuardar.AutoSize = btnCancelar.AutoSize = true;
        btnGuardar.Anchor = btnCancelar.Anchor = AnchorStyles.None;
        botones.Controls.Add(btnCancelar); botones.Controls.Add(btnGuardar);
        raiz.Controls.Add(botones, 0, 3); raiz.SetColumnSpan(botones, 2);
        Controls.Add(raiz);
        var disponibles = opciones.Select(o => new Opcion(o, o switch
        {
            Rol r => T("tipoRol", "Rol") + " · " + r.Nombre,
            PermisoCompuesto f => T("familia", "Familia") + " · " + f.Nombre,
            Permiso p => T("permiso", "Permiso") + " · " + p.Nombre,
            _ => ""
        })).ToArray();
        seleccionOpciones.Clear();
        var marcados = seleccionOpciones;
        bool filtrando = false;
        void Filtrar()
        {
            filtrando = true; lista.Items.Clear();
            foreach (var o in disponibles.Where(o => o.Texto.Contains(buscar.Text, StringComparison.CurrentCultureIgnoreCase)))
                lista.Items.Add(o, marcados.Contains(o.Componente));
            filtrando = false;
        }
        TreeNode Nodo(object o)
        {
            var n = new TreeNode(o is Rol r ? r.Nombre : ((Permiso)o).Nombre);
            if (o is Rol rol)
            {
                foreach (var p in rol.Permisos) n.Nodes.Add(Nodo(p));
                foreach (var h in rol.Roles) n.Nodes.Add(Nodo(h));
            }
            else foreach (var p in ((Permiso)o).ObtenerHijos()) n.Nodes.Add(Nodo(p));
            return n;
        }
        buscar.TextChanged += (_, _) => Filtrar();
        lista.ItemCheck += (_, e) =>
        {
            if (filtrando) return;
            var o = (Opcion)lista.Items[e.Index];
            if (e.NewValue == CheckState.Checked) marcados.Add(o.Componente); else marcados.Remove(o.Componente);
        };
        lista.SelectedIndexChanged += (_, _) =>
        {
            preview.Nodes.Clear();
            if (lista.SelectedItem is Opcion o) { preview.Nodes.Add(Nodo(o.Componente)); preview.ExpandAll(); }
        };
        Filtrar();
        Shown += (_, _) => TemaVisual.Aplicar(this);
    }
}
