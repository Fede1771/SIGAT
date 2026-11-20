using SIGAT.BE;

namespace SIGAT.UI;

public partial class FormGestionRoles
{
    private float proporcionUsuarios = .25f, proporcionFamilias = .30f;
    private bool arrastrandoColumna;

    // Se reutilizan los controles, las selecciones y los comandos existentes. La presentación no modifica el borrador.
    private void ConfigurarPresentacion()
    {
        Label Texto(string nombre, string texto, float tamano = 10, bool destacado = false) => new()
        {
            Name = nombre, Text = texto, AutoSize = true, Dock = DockStyle.Top,
            Font = new Font("Segoe UI", tamano, destacado ? FontStyle.Bold : FontStyle.Regular),
            ForeColor = destacado ? TemaVisual.Texto : TemaVisual.Secundario, Margin = new Padding(0, 0, 0, 8)
        };
        Button Accion(string nombre, string texto, string clave) => new()
        {
            Name = nombre, Text = texto, Tag = "roles_v2_" + clave, AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink, FlatStyle = FlatStyle.Flat,
            BackColor = Color.White, ForeColor = TemaVisual.Texto, MinimumSize = new Size(0, 36),
            Padding = new Padding(10, 3, 10, 3), Margin = new Padding(0, 0, 8, 6)
        };
        void Tabla(TableLayoutPanel tabla, params SizeType[] filas)
        {
            tabla.Controls.Clear(); tabla.RowStyles.Clear(); tabla.ColumnStyles.Clear();
            tabla.ColumnCount = 1; tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tabla.RowCount = filas.Length;
            foreach (var tipo in filas) tabla.RowStyles.Add(new RowStyle(tipo, tipo == SizeType.Percent ? 100 : 0));
        }
        void Etiqueta(Label etiqueta, string texto, string clave)
        { etiqueta.Text = texto; etiqueta.Tag = "roles_v2_" + clave; etiqueta.Font = new Font("Segoe UI", 12, FontStyle.Bold); }

        layoutPrincipal.Padding = new Padding(18, 12, 18, 10);
        encabezadoRoles.Margin = new Padding(0, 0, 0, 12);
        label4.Text = "Asigná roles a un usuario o seleccioná una rama para editar su contenido.";
        label4.Tag = "roles_v2_ayudaRedisenada";
        // Recargar es una acción secundaria del catálogo, separada de Guardar y Descartar.
        encabezadoRoles.ColumnCount = 2;
        encabezadoRoles.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        encabezadoRoles.SetColumnSpan(label3, 2);
        encabezadoRoles.Controls.Add(btnRecargarDatos, 1, 1);
        btnRecargarDatos.Text = "Actualizar catálogo"; btnRecargarDatos.Tag = "roles_v2_actualizarCatalogo";
        btnRecargarDatos.Margin = new Padding(16, 0, 0, 0);
        btnRecargarDatos.MinimumSize = new Size(0, 32);

        foreach (var panel in new[] { paginaUsuarios, paginaRoles, paginaFamilias })
        { panel.Padding = new Padding(14); panel.Margin = new Padding(0, 0, 10, 0); }
        paginaFamilias.Margin = Padding.Empty;
        foreach (var tabla in new[] { detalleUsuarios, detalleRoles, detalleFamilias }) tabla.Padding = new Padding(0, 12, 0, 0);
        Etiqueta(encabezadoUsuarios, "1. Usuarios", "usuariosRedisenados");
        Etiqueta(encabezadoRol, "2. Roles", "rolesRedisenados");
        Etiqueta(encabezadoFamilias, "3. Catálogo de permisos", "catalogoRedisenado");
        foreach (var lista in new ListBox[] { listaUsuarios, listaFamilias })
        {
            lista.Font = new Font("Segoe UI", 11); lista.BorderStyle = BorderStyle.None;
            lista.IntegralHeight = false; lista.DrawMode = DrawMode.OwnerDrawFixed;
            lista.ItemHeight = lista == listaFamilias ? 46 : 34;
        }
        foreach (var buscador in new[] { buscarUsuarios, buscarRoles, buscarFamilias })
        { buscador.Font = new Font("Segoe UI", 11); buscador.Margin = new Padding(0, 2, 0, 12); }
        rolesUsuario.Font = new Font("Segoe UI", 11); rolesUsuario.BorderStyle = BorderStyle.None;
        rolesUsuario.ItemHeight = 30; rolesUsuario.IntegralHeight = false;
        foreach (var tree in new[] { tvUsuarioPermisos, tvRolesJerarquia, tvCatalogoGeneral })
        {
            tree.Font = new Font("Segoe UI", 11); tree.ItemHeight = 30; tree.Indent = 24;
            tree.DrawMode = TreeViewDrawMode.OwnerDrawText; tree.Margin = new Padding(0, 10, 0, 0);
            tree.ShowLines = tree.ShowPlusMinus = true;
        }
        foreach (var titulo in new[] { tituloUsuarios, tituloRoles, tituloFamilias })
        { titulo.Font = new Font("Segoe UI", 11.5f, FontStyle.Bold); titulo.Margin = new Padding(0, 0, 0, 6); }
        foreach (var ayuda in new[] { ayudaUsuarios, ayudaRoles, ayudaFamilias })
        { ayuda.Font = new Font("Segoe UI", 10); ayuda.Margin = new Padding(0, 0, 0, 10); }

        lblDestinoRol = Texto("lblDestinoRol", "Seleccioná un rol o una rama.");
        lblAlcanceRol = Texto("lblAlcanceRol", "Los cambios de un rol afectan a quienes lo tienen asignado.");
        lblTipoFamilia = Texto("lblTipoFamilia", "Familia o permiso simple");
        btnIncluirRolVisible = Accion("btnIncluirRolVisible", "Incluir otro rol…", "incluirRolVisible");
        btnOrigenVisible = Accion("btnOrigenVisible", "Ir al origen", "irAlOrigen");
        btnNuevaFamiliaVisible = Accion("btnNuevaFamiliaVisible", "+ Nueva familia", "nuevaFamiliaVisible");
        btnSubfamiliaVisible = Accion("btnSubfamiliaVisible", "Crear subfamilia…", "subfamiliaVisible");

        // El encabezado describe el catálogo completo; el bloque de edición identifica la selección exacta.
        Tabla(detalleRoles, SizeType.AutoSize, SizeType.AutoSize, SizeType.AutoSize, SizeType.AutoSize,
            SizeType.AutoSize, SizeType.AutoSize, SizeType.AutoSize, SizeType.Percent);
        detalleRoles.Controls.Add(tituloRoles, 0, 0);
        detalleRoles.Controls.Add(lblDestinoRol, 0, 1);
        detalleRoles.Controls.Add(ayudaRoles, 0, 2);
        var contexto = new FlowLayoutPanel { Name = "accionesContextoRol", AutoSize = true, Dock = DockStyle.Top, Margin = Padding.Empty };
        contexto.Controls.Add(btnIncluirRolVisible); contexto.Controls.Add(btnOrigenVisible);
        contexto.Controls.Add(mostrarDetalleRol);
        detalleRoles.Controls.Add(accionesRol, 0, 3);
        detalleRoles.Controls.Add(contexto, 0, 4);
        detalleRoles.Controls.Add(lblAlcanceRol, 0, 5);
        detalleRoles.Controls.Add(rutaRol, 0, 6);
        detalleRoles.Controls.Add(tvRolesJerarquia, 0, 7);
        var cabeceraCatalogo = new TableLayoutPanel { Name = "cabeceraCatalogo", AutoSize = true, Dock = DockStyle.Top,
            ColumnCount = 2, RowCount = 1, Margin = Padding.Empty };
        cabeceraCatalogo.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        cabeceraCatalogo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        cabeceraCatalogo.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cabeceraCatalogo.Controls.Add(encabezadoFamilias, 0, 0);
        cabeceraCatalogo.Controls.Add(btnNuevaFamiliaVisible, 1, 0);
        seleccionFamilias.Controls.Add(cabeceraCatalogo, 0, 0);
        var cabeceraDetalleFamilia = new TableLayoutPanel { Name = "cabeceraDetalleFamilia", AutoSize = true,
            Dock = DockStyle.Top, ColumnCount = 1, RowCount = 3, Margin = Padding.Empty };
        cabeceraDetalleFamilia.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < 3; i++) cabeceraDetalleFamilia.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        cabeceraDetalleFamilia.Controls.Add(tituloFamilias, 0, 0);
        cabeceraDetalleFamilia.Controls.Add(lblTipoFamilia, 0, 1);
        cabeceraDetalleFamilia.Controls.Add(btnSubfamiliaVisible, 0, 2);
        detalleFamilias.Controls.Add(cabeceraDetalleFamilia, 0, 0);
        // Los detalles se despliegan junto a su contexto, sin cambiar las selecciones de las demás columnas.
        foreach (var boton in new[] { mostrarPermisosUsuario, mostrarDetalleRol, mostrarDetalleFamilia })
        {
            boton.Appearance = Appearance.Button; boton.FlatStyle = FlatStyle.Flat;
            boton.FlatAppearance.BorderSize = 0; boton.Padding = new Padding(4, 3, 4, 3);
            boton.Margin = new Padding(0, 0, 8, 6); boton.MinimumSize = new Size(0, 30);
        }
        mostrarPermisosUsuario.Text = "Ver accesos del usuario ▸"; mostrarPermisosUsuario.Tag = "roles_v2_accesosUsuarioClaros";
        mostrarDetalleRol.Text = "Procedencia y herencia ▸"; mostrarDetalleRol.Tag = "roles_v2_procedenciaClara";
        mostrarDetalleFamilia.Text = "Dónde se utiliza ▸"; mostrarDetalleFamilia.Tag = "roles_v2_usoClaro";
        foreach (var boton in new[] { btnAgregarPermisoRol, btnQuitarAsignacion, btnAgregarComponenteFamilia, btnQuitarComponenteFamilia,
            btnMasOpcionesRol, btnMasOpcionesFamilia, btnGuardarCambios, btnDescartarCambios })
        { boton.MinimumSize = new Size(0, 36); boton.Padding = new Padding(10, 3, 10, 3); boton.Margin = new Padding(0, 0, 8, 6); }
        // Los textos de acción se resuelven según el destino seleccionado y se registran como leyendas estáticas.
        btnAgregarPermisoRol.Tag = btnQuitarAsignacion.Tag = btnAgregarComponenteFamilia.Tag = btnQuitarComponenteFamilia.Tag = null;
        btnAgregarPermisoRol.Text = "Agregar permiso o familia…";
        btnAgregarComponenteFamilia.Text = "Agregar a esta familia…";
        btnQuitarAsignacion.Text = "Quitar asignación"; btnQuitarComponenteFamilia.Text = "Quitar de esta familia";
        btnMasOpcionesRol.Text = btnMasOpcionesFamilia.Text = "Opciones ▾";
        btnMasOpcionesRol.Tag = btnMasOpcionesFamilia.Tag = "roles_v2_opcionesClaras";
        btnVerResumen = new LinkLabel { Name = "btnVerResumen", Text = "Ver resumen", Tag = "roles_v2_verResumenClaro",
            AutoSize = true, Anchor = AnchorStyles.None, Margin = new Padding(0, 8, 18, 0) };
        accionesGuardado.Controls.Clear();
        accionesGuardado.Controls.Add(btnVerResumen); accionesGuardado.Controls.Add(btnDescartarCambios); accionesGuardado.Controls.Add(btnGuardarCambios);
        pieRoles.Padding = new Padding(0, 12, 0, 0);
        separadorUsuarios = new Panel { Name = "separadorUsuarios", Dock = DockStyle.Right, Width = 5, Cursor = Cursors.VSplit, BackColor = TemaVisual.Borde };
        separadorRoles = new Panel { Name = "separadorRoles", Dock = DockStyle.Right, Width = 5, Cursor = Cursors.VSplit, BackColor = TemaVisual.Borde };
        paginaUsuarios.Controls.Add(separadorUsuarios); separadorUsuarios.BringToFront();
        paginaRoles.Controls.Add(separadorRoles); separadorRoles.BringToFront();
    }

    private void ConectarPresentacion()
    {
        listaUsuarios.DrawItem += DibujarLista;
        listaFamilias.DrawItem += DibujarLista;
        foreach (var arbol in new[] { tvUsuarioPermisos, tvRolesJerarquia, tvCatalogoGeneral }) arbol.DrawNode += DibujarNodo;
        btnIncluirRolVisible.Click += (_, _) => btnIncluirRol.PerformClick();
        btnOrigenVisible.Click += (_, _) => btnAbrirOrigenRol.PerformClick();
        btnNuevaFamiliaVisible.Click += (_, _) => btnCrearFamilia.PerformClick();
        btnSubfamiliaVisible.Click += (_, _) => btnCrearFamiliaAnidada.PerformClick();
        btnVerResumen.LinkClicked += (_, _) =>
        {
            using var resumen = new DlgFamiliaPermisos();
            resumen.CargarResumen(T("revision", "Revisar cambios antes de guardar"), ResumenCambios(), soloLectura: true);
            resumen.ShowDialog(this);
        };
        btnVerResumen.Paint += (_, _) => btnVerResumen.LinkColor = TemaVisual.ColorActual(TemaVisual.Azul, true);
        foreach (var (separador, primera) in new[] { (separadorUsuarios, true), (separadorRoles, false) })
        {
            separador.MouseDown += (_, e) => { if (e.Button == MouseButtons.Left) { arrastrandoColumna = true; separador.Capture = true; } };
            separador.MouseUp += (_, _) => { arrastrandoColumna = false; separador.Capture = false; };
            separador.MouseMove += (_, _) =>
            {
                if (!arrastrandoColumna || !separador.Capture) return;
                float ancho = Math.Max(1, columnasRoles.ClientSize.Width);
                float proporcion = columnasRoles.PointToClient(Cursor.Position).X / ancho;
                if (primera) proporcionUsuarios = Math.Clamp(proporcion, .18f, .40f);
                else proporcionFamilias = Math.Clamp(1 - proporcion, .24f, .42f);
                AjustarDistribucion();
            };
            separador.DoubleClick += (_, _) => { proporcionUsuarios = .25f; proporcionFamilias = .30f; AjustarDistribucion(); };
            ayudaContextual.SetToolTip(separador, T("ajustarColumnas", "Arrastrá para ajustar el ancho. Doble clic: restaurar distribución."));
        }
    }

    private void DibujarLista(object sender, DrawItemEventArgs e)
    {
        var lista = (ListBox)sender;
        if (e.Index < 0 || e.Index >= lista.Items.Count || lista.Items[e.Index] is not Opcion opcion) return;
        bool elegido = (e.State & DrawItemState.Selected) != 0;
        Color fondo = TemaVisual.ColorActual(elegido ? TemaVisual.AzulSuave : Color.White);
        Color texto = TemaVisual.ColorActual(elegido ? TemaVisual.Azul : TemaVisual.Texto, true);
        using var brush = new SolidBrush(fondo); e.Graphics.FillRectangle(brush, e.Bounds);
        int margen = (int)(8 * DeviceDpi / 96f);
        var r = Rectangle.Inflate(e.Bounds, -margen, 0);
        if (lista == listaFamilias)
        {
            var p = Estado.Permisos.Find(p => p.Id == opcion.Id);
            using var icono = TemaVisual.Icono(p is PermisoCompuesto ? IconoVisual.Familia : IconoVisual.Permiso, texto, 18);
            e.Graphics.DrawImage(icono, r.Left, r.Top + 9, 18, 18); r.X += 27; r.Width -= 27;
            TextRenderer.DrawText(e.Graphics, opcion.Texto, e.Font, new Rectangle(r.Left, r.Top + 3, r.Width, e.Font.Height + 3), texto,
                TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
            string tipo = p is PermisoCompuesto ? T("familia", "Familia") + " · " + p.ObtenerHijos().Count : T("permisoSimpleClaro", "Permiso simple");
            using var pequena = new Font(e.Font.FontFamily, e.Font.Size * .82f);
            TextRenderer.DrawText(e.Graphics, tipo, pequena, new Rectangle(r.Left, r.Top + e.Font.Height + 5, r.Width, pequena.Height + 2),
                TemaVisual.ColorActual(TemaVisual.Secundario, true), TextFormatFlags.EndEllipsis | TextFormatFlags.SingleLine);
        }
        else TextRenderer.DrawText(e.Graphics, opcion.Texto, e.Font, r, texto,
            TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
        if (elegido && lista.Focused) ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(e.Bounds, -1, -1), texto, fondo);
    }

    private void DibujarNodo(object sender, DrawTreeNodeEventArgs e)
    {
        if (e.Node == null) return;
        var arbol = (TreeView)sender;
        bool elegido = e.Node == arbol.SelectedNode;
        Color fondo = TemaVisual.ColorActual(elegido ? TemaVisual.AzulSuave : Color.White);
        Color texto = TemaVisual.ColorActual(elegido ? TemaVisual.Azul : TemaVisual.Texto, true);
        var rect = new Rectangle(e.Bounds.Left, e.Bounds.Top, Math.Max(e.Bounds.Width, arbol.ClientSize.Width - e.Bounds.Left), e.Bounds.Height);
        using var brush = new SolidBrush(fondo); e.Graphics.FillRectangle(brush, rect);
        using var fuente = new Font(arbol.Font, e.Node.Tag is Nodo { Componente: Rol } ? FontStyle.Bold : FontStyle.Regular);
        TextRenderer.DrawText(e.Graphics, e.Node.Text, fuente, rect, texto, TextFormatFlags.VerticalCenter | TextFormatFlags.SingleLine | TextFormatFlags.EndEllipsis);
    }

    private void ActualizarPresentacion()
    {
        btnIncluirRolVisible.Enabled = btnIncluirRol.Enabled;
        btnIncluirRolVisible.Visible = ComponenteSeleccionado is Rol;
        btnOrigenVisible.Visible = btnAbrirOrigenRol.Enabled;
        btnOrigenVisible.Enabled = btnAbrirOrigenRol.Enabled;
        btnNuevaFamiliaVisible.Enabled = datosCargados && !consultando;
        btnSubfamiliaVisible.Enabled = btnCrearFamiliaAnidada.Enabled;
        btnSubfamiliaVisible.Visible = Estado.Permisos.Find(p => p.Id == familiaId) is PermisoCompuesto;
        btnVerResumen.Enabled = TieneCambios && !consultando;
        btnGuardarCambios.BackColor = TieneCambios ? TemaVisual.Azul : TemaVisual.ColorActual(TemaVisual.Borde);
        btnGuardarCambios.ForeColor = TieneCambios ? Color.White : TemaVisual.ColorActual(TemaVisual.Secundario, true);
        if (!datosCargados) return;
        lblDestinoRol.Text = tvRolesJerarquia.SelectedNode?.Tag is Nodo ?
            tvRolesJerarquia.SelectedNode.FullPath.Replace("\\", "  ›  ") : T("seleccionarDestinoClaro", "Seleccioná un rol o una rama para editar.");
        IEnumerable<string> alcance = ComponenteSeleccionado is Permiso p ? edicion.Impacto(p.Id) :
            ComponenteSeleccionado is Rol r ? edicion.ImpactoRol(r.Id) : Array.Empty<string>();
        int usuarios = alcance.Count(a => a.StartsWith("Usuario:", StringComparison.Ordinal));
        lblAlcanceRol.Text = T("alcanceClaro", "Usuarios que reciben este componente:") + " " + usuarios;
        var familia = Estado.Permisos.Find(p => p.Id == familiaId);
        lblTipoFamilia.Text = familia is PermisoCompuesto ? T("familiaCompartidaClara", "Familia compartida") + " · " +
            T("elementosClaros", "Elementos:") + " " + familia.ObtenerHijos().Count : T("accionSimpleClara", "Permiso simple · Acción sin hijos");
        if (familia != null && familia.Id <= 7) lblTipoFamilia.Text += " · " + T("delSistemaClaro", "Del sistema");
        btnAgregarPermisoRol.Text = T("agregarPermisoFamiliaClaro", "Agregar permiso o familia…");
        btnAgregarComponenteFamilia.Text = T("agregarFamiliaClaro", "Agregar a esta familia…");
        btnQuitarAsignacion.Text = T("quitarAsignacionClaro", "Quitar asignación");
        btnQuitarComponenteFamilia.Text = T("quitarFamiliaClaro", "Quitar de esta familia");
        listaUsuarios.Invalidate(); listaFamilias.Invalidate();
    }

    private string ResumenCambios()
    {
        var impacto = familiasModificadas.SelectMany(edicion.Impacto).Concat(rolesModificados.SelectMany(edicion.ImpactoRol)).Distinct();
        return string.Join(Environment.NewLine, edicion.Cambios) + Environment.NewLine + Environment.NewLine + string.Join(Environment.NewLine, impacto);
    }
}
