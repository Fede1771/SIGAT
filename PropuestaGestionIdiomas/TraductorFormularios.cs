using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public static class TraductorFormularios
    {
        private static IdiomaBLL _idiomaBLL = new IdiomaBLL();

        public static void TraducirFormulario(Form formulario)
        {
            string nombreForm = formulario.Name;

            if (formulario.Tag != null)
            {
                RegistrarClave(nombreForm, formulario.Tag.ToString(), formulario.Text);
                formulario.Text = Traducir(nombreForm, formulario.Tag.ToString(), formulario.Text);
            }

            if (formulario.MainMenuStrip != null)
            {
                TraducirMenu(nombreForm, formulario.MainMenuStrip.Items);
            }

            TraducirControles(nombreForm, formulario.Controls);
        }

        private static void TraducirControles(string nombreForm, Control.ControlCollection controles)
        {
            foreach (Control control in controles)
            {
                if (control.Tag != null)
                {
                    RegistrarClave(nombreForm, control.Tag.ToString(), control.Text);
                    control.Text = Traducir(nombreForm, control.Tag.ToString(), control.Text);
                }

                if (control is DataGridView grilla)
                {
                    foreach (DataGridViewColumn columna in grilla.Columns)
                    {
                        if (columna.Tag != null)
                        {
                            RegistrarClave(nombreForm, columna.Tag.ToString(), columna.HeaderText);
                            columna.HeaderText = Traducir(nombreForm, columna.Tag.ToString(), columna.HeaderText);
                        }
                    }
                }

                if (control.HasChildren)
                {
                    TraducirControles(nombreForm, control.Controls);
                }
            }
        }

        private static void TraducirMenu(string nombreForm, ToolStripItemCollection items)
        {
            foreach (ToolStripItem item in items)
            {
                if (item.Tag != null)
                {
                    RegistrarClave(nombreForm, item.Tag.ToString(), item.Text);
                    item.Text = Traducir(nombreForm, item.Tag.ToString(), item.Text);
                }

                if (item is ToolStripMenuItem menuItem)
                {
                    TraducirMenu(nombreForm, menuItem.DropDownItems);
                }
            }
        }

        private static void RegistrarClave(string nombreForm, string nombreControl, string textoBase)
        {
            _idiomaBLL.RegistrarClaveNueva(nombreForm, nombreControl, textoBase);
        }

        private static string Traducir(string nombreForm, string clave, string textoOriginal)
        {
            return IdiomaManager.ObtenerInstancia().Traducir(nombreForm, clave, textoOriginal);
        }
    }
}
