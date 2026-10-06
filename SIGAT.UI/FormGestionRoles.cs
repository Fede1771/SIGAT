using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;

namespace SIGAT.UI
{
    public class FormGestionRoles : Form
    {
        private readonly List<Rol> roles = new List<Rol>();
        private readonly List<Permiso> patentes = new List<Permiso>();
        private readonly List<Usuario> usuarios = new List<Usuario>();
        private readonly Dictionary<Usuario, Rol> personales = new Dictionary<Usuario, Rol>();
        private readonly TreeView tvRolesJerarquia = new TreeView();
        private readonly TreeView tvUsuarioPermisos = new TreeView();
        private readonly TreeView tvCatalogoGeneral = new TreeView();
        private readonly ComboBox cbUsuarios = new ComboBox();
        private object origenUsuario;
        private int siguienteId = 1;

        public FormGestionRoles()
        {
            CrearControles();
            Load += CargarDatos;
        }

        // Datos del TP: no son los usuarios ni los permisos de SQL Server.
        private void CargarDatos(object sender, EventArgs e)
        {
            foreach (string nombre in new string[]
            { "Bitácora", "Control de Cambios", "Gestión de Roles", "Generar Copia de Seguridad" })
            {
                patentes.Add(new PermisoSimple { Id = siguienteId++, Nombre = nombre });
            }
            Rol admin = NuevoRol("Administrador");
            roles.Add(admin);
            roles.Add(NuevoRol("Usuario Simple"));
            roles.Add(NuevoRol("Gestión"));
            admin.AgregarPermiso(patentes[2]);
            admin.AgregarPermiso(patentes[3]);
            Usuario usuario = new Usuario { Id = 1, NombreUsuario = "admin" };
            usuario.AsignarRol(admin);
            usuarios.Add(usuario);
            usuarios.Add(new Usuario { Id = 2, NombreUsuario = "alumno" });
            cbUsuarios.DisplayMember = "NombreUsuario";
            cbUsuarios.DataSource = usuarios;
            RefrescarArboles();
        }

        private Rol NuevoRol(string nombre)
        {
            Rol rol = new Rol { Id = siguienteId++, Nombre = nombre };
            rol.Familia.Id = siguienteId++;
            return rol;
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
            TreeNode catalogoRoles = tvCatalogoGeneral.Nodes.Add("ROLES (Familias)");
            TreeNode catalogoPatentes = tvCatalogoGeneral.Nodes.Add("PERMISOS SIMPLES (Patentes)");
            foreach (Rol rol in roles)
            {
                CargarRol(tvRolesJerarquia.Nodes, rol);
                CargarRol(catalogoRoles.Nodes, rol);
            }
            foreach (Permiso patente in patentes)
                CargarNodosRecursivo(catalogoPatentes, patente);
            tvRolesJerarquia.ExpandAll();
            tvCatalogoGeneral.ExpandAll();
            RefrescarUsuario();
        }

        private void RefrescarUsuario()
        {
            tvUsuarioPermisos.Nodes.Clear();
            if (cbUsuarios.SelectedItem is Usuario usuario)
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

        private PermisoCompuesto DestinoSeleccionado()
        {
            object destino = tvRolesJerarquia.SelectedNode?.Tag;
            if (destino is Rol rol) return rol.Familia;
            if (destino is PermisoCompuesto familia) return familia;
            throw new InvalidOperationException("Seleccione un rol o familia en la jerarquía superior.");
        }

        private void AsignarARol()
        {
            PermisoCompuesto destino = DestinoSeleccionado();
            object origen = tvCatalogoGeneral.SelectedNode?.Tag;
            // Se comparte la familia del rol, conservando Rol fuera del Composite.
            if (origen is Rol rol) destino.Agregar(rol.Familia);
            else if (origen is Permiso permiso) destino.Agregar(permiso);
            else throw new InvalidOperationException("Seleccione un elemento del catálogo.");
        }

        private void AsignarAUsuario()
        {
            Usuario usuario = UsuarioSeleccionado();
            if (origenUsuario is Rol rol)
            {
                usuario.AsignarRol(rol);
            }
            else if (origenUsuario is Permiso permiso)
            {
                // El modelo solo asigna roles: los permisos directos usan un rol personal.
                if (!personales.TryGetValue(usuario, out Rol personal))
                {
                    personal = NuevoRol("Asignaciones directas de " + usuario.NombreUsuario);
                    personales.Add(usuario, personal);
                }
                personal.AgregarPermiso(permiso);
                usuario.AsignarRol(personal);
            }
            else throw new InvalidOperationException("Seleccione un rol o permiso en un árbol izquierdo.");
        }

        private void QuitarAUsuario()
        {
            Usuario usuario = UsuarioSeleccionado();
            TreeNode nodo = tvUsuarioPermisos.SelectedNode;
            if (nodo == null) throw new InvalidOperationException("Seleccione una asignación en el árbol del usuario.");
            if (nodo.Tag is Rol rol)
            {
                usuario.QuitarRol(rol);
                if (personales.TryGetValue(usuario, out Rol personal) && ReferenceEquals(rol, personal))
                    personales.Remove(usuario);
                return;
            }
            if (nodo.Tag is Permiso permiso &&
                personales.TryGetValue(usuario, out Rol directo) &&
                ReferenceEquals(nodo.Parent?.Tag, directo))
            {
                directo.QuitarPermiso(permiso);
                if (directo.Permisos.Count == 0)
                {
                    usuario.QuitarRol(directo);
                    personales.Remove(usuario);
                }
                return;
            }
            throw new InvalidOperationException(
                "El permiso es heredado. Quite el rol completo o su asignación directa; no se modifican roles compartidos desde aquí.");
        }

        private string PedirNombre()
        {
            string nombre = Microsoft.VisualBasic.Interaction.InputBox(
                "Nombre del rol o familia:", "Gestión de roles").Trim();
            if (nombre == "") return "";
            foreach (Rol rol in roles)
            {
                if (rol.Familia.ContienePermiso(nombre))
                    throw new InvalidOperationException("Ya existe ese nombre.");
            }
            foreach (Permiso patente in patentes)
            {
                if (patente.ContienePermiso(nombre))
                    throw new InvalidOperationException("Ya existe ese nombre.");
            }
            return nombre;
        }

        private void CrearRol()
        {
            string nombre = PedirNombre();
            if (nombre != "") roles.Add(NuevoRol(nombre));
        }

        private void CrearRolAnidado()
        {
            PermisoCompuesto destino = DestinoSeleccionado();
            string nombre = PedirNombre();
            if (nombre != "")
                destino.Agregar(new PermisoCompuesto { Id = siguienteId++, Nombre = nombre });
        }

        private void EliminarRol()
        {
            object elegido = tvRolesJerarquia.SelectedNode?.Tag;
            Rol rol = elegido as Rol;
            PermisoCompuesto familia = rol != null ? rol.Familia : elegido as PermisoCompuesto;
            if (familia == null) throw new InvalidOperationException("Seleccione un rol o familia para eliminar.");
            // Una familia anidada puede ser la misma familia de un rol del catálogo.
            foreach (Rol candidato in roles)
            {
                if (ReferenceEquals(candidato.Familia, familia)) rol = candidato;
            }
            if (MessageBox.Show(this, "¿Eliminar '" + familia.Nombre +
                "' y todas sus asignaciones?", "Confirmar baja",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            foreach (Rol actual in roles) Desvincular(actual.Familia, familia);
            foreach (Usuario usuario in usuarios)
            {
                if (rol != null) usuario.QuitarRol(rol);
                if (personales.TryGetValue(usuario, out Rol personal))
                {
                    Desvincular(personal.Familia, familia);
                    if (personal.Permisos.Count == 0)
                    {
                        usuario.QuitarRol(personal);
                        personales.Remove(usuario);
                    }
                }
            }
            if (rol != null) roles.Remove(rol);
        }

        private void Desvincular(Permiso padre, Permiso buscado)
        {
            foreach (Permiso hijo in padre.ObtenerHijos())
            {
                if (ReferenceEquals(hijo, buscado)) padre.Quitar(hijo);
                else Desvincular(hijo, buscado);
            }
        }

        private void Ejecutar(Action accion)
        {
            try
            {
                accion();
                RefrescarArboles();
            }
            catch (InvalidOperationException ex)
            {
                MessageBox.Show(this, ex.Message, "Gestión de roles",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // Creación de controles: el formulario no utiliza Designer.cs.
        private void CrearControles()
        {
            Name = "FormGestionRoles";
            Text = "SIGAT - Roles y permisos (TP en memoria)";
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
            tabla.Controls.Add(Sector("Estructura Jerárquica de Roles (Árbol de Familias)", tvRolesJerarquia), 0, 0);
            tabla.Controls.Add(Sector("Permisos Efectivos del Usuario Seleccionado", tvUsuarioPermisos), 2, 0);
            tabla.SetRowSpan(tvUsuarioPermisos.Parent, 2);
            tabla.Controls.Add(Sector("Catálogo General de Permisos y Roles Disponibles", tvCatalogoGeneral), 0, 1);
            FlowLayoutPanel superior = PanelBotones();
            tabla.Controls.Add(superior, 1, 0);
            superior.Controls.Add(new Label { Text = "Seleccionar Usuario", AutoSize = true });
            cbUsuarios.Name = "cbUsuarios";
            cbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuarios.Width = 260;
            superior.Controls.Add(cbUsuarios);
            superior.Controls.Add(Boton("btnAsignarAUsuario", "Asignar Rol/Permiso a Usuario", AsignarAUsuario));
            superior.Controls.Add(Boton("btnQuitarAUsuario", "Quitar Rol/Permiso a Usuario", QuitarAUsuario));
            superior.Controls.Add(new Label { AutoSize = true, MaximumSize = new Size(260, 0),
                Text = "Asignar: seleccione el origen en un árbol izquierdo.\nQuitar: seleccione la asignación en el árbol derecho.\nLos datos se reinician al abrir este formulario." });
            FlowLayoutPanel inferior = PanelBotones();
            tabla.Controls.Add(inferior, 1, 1);
            inferior.Controls.Add(Boton("btnAsignarARol", "Asignar Rol/Permiso a Rol", AsignarARol));
            inferior.Controls.Add(Boton("btnCrearRol", "Crear Rol", CrearRol));
            inferior.Controls.Add(Boton("btnCrearRolAnidado", "Crear Rol Anidado", CrearRolAnidado));
            inferior.Controls.Add(Boton("btnEliminarRol", "Eliminar Rol", EliminarRol));
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
