using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI;

public partial class FormGestionRoles : Form, IIdiomaObserver
{
    private readonly RolesBLL rolesBLL = new();
    private EdicionRoles edicion = new(new EstadoRoles());
    private bool cargando;
    private bool consultando, datosCargados;
    private readonly System.Windows.Forms.Timer[] filtros = new System.Windows.Forms.Timer[3];
    private ToolTip ayudaContextual;
    private bool distribuyendo, distribucionPendiente;
    private int usuarioId, rolId, familiaId;
    private readonly HashSet<int> familiasModificadas = new();
    private readonly HashSet<int> rolesModificados = new();
    private sealed record Opcion(int Id, string Texto);
    private sealed record Nodo(object Componente, bool Directo, int? FamiliaOrigen, int? RolOrigen);
    private EstadoRoles Estado => edicion.Estado;
    public bool TieneCambios => edicion.TieneCambios;

    public FormGestionRoles()
    {
        InitializeComponent();
        if (EntornoDiseno.Activo) return;
        ayudaContextual = new ToolTip(components) { AutoPopDelay = 12000 };
        // Los títulos dinámicos no se registran como leyendas con nombres de usuarios.
        foreach (var c in new Control[] { tituloUsuarios, tituloRoles, tituloFamilias, ayudaUsuarios, ayudaRoles,
            ayudaFamilias, rutaUsuario, rutaRol, rutaFamilia, lblEstado }) c.Tag = null;
        TemaVisual.Formulario(this);
        foreach (var tree in new[] { tvUsuarioPermisos, tvRolesJerarquia, tvCatalogoGeneral })
        {
            TemaVisual.CargarIconosArbol(tree);
            if (!tree.ImageList.Images.ContainsKey("Vacio"))
                tree.ImageList.Images.Add("Vacio", new Bitmap(tree.ImageList.ImageSize.Width, tree.ImageList.ImageSize.Height));
            tree.ItemHeight = Math.Max(tree.Font.Height + 4, (int)(22 * DeviceDpi / 96f));
            var menu = new ContextMenuStrip(components);
            menu.Items.Add(T("expandir", "Expandir todo"), null, (_, _) => tree.ExpandAll());
            menu.Items.Add(T("contraer", "Contraer todo"), null, (_, _) => tree.CollapseAll());
            tree.ContextMenuStrip = menu;
            menu.Opening += (_, _) => TemaVisual.Aplicar(menu);
        }
        Load += CargarDatos;
        Load += (_, _) => { if (!IsDisposed) IdiomaManager.ObtenerInstancia().Suscribir(this); };
        FormClosed += (_, _) => IdiomaManager.ObtenerInstancia().Desuscribir(this);
        FormClosing += (_, e) => { if (!ConfirmarSalida()) e.Cancel = true; };
        ClientSizeChanged += (_, _) => ProgramarDistribucion();
        foreach (var visor in new[] { contenedorDetalleUsuarios, contenedorDetalleRoles, contenedorDetalleFamilias })
            visor.ClientSizeChanged += (_, _) => ProgramarDistribucion();
        Shown += (_, _) => AjustarDistribucion();
        DpiChanged += (_, _) => AjustarDistribucion();
        visorColumnas.ClientSizeChanged += (_, _) => ProgramarDistribucion();
        var buscadores = new[] { buscarUsuarios, buscarRoles, buscarFamilias };
        for (int i = 0; i < buscadores.Length; i++)
        {
            int columna = i;
            var temporizador = filtros[i] = new System.Windows.Forms.Timer(components) { Interval = 180 };
            temporizador.Tick += (_, _) => { temporizador.Stop(); if (!consultando) PoblarListas(columna); };
            buscadores[i].TextChanged += (_, _) => { temporizador.Stop(); if (!cargando && !consultando) temporizador.Start(); };
        }
        listaUsuarios.SelectedIndexChanged += (_, _) => Seleccionar(listaUsuarios, id => usuarioId = id, 0);
        listaRoles.SelectedIndexChanged += (_, _) => Seleccionar(listaRoles, id => rolId = id, 1);
        listaFamilias.SelectedIndexChanged += (_, _) => Seleccionar(listaFamilias, id => familiaId = id, 2);
        rolesUsuario.ItemCheck += CambiarRolUsuario;
        tvRolesJerarquia.AfterSelect += (_, _) => { if (!cargando) { MostrarProcedencia(tvRolesJerarquia, rutaRol); ActualizarAcciones(); } };
        tvUsuarioPermisos.AfterSelect += (_, _) => { if (!cargando) MostrarProcedencia(tvUsuarioPermisos, rutaUsuario); };
        tvCatalogoGeneral.AfterSelect += (_, _) => { if (!cargando) { MostrarProcedencia(tvCatalogoGeneral, rutaFamilia); ActualizarAcciones(); } };
        btnAgregarPermisoRol.Click += (_, _) => AgregarAlRol(false);
        btnIncluirRol.Click += (_, _) => AgregarAlRol(true);
        btnQuitarAsignacion.Click += (_, _) =>
        {
            if (tvRolesJerarquia.SelectedNode?.Tag is Nodo n && n.Directo)
                Ejecutar(() => edicion.QuitarDelRol(rolId, Id(n.Componente), n.Componente is Rol), rol: rolId);
        };
        btnAbrirOrigenRol.Click += (_, _) => AbrirOrigen();
        btnCrearFamilia.Click += (_, _) => CrearFamilia(false);
        btnCrearFamiliaAnidada.Click += (_, _) => CrearFamilia(true);
        btnAgregarComponenteFamilia.Click += (_, _) => AgregarAFamilia();
        btnQuitarComponenteFamilia.Click += (_, _) =>
        {
            if (tvCatalogoGeneral.SelectedNode?.Tag is Nodo n && n.Directo)
                Ejecutar(() => edicion.QuitarDeFamilia(familiaId, Id(n.Componente)), familiaId);
        };
        btnEliminarFamilia.Click += (_, _) =>
        {
            if (MessageBox.Show(this, T("eliminar", "¿Eliminar la familia del catálogo? Sus componentes se conservarán."), Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
                Ejecutar(() => edicion.EliminarFamilia(familiaId), familiaId);
        };
        btnRecargarDatos.Click += (_, _) => { if (ResolverPendientes()) CargarDatos(this, EventArgs.Empty); };
        btnGuardarCambios.Click += (_, _) => GuardarCambios();
        btnDescartarCambios.Click += (_, _) =>
        {
            if (MessageBox.Show(this, T("descartar", "¿Descartar todos los cambios pendientes de este módulo?"), Text,
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) DescartarCambios();
        };
        AjustarDistribucion();
        ActualizarAcciones();
    }

    private string T(string clave, string respaldo) => IdiomaManager.ObtenerInstancia().Traducir(Name, "roles_v2_" + clave, respaldo);
    private static int Id(object o) => o is Rol r ? r.Id : ((Permiso)o).Id;
    private static string Nombre(object o) => o is Rol r ? r.Nombre : ((Permiso)o).Nombre;

    private async void CargarDatos(object sender, EventArgs e)
    {
        if (consultando) return;
        consultando = true;
        foreach (var filtro in filtros) filtro.Stop();
        visorColumnas.Enabled = false;
        UseWaitCursor = true;
        ActualizarAcciones();
        // Capturamos los textos en el hilo de UI; SQL trabaja sin tocar controles.
        var claves = TextosDinamicos.ToDictionary(p => "roles_v2_" + p.Key, p => p.Value);
        void Recorrer(Control padre)
        {
            foreach (Control c in padre.Controls)
            {
                if (c.Tag is string clave) claves.TryAdd(clave, c.Text);
                Recorrer(c);
            }
        }
        Recorrer(this);
        string nombreForm = Name;
        try
        {
            var datos = await Task.Run(() =>
            {
                var estado = rolesBLL.CargarParaGestion();
                new IdiomaBLL().RegistrarClavesNuevas(nombreForm, claves);
                return estado;
            });
            if (IsDisposed || Disposing) return;
            TraducirControles();
            InicializarDatos(datos);
        }
        catch (Exception ex)
        {
            if (IsDisposed || Disposing) return;
            MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            if (!datosCargados) Close();
        }
        finally
        {
            if (!IsDisposed && !Disposing)
            {
                consultando = false;
                visorColumnas.Enabled = datosCargados;
                UseWaitCursor = false;
                ActualizarAcciones();
            }
        }
    }

    private void InicializarDatos(EstadoRoles datos)
    {
        edicion = new EdicionRoles(datos);
        datosCargados = true;
        visorColumnas.Enabled = true;
        familiasModificadas.Clear(); rolesModificados.Clear();
        PoblarListas();
    }

    private void Poblar(ListBox lista, IEnumerable<Opcion> opciones, int seleccionado)
    {
        lista.BeginUpdate();
        try
        {
            lista.Items.Clear();
            foreach (var o in opciones) lista.Items.Add(o);
            lista.SelectedIndex = Enumerable.Range(0, lista.Items.Count).FirstOrDefault(i => ((Opcion)lista.Items[i]).Id == seleccionado, -1);
            if (lista.SelectedIndex < 0 && lista.Items.Count > 0) lista.SelectedIndex = 0;
        }
        finally { lista.EndUpdate(); }
    }

    private void PoblarListas(int columna = -1)
    {
        if (cargando) return;
        if (columna == -1) foreach (var filtro in filtros) filtro?.Stop();
        cargando = true;
        try
        {
            bool Coincide(string nombre, TextBox filtro) => nombre.Contains(filtro.Text.Trim(), StringComparison.CurrentCultureIgnoreCase);
            if (columna is -1 or 0) Poblar(listaUsuarios, Estado.Usuarios.Where(u => Coincide(u.NombreUsuario, buscarUsuarios)).Select(u => new Opcion(u.IdUsuario,
                u.NombreUsuario + (u.Activo ? "" : " · " + T("inactivo", "Inactivo")))), usuarioId);
            if (columna is -1 or 1) Poblar(listaRoles, Estado.Roles.Where(r => Coincide(r.Nombre, buscarRoles)).Select(r => new Opcion(r.Id, r.Nombre)), rolId);
            if (columna is -1 or 2) Poblar(listaFamilias, Estado.Permisos.Where(p => Coincide(p.Nombre, buscarFamilias)).OrderBy(p => p.Nombre)
                .Select(p => new Opcion(p.Id, (p is PermisoCompuesto ? T("familia", "Familia") : T("permiso", "Permiso")) + " · " + p.Nombre)), familiaId);
            usuarioId = (listaUsuarios.SelectedItem as Opcion)?.Id ?? 0;
            rolId = (listaRoles.SelectedItem as Opcion)?.Id ?? 0;
            familiaId = (listaFamilias.SelectedItem as Opcion)?.Id ?? 0;
        }
        finally { cargando = false; }
        MostrarDetalles(columna);
    }

    private void Seleccionar(ListBox lista, Action<int> seleccion, int columna)
    {
        if (cargando) return;
        seleccion((lista.SelectedItem as Opcion)?.Id ?? 0);
        MostrarDetalles(columna);
    }

    private TreeNode NodoPermiso(Permiso p, bool directo, int? familia, int? rol)
    {
        var n = new TreeNode(p.Nombre + " · " + (directo ? T("directo", "Directo") : T("heredado", "Heredado")))
        { Tag = new Nodo(p, directo, familia, rol), ImageKey = p is PermisoCompuesto ? "Familia" : "Permiso" };
        n.SelectedImageKey = n.ImageKey;
        foreach (var h in p.ObtenerHijos()) n.Nodes.Add(NodoPermiso(h, false, p.Id, rol));
        return n;
    }

    private TreeNode NodoRol(Rol r, bool directo, int? rolOrigen)
    {
        var n = new TreeNode(r.Nombre + " · " + (directo ? T("directo", "Directo") : T("heredado", "Heredado")))
        { Tag = new Nodo(r, directo, null, rolOrigen), ImageKey = "Roles", SelectedImageKey = "Roles" };
        foreach (var p in r.Permisos) n.Nodes.Add(NodoPermiso(p, false, null, r.Id));
        foreach (var h in r.Roles) n.Nodes.Add(NodoRol(h, false, r.Id));
        return n;
    }

    private static void ActualizarArbol(TreeView tree, Action llenar)
    {
        string Clave(TreeNode n) => (n.Parent == null ? "" : Clave(n.Parent) + "/") +
            (n.Tag is Nodo dato ? (dato.Componente is Rol ? "r" : "p") + Id(dato.Componente) : n.Text);
        IEnumerable<TreeNode> Todos(TreeNodeCollection ns)
        { foreach (TreeNode n in ns) { yield return n; foreach (var h in Todos(n.Nodes)) yield return h; } }
        var plegados = Todos(tree.Nodes).Where(n => n.Nodes.Count > 0 && !n.IsExpanded).Select(Clave).ToHashSet();
        string seleccion = tree.SelectedNode == null ? null : Clave(tree.SelectedNode);
        string superior = tree.TopNode == null ? null : Clave(tree.TopNode);
        tree.BeginUpdate();
        try
        {
            tree.Nodes.Clear(); llenar(); tree.ExpandAll();
            foreach (var n in Todos(tree.Nodes))
            {
                string clave = Clave(n);
                if (plegados.Contains(clave)) n.Collapse();
                if (clave == seleccion) tree.SelectedNode = n;
            }
            var top = Todos(tree.Nodes).FirstOrDefault(n => Clave(n) == superior);
            if (top != null) tree.TopNode = top;
        }
        finally { tree.EndUpdate(); }
    }

    private void MostrarDetalles(int columna = -1)
    {
        cargando = true;
        try
        {
            if (columna is -1 or 0)
            {
                var usuario = Estado.Usuarios.Find(u => u.IdUsuario == usuarioId);
                tituloUsuarios.Text = usuario == null ? T("seleccionarUsuario", "Seleccioná un usuario") : T("rolesDe", "Roles asignados a:") + " " + usuario.NombreUsuario;
                ayudaUsuarios.Text = T("marcarRoles", "Marcá los roles que querés asignar.");
                bool cuentaProtegida = usuario != null && usuarioId == Estado.IdAdministradorOriginal;
                var administrador = cuentaProtegida ? Estado.Roles.Find(r => r.Id == 1) : null;
                lblRolProtegido.Visible = administrador != null;
            lblRolProtegido.Text = administrador == null ? "" : administrador.Nombre + " · " + T("protegido", "Protegido");
                rolesUsuario.Items.Clear();
                // Una asignación inamovible se presenta como información, sin casilla editable.
                foreach (var r in Estado.Roles.Where(r => !cuentaProtegida || r.Id != 1))
                    rolesUsuario.Items.Add(new Opcion(r.Id, r.Nombre), usuario?.Roles.Any(h => h.Id == r.Id) == true);
                rolesUsuario.Enabled = usuario != null;
                ActualizarArbol(tvUsuarioPermisos, () => { if (usuario != null) foreach (var r in usuario.Roles) tvUsuarioPermisos.Nodes.Add(NodoRol(r, true, null)); });
            MostrarVacio(tvUsuarioPermisos, usuario == null ? T("seleccionarUsuario", "Seleccioná un usuario") : T("sinRoles", "Este usuario no tiene roles asignados."));
                MostrarProcedencia(tvUsuarioPermisos, rutaUsuario);
            }
            if (columna is -1 or 1)
            {
                var rol = Estado.Roles.Find(r => r.Id == rolId);
                tituloRoles.Text = rol == null ? T("seleccionarRol", "Seleccioná un rol") : T("contenidoRol", "Contenido del rol:") + " " + rol.Nombre;
                ayudaRoles.Text = rolId == 1 ? T("soloLectura", "Rol protegido · Solo lectura") :
                    T("editarDirectos", "Editá los componentes directos. Los heredados se cambian en su origen.");
                ActualizarArbol(tvRolesJerarquia, () =>
                {
                    if (rol == null) return;
                    foreach (var p in rol.Permisos) tvRolesJerarquia.Nodes.Add(NodoPermiso(p, true, null, rol.Id));
                    foreach (var h in rol.Roles) tvRolesJerarquia.Nodes.Add(NodoRol(h, true, rol.Id));
                });
            MostrarVacio(tvRolesJerarquia, rol == null ? T("seleccionarRol", "Seleccioná un rol") : T("rolVacio", "Este rol no contiene componentes."));
                MostrarProcedencia(tvRolesJerarquia, rutaRol);
            }
            if (columna is -1 or 2)
            {
                var permiso = Estado.Permisos.Find(p => p.Id == familiaId);
                tituloFamilias.Text = permiso == null ? T("seleccionarFamilia", "Seleccioná un permiso o familia") : T("contenidoDe", "Contenido de:") + " " + permiso.Nombre;
                ayudaFamilias.Text = permiso is PermisoSimple ? T("accionSimple", "Permiso simple · No admite componentes") :
                    T("agrupar", "Agrupá permisos y familias. Quitar retira solo la asignación.");
                if (permiso != null && permiso.Id <= 7) ayudaFamilias.Text += " " + T("noEliminarBase", "Elemento del sistema: no se puede eliminar.");
                var impacto = permiso == null ? Array.Empty<string>() : edicion.Impacto(familiaId).ToArray();
                txtImpacto.Text = T("impacto", "Se utiliza en:") + Environment.NewLine + (impacto.Length == 0 ? T("sinUso", "Sin asignaciones.") : string.Join(Environment.NewLine, impacto));
                ActualizarArbol(tvCatalogoGeneral, () =>
                { if (permiso != null) foreach (var h in permiso.ObtenerHijos()) tvCatalogoGeneral.Nodes.Add(NodoPermiso(h, true, permiso.Id, null)); });
            MostrarVacio(tvCatalogoGeneral, permiso == null ? T("seleccionarFamilia", "Seleccioná un permiso o familia") : permiso is PermisoSimple ? T("sinHijosPermiso", "Este permiso representa una acción del sistema.") : T("familiaVacia", "Esta familia no contiene elementos."));
                MostrarProcedencia(tvCatalogoGeneral, rutaFamilia);
            }
        }
        finally { cargando = false; }
        ActualizarAcciones();
        ProgramarDistribucion();
    }

    private void MostrarVacio(TreeView tree, string mensaje)
    {
        if (tree.Nodes.Count == 0) tree.Nodes.Add(new TreeNode(mensaje)
            { ImageKey = "Vacio", SelectedImageKey = "Vacio", ForeColor = TemaVisual.ColorActual(TemaVisual.Secundario, true) });
    }

    private void MostrarProcedencia(TreeView tree, TextBox destino)
    {
        bool seleccionado = tree.SelectedNode?.Tag is Nodo;
        destino.Text = Procedencia(tree);
        destino.Height = (int)((seleccionado ? 48 : 28) * Math.Max(DeviceDpi / 96f, Font.Height / 17f));
        destino.ScrollBars = seleccionado ? ScrollBars.Vertical : ScrollBars.None;
        ayudaContextual.SetToolTip(destino, destino.Text);
        ProgramarDistribucion();
    }

    private string Procedencia(TreeView tree)
    {
        if (tree.SelectedNode?.Tag is not Nodo) return T("elegirProcedencia", "Seleccioná un componente para ver su procedencia.");
        string actual = T("ruta", "Procedencia:") + " " + tree.SelectedNode.FullPath;
        if (tree.SelectedNode.Tag is not Nodo dato || dato.Componente is not PermisoSimple p) return actual;
        IEnumerable<TreeNode> Recorrer(TreeNodeCollection ns)
        { foreach (TreeNode n in ns) { yield return n; foreach (var h in Recorrer(n.Nodes)) yield return h; } }
        var otras = Recorrer(tree.Nodes).Where(n => n != tree.SelectedNode && n.Tag is Nodo d &&
            d.Componente is Permiso permiso && permiso.Id == p.Id).Select(n => n.FullPath).Distinct().ToArray();
        return otras.Length == 0 ? actual : actual + Environment.NewLine + T("otrasRutas", "También se obtiene por:") +
            Environment.NewLine + string.Join(Environment.NewLine, otras);
    }

    private void ActualizarAcciones()
    {
        if (consultando || !datosCargados)
        {
            btnGuardarCambios.Enabled = btnDescartarCambios.Enabled = btnRecargarDatos.Enabled = false;
            lblEstado.Text = T("cargando", "Cargando roles y permisos…");
            return;
        }
        btnRecargarDatos.Enabled = true;
        bool rolEditable = Estado.Roles.Any(r => r.Id == rolId) && rolId != 1;
        btnAgregarPermisoRol.Enabled = btnIncluirRol.Enabled = rolEditable;
        var n = tvRolesJerarquia.SelectedNode?.Tag as Nodo;
        btnQuitarAsignacion.Enabled = rolEditable && n?.Directo == true;
        btnAbrirOrigenRol.Enabled = n != null && !n.Directo && (n.FamiliaOrigen.HasValue || n.RolOrigen.HasValue);
        btnAbrirOrigenRol.Visible = btnAbrirOrigenRol.Enabled;
        bool familia = Estado.Permisos.Find(p => p.Id == familiaId) is PermisoCompuesto;
        btnCrearFamiliaAnidada.Enabled = btnAgregarComponenteFamilia.Enabled = familia;
        btnQuitarComponenteFamilia.Enabled = familia && (tvCatalogoGeneral.SelectedNode?.Tag as Nodo)?.Directo == true;
        btnEliminarFamilia.Enabled = familia && familiaId > 7 && !edicion.Impacto(familiaId).Any();
        btnGuardarCambios.Enabled = btnDescartarCambios.Enabled = TieneCambios;
        lblEstado.Text = TieneCambios ? T("pendientes", "Cambios pendientes:") + " " + edicion.Cambios.Count : T("sinCambios", "Sin cambios pendientes.");
        lblEstado.ForeColor = TemaVisual.ColorActual(TieneCambios ? TemaVisual.Ambar : TemaVisual.Secundario, true);
    }

    private void CambiarRolUsuario(object sender, ItemCheckEventArgs e)
    {
        if (cargando) return;
        var item = (Opcion)rolesUsuario.Items[e.Index];
        if (usuarioId == Estado.IdAdministradorOriginal && item.Id == 1 && e.NewValue != CheckState.Checked)
        { e.NewValue = e.CurrentValue; return; }
        try { edicion.AsignarRolUsuario(usuarioId, item.Id, e.NewValue == CheckState.Checked); }
        catch (Exception ex) { e.NewValue = e.CurrentValue; MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information); }
        // ItemCheck ocurre antes de que WinForms actualice las marcas.
        if (IsHandleCreated) BeginInvoke((Action)(() => { if (!IsDisposed) MostrarDetalles(); }));
    }

    private void Ejecutar(Action accion, int? familia = null, int? rol = null)
    {
        try
        {
            accion();
            if (familia.HasValue) familiasModificadas.Add(familia.Value);
            if (rol.HasValue) rolesModificados.Add(rol.Value);
            PoblarListas();
        }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Information); }
    }

    private void AgregarAlRol(bool roles)
    {
        var destino = Estado.Roles.Find(r => r.Id == rolId);
        if (destino == null || destino.Id == 1) return;
        IEnumerable<object> opciones = roles ? Estado.Roles.Where(r => !r.ContieneRol(destino) && !destino.Roles.Contains(r)).Cast<object>() :
            Estado.Permisos.Where(p => !destino.Permisos.Contains(p)).Cast<object>();
        using var dialogo = new DlgFamiliaPermisos();
        dialogo.CargarOpciones(opciones, roles ? T("incluirRol", "Incluir otro rol") : T("agregarPermiso", "Agregar permiso o familia"));
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        var seleccion = dialogo.SeleccionadosObjetos.ToArray();
        Ejecutar(() => edicion.AgregarAlRol(rolId, seleccion.OfType<Permiso>().Select(p => p.Id), seleccion.OfType<Rol>().Select(r => r.Id)), rol: rolId);
    }

    private void AgregarAFamilia()
    {
        var f = Estado.Permisos.Find(p => p.Id == familiaId);
        if (f is not PermisoCompuesto) return;
        using var dialogo = new DlgFamiliaPermisos();
        dialogo.CargarOpciones(Estado.Permisos.Where(p => !EdicionRoles.Contiene(p, familiaId) && !f.ObtenerHijos().Contains(p)).Cast<object>(), T("agregarComponente", "Agregar componente existente"));
        if (dialogo.ShowDialog(this) == DialogResult.OK)
            Ejecutar(() => edicion.AgregarAFamilia(familiaId, dialogo.SeleccionadosObjetos.OfType<Permiso>().Select(p => p.Id)), familiaId);
    }

    private void CrearFamilia(bool hija)
    {
        using var dialogo = new DlgNombreRol();
        dialogo.ConfigurarFamilia();
        if (hija) dialogo.Text = T("subfamilia", "Crear subfamilia") + " · " + (Estado.Permisos.Find(p => p.Id == familiaId)?.Nombre ?? "");
        if (dialogo.ShowDialog(this) != DialogResult.OK) return;
        int? padre = hija ? familiaId : null;
        buscarFamilias.Clear();
        Ejecutar(() => familiaId = edicion.CrearFamilia(dialogo.NombreRol, padre), padre);
    }

    private void AbrirOrigen()
    {
        if (tvRolesJerarquia.SelectedNode?.Tag is not Nodo n) return;
        if (n.FamiliaOrigen.HasValue)
        {
            familiaId = n.FamiliaOrigen.Value; buscarFamilias.Clear();
            visorColumnas.ScrollControlIntoView(paginaFamilias);
        }
        else if (n.RolOrigen.HasValue) { rolId = n.RolOrigen.Value; buscarRoles.Clear(); }
        PoblarListas(); AjustarDistribucion();
        if (n.FamiliaOrigen.HasValue) tvCatalogoGeneral.Focus(); else tvRolesJerarquia.Focus();
    }

    public void DescartarCambios()
    {
        edicion.Descartar(); familiasModificadas.Clear(); rolesModificados.Clear(); PoblarListas();
    }

    private bool GuardarCambios()
    {
        if (!TieneCambios) return true;
        var impacto = familiasModificadas.SelectMany(edicion.Impacto).Concat(rolesModificados.SelectMany(edicion.ImpactoRol)).Distinct();
        string resumen = string.Join(Environment.NewLine, edicion.Cambios) + Environment.NewLine + Environment.NewLine +
            string.Join(Environment.NewLine, impacto);
        using var confirmar = new DlgFamiliaPermisos();
        confirmar.CargarResumen(T("revision", "Revisar cambios antes de guardar"), resumen + Environment.NewLine + Environment.NewLine +
            T("confirmarGuardado", "Los accesos de sesiones abiertas se actualizarán al volver a iniciar sesión."));
        if (confirmar.ShowDialog(this) != DialogResult.OK) return false;
        try
        {
            edicion.Guardar(rolesBLL); familiasModificadas.Clear(); rolesModificados.Clear(); PoblarListas();
            lblEstado.Text = T("guardado", "Cambios guardados. Se aplican al volver a iniciar sesión.");
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message + Environment.NewLine + T("errorGuardar", "No se guardó. La copia de trabajo se conserva; descartá y pulsá Recargar si otra persona cambió el catálogo."), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
            return false;
        }
    }

    private bool ResolverPendientes()
    {
        if (!TieneCambios) return true;
        var respuesta = MessageBox.Show(this, T("salir", "Hay cambios pendientes. Sí: guardar. No: descartar. Cancelar: seguir editando."), Text,
            MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
        if (respuesta == DialogResult.Cancel) return false;
        if (respuesta == DialogResult.Yes) return GuardarCambios();
        DescartarCambios(); return true;
    }
    public bool ConfirmarSalida() => ResolverPendientes();

    private void ProgramarDistribucion()
    {
        if (distribucionPendiente || !IsHandleCreated || IsDisposed) return;
        distribucionPendiente = true;
        BeginInvoke((Action)(() => { distribucionPendiente = false; if (!IsDisposed) AjustarDistribucion(); }));
    }

    private void AjustarDistribucion()
    {
        if (columnasRoles == null || distribuyendo) return;
        distribuyendo = true;
        try
        {
            float escala = Math.Max(DeviceDpi / 96f, Font.Height / 17f);
            int disponible = Math.Max(1, visorColumnas.ClientSize.Width);
            int ancho = Math.Max(disponible, (int)Math.Ceiling(3 * 280 * escala));
            int alto = Math.Max(1, visorColumnas.ClientSize.Height - (ancho > disponible ? SystemInformation.HorizontalScrollBarHeight : 0));
            visorColumnas.AutoScrollMinSize = new Size(ancho, 0);
            columnasRoles.Bounds = new Rectangle(visorColumnas.AutoScrollPosition, new Size(ancho, alto));
            foreach (var vista in new[] { vistaUsuarios, vistaRoles, vistaFamilias })
                vista.RowStyles[0].Height = 210 * escala;
            foreach (var (visor, tabla, acciones) in new[] {
                (contenedorDetalleUsuarios, detalleUsuarios, (FlowLayoutPanel)null),
                (contenedorDetalleRoles, detalleRoles, accionesRol),
                (contenedorDetalleFamilias, detalleFamilias, accionesFamilia) })
            {
                int anchoDetalle = Math.Max(1, visor.ClientSize.Width - SystemInformation.VerticalScrollBarWidth - 2);
                foreach (var l in tabla.Controls.OfType<Label>()) l.MaximumSize = new Size(anchoDetalle, 0);
                if (acciones != null)
                {
                    acciones.MaximumSize = new Size(anchoDetalle, 0);
                    foreach (Button boton in acciones.Controls.OfType<Button>())
                        boton.MaximumSize = new Size(Math.Max(100, anchoDetalle - boton.Margin.Horizontal), 0);
                }
                int minimo = 0;
                foreach (Control c in tabla.Controls)
                {
                    var fila = tabla.RowStyles[tabla.GetRow(c)];
                    minimo += fila.SizeType == SizeType.Percent ? (int)((c is CheckedListBox ? 115 : 200) * escala) :
                        fila.SizeType == SizeType.Absolute ? (int)(84 * escala) :
                        c.GetPreferredSize(new Size(Math.Max(1, anchoDetalle - c.Margin.Horizontal), 0)).Height + c.Margin.Vertical;
                    if (fila.SizeType == SizeType.Absolute) fila.Height = 84 * escala;
                }
                int altura = Math.Max(visor.ClientSize.Height, minimo);
                visor.AutoScrollMinSize = new Size(0, altura);
                tabla.Bounds = new Rectangle(visor.AutoScrollPosition, new Size(anchoDetalle, altura));
            }
            int espacioPie = Math.Max(1, pieRoles.ClientSize.Width);
            int accionesAncho = accionesGuardado.Controls.Cast<Control>().Sum(c => c.GetPreferredSize(Size.Empty).Width + c.Margin.Horizontal);
            bool pieCompacto = espacioPie < accionesAncho + 230 * escala;
            pieRoles.ColumnStyles[1].SizeType = SizeType.Absolute;
            pieRoles.ColumnStyles[1].Width = pieCompacto ? 0 : accionesAncho;
            pieRoles.RowCount = pieCompacto ? 2 : 1;
            pieRoles.RowStyles.Clear();
            pieRoles.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            if (pieCompacto) pieRoles.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            pieRoles.SetCellPosition(accionesGuardado, new TableLayoutPanelCellPosition(pieCompacto ? 0 : 1, pieCompacto ? 1 : 0));
            pieRoles.SetColumnSpan(accionesGuardado, pieCompacto ? 2 : 1);
            pieRoles.SetColumnSpan(lblEstado, pieCompacto ? 2 : 1);
            accionesGuardado.MaximumSize = new Size(espacioPie, 0);
            lblEstado.MaximumSize = new Size(Math.Max(1, espacioPie - (pieCompacto ? 0 : accionesAncho)), 0);
            foreach (var l in encabezadoRoles.Controls.OfType<Label>())
                l.MaximumSize = new Size(Math.Max(1, encabezadoRoles.ClientSize.Width), 0);
        }
        finally { distribuyendo = false; }
    }

    private void TraducirControles()
    {
        TraductorFormularios.TraducirFormulario(this, registrarClaves: false);
        foreach (var tree in new[] { tvUsuarioPermisos, tvRolesJerarquia, tvCatalogoGeneral })
        {
            tree.ContextMenuStrip.Items[0].Text = T("expandir", "Expandir todo");
            tree.ContextMenuStrip.Items[1].Text = T("contraer", "Contraer todo");
        }
    }

    public void ActualizarIdioma()
    {
        TraducirControles();
        if (datosCargados && !consultando) PoblarListas();
        else ActualizarAcciones();
        AjustarDistribucion();
    }
}
