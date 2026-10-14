using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SIGAT.BE;
using SIGAT.BLL;
using SIGAT.SERVICIOS.Idiomas;

namespace SIGAT.UI
{
    public partial class FormGestionRoles : Form, IIdiomaObserver
    {
        private readonly List<Rol> roles = new List<Rol>();

        private readonly List<Usuario> usuarios = new List<Usuario>();

        private object origenUsuario;

        private EstadoRoles estado = new EstadoRoles();
        private readonly RolesBLL rolesBLL = new RolesBLL();

        public FormGestionRoles()
        {
            InitializeComponent();
            if (EntornoDiseno.Activo) return;
            TemaVisual.Formulario(this);
            ConfigurarComportamiento();
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

        // La recursión agrega permisos y roles anidados al árbol. La jerarquía visual representa las relaciones del catálogo.
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

        // El origen puede ser un permiso o un rol. El combo Rol destino indica dónde agregamos ese componente.
        private void AsignarARol()
        {
            Rol destino = Destino();
            if (origenUsuario is Rol rol) destino.AgregarRol(rol);
            else if (origenUsuario is Permiso permiso) destino.AgregarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione un permiso en el catálogo o un rol en el árbol superior.");
        }

        private void CrearRol(bool anidado)
        {
            Rol padre = anidado ? Destino() : null;
            using var dialogo = new DlgNombreRol { Text = anidado ? "Crear rol anidado" : "Crear rol" };
            if (dialogo.ShowDialog(this) != DialogResult.OK) throw new OperationCanceledException();
            string texto = dialogo.NombreRol.Trim();
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

        // Quitamos relaciones directas. Un permiso heredado se modifica en el rol o familia que lo contiene.
        private void QuitarDeRol()
        {
            Rol destino = Destino();
            if (origenUsuario is Rol rol && destino.Roles.Contains(rol)) destino.QuitarRol(rol);
            else if (origenUsuario is Permiso permiso && destino.Permisos.Contains(permiso)) destino.QuitarPermiso(permiso);
            else throw new InvalidOperationException("Seleccione una asignación directa del rol destino. Los permisos heredados se quitan del rol o familia que los contiene.");
        }

        // Las marcas del diálogo representan hijos directos. La familia es compartida: editarla afecta a los roles que la utilizan.
        private void EditarFamilia()
        {
            if (!(tvCatalogoGeneral.SelectedNode?.Tag is PermisoCompuesto familia))
                throw new InvalidOperationException("Seleccione un permiso compuesto del catálogo. Los permisos simples no se editan.");
            using var dialogo = new DlgFamiliaPermisos();
            dialogo.Cargar(familia, estado.Permisos);
            if (dialogo.ShowDialog(this) != DialogResult.OK) throw new OperationCanceledException();
            foreach (Permiso hijo in familia.ObtenerHijos()) familia.Quitar(hijo);
            // Reemplazamos los hijos anteriores por los marcados. Agregar sigue comprobando duplicados y ciclos.
            foreach (Permiso hijo in dialogo.Seleccionados) familia.Agregar(hijo);
        }
        // Todos los botones pasan por este bloque. Guardamos y refrescamos; si falla, recargamos el estado desde SQL.
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
            // Cancelar el diálogo es una salida normal; los otros errores se muestran y obligan a recargar el estado.
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

        private void ConfigurarComportamiento()
        {
            foreach (var tree in new[] { tvRolesJerarquia, tvCatalogoGeneral, tvUsuarioPermisos })
            {
                TemaVisual.CargarIconosArbol(tree);
                bool pendiente = false;
                void ProgramarAjuste()
                {
                    if (pendiente || tree.IsDisposed || !tree.IsHandleCreated) return;
                    pendiente = true;
                    // Ajustar al terminar el mensaje nativo de tamaño/expansión del árbol.
                    tree.BeginInvoke((Action)(() =>
                    {
                        pendiente = false;
                        if (!tree.IsDisposed) AjustarFilasArbol(tree);
                    }));
                }
                tree.HandleCreated += (_, _) => ProgramarAjuste();
                tree.ClientSizeChanged += (_, _) => ProgramarAjuste();
                tree.AfterExpand += (_, _) => ProgramarAjuste();
                tree.AfterCollapse += (_, _) => ProgramarAjuste();
            }
            cbUsuarios.Format += (s, e) =>
            {
                if (e.ListItem is Usuario usuario)
                    e.Value = usuario.NombreUsuario + (usuario.Activo ? "" : " (inactivo)");
            };
            layoutRoles.Layout += (_, _) =>
            {
                if (layoutRoles.Height <= 0 || panelComposicion.Width <= 0) return;
                int filaMinima = Math.Max(tvCatalogoGeneral.Font.Height + 3, 22);
                int altoCatalogo = ContarFilasVisibles(tvCatalogoGeneral.Nodes) * filaMinima + 76;
                int ancho = Math.Max(1, panelComposicion.ClientSize.Width - editorComposicion.Padding.Horizontal);
                int preferido = editorComposicion.Padding.Vertical + editorComposicion.Controls.Cast<Control>().Sum(control =>
                    control.GetPreferredSize(new Size(Math.Max(1, ancho - control.Margin.Horizontal), 0)).Height
                    + control.Margin.Vertical) + 8;
                if (editorComposicion.Height != preferido) editorComposicion.Height = preferido;
                int alto = Math.Min(preferido, Math.Max(160, layoutRoles.Height - altoCatalogo));
                if (Math.Abs(layoutRoles.RowStyles[0].Height - alto) > 1)
                    layoutRoles.RowStyles[0].Height = alto;
                panelComposicion.AutoScroll = preferido > alto;
            };


            cbUsuarios.SelectedIndexChanged += (_, _) => RefrescarUsuario();
            tvRolesJerarquia.AfterSelect += (_, e) => origenUsuario = e.Node.Tag;
            tvCatalogoGeneral.AfterSelect += (_, e) => origenUsuario = e.Node.Tag;
        }
        private void AsignarComponente_Click(object sender, EventArgs e) => Ejecutar(AsignarARol);
        private void QuitarComponente_Click(object sender, EventArgs e) => Ejecutar(QuitarDeRol);
        private void EditarFamilia_Click(object sender, EventArgs e) => Ejecutar(EditarFamilia);
        private void AsignarUsuario_Click(object sender, EventArgs e) => Ejecutar(AsignarAUsuario);
        private void QuitarUsuario_Click(object sender, EventArgs e) => Ejecutar(QuitarAUsuario);

        public void ActualizarIdioma() => TraductorFormularios.TraducirFormulario(this);

        private static int ContarFilasVisibles(TreeNodeCollection nodos)
        {
            int cantidad = 0;
            foreach (TreeNode nodo in nodos)
                cantidad += 1 + (nodo.IsExpanded ? ContarFilasVisibles(nodo.Nodes) : 0);
            return cantidad;
        }

        private static void AjustarFilasArbol(TreeView arbol)
        {
            int cantidad = ContarFilasVisibles(arbol.Nodes);
            if (cantidad == 0 || arbol.ClientSize.Height <= 0) return;
            int minimo = Math.Max(arbol.Font.Height + 3, arbol.ImageList?.ImageSize.Height + 2 ?? 22);
            int maximo = Math.Max(minimo, (int)Math.Round(28 * arbol.DeviceDpi / 96F));
            int altura = Math.Clamp((arbol.ClientSize.Height - 4) / cantidad, minimo, maximo);
            if (arbol.ItemHeight == altura) return;
            var expandidos = new List<TreeNode>();
            void Recordar(TreeNodeCollection nodos)
            {
                foreach (TreeNode nodo in nodos)
                {
                    if (nodo.IsExpanded) expandidos.Add(nodo);
                    Recordar(nodo.Nodes);
                }
            }
            Recordar(arbol.Nodes);
            var seleccionado = arbol.SelectedNode;
            var superior = arbol.TopNode;
            arbol.BeginUpdate();
            try
            {
                // El control nativo recrea su ventana al cambiar ItemHeight.
                arbol.ItemHeight = altura;
                foreach (var nodo in expandidos) nodo.Expand();
                arbol.SelectedNode = seleccionado;
                arbol.TopNode = cantidad * altura <= arbol.ClientSize.Height - 4 ? arbol.Nodes[0] : superior;
            }
            finally { arbol.EndUpdate(); }
        }
    }
}
