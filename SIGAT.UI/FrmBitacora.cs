using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;
using SIGAT.BE.Auditoria;

namespace SIGAT.UI
{
    public partial class FrmBitacora : Form, IIdiomaObserver
    {
        private BitacoraBLL _bll = new BitacoraBLL();
        private List<string> actividades = new();
        private static readonly IReadOnlyDictionary<string, string> NombresActividades = new Dictionary<string, string>
        {
            [""] = "Todas las actividades",
            ["LoginExitoso"] = "Ingreso al sistema",
            ["LoginFallido"] = "Intento de ingreso fallido",
            ["AltaUsuario"] = "Alta de usuario",
            ["ModificacionUsuario"] = "Modificación de usuario",
            ["BajaUsuario"] = "Baja de usuario",
            ["Logout"] = "Cierre de sesión",
            ["ErrorCritico"] = "Error crítico",
            ["Login"] = "Ingreso al sistema (registro anterior)",
            ["Error Crítico del Sistema"] = "Error crítico del sistema",
            ["RecalculoIntegridad"] = "Recálculo de integridad"
        };

        public FrmBitacora()
        {
            InitializeComponent();
            // El enlace usa las columnas del diseñador; nunca expone propiedades internas.
            dgv.AutoGenerateColumns = false;
            if (EntornoDiseno.Activo) return;
            TemaVisual.Formulario(this);
            MostrarActividades();

            // Después de cargar datos actualizamos los títulos con el idioma de la sesión.
            dgv.DataBindingComplete += Dgv_DataBindingComplete;
            this.Load += FrmBitacora_Load;
            this.FormClosed += FrmBitacora_FormClosed;
        }

        private void FrmBitacora_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            try
            {
                var idiomas = new IdiomaBLL();
                foreach (var (codigo, nombre) in NombresActividades)
                    idiomas.RegistrarClaveNueva(Name, ClaveActividad(codigo), nombre);
                Buscar();
                ActualizarIdioma();
            }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void FrmBitacora_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void BtnBuscar_Click(object sender, EventArgs e)
        {
            try { Buscar(); }
            catch (Exception ex) { MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error); }
        }

        private void Buscar()
        {
            // Las opciones no dependen de los resultados filtrados: no desaparecen al combinar usuario y fechas.
            actividades = _bll.ObtenerActividades();
            MostrarActividades();
            DateTime? fechaDesde = null;
            DateTime? fechaHasta = null;

            if (chkFechas.Checked == true)
            {
                fechaDesde = dtpDesde.Value.Date;
                // La columna SQL datetime admite hasta 23:59:59.997; .999 se redondea al día siguiente.
                fechaHasta = dtpHasta.Value.Date.AddDays(1).AddMilliseconds(-3);
            }

            dgv.DataSource = _bll.Buscar(fechaDesde, fechaHasta, txtUser.Text, ActividadSeleccionada, actividadExacta: true);
        }

        private string ActividadSeleccionada => cmbActividad.SelectedItem is KeyValuePair<string, string> opcion ? opcion.Key : "";

        private static string ClaveActividad(string codigo) => codigo == "" ? "actividad_todas" : "actividad_" + codigo;

        private string NombreActividad(string codigo) => NombresActividades.TryGetValue(codigo, out string nombre)
            ? IdiomaManager.ObtenerInstancia().Traducir(Name, ClaveActividad(codigo), nombre) : codigo;

        private void MostrarActividades()
        {
            string seleccion = ActividadesBitacora.CodigoCanonico(ActividadSeleccionada);
            var opciones = new List<KeyValuePair<string, string>> { new("", NombreActividad("")) };
            // Conservamos la selección aunque otra sesión retire sus últimos eventos; no ampliamos el filtro sin avisar.
            opciones.AddRange(actividades.Select(ActividadesBitacora.CodigoCanonico).Append(seleccion).Where(codigo => !string.IsNullOrWhiteSpace(codigo))
                .Distinct(StringComparer.OrdinalIgnoreCase).Select(codigo => new KeyValuePair<string, string>(codigo, NombreActividad(codigo)))
                .OrderBy(opcion => opcion.Value, StringComparer.CurrentCultureIgnoreCase));
            cmbActividad.DisplayMember = "Value";
            cmbActividad.ValueMember = "Key";
            cmbActividad.DataSource = opciones;
            cmbActividad.SelectedValue = seleccion;
        }

        private void Dgv_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            // Las columnas están en Designer; esta tabla ya no está protegida por DVH/DVV.
            TraducirColumnasVisibles();
            lblVacio.Visible = dgv.DataSource != null && dgv.Rows.Count == 0;
        }

        private void TraducirColumnasVisibles()
        {
            foreach (DataGridViewColumn columna in dgv.Columns)
            {
                if (columna.Tag != null)
                {
                    columna.HeaderText = IdiomaManager.ObtenerInstancia()
                        .Traducir(this.Name, columna.Tag.ToString(), columna.HeaderText);
                }
            }
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
            TraducirColumnasVisibles();
            MostrarActividades();
        }
    }
}
