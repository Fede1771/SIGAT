using System.Text;
using SIGAT.BE.Idiomas;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FrmGestionIdiomas : Form, IIdiomaObserver
    {
        private IdiomaBLL _idiomaBLL = new IdiomaBLL();
        private List<Traduccion> _traduccionesActuales = new List<Traduccion>();

        public FrmGestionIdiomas()
        {
            InitializeComponent();

            Tag = "frmgestionidiomas_titulo";
            lblIdiomaActivo.Tag = "lbl_idioma_activo";
            btnAplicarIdioma.Tag = "btn_aplicar_idioma";
            lblNuevoIdioma.Tag = "lbl_nuevo_idioma";
            lblCodigoIdioma.Tag = "lbl_codigo_idioma";
            lblNombreNativo.Tag = "lbl_nombre_nativo";
            chkIdiomaActivo.Tag = "chk_idioma_activo";
            btnNuevoIdioma.Tag = "btn_nuevo_idioma";
            btnGuardarTraduccion.Tag = "btn_guardar_traduccion";
            btnCambiarEstado.Tag = "btn_cambiar_estado";
            btnExportarTraducciones.Tag = "btn_exportar_traducciones";
            btnImportarTraducciones.Tag = "btn_importar_traducciones";

            chkIdiomaActivo.Checked = true;
            Load += FrmGestionIdiomas_Load;
            FormClosed += FrmGestionIdiomas_FormClosed;
        }

        private void FrmGestionIdiomas_Load(object sender, EventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Suscribir(this);
            CargarComboIdiomas();
            ActualizarIdioma();
            CargarTraducciones();
        }

        private void FrmGestionIdiomas_FormClosed(object sender, FormClosedEventArgs e)
        {
            IdiomaManager.ObtenerInstancia().Desuscribir(this);
        }

        private void CargarComboIdiomas()
        {
            cmbIdiomas.DataSource = null;
            cmbIdiomas.DataSource = _idiomaBLL.ObtenerTodosLosIdiomas();
            cmbIdiomas.DisplayMember = "Nombre";
            cmbIdiomas.ValueMember = "Id";
        }

        private void cmbIdiomas_SelectedIndexChanged(object sender, EventArgs e)
        {
            CargarTraducciones();
        }

        private void btnNuevoIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                int idIdiomaNuevo = _idiomaBLL.CrearIdioma(
                    txtNuevoIdioma.Text,
                    txtCodigoIdioma.Text,
                    txtNombreNativo.Text,
                    chkIdiomaActivo.Checked);

                MessageBox.Show("Idioma creado correctamente. Las traducciones quedaron pendientes.");

                txtNuevoIdioma.Clear();
                txtCodigoIdioma.Clear();
                txtNombreNativo.Clear();
                chkIdiomaActivo.Checked = true;

                CargarComboIdiomas();
                cmbIdiomas.SelectedValue = idIdiomaNuevo;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnAplicarIdioma_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedValue == null) return;

                _idiomaBLL.CambiarIdioma((int)cmbIdiomas.SelectedValue);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnCambiarEstado_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedItem == null) return;

                Idioma idioma = (Idioma)cmbIdiomas.SelectedItem;
                bool nuevoEstado = !idioma.Activo;

                string accion = nuevoEstado ? "activar" : "desactivar";
                DialogResult respuesta = MessageBox.Show(
                    "¿Desea " + accion + " el idioma " + idioma.Nombre + "?",
                    "Confirmar",
                    MessageBoxButtons.YesNo,
                    MessageBoxIcon.Question);

                if (respuesta != DialogResult.Yes) return;

                _idiomaBLL.CambiarEstadoIdioma(idioma.Id, nuevoEstado);
                CargarComboIdiomas();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void CargarTraducciones()
        {
            if (cmbIdiomas.SelectedValue == null) return;
            if (!(cmbIdiomas.SelectedValue is int)) return;

            int idIdioma = (int)cmbIdiomas.SelectedValue;
            _traduccionesActuales = _idiomaBLL.ObtenerTraduccionesParaGestion(idIdioma);
            dgvTraducciones.DataSource = _traduccionesActuales;

            if (dgvTraducciones.Columns.Count == 0) return;

            dgvTraducciones.Columns["IdIdioma"].Visible = false;
            dgvTraducciones.Columns["IdControl"].Visible = false;
            dgvTraducciones.Columns["DigitoVerificador"].Visible = false;
            dgvTraducciones.Columns["FormNombre"].HeaderText = "Formulario";
            dgvTraducciones.Columns["ControlNombre"].HeaderText = "Clave";
            dgvTraducciones.Columns["TextoBase"].HeaderText = "Español";
            dgvTraducciones.Columns["Texto"].HeaderText = "Traducción";
            dgvTraducciones.Columns["Estado"].HeaderText = "Estado";

            dgvTraducciones.Columns["FormNombre"].ReadOnly = true;
            dgvTraducciones.Columns["ControlNombre"].ReadOnly = true;
            dgvTraducciones.Columns["TextoBase"].ReadOnly = true;
            dgvTraducciones.Columns["Estado"].ReadOnly = true;
        }

        private void btnGuardarTraduccion_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvTraducciones.CurrentRow == null)
                {
                    MessageBox.Show("Seleccione una traducción.");
                    return;
                }

                dgvTraducciones.EndEdit();

                Traduccion traduccion = (Traduccion)dgvTraducciones.CurrentRow.DataBoundItem;

                _idiomaBLL.GuardarTraduccionPorControl(
                    traduccion.IdIdioma,
                    traduccion.IdControl,
                    traduccion.Texto);

                CargarTraducciones();
                MessageBox.Show("Traducción guardada correctamente.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnExportarTraducciones_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedValue == null || !(cmbIdiomas.SelectedValue is int))
                {
                    MessageBox.Show("Seleccione un idioma.");
                    return;
                }

                dgvTraducciones.EndEdit();

                Idioma idiomaSeleccionado = (Idioma)cmbIdiomas.SelectedItem;

                using (SaveFileDialog dialogo = new SaveFileDialog())
                {
                    dialogo.Filter = "Archivo de texto (*.txt)|*.txt";
                    dialogo.FileName = "Traducciones_" + idiomaSeleccionado.Codigo + ".txt";

                    if (dialogo.ShowDialog() != DialogResult.OK) return;

                    StringBuilder contenido = new StringBuilder();
                    contenido.AppendLine("Clave\tFormulario\tEspañol\tTraduccion");

                    foreach (Traduccion traduccion in _traduccionesActuales)
                    {
                        contenido.AppendLine(string.Join("\t", new string[]
                        {
                            LimpiarParaLinea(traduccion.ControlNombre),
                            LimpiarParaLinea(traduccion.FormNombre),
                            LimpiarParaLinea(traduccion.TextoBase),
                            LimpiarParaLinea(traduccion.Texto)
                        }));
                    }

                    File.WriteAllText(dialogo.FileName, contenido.ToString(), Encoding.UTF8);
                }

                MessageBox.Show("Traducciones exportadas correctamente. Editá solo la última columna (Traducción) y guardá el archivo.");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void btnImportarTraducciones_Click(object sender, EventArgs e)
        {
            try
            {
                if (cmbIdiomas.SelectedValue == null || !(cmbIdiomas.SelectedValue is int))
                {
                    MessageBox.Show("Seleccione un idioma.");
                    return;
                }

                int idIdioma = (int)cmbIdiomas.SelectedValue;

                using (OpenFileDialog dialogo = new OpenFileDialog())
                {
                    dialogo.Filter = "Archivo de texto (*.txt)|*.txt";

                    if (dialogo.ShowDialog() != DialogResult.OK) return;

                    string[] lineas = File.ReadAllLines(dialogo.FileName, Encoding.UTF8);

                    Dictionary<string, int> mapaClaves = new Dictionary<string, int>();
                    foreach (Traduccion traduccion in _traduccionesActuales)
                    {
                        string clave = traduccion.FormNombre + "|" + traduccion.ControlNombre;
                        mapaClaves[clave] = traduccion.IdControl;
                    }

                    int actualizadas = 0;
                    int omitidas = 0;

                    foreach (string linea in lineas)
                    {
                        if (string.IsNullOrWhiteSpace(linea)) continue;
                        if (linea.StartsWith("Clave\t")) continue; // encabezado

                        string[] columnas = linea.Split('\t');
                        if (columnas.Length < 4)
                        {
                            omitidas++;
                            continue;
                        }

                        string clave = columnas[1].Trim() + "|" + columnas[0].Trim();
                        string nuevaTraduccion = columnas[3];

                        if (!mapaClaves.ContainsKey(clave))
                        {
                            omitidas++;
                            continue;
                        }

                        int idControl = mapaClaves[clave];
                        _idiomaBLL.GuardarTraduccionPorControl(idIdioma, idControl, nuevaTraduccion);
                        actualizadas++;
                    }

                    CargarTraducciones();
                    MessageBox.Show("Importación finalizada. Actualizadas: " + actualizadas + ". Omitidas: " + omitidas + ".");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private static string LimpiarParaLinea(string texto)
        {
            if (string.IsNullOrEmpty(texto)) return "";
            return texto.Replace("\t", " ").Replace("\r", " ").Replace("\n", " ");
        }

        public void ActualizarIdioma()
        {
            TraductorFormularios.TraducirFormulario(this);
        }
    }
}