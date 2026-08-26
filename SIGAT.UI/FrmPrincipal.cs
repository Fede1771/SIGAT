using System;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BLL;
using SIGAT.SERVICIOS;
using System.Resources;
using System.Reflection;

namespace SIGAT.UI
{
    public partial class FrmPrincipal : Form
    {
        private BitacoraBLL _bitacora = new BitacoraBLL();

        private ToolStripMenuItem itemSistema;
        private ToolStripMenuItem itemUsuarios;
        private ToolStripMenuItem itemBitacora;
        private ToolStripMenuItem itemLogout;
        private ToolStripMenuItem itemIdioma;
        private ToolStripMenuItem itemEspañol;
        private ToolStripMenuItem itemIngles;

        public FrmPrincipal()
        {
            this.IsMdiContainer = true;
            this.WindowState = FormWindowState.Maximized;

            // ESTÉTICA: Pintamos el fondo MDI de un gris muy suave y moderno (Off-White)
            foreach (Control c in this.Controls)
            {
                if (c is MdiClient)
                {
                    c.BackColor = Color.FromArgb(245, 247, 250);
                    break;
                }
            }

            var menu = new MenuStrip
            {
                BackColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular)
            };

            itemSistema = new ToolStripMenuItem();
            itemUsuarios = new ToolStripMenuItem();
            itemBitacora = new ToolStripMenuItem();
            itemLogout = new ToolStripMenuItem();
            itemIdioma = new ToolStripMenuItem();
            itemEspañol = new ToolStripMenuItem("Español");
            itemIngles = new ToolStripMenuItem("English");

            itemUsuarios.Click += (s, e) => { AbrirFormulario(new FrmGestionUsuarios()); };
            itemBitacora.Click += (s, e) => { AbrirFormulario(new FrmBitacora()); };

            itemLogout.Click += (s, e) =>
            {
                string u = SesionServicio.ObtenerInstancia().UsuarioActual.NombreUsuario;
                _bitacora.Registrar(u, "Logout", "Cierre de sesión seguro.");
                SesionServicio.ObtenerInstancia().CerrarSesion();
                this.Hide();
                new FrmLogin().ShowDialog();
                this.Close();
            };

            itemEspañol.Click += (s, e) => IdiomaServicio.ObtenerInstancia().CambiarIdioma("es-AR");
            itemIngles.Click += (s, e) => IdiomaServicio.ObtenerInstancia().CambiarIdioma("en-US");

            itemIdioma.DropDownItems.AddRange(new ToolStripItem[] { itemEspañol, itemIngles });

            var usuarioLogueado = SesionServicio.ObtenerInstancia().UsuarioActual;
            if (usuarioLogueado != null && usuarioLogueado.Perfil.NombrePerfil != "Administrador")
            {
                itemUsuarios.Visible = false;
            }

            itemSistema.DropDownItems.AddRange(new ToolStripItem[] { itemUsuarios, itemBitacora, new ToolStripSeparator(), itemLogout });

            menu.Items.Add(itemSistema);
            menu.Items.Add(itemIdioma);

            this.MainMenuStrip = menu;
            this.Controls.Add(menu);

            IdiomaServicio.ObtenerInstancia().IdiomaCambiado += ActualizarTextos;
            ActualizarTextos();
        }

        private void ActualizarTextos()
        {
            ResourceManager rm = new ResourceManager("SIGAT.UI.Idiomas", Assembly.GetExecutingAssembly());

            string txtUsuario = rm.GetString("TituloUsuario") ?? "Usuario";
            string txtPerfil = rm.GetString("TituloPerfil") ?? "Perfil";
            this.Text = $"SIGAT - {txtUsuario}: {SesionServicio.ObtenerInstancia().UsuarioActual?.NombreUsuario} | {txtPerfil}: {SesionServicio.ObtenerInstancia().PerfilActual?.NombrePerfil}";

            itemSistema.Text = rm.GetString("MenuSistema") ?? "Sistema";
            itemUsuarios.Text = rm.GetString("MenuUsuarios") ?? "Gestión de Usuarios";
            itemBitacora.Text = rm.GetString("MenuBitacora") ?? "Bitácora";
            itemLogout.Text = rm.GetString("MenuLogout") ?? "Cerrar Sesión";
            itemIdioma.Text = rm.GetString("MenuIdioma") ?? "Idioma";
        }

        // MÉTODO CLAVE: Hace que cualquier ventana hija se abra maximizada ocupando todo el fondo limpio
        private void AbrirFormulario(Form formHijo)
        {
            foreach (Form frm in this.MdiChildren)
            {
                frm.Close();
            }
            formHijo.MdiParent = this;
            formHijo.WindowState = FormWindowState.Maximized;
            formHijo.Show();
        }
    }
}