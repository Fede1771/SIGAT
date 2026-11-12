#nullable enable

using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI;

public partial class DlgHistorialUsuario : Form, IIdiomaObserver
{
    private UsuarioBLL servicio = new();
    private int idInicial;
    private bool hayBorrador;
    private HistorialUsuario? historial;
    private static readonly IReadOnlyDictionary<string, string> Textos = new Dictionary<string, string>
    {
        ["op_" + nameof(OperacionUsuarioVersion.EstadoInicial)] = "Estado inicial registrado",
        ["op_" + nameof(OperacionUsuarioVersion.Alta)] = "Alta",
        ["op_" + nameof(OperacionUsuarioVersion.Edicion)] = "Edición",
        ["op_" + nameof(OperacionUsuarioVersion.Baja)] = "Baja lógica",
        ["op_" + nameof(OperacionUsuarioVersion.Reactivacion)] = "Reactivación",
        ["op_" + nameof(OperacionUsuarioVersion.Roles)] = "Cambio de roles directos",
        ["op_" + nameof(OperacionUsuarioVersion.Restauracion)] = "Restauración de la versión {0}",
        ["op_" + nameof(OperacionUsuarioVersion.Recuperacion)] = "Recuperación desde respaldo",
        ["clave"] = "Contraseña modificada",
        ["retirada"] = "Cuenta retirada",
        ["usuario"] = "Nombre de usuario",
        ["nombre"] = "Nombre",
        ["apellido"] = "Apellido",
        ["estado"] = "Estado",
        ["roles"] = "Roles directos",
        ["activo"] = "Activo",
        ["inactivo"] = "Inactivo",
        ["sinRoles"] = "Sin roles",
        ["sinVersiones"] = "No hay versiones para mostrar.",
        ["comparacion_cambios"] = "Compará las versiones. Los valores resaltados en negrita cambiaron respecto de la versión anterior.",
        ["modificado"] = "Cambió respecto de la versión anterior.",
        ["coincideActual"] = "Esta versión coincide con el estado actual. No es necesario restaurarla.",
        ["cuentaRetirada"] = "La cuenta fue retirada. Su historial se conserva para consulta.",
        ["versionRetirada"] = "Esta versión registra una cuenta retirada y no puede restaurarse.",
        ["sinSeleccion"] = "Seleccioná una versión para consultar sus datos.",
        ["sinCuentas"] = "No hay cuentas con historial registrado.",
        ["ayudaRestaurar"] = "Restaurar crea una versión nueva y conserva la contraseña actual.",
        ["confirmar"] = "¿Restaurar la versión {0} de la cuenta #{1}? Se creará una versión nueva. Los roles se aplicarán con los permisos vigentes.",
        ["descartar"] = "Al cerrar el historial se descartarán los cambios sin guardar en el formulario de usuarios."
    };

    public DlgHistorialUsuario()
    {
        InitializeComponent();
        if (EntornoDiseno.Activo) return;
        TemaVisual.Formulario(this);
        Shown += CargarHistorial;
        FormClosed += (_, _) => IdiomaManager.ObtenerInstancia().Desuscribir(this);
        dgvDetalle.CellFormatting += (_, e) =>
        {
            if (e.RowIndex < 0 || dgvDetalle.Rows[e.RowIndex].Cells[e.ColumnIndex].Tag is not true) return;
            e.CellStyle!.BackColor = TemaVisual.ColorActual(TemaVisual.AmbarSuave);
            e.CellStyle.ForeColor = TemaVisual.ColorActual(TemaVisual.Ambar, true);
            e.CellStyle.Font = TemaVisual.FuenteResaltada;
            e.CellStyle.SelectionBackColor = e.CellStyle.BackColor;
            e.CellStyle.SelectionForeColor = e.CellStyle.ForeColor;
        };
        cmbCuentas.SelectionChangeCommitted += (_, _) => Intentar(CargarVersiones);
        dgvVersiones.CurrentCellChanged += (_, _) => MostrarDetalle();
        btnRecargar.Click += (_, _) => Intentar(() => CargarCuentas(CuentaSeleccionada?.IdUsuario ?? idInicial));
        btnRestaurar.Click += (_, _) => Intentar(Restaurar);
    }

    private void CargarHistorial(object? sender, EventArgs e) => Intentar(() =>
    {
        var idiomas = new IdiomaBLL();
        foreach (var (clave, texto) in Textos) idiomas.RegistrarClaveNueva(Name, clave, texto);
        TraductorFormularios.TraducirFormulario(this);
        IdiomaManager.ObtenerInstancia().Suscribir(this);
        CargarCuentas(idInicial);
    });

    public void ActualizarIdioma()
    {
        TraductorFormularios.TraducirFormulario(this);
        MostrarVersiones(dgvVersiones.CurrentRow?.Cells["Version"].Value as int?);
    }

    public bool HuboRestauracion { get; private set; }

    public void Configurar(UsuarioBLL usuarioBLL, int idUsuario, bool cambiosSinGuardar)
    {
        servicio = usuarioBLL;
        idInicial = idUsuario;
        hayBorrador = cambiosSinGuardar;
    }

    private CuentaConHistorial? CuentaSeleccionada =>
        cmbCuentas.SelectedItem is KeyValuePair<CuentaConHistorial, string> item ? item.Key : null;

    private string T(string clave) => IdiomaManager.ObtenerInstancia().Traducir(Name, clave, Textos[clave]);

    private void CargarCuentas(int idUsuario)
    {
        var cuentas = servicio.ListarCuentasConHistorial();
        cmbCuentas.DisplayMember = "Value";
        cmbCuentas.DataSource = cuentas.Select(cuenta => KeyValuePair.Create(cuenta,
            $"{cuenta.NombreUsuario} · #{cuenta.IdUsuario}" + (cuenta.Existe ? "" : " · " + T("retirada")))).ToList();
        int indice = cuentas.ToList().FindIndex(cuenta => cuenta.IdUsuario == idUsuario);
        if (indice >= 0) cmbCuentas.SelectedIndex = indice;
        CargarVersiones();
    }

    private void CargarVersiones()
    {
        historial = null;
        dgvVersiones.Rows.Clear();
        MostrarDetalle();
        if (CuentaSeleccionada is not { } cuenta) return;
        historial = servicio.ConsultarHistorial(cuenta.IdUsuario);
        MostrarVersiones();
    }

    // Volvemos a presentar las versiones al cambiar el idioma, conservando la selección y sin consultar ni modificar cuentas.
    private void MostrarVersiones(int? numeroSeleccionado = null)
    {
        dgvVersiones.Rows.Clear();
        if (historial is not { } datos) { MostrarDetalle(); return; }
        foreach (var version in datos.Versiones)
        {
            string operacion = T("op_" + version.Operacion);
            if (version.VersionOrigen is int origen) operacion = string.Format(operacion, origen);
            if (version.CambioClave) operacion += " · " + T("clave");
            string autor = version.Autor.Nombre;
            if (version.Autor.IdUsuario is int idActor) autor += $" · #{idActor}";
            dgvVersiones.Rows.Add(version.Numero, version.FechaUtc.ToLocalTime().ToString("G"), autor, operacion);
        }
        if (numeroSeleccionado is int numero)
            foreach (DataGridViewRow fila in dgvVersiones.Rows)
                if (fila.Cells["Version"].Value is int valor && valor == numero) dgvVersiones.CurrentCell = fila.Cells["Version"];
        MostrarDetalle();
    }

    private void MostrarDetalle()
    {
        dgvDetalle.Rows.Clear();
        btnRestaurar.Enabled = false;
        lblRestauracion.Text = "";
        if (historial is not { } datos || dgvVersiones.CurrentRow is not { } fila)
        {
            lblAyuda.Text = T(CuentaSeleccionada == null ? "sinCuentas" :
                historial?.Versiones.Count > 0 ? "sinSeleccion" : "sinVersiones");
            return;
        }

        lblAyuda.Text = T("comparacion_cambios");
        var version = datos.Versiones[fila.Index];
        var anterior = datos.Versiones.Skip(fila.Index + 1).FirstOrDefault();
        Comparar("usuario", estado => estado.NombreUsuario, (a, b) => a.NombreUsuario == b.NombreUsuario);
        Comparar("nombre", estado => estado.Nombre, (a, b) => a.Nombre == b.Nombre);
        Comparar("apellido", estado => estado.Apellido, (a, b) => a.Apellido == b.Apellido);
        Comparar("estado", estado => T(estado.Activo ? "activo" : "inactivo"), (a, b) => a.Activo == b.Activo);
        Comparar("roles", estado => estado.Roles.Count == 0 ? T("sinRoles") : string.Join(", ", estado.Roles.Select(rol => rol.Nombre)),
            (a, b) => a.MismosRolesQue(b));
        btnRestaurar.Enabled = version.Existe && datos.EstadoActual is { } vigente && !vigente.CoincideCon(version.Estado);
        lblRestauracion.Text = T(datos.EstadoActual == null ? "cuentaRetirada" : !version.Existe ? "versionRetirada" :
            btnRestaurar.Enabled ? "ayudaRestaurar" : "coincideActual");

        void Comparar(string campo, Func<EstadoUsuarioVersion, string> mostrar,
            Func<EstadoUsuarioVersion, EstadoUsuarioVersion, bool> iguales)
        {
            int indice = dgvDetalle.Rows.Add(T(campo), anterior == null ? "—" : anterior.Existe ? mostrar(anterior.Estado) : T("retirada"),
                version.Existe ? mostrar(version.Estado) : T("retirada"),
                datos.EstadoActual is { } actual ? mostrar(actual) : T("retirada"));
            // Comparamos valores de la entidad, no textos traducidos. El alta inicial no tiene un estado anterior comparable.
            if (anterior == null || (anterior.Existe == version.Existe &&
                (!version.Existe || iguales(anterior.Estado, version.Estado)))) return;
            var celda = dgvDetalle.Rows[indice].Cells["Seleccionada"];
            celda.Tag = true;
            celda.ToolTipText = T("modificado");
        }
    }

    private void Restaurar()
    {
        if (historial is not { } datos || dgvVersiones.CurrentRow is not { } fila || !btnRestaurar.Enabled) return;
        var version = datos.Versiones[fila.Index];
        string mensaje = string.Format(T("confirmar"), version.Numero, version.IdUsuario);
        if (hayBorrador) mensaje += Environment.NewLine + Environment.NewLine + T("descartar");
        if (MessageBox.Show(this, mensaje, Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning,
            MessageBoxDefaultButton.Button2) != DialogResult.Yes) return;
        servicio.RestaurarVersion(version.IdUsuario, version.Numero, datos.VersionSeguridad);
        HuboRestauracion = true;
        CargarCuentas(version.IdUsuario);
    }

    private void Intentar(Action accion)
    {
        try { accion(); }
        catch (Exception ex) { MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
    }
}
