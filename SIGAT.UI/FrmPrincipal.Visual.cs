using SIGAT.SERVICIOS;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI;

public partial class FrmPrincipal
{
    private Panel panelContenido;
    private Form formularioActual;
    private Label lblCuenta, lblRoles, lblSeccion;
    private Button btnIdiomaVisual;
    private ContextMenuStrip selectorIdioma;
    private readonly Dictionary<string, BotonNavegacion> navegacion = new();
    private readonly List<(string Seccion, ToolStripMenuItem Menu, IconoVisual Icono)> accesos = new();

    private void CrearMarcoVisual(ToolStripMenuItem itemRoles)
    {
        SuspendLayout();
        var marco = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Margin = Padding.Empty };
        marco.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 236));
        marco.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        marco.RowStyles.Add(new RowStyle(SizeType.Absolute, 76));
        marco.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var marca = new Label { Text = "SIGAT", Dock = DockStyle.Fill, BackColor = Color.White, ForeColor = TemaVisual.Azul, Font = new Font("Segoe UI", 22F, FontStyle.Bold), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(26, 0, 0, 0), Margin = Padding.Empty };
        marco.Controls.Add(marca, 0, 0);

        var header = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, BackColor = Color.White, Padding = new Padding(24, 12, 24, 12), Margin = Padding.Empty };
        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        lblSeccion = new Label { Text = "Inicio", Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Font = new Font("Segoe UI", 12F, FontStyle.Bold), AutoEllipsis = true };
        lblCuenta = new Label { Dock = DockStyle.Fill, AutoSize = true, TextAlign = ContentAlignment.MiddleRight, ForeColor = TemaVisual.Secundario, Padding = new Padding(0, 0, 24, 0) };
        btnIdiomaVisual = TemaVisual.Boton("btnIdiomaVisual", "Idioma");
        btnIdiomaVisual.Tag = "menu_idioma";
        btnIdiomaVisual.Margin = Padding.Empty;
        selectorIdioma = new ContextMenuStrip(components)
        {
            Font = new Font("Segoe UI", 10F), BackColor = Color.White,
            ForeColor = TemaVisual.Texto, Padding = new Padding(4), ShowImageMargin = false, ShowCheckMargin = true
        };
        btnIdiomaVisual.Click += (_, _) => MostrarSelectorIdioma();
        header.Controls.Add(lblSeccion, 0, 0);
        header.Controls.Add(lblCuenta, 1, 0);
        header.Controls.Add(btnIdiomaVisual, 2, 0);
        marco.Controls.Add(header, 1, 0);

        var sidebar = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Color.White, ColumnCount = 1, RowCount = 3, Padding = new Padding(12, 20, 12, 16), Margin = Padding.Empty };
        sidebar.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        sidebar.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        sidebar.RowStyles.Add(new RowStyle(SizeType.Absolute, 58));
        var links = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = Padding.Empty };
        links.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var inicio = AgregarNavegacion(links, "Inicio", "Inicio", "menu_inicio", IconoVisual.Inicio);
        inicio.Click += (_, _) => MostrarInicio();
        foreach (var acceso in new[]
        {
            ("FrmGestionUsuarios", itemUsuarios, IconoVisual.Usuarios, "menu_usuarios"),
            ("FormGestionRoles", itemRoles, IconoVisual.Roles, "menu_roles"),
            ("FrmGestionIdiomas", itemGestionIdiomas, IconoVisual.Idiomas, "menu_gestion_idiomas"),
            ("FrmBitacora", itemBitacora, IconoVisual.Bitacora, "menu_bitacora")
        })
        {
            // Available conserva la condición de permisos aunque el menú no esté mostrado.
            if (!acceso.Item2.Available) continue;
            accesos.Add((acceso.Item1, acceso.Item2, acceso.Item3));
            var link = AgregarNavegacion(links, acceso.Item1, acceso.Item2.Text, acceso.Item4, acceso.Item3);
            link.Click += (_, _) => acceso.Item2.PerformClick();
        }
        sidebar.Controls.Add(links, 0, 0);
        lblRoles = new Label { AutoSize = true, MaximumSize = new Size(212, 96), AutoEllipsis = true, Dock = DockStyle.Fill, ForeColor = TemaVisual.Secundario, Padding = new Padding(12, 18, 12, 18), Margin = Padding.Empty };
        sidebar.Controls.Add(lblRoles, 0, 1);
        var salir = new BotonNavegacion(IconoVisual.Salir) { Name = "navSalir", Text = "Cerrar sesión", Tag = "menu_logout", Dock = DockStyle.Fill, Margin = Padding.Empty };
        salir.Click += (_, _) => itemLogout.PerformClick();
        sidebar.Controls.Add(salir, 0, 2);
        marco.Controls.Add(sidebar, 0, 1);

        panelContenido = new Panel { Name = "panelContenido", Dock = DockStyle.Fill, Margin = Padding.Empty, BackColor = TemaVisual.Fondo };
        marco.Controls.Add(panelContenido, 1, 1);
        Controls.Add(marco);
        MostrarInicio();
        ActualizarMarcoVisual();
        ResumeLayout(true);
    }

    private void MostrarSelectorIdioma()
    {
        foreach (ToolStripItem item in selectorIdioma.Items.Cast<ToolStripItem>().ToArray()) item.Dispose();
        selectorIdioma.Items.Clear();
        foreach (ToolStripItem item in itemIdioma.DropDownItems)
        {
            if (!item.Available) continue;
            if (item is ToolStripSeparator)
            {
                selectorIdioma.Items.Add(new ToolStripSeparator());
            }
            else if (item is ToolStripMenuItem opcion)
            {
                var entrada = new ToolStripMenuItem(opcion.Text)
                {
                    Checked = opcion.Checked, Enabled = opcion.Enabled,
                    Padding = new Padding(10, 6, 10, 6)
                };
                entrada.Click += (_, _) => opcion.PerformClick();
                selectorIdioma.Items.Add(entrada);
            }
        }
        selectorIdioma.MinimumSize = new Size(Math.Max(210, btnIdiomaVisual.Width), 0);
        // Un menú independiente evita que el MenuStrip oculto lo reposicione sobre el logo.
        selectorIdioma.Show(btnIdiomaVisual, new Point(btnIdiomaVisual.Width, btnIdiomaVisual.Height + 6), ToolStripDropDownDirection.BelowLeft);
    }

    private BotonNavegacion AgregarNavegacion(TableLayoutPanel panel, string section, string text, string key, IconoVisual icon)
    {
        var button = new BotonNavegacion(icon) { Name = "nav" + section, Text = text, Tag = key, Dock = DockStyle.Fill, Margin = new Padding(0, 0, 0, 6) };
        int row = panel.RowCount++;
        panel.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
        panel.Controls.Add(button, 0, row);
        navegacion.Add(section, button);
        return button;
    }

    private void MostrarInicio()
    {
        CerrarFormularioActual();
        foreach (Control control in panelContenido.Controls.Cast<Control>().ToArray()) control.Dispose();
        var home = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(32) };
        home.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        home.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        home.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        home.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        home.Controls.Add(TemaVisual.Encabezado("Tu espacio de trabajo", "Seleccioná una sección para comenzar.", "inicio_titulo", "inicio_descripcion"), 0, 0);
        var cards = new FlowLayoutPanel { Dock = DockStyle.Top, AutoSize = true, WrapContents = true, Margin = Padding.Empty };
        foreach (var acceso in accesos)
        {
            var button = new BotonNavegacion(acceso.Icono)
            {
                Name = "inicio" + acceso.Seccion, Text = acceso.Menu.Text, Dock = DockStyle.None,
                MinimumSize = new Size(250, 88), Size = new Size(250, 88),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(48, 0, 16, 0)
            };
            button.FlatAppearance.BorderSize = 1;
            button.FlatAppearance.BorderColor = TemaVisual.Borde;
            button.Tag = navegacion[acceso.Seccion].Tag;
            button.MinimumSize = new Size(250, 88);
            button.Margin = new Padding(0, 0, 16, 16);
            button.Click += (_, _) => acceso.Menu.PerformClick();
            cards.Controls.Add(button);
        }
        home.Controls.Add(cards, 0, 1);
        panelContenido.Controls.Add(home);
        SeleccionarSeccion("Inicio");
    }

    private void CerrarFormularioActual()
    {
        if (formularioActual == null) return;
        formularioActual.Close();
        formularioActual.Dispose();
        formularioActual = null;
    }

    private void SeleccionarSeccion(string section)
    {
        foreach (var entry in navegacion)
        {
            entry.Value.Seleccionado = entry.Key == section;
            entry.Value.Invalidate();
        }
        if (navegacion.TryGetValue(section, out var active)) lblSeccion.Text = active.Text;
    }

    private void ActualizarMarcoVisual()
    {
        if (lblCuenta == null) return;
        var user = SesionServicio.ObtenerInstancia().UsuarioActual;
        lblCuenta.Text = user?.NombreUsuario ?? "";
        lblRoles.Text = user == null || user.Roles.Count == 0
            ? IdiomaManager.ObtenerInstancia().Traducir(Name, "sin_roles", "Sin roles asignados")
            : string.Join(Environment.NewLine, user.Roles.Select(r => r.Nombre));
        string idioma = IdiomaManager.ObtenerInstancia().IdiomaActual?.NombreNativo;
        btnIdiomaVisual.Text = IdiomaManager.ObtenerInstancia().Traducir(Name, "menu_idioma", "Idioma")
            + (string.IsNullOrEmpty(idioma) ? "" : " · " + idioma);
        foreach (var entry in navegacion)
        {
            entry.Value.Text = IdiomaManager.ObtenerInstancia().Traducir(Name, entry.Value.Tag.ToString(), entry.Value.Text);
            if (entry.Value.Seleccionado) lblSeccion.Text = entry.Value.Text;
        }
    }
}
