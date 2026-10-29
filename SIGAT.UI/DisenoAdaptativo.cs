namespace SIGAT.UI;

// La estructura sigue en el diseñador. Aquí solo se reordena cuando cambia el
// espacio disponible, el DPI o el texto traducido; no se recrean los controles.
internal static class DisenoAdaptativo
{
    private sealed class VisorAdaptativo : Panel
    {
        // Un relayout de los árboles no debe devolver el scroll al control que
        // tenía foco. El acceso por teclado se atiende explícitamente en Enter.
        protected override Point ScrollToControl(Control activeControl) => DisplayRectangle.Location;
    }

    public static void Configurar(Form form)
    {
        form.Shown += (_, _) =>
        {
            var area = Screen.FromControl(form).WorkingArea;
            form.MinimumSize = new Size(Math.Min(form.MinimumSize.Width, area.Width), Math.Min(form.MinimumSize.Height, area.Height));
            if (form.TopLevel && form.WindowState == FormWindowState.Normal && (form.Width > area.Width || form.Height > area.Height))
            {
                form.Size = new Size(Math.Min(form.Width, area.Width), Math.Min(form.Height, area.Height));
                form.Location = new Point(area.Left, area.Top);
            }
        };
        if (form is FormGestionRoles) return;
        if (form is FrmPrincipal) { ConfigurarPrincipal(form); return; }
        if (form is DlgFamiliaPermisos || form is DlgNombreRol)
        {
            ConfigurarDialogo(form);
            return;
        }
        var raiz = form.Controls.OfType<TableLayoutPanel>().FirstOrDefault();
        if (raiz == null) return;
        var visor = new VisorAdaptativo { Name = "visorAdaptativo", Dock = DockStyle.Fill, AutoScroll = true,
            BackColor = raiz.BackColor, Margin = Padding.Empty };
        form.Controls.Remove(raiz);
        visor.Controls.Add(raiz);
        form.Controls.Add(visor);
        raiz.Dock = DockStyle.None;
        raiz.Anchor = AnchorStyles.Top | AnchorStyles.Left;
        bool ajustando = false;
        void Ajustar()
        {
            if (ajustando || form.IsDisposed || visor.ClientSize.Width <= 0) return;
            ajustando = true;
            raiz.SuspendLayout();
            try
            {
                int Dp(int n) => (int)Math.Ceiling(n * form.DeviceDpi / 96f);
                int ancho = visor.ClientSize.Width;
                raiz.Width = Math.Max(1, ancho - 1);
                EnvolverTextos(raiz);
                int alto = Dp(form is FrmLogin ? 550 : 620);
                if (form is FrmGestionUsuarios || form is FrmGestionIdiomas)
                    alto = EditorLateral(raiz, ancho, Dp, form is FrmGestionIdiomas);
                if (form is FrmLogin)
                {
                    // Las filas ya están escaladas por WinForms. El margen del
                    // formulario queda fuera del visor y no forma parte del contenido.
                    alto = raiz.Padding.Vertical + (int)Math.Ceiling(raiz.RowStyles.Cast<RowStyle>()
                        .Where(fila => fila.SizeType == SizeType.Absolute).Sum(fila => fila.Height));
                }
                if (form is FrmBitacora || form is FrmIntegridad)
                {
                    int fijos = raiz.Controls.Cast<Control>().Where(c =>
                        raiz.RowStyles[raiz.GetRow(c)].SizeType == SizeType.AutoSize)
                        .Sum(c => c.GetPreferredSize(new Size(Math.Max(1, ancho - raiz.Padding.Horizontal - c.Margin.Horizontal), 0)).Height + c.Margin.Vertical);
                    alto = Math.Max(alto, fijos + raiz.Padding.Vertical + Dp(300));
                }
                // Un formulario alojado también puede ser menor que su MinimumSize.
                // El visor permite llegar al contenido sin reducir las grillas a cero.
                int altoContenido = Math.Max(visor.ClientSize.Height, alto);
                visor.AutoScrollMinSize = new Size(0, altoContenido);
                raiz.Bounds = new Rectangle(visor.AutoScrollPosition, new Size(Math.Max(1, visor.ClientSize.Width - 1), altoContenido));
            }
            finally
            {
                raiz.ResumeLayout(true);
                // Medir con las celdas ya distribuidas, no con los anchos de
                // la disposición anterior (por ejemplo, al volver de una columna).
                EnvolverTextos(raiz);
                ajustando = false;
            }
        }
        bool pendiente = false;
        void ProgramarAjuste()
        {
            if (pendiente || !form.IsHandleCreated || form.IsDisposed) return;
            pendiente = true;
            // Esperar a que WinForms termine de calcular sus barras nativas.
            // Cambiar el tamaño durante ese cálculo deja un rango de scroll viejo.
            form.BeginInvoke((Action)(() =>
            {
                pendiente = false;
                if (!form.IsDisposed) { Ajustar(); visor.PerformLayout(); }
            }));
        }
        visor.ClientSizeChanged += (_, _) => ProgramarAjuste();
        form.Shown += (_, _) => Ajustar();
        form.DpiChanged += (_, _) => ProgramarAjuste();
        form.FontChanged += (_, _) => ProgramarAjuste();
        foreach (Control control in Descendientes(raiz))
        {
            if (control is Label || control is ButtonBase)
                control.TextChanged += (_, _) => Ajustar();
            if (!control.TabStop || control is ScrollableControl) continue;
            control.Enter += (_, _) =>
            {
                var rect = visor.RectangleToClient(control.RectangleToScreen(control.ClientRectangle));
                int delta = rect.Top < 0 || rect.Height > visor.ClientSize.Height ? rect.Top : rect.Bottom > visor.ClientSize.Height ? rect.Bottom - visor.ClientSize.Height : 0;
                if (delta != 0) visor.AutoScrollPosition = new Point(0, Math.Max(0, -visor.AutoScrollPosition.Y + delta));
            };
        }
        Ajustar();
    }

    private static T Buscar<T>(Control raiz, string nombre) where T : Control =>
        (T)raiz.Controls.Find(nombre, true)[0];

    private static IEnumerable<Control> Descendientes(Control padre)
    {
        foreach (Control hijo in padre.Controls)
        {
            yield return hijo;
            foreach (var nieto in Descendientes(hijo)) yield return nieto;
        }
    }

    private static void EnvolverTextos(Control raiz)
    {
        foreach (var etiqueta in Descendientes(raiz).OfType<Label>().Where(l => l.AutoSize))
        {
            int ancho = Math.Max(1, etiqueta.Parent.ClientSize.Width - etiqueta.Parent.Padding.Horizontal - etiqueta.Margin.Horizontal);
            etiqueta.MaximumSize = new Size(ancho, 0);
        }
    }

    private static void Ubicar(TableLayoutPanel tabla, Control control, int columna, int fila, int filas = 1, int columnas = 1)
    {
        tabla.SetRowSpan(control, filas);
        tabla.SetColumnSpan(control, columnas);
        tabla.SetCellPosition(control, new TableLayoutPanelCellPosition(columna, fila));
    }

    private static int EditorLateral(TableLayoutPanel raiz, int ancho, Func<int, int> dp, bool idiomas)
    {
        var editor = Buscar<Panel>(raiz, "panel7");
        var datos = idiomas ? (Control)Buscar<TableLayoutPanel>(raiz, "tablelayoutpanel11") : Buscar<DataGridView>(raiz, "dgvUsuarios");
        var encabezado = Buscar<TableLayoutPanel>(raiz, "tablelayoutpanel2");
        editor.AutoScroll = false;
        int fila = idiomas ? 2 : 1;
        bool apilado = ancho < dp(idiomas ? 900 : 1000);
        raiz.ColumnCount = apilado ? 1 : 2;
        raiz.ColumnStyles.Clear();
        raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        if (!apilado) raiz.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, dp(idiomas ? 280 : 360)));
        raiz.SetColumnSpan(encabezado, apilado ? 1 : 2);
        if (idiomas) raiz.SetColumnSpan(Buscar<FlowLayoutPanel>(raiz, "flowlayoutpanel5"), apilado ? 1 : 2);
        raiz.RowCount = fila + (apilado ? 2 : 1);
        raiz.RowStyles.Clear();
        for (int i = 0; i < fila; i++) raiz.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        raiz.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        var contenido = editor.Controls.OfType<TableLayoutPanel>().First();
        int anchoEditor = apilado ? ancho - raiz.Padding.Horizontal : dp(idiomas ? 280 : 360);
        foreach (var label in contenido.Controls.OfType<Label>())
            label.MaximumSize = new Size(Math.Max(1, anchoEditor - contenido.Padding.Horizontal - label.Margin.Horizontal), 0);
        int altoEditor = Math.Max(dp(idiomas ? 470 : 500), contenido.GetPreferredSize(new Size(anchoEditor, 0)).Height + dp(24));
        if (apilado) raiz.RowStyles.Add(new RowStyle(SizeType.Absolute, altoEditor));
        Ubicar(raiz, datos, 0, fila);
        Ubicar(raiz, editor, apilado ? 0 : 1, apilado ? fila + 1 : fila);
        return apilado ? altoEditor + dp(idiomas ? 800 : 560) : Math.Max(dp(720), altoEditor + dp(220));
    }

    private static void ConfigurarPrincipal(Form form)
    {
        form.MinimumSize = new Size((int)(760 * form.DeviceDpi / 96f), (int)(520 * form.DeviceDpi / 96f));
        var marco = Buscar<TableLayoutPanel>(form, "marcoPrincipal");
        var encabezado = Buscar<TableLayoutPanel>(form, "encabezadoPrincipal");
        var seccion = Buscar<Label>(encabezado, "lblSeccion");
        var cuenta = Buscar<Label>(encabezado, "lblCuenta");
        var tema = Buscar<Button>(encabezado, "btnTemaVisual");
        var idioma = Buscar<Button>(encabezado, "btnIdiomaVisual");
        var inicio = Buscar<TableLayoutPanel>(form, "panelInicio");
        inicio.AutoScroll = true;
        var barra = Buscar<TableLayoutPanel>(form, "barraLateral");
        var navegacion = Buscar<TableLayoutPanel>(barra, "panelNavegacion");
        var scroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true, Margin = Padding.Empty };
        barra.Controls.Remove(navegacion); scroll.Controls.Add(navegacion); barra.Controls.Add(scroll, 0, 0);
        bool ajustando = false;
        void Ajustar()
        {
            if (ajustando || form.IsDisposed) return;
            ajustando = true;
            try
            {
                float escala = form.DeviceDpi / 96f;
                bool compacto = form.ClientSize.Width < 1100 * escala;
                marco.ColumnStyles[0].Width = (compacto ? 180 : 236) * escala;
                encabezado.SuspendLayout();
                encabezado.RowCount = compacto ? 2 : 1;
                encabezado.RowStyles.Clear();
                encabezado.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                if (compacto) encabezado.RowStyles.Add(new RowStyle(SizeType.AutoSize));
                Ubicar(encabezado, seccion, 0, 0, columnas: compacto ? 4 : 1);
                Ubicar(encabezado, cuenta, compacto ? 0 : 1, compacto ? 1 : 0);
                Ubicar(encabezado, tema, 2, compacto ? 1 : 0);
                Ubicar(encabezado, idioma, 3, compacto ? 1 : 0);
                encabezado.ResumeLayout(true);
                marco.RowStyles[0].Height = Math.Max((compacto ? 115 : 76) * escala, encabezado.GetPreferredSize(new Size(encabezado.Width, 0)).Height);
                var tarjetas = Buscar<FlowLayoutPanel>(inicio, "tarjetasInicio");
                foreach (Control tarjeta in tarjetas.Controls)
                {
                    tarjeta.MinimumSize = new Size(Math.Min((int)(250 * escala), Math.Max(1, tarjetas.ClientSize.Width - tarjeta.Margin.Horizontal)), (int)(88 * escala));
                    tarjeta.Width = tarjeta.MinimumSize.Width;
                }
            }
            finally { ajustando = false; }
        }
        form.ClientSizeChanged += (_, _) => Ajustar();
        form.Shown += (_, _) => Ajustar();
        form.DpiChanged += (_, _) => Ajustar();
    }

    private static void ConfigurarDialogo(Form form)
    {
        // Los diálogos conservan sus controles y botones predeterminados, pero
        // pueden redimensionarse y desplazarse en un área de trabajo pequeña.
        form.FormBorderStyle = FormBorderStyle.Sizable;
        foreach (Control c in form.Controls)
            c.Anchor = c is Button ? AnchorStyles.Bottom | AnchorStyles.Right :
                c is CheckedListBox ? AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right :
                AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        form.MinimumSize = form.Size;
        form.Shown += (_, _) =>
        {
            var area = Screen.FromControl(form).WorkingArea;
            form.MinimumSize = new Size(Math.Min(form.MinimumSize.Width, area.Width), Math.Min(form.MinimumSize.Height, area.Height));
            if (form.Height > area.Height || form.Width > area.Width)
            {
                form.AutoScrollMinSize = form.ClientSize;
                form.AutoScroll = true;
                form.Size = new Size(Math.Min(form.Width, area.Width), Math.Min(form.Height, area.Height));
            }
        };
    }
}
