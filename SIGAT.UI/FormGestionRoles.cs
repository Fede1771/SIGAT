using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public class FormGestionRoles : Form, IIdiomaObserver
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
            Load += (_, _) =>
            {
                if (IsDisposed) return;
                IdiomaManager.ObtenerInstancia().Suscribir(this);
                ActualizarIdioma();
            };
            FormClosed += (_, _) => IdiomaManager.ObtenerInstancia().Desuscribir(this);
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
            string icon = permiso is PermisoCompuesto ? "Familia" : "Permiso";
            TreeNode nodo = new TreeNode(permiso.Nombre) { Tag = permiso, ImageKey = icon, SelectedImageKey = icon };
            nodoPadre.Nodes.Add(nodo);
            // Cada llamada carga un nivel; las hojas devuelven una lista vacía.
            foreach (Permiso hijo in permiso.ObtenerHijos())
                CargarNodosRecursivo(nodo, hijo);
        }

        private void CargarRol(TreeNodeCollection nodos, Rol rol)
        {
            TreeNode nodo = new TreeNode(rol.Nombre) { Tag = rol, ImageKey = "Roles", SelectedImageKey = "Roles" };
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
            if (origenUsuario is Rol rol) destino.AgregarRol(rol);
            else if (origenUsuario is Permiso permiso) destino.AgregarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione un permiso en el catálogo o un rol en el árbol superior.");
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
            TemaVisual.Formulario(dialogo);
            TemaVisual.EstilizarBoton(aceptar, primary: true);
            TemaVisual.EstilizarBoton(cancelar);
            aceptar.Size = cancelar.Size = new Size(88, 36);
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
            if (origenUsuario is Rol rol && destino.Roles.Contains(rol)) destino.QuitarRol(rol);
            else if (origenUsuario is Permiso permiso && destino.Permisos.Contains(permiso)) destino.QuitarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione una asignación directa del rol destino. Los permisos heredados se quitan del rol o familia que los contiene.");
        }

        private void EditarFamilia()
        {
            if (!(tvCatalogoGeneral.SelectedNode?.Tag is PermisoCompuesto familia))
                throw new InvalidOperationException("Seleccione un permiso compuesto del catálogo. Los permisos simples no se editan.");
            using Form dialogo = new Form { Text = "Componentes de " + familia.Nombre, ClientSize = new Size(460, 360),
                StartPosition = FormStartPosition.CenterParent, FormBorderStyle = FormBorderStyle.FixedDialog, MinimizeBox = false, MaximizeBox = false };
            var ayuda = new Label { Text = "Marcá los permisos que integran esta familia.\nEl cambio afecta a todos los roles que la utilizan.", Left = 16, Top = 16, Width = 428, Height = 45 };
            var lista = new CheckedListBox { Left = 16, Top = 65, Width = 428, Height = 230, CheckOnClick = true, DisplayMember = "Nombre" };
            foreach (Permiso permiso in estado.Permisos)
                if (!ReferenceEquals(permiso, familia)) lista.Items.Add(permiso, familia.ObtenerHijos().Contains(permiso));
            var guardar = new Button { Text = "Guardar", Left = 274, Top = 315, DialogResult = DialogResult.OK };
            var cancelar = new Button { Text = "Cancelar", Left = 364, Top = 315, DialogResult = DialogResult.Cancel };
            dialogo.Controls.AddRange(new Control[] { ayuda, lista, guardar, cancelar });
            dialogo.AcceptButton = guardar;
            dialogo.CancelButton = cancelar;
            TemaVisual.Formulario(dialogo);
            TemaVisual.EstilizarBoton(guardar, primary: true);
            TemaVisual.EstilizarBoton(cancelar);
            guardar.Size = cancelar.Size = new Size(88, 36);
            lista.BorderStyle = BorderStyle.None;
            if (dialogo.ShowDialog(this) != DialogResult.OK) throw new OperationCanceledException();
            foreach (Permiso hijo in familia.ObtenerHijos()) familia.Quitar(hijo);
            foreach (Permiso hijo in lista.CheckedItems) familia.Agregar(hijo);
        }
        private void Ejecutar(Action accion)
        {
            try
            {
                accion();

                rolesBLL.Guardar(estado);
                RefrescarArboles();
                lblEstado.Text = "Cambios guardados. Se aplican al volver a iniciar sesión.";
                lblEstado.BackColor = TemaVisual.VerdeSuave;
                lblEstado.ForeColor = TemaVisual.Verde;
            }
            catch (OperationCanceledException) { }
            catch (Exception ex)
            {
                lblEstado.Text = "No se guardó el cambio.";
                lblEstado.BackColor = TemaVisual.AmbarSuave;
                lblEstado.ForeColor = TemaVisual.Ambar;
                MessageBox.Show(this, "No se guardó el cambio.\n" + ex.Message, "Gestión de roles",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                CargarDatos(this, EventArgs.Empty);
            }
        }

        // La presentación conserva los mismos comandos de asignación y persistencia.
        private void CrearControles()
        {
            SuspendLayout();
            TemaVisual.Formulario(this);
            Name = "FormGestionRoles";
            Text = "Gestión de roles y permisos";
            Tag = "gestion_roles_titulo";
            ClientSize = new Size(1240, 780);
            MinimumSize = new Size(1000, 650);
            StartPosition = FormStartPosition.CenterParent;
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 3, Padding = new Padding(24) };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.Controls.Add(TemaVisual.Encabezado("Roles y permisos", "Organizá los accesos de cada usuario y los componentes de cada rol.", "gestion_roles_titulo", "gestion_roles_descripcion"), 0, 0);
            var workspace = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, RowCount = 2, Margin = Padding.Empty };
            workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30));
            workspace.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36));
            workspace.RowStyles.Add(new RowStyle(SizeType.Percent, 52));
            workspace.RowStyles.Add(new RowStyle(SizeType.Percent, 48));
            layout.Controls.Add(workspace, 0, 1);

            tvRolesJerarquia.Name = "tvRolesJerarquia";
            tvCatalogoGeneral.Name = "tvCatalogoGeneral";
            tvUsuarioPermisos.Name = "tvUsuarioPermisos";
            foreach (var tree in new[] { tvRolesJerarquia, tvCatalogoGeneral, tvUsuarioPermisos }) TemaVisual.Arbol(tree);
            workspace.Controls.Add(Sector("Roles disponibles", "roles_disponibles", tvRolesJerarquia), 0, 0);
            var catalog = Sector("Catálogo de permisos", "catalogo_permisos", tvCatalogoGeneral);
            catalog.Margin = new Padding(0, 16, 16, 0);
            workspace.Controls.Add(catalog, 0, 1);

            var editor = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(18) };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            editor.Controls.Add(TemaVisual.Etiqueta("Composición del rol", "composicion_rol", true));
            editor.Controls.Add(TemaVisual.Etiqueta("Rol destino", "rol_destino"));
            cbRolDestino.Name = "cbRolDestino";
            cbRolDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRolDestino.Dock = DockStyle.Fill;
            cbRolDestino.Margin = new Padding(0, 0, 0, 16);
            editor.Controls.Add(cbRolDestino);
            editor.Controls.Add(Ayuda("Seleccioná un rol o permiso a la izquierda y elegí dónde asignarlo.", "ayuda_composicion"));
            editor.Controls.Add(Boton("btnAsignarARol", "Asignar componente", AsignarARol, "asignar_componente", primary: true));
            editor.Controls.Add(Boton("btnQuitarDeRol", "Quitar componente", QuitarDeRol, "quitar_componente"));
            editor.Controls.Add(Boton("btnEditarFamilia", "Editar familia de permisos", EditarFamilia, "editar_familia"));
            var divider = new Panel { Height = 1, Dock = DockStyle.Top, BackColor = TemaVisual.Borde, Margin = new Padding(0, 12, 0, 16) };
            editor.Controls.Add(divider);
            editor.Controls.Add(Boton("btnCrearRol", "Crear rol", () => CrearRol(false), "crear_rol"));
            editor.Controls.Add(Boton("btnEliminarRol", "Eliminar rol", EliminarRol, "eliminar_rol", danger: true));
            editor.Controls.Add(Ayuda("El rol Administrador está protegido. Los cambios se aplican al volver a iniciar sesión.", "ayuda_administrador"));
            var editorCard = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, AutoScroll = true, Margin = new Padding(0, 0, 16, 0) };
            editorCard.Controls.Add(editor);
            workspace.Controls.Add(editorCard, 1, 0);
            workspace.SetRowSpan(editorCard, 2);

            var userCard = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 5, BackColor = Color.White, Padding = new Padding(18), Margin = Padding.Empty };
            userCard.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            for (int row = 0; row < 4; row++) userCard.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            userCard.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            userCard.Controls.Add(TemaVisual.Etiqueta("Accesos del usuario", "accesos_usuario", true), 0, 0);
            cbUsuarios.Name = "cbUsuarios";
            cbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuarios.Dock = DockStyle.Fill;
            cbUsuarios.Margin = new Padding(0, 0, 0, 14);
            cbUsuarios.FormattingEnabled = true;
            cbUsuarios.Format += (s, e) =>
            {
                if (e.ListItem is Usuario usuario)
                    e.Value = usuario.NombreUsuario + (usuario.Activo ? "" : " (inactivo)");
            };
            userCard.Controls.Add(cbUsuarios, 0, 1);
            var actions = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Top, WrapContents = true, Margin = Padding.Empty };
            var assign = Boton("btnAsignarAUsuario", "Asignar rol", AsignarAUsuario, "asignar_rol_usuario", primary: true);
            var remove = Boton("btnQuitarAUsuario", "Quitar rol", QuitarAUsuario, "quitar_rol_usuario");
            assign.Dock = remove.Dock = DockStyle.None;
            assign.AutoSize = remove.AutoSize = true;
            assign.Width = remove.Width = 126;
            actions.Controls.AddRange(new Control[] { assign, remove });
            userCard.Controls.Add(actions, 0, 2);
            userCard.Controls.Add(Ayuda("Para asignar, seleccioná un rol a la izquierda. Para quitar, seleccioná el rol asignado en este panel.", "ayuda_asignacion"), 0, 3);
            tvUsuarioPermisos.Dock = DockStyle.Fill;
            userCard.Controls.Add(tvUsuarioPermisos, 0, 4);
            workspace.Controls.Add(userCard, 2, 0);
            workspace.SetRowSpan(userCard, 2);

            lblEstado.AutoSize = true;
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Padding = new Padding(14, 12, 14, 12);
            lblEstado.Margin = new Padding(0, 16, 0, 0);
            lblEstado.BackColor = TemaVisual.AzulSuave;
            lblEstado.ForeColor = TemaVisual.Azul;
            lblEstado.Text = "Los cambios se guardan automáticamente.";
            layout.Controls.Add(lblEstado, 0, 2);
            Controls.Add(layout);
            cbUsuarios.SelectedIndexChanged += (s, e) => RefrescarUsuario();
            tvRolesJerarquia.AfterSelect += (s, e) => origenUsuario = e.Node.Tag;
            tvCatalogoGeneral.AfterSelect += (s, e) => origenUsuario = e.Node.Tag;
            ResumeLayout(true);
        }

        private static Panel Sector(string title, string key, TreeView tree)
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 2 };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            layout.Controls.Add(TemaVisual.Etiqueta(title, key, true), 0, 0);
            tree.Dock = DockStyle.Fill;
            layout.Controls.Add(tree, 0, 1);
            var card = TemaVisual.Tarjeta(layout);
            card.Margin = new Padding(0, 0, 16, 0);
            return card;
        }

        private static Label Ayuda(string text, string key)
            => new Label { Text = text, Tag = key, Dock = DockStyle.Fill, AutoSize = true, ForeColor = TemaVisual.Secundario, Margin = new Padding(0, 0, 0, 14) };

        private Button Boton(string name, string text, Action action, string key, bool primary = false, bool danger = false)
        {
            var button = TemaVisual.Boton(name, text, primary, danger);
            button.Tag = key;
            button.Dock = DockStyle.Fill;
            button.AutoSize = true;
            button.Margin = new Padding(0, 0, 0, 10);
            button.Click += (_, _) => Ejecutar(action);
            return button;
        }

        public void ActualizarIdioma() => TraductorFormularios.TraducirFormulario(this);
    }
}
