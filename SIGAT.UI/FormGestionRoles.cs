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
        private readonly ComboBox cbRolDestino = new ComboBox();
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
            ActualizarDestinos();
            usuarios.Clear();
            usuarios.AddRange(datos.Usuarios);
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
            foreach (Rol hijo in rol.Roles) CargarRol(nodo.Nodes, hijo);
        }

        private void RefrescarArboles()
        {
            origenUsuario = null;
            tvRolesJerarquia.Nodes.Clear();
            tvCatalogoGeneral.Nodes.Clear();
            roles.Clear();
            roles.AddRange(estado.Roles);
            ActualizarDestinos();
            foreach (Rol rol in roles) CargarRol(tvRolesJerarquia.Nodes, rol);
            TreeNode catalogoRoles = tvCatalogoGeneral.Nodes.Add("ROLES");
            foreach (Rol rol in roles) CargarRol(catalogoRoles.Nodes, rol);
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
                throw new InvalidOperationException("Seleccione un rol. Los permisos se asignan a roles, no directamente a usuarios.");
            UsuarioSeleccionado().AsignarRol(rol);
        }

        private void QuitarAUsuario()
        {
            if (tvUsuarioPermisos.SelectedNode?.Parent != null || !(tvUsuarioPermisos.SelectedNode?.Tag is Rol rol))
                throw new InvalidOperationException("Seleccione el rol completo que desea quitar.");
            UsuarioSeleccionado().QuitarRol(rol);
        }

        private void ActualizarDestinos()
        {
            int? id = (cbRolDestino.SelectedItem as Rol)?.Id;
            cbRolDestino.DataSource = null;
            cbRolDestino.DisplayMember = "Nombre";
            cbRolDestino.DataSource = estado.Roles.ToList();
            cbRolDestino.SelectedItem = estado.Roles.Find(r => r.Id == id);
        }

        private Rol Destino()
        {
            Rol rol = cbRolDestino.SelectedItem as Rol ?? throw new InvalidOperationException("Seleccione el rol destino.");
            if (rol.Id == 1) throw new InvalidOperationException("El rol Administrador está protegido.");
            return rol;
        }

        private void AsignarARol()
        {
            Rol destino = Destino();
            if (tvCatalogoGeneral.SelectedNode?.Tag is Rol rol) destino.AgregarRol(rol);
            else if (tvCatalogoGeneral.SelectedNode?.Tag is Permiso permiso) destino.AgregarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione un rol o permiso en el catálogo inferior izquierdo.");
        }

        private void CrearRol(bool anidado)
        {
            Rol? padre = anidado ? Destino() : null;
            using Form dialogo = new Form { Text = anidado ? "Crear rol anidado" : "Crear rol", ClientSize = new Size(400, 135),
                StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false };
            var nombre = new TextBox { Left = 16, Top = 40, Width = 368, MaxLength = 150 };
            var aceptar = new Button { Text = "Crear", Left = 204, Top = 88, DialogResult = DialogResult.OK };
            var cancelar = new Button { Text = "Cancelar", Left = 294, Top = 88, DialogResult = DialogResult.Cancel };
            dialogo.Controls.AddRange(new Control[] { new Label { Text = "Nombre del rol", Left = 16, Top = 16, AutoSize = true }, nombre, aceptar, cancelar });
            dialogo.AcceptButton = aceptar;
            dialogo.CancelButton = cancelar;
            if (dialogo.ShowDialog(this) != DialogResult.OK) throw new OperationCanceledException();
            string texto = nombre.Text.Trim();
            if (texto.Length == 0 || estado.Roles.Any(r => string.Equals(r.Nombre, texto, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException("Ingrese un nombre de rol único y no vacío.");
            Rol nuevo = new Rol { Id = estado.Roles.Max(r => r.Id) + 1, Nombre = texto };
            estado.Roles.Add(nuevo);
            padre?.AgregarRol(nuevo);
        }

        private void EliminarRol()
        {
            Rol rol = Destino();
            if (estado.Usuarios.Any(u => u.Roles.Contains(rol)) || estado.Roles.Any(r => r.Roles.Contains(rol)))
                throw new InvalidOperationException("Quite primero las asignaciones de este rol a usuarios y a otros roles.");
            if (MessageBox.Show(this, "¿Eliminar el rol " + rol.Nombre + "?", "Eliminar rol", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                estado.Roles.Remove(rol);
            else throw new OperationCanceledException();
        }

        private void QuitarDeRol()
        {
            Rol destino = Destino();
            if (tvCatalogoGeneral.SelectedNode?.Tag is Rol rol && destino.Roles.Contains(rol)) destino.QuitarRol(rol);
            else if (tvCatalogoGeneral.SelectedNode?.Tag is Permiso permiso && destino.Permisos.Contains(permiso)) destino.QuitarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione en el catálogo una asignación directa del rol destino.");
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
            catch (OperationCanceledException) { }
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
            tabla.Controls.Add(Sector("Roles y componentes", tvRolesJerarquia), 0, 0);
            tabla.Controls.Add(Sector("Roles y permisos asignados al usuario", tvUsuarioPermisos), 2, 0);
            tabla.SetRowSpan(tvUsuarioPermisos.Parent, 2);
            tabla.Controls.Add(Sector("Catálogo de roles y permisos", tvCatalogoGeneral), 0, 1);
            FlowLayoutPanel gestion = PanelBotones();
            tabla.Controls.Add(gestion, 1, 1);
            gestion.Controls.Add(new Label { Text = "Rol destino", AutoSize = true });
            cbRolDestino.Name = "cbRolDestino";
            cbRolDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRolDestino.Width = 270;
            gestion.Controls.Add(cbRolDestino);
            gestion.Controls.Add(Boton("btnAsignarARol", "Asignar rol / permiso a rol", AsignarARol));
            gestion.Controls.Add(Boton("btnQuitarDeRol", "Quitar rol / permiso de rol", QuitarDeRol));
            gestion.Controls.Add(Boton("btnCrearRol", "Crear rol", () => CrearRol(false)));
            gestion.Controls.Add(Boton("btnCrearRolAnidado", "Crear rol anidado", () => CrearRol(true)));
            gestion.Controls.Add(Boton("btnEliminarRol", "Eliminar rol", EliminarRol));
            gestion.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(270, 0), Text = "Elegí el rol destino y un componente del catálogo. El rol destino hereda los permisos de sus roles anidados. Administrador está protegido." });
            FlowLayoutPanel superior = PanelBotones();
            tabla.Controls.Add(superior, 1, 0);
            superior.Controls.Add(new Label { Text = "Usuario de SIGAT", AutoSize = true });
            cbUsuarios.Name = "cbUsuarios";
            cbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuarios.Width = 260;
            cbUsuarios.FormattingEnabled = true;
            cbUsuarios.Format += (s, e) =>
            {
                if (e.ListItem is Usuario usuario)
                    e.Value = usuario.NombreUsuario + (usuario.Activo ? "" : " (inactivo)");
            };
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
