using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;

namespace SIGAT.UI
{
    public class FormGestionRoles : Form
    {
        private readonly List<Rol> roles = new List<Rol>();

        private readonly List<Usuario> usuarios = new List<Usuario>();

        private readonly TreeView tvRolesJerarquia = new TreeView();
        private readonly TreeView tvUsuarioPermisos = new TreeView();
        private readonly TreeView tvCatalogoGeneral = new TreeView();
        private readonly ComboBox cbUsuarios = new ComboBox();
        private object origenUsuario;

        private EstadoRoles estado = new EstadoRoles();
        private readonly RolesBLL rolesBLL = new RolesBLL();
        private readonly Label lblEstado = new Label();

        public FormGestionRoles()
        {
            CrearControles();
            Load += CargarDatos;
        }

        private void CargarDatos(object sender, EventArgs e)
        {
            try
            {
                InicializarDatos(rolesBLL.CargarParaGestion());
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "No se pudieron cargar los usuarios de SIGAT.\n" + ex.Message,
                    "Gestión de roles y permisos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Close();
            }
        }

        private void InicializarDatos(EstadoRoles datos)
        {
            int? seleccionado = (cbUsuarios.SelectedItem as Usuario)?.IdUsuario;
            cbUsuarios.DataSource = null;
            estado = datos;
            roles.Clear();
            roles.AddRange(datos.Roles);
            usuarios.Clear();
            foreach (Usuario usuario in datos.Usuarios)
                if (usuario.Activo) usuarios.Add(usuario);
            cbUsuarios.DisplayMember = "NombreUsuario";
            cbUsuarios.DataSource = usuarios;
            foreach (Usuario usuario in usuarios)
                if (usuario.IdUsuario == seleccionado) cbUsuarios.SelectedItem = usuario;
            RefrescarArboles();
        }
        private void CargarNodosRecursivo(TreeNode nodoPadre, Permiso permiso)
        {
            TreeNode nodo = new TreeNode(permiso.Nombre) { Tag = permiso };
            nodoPadre.Nodes.Add(nodo);
            // Cada llamada carga un nivel; las hojas devuelven una lista vacía.
            foreach (Permiso hijo in permiso.ObtenerHijos())
                CargarNodosRecursivo(nodo, hijo);
        }

        private void CargarRol(TreeNodeCollection nodos, Rol rol)
        {
            TreeNode nodo = new TreeNode(rol.Nombre) { Tag = rol };
            nodos.Add(nodo);
            foreach (Permiso permiso in rol.Permisos)
                CargarNodosRecursivo(nodo, permiso);
        }

        private void RefrescarArboles()
        {
            origenUsuario = null;
            tvRolesJerarquia.Nodes.Clear();
            tvCatalogoGeneral.Nodes.Clear();
            foreach (Rol rol in roles) CargarRol(tvRolesJerarquia.Nodes, rol);
            TreeNode simples = tvCatalogoGeneral.Nodes.Add("PERMISOS SIMPLES");
            TreeNode familias = tvCatalogoGeneral.Nodes.Add("PERMISOS COMPUESTOS (FAMILIAS)");
            foreach (Permiso permiso in estado.Permisos)
                CargarNodosRecursivo(permiso is PermisoSimple ? simples : familias, permiso);
            tvRolesJerarquia.ExpandAll();
            tvCatalogoGeneral.ExpandAll();
            RefrescarUsuario();
        }

        private void RefrescarUsuario()
        {
            tvUsuarioPermisos.Nodes.Clear();
            if (cbUsuarios.SelectedIndex >= 0 && cbUsuarios.SelectedIndex < cbUsuarios.Items.Count &&
                cbUsuarios.SelectedItem is Usuario usuario)
            {
                foreach (Rol rol in usuario.Roles)
                    CargarRol(tvUsuarioPermisos.Nodes, rol);
            }
            tvUsuarioPermisos.ExpandAll();
        }

        private Usuario UsuarioSeleccionado()
        {
            if (cbUsuarios.SelectedItem is Usuario usuario) return usuario;
            throw new InvalidOperationException("Seleccione un usuario.");
        }

        private void AsignarAUsuario()
        {
            if (!(origenUsuario is Rol rol))
                throw new InvalidOperationException("Seleccione uno de los cinco roles. Los permisos no se asignan directamente.");
            UsuarioSeleccionado().AsignarRol(rol);
        }

        private void QuitarAUsuario()
        {
            if (!(tvUsuarioPermisos.SelectedNode?.Tag is Rol rol))
                throw new InvalidOperationException("Seleccione el rol completo que desea quitar.");
            UsuarioSeleccionado().QuitarRol(rol);
        }
        private void Ejecutar(Action accion)
        {
            try
            {
                accion();

                rolesBLL.Guardar(estado);
                RefrescarArboles();
                lblEstado.Text = "Cambios guardados. Se aplican al volver a iniciar sesión.";
            }
            catch (Exception ex)
            {
                lblEstado.Text = "No se guardó el cambio.";
                MessageBox.Show(this, "No se guardó el cambio.\n" + ex.Message, "Gestión de roles",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(this, EventArgs.Empty);
            }
        }

        // Creación de controles: el formulario no utiliza Designer.cs.
        private void CrearControles()
        {
            Name = "FormGestionRoles";
            Text = "Gestión de roles y permisos";
            Size = new Size(1240, 780);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;
            TableLayoutPanel tabla = new TableLayoutPanel
            { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Padding = new Padding(10) };
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            tabla.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
            tabla.RowStyles.Add(new RowStyle(SizeType.Percent, 45));
            Controls.Add(tabla);
            tvRolesJerarquia.Name = "tvRolesJerarquia";
            tvUsuarioPermisos.Name = "tvUsuarioPermisos";
            tvCatalogoGeneral.Name = "tvCatalogoGeneral";
            tabla.Controls.Add(Sector("Roles y componentes de la matriz", tvRolesJerarquia), 0, 0);
            tabla.Controls.Add(Sector("Roles y permisos asignados al usuario", tvUsuarioPermisos), 2, 0);
            tabla.SetRowSpan(tvUsuarioPermisos.Parent, 2);
            tabla.Controls.Add(Sector("Catálogo de permisos de la matriz", tvCatalogoGeneral), 0, 1);
            FlowLayoutPanel superior = PanelBotones();
            tabla.Controls.Add(superior, 1, 0);
            superior.Controls.Add(new Label { Text = "Usuario activo de SIGAT", AutoSize = true });
            cbUsuarios.Name = "cbUsuarios";
            cbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuarios.Width = 260;
            superior.Controls.Add(cbUsuarios);
            superior.Controls.Add(Boton("btnAsignarAUsuario", "Asignar rol al usuario", AsignarAUsuario));
            superior.Controls.Add(Boton("btnQuitarAUsuario", "Quitar rol al usuario", QuitarAUsuario));
            superior.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(260, 0),
                Text = "Seleccione un rol en el árbol superior izquierdo.\nQuitar: seleccione una asignación a la derecha.\nLos cambios se guardan automáticamente y se aplican al volver a iniciar sesión." });
            lblEstado.AutoSize = true;
            lblEstado.MaximumSize = new Size(260, 0);
            superior.Controls.Add(lblEstado);
            cbUsuarios.SelectedIndexChanged += (s, e) => RefrescarUsuario();
            tvRolesJerarquia.AfterSelect += (s, e) => origenUsuario = e.Node.Tag;
            tvCatalogoGeneral.AfterSelect += (s, e) => origenUsuario = e.Node.Tag;
        }

        private GroupBox Sector(string titulo, TreeView arbol)
        {
            GroupBox grupo = new GroupBox { Text = titulo, Dock = DockStyle.Fill, Padding = new Padding(8) };
            arbol.Dock = DockStyle.Fill;
            arbol.HideSelection = false;
            grupo.Controls.Add(arbol);
            return grupo;
        }

        private FlowLayoutPanel PanelBotones()
        {
            return new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.TopDown,
                WrapContents = false, AutoScroll = true, Padding = new Padding(10) };
        }

        private Button Boton(string nombre, string texto, Action accion)
        {
            Button boton = new Button { Name = nombre, Text = texto, Width = 270, Height = 44 };
            boton.Click += (s, e) => Ejecutar(accion);
            return boton;
        }
    }
}
