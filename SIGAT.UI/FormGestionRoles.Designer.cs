namespace SIGAT.UI
{
    partial class FormGestionRoles
    {
        private System.ComponentModel.IContainer components = null;
        private TableLayoutPanel layoutPrincipal;
        private TableLayoutPanel tablelayoutpanel2;
        private Label label3;
        private Label label4;
        private TableLayoutPanel layoutRoles;
        private Panel panel6;
        private TableLayoutPanel tablelayoutpanel7;
        private Label label8;
        private TreeView tvRolesJerarquia;
        private Panel panel9;
        private TableLayoutPanel tablelayoutpanel10;
        private Label label11;
        private TreeView tvCatalogoGeneral;
        private Panel panelComposicion;
        private TableLayoutPanel editorComposicion;
        private Label label14;
        private Label label15;
        private ComboBox cbRolDestino;
        private Label label16;
        private Button btnAsignarARol;
        private Button btnQuitarDeRol;
        private Button btnEditarFamilia;
        private Label label17;
        private TableLayoutPanel panelAccesos;
        private Label label19;
        private ComboBox cbUsuarios;
        private FlowLayoutPanel flowlayoutpanel20;
        private Button btnAsignarAUsuario;
        private Button btnQuitarAUsuario;
        private Label label21;
        private TreeView tvUsuarioPermisos;
        private Label lblEstado;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        // Estructura visual editable desde el diseÃ±ador; el comportamiento queda en el .cs.
        private void InitializeComponent()
        {
            layoutPrincipal = new TableLayoutPanel();
            tablelayoutpanel2 = new TableLayoutPanel();
            label3 = new Label();
            label4 = new Label();
            layoutRoles = new TableLayoutPanel();
            panel6 = new Panel();
            tablelayoutpanel7 = new TableLayoutPanel();
            label8 = new Label();
            tvRolesJerarquia = new TreeView();
            panel9 = new Panel();
            tablelayoutpanel10 = new TableLayoutPanel();
            label11 = new Label();
            tvCatalogoGeneral = new TreeView();
            panelComposicion = new Panel();
            editorComposicion = new TableLayoutPanel();
            label14 = new Label();
            label15 = new Label();
            cbRolDestino = new ComboBox();
            label16 = new Label();
            btnAsignarARol = new Button();
            btnQuitarDeRol = new Button();
            btnEditarFamilia = new Button();
            label17 = new Label();
            panelAccesos = new TableLayoutPanel();
            label19 = new Label();
            cbUsuarios = new ComboBox();
            flowlayoutpanel20 = new FlowLayoutPanel();
            btnAsignarAUsuario = new Button();
            btnQuitarAUsuario = new Button();
            label21 = new Label();
            tvUsuarioPermisos = new TreeView();
            lblEstado = new Label();
            layoutPrincipal.SuspendLayout();
            tablelayoutpanel2.SuspendLayout();
            layoutRoles.SuspendLayout();
            panel6.SuspendLayout();
            tablelayoutpanel7.SuspendLayout();
            panel9.SuspendLayout();
            tablelayoutpanel10.SuspendLayout();
            panelComposicion.SuspendLayout();
            editorComposicion.SuspendLayout();
            panelAccesos.SuspendLayout();
            flowlayoutpanel20.SuspendLayout();
            SuspendLayout();
            // 
            // layoutPrincipal
            // 
            layoutPrincipal.BackColor = Color.FromArgb(245, 247, 251);
            layoutPrincipal.ColumnCount = 1;
            layoutPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            layoutPrincipal.Controls.Add(tablelayoutpanel2, 0, 0);
            layoutPrincipal.Controls.Add(layoutRoles, 0, 1);
            layoutPrincipal.Controls.Add(lblEstado, 0, 2);
            layoutPrincipal.Dock = DockStyle.Fill;
            layoutPrincipal.Font = new Font("Segoe UI", 10F);
            layoutPrincipal.ForeColor = Color.FromArgb(30, 45, 65);
            layoutPrincipal.Location = new Point(0, 0);
            layoutPrincipal.Name = "layoutPrincipal";
            layoutPrincipal.Padding = new Padding(24);
            layoutPrincipal.RowCount = 3;
            layoutPrincipal.RowStyles.Add(new RowStyle());
            layoutPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutPrincipal.RowStyles.Add(new RowStyle());
            layoutPrincipal.Size = new Size(1240, 780);
            layoutPrincipal.TabIndex = 0;
            // 
            // tablelayoutpanel2
            // 
            tablelayoutpanel2.AutoSize = true;
            tablelayoutpanel2.BackColor = Color.FromArgb(245, 247, 251);
            tablelayoutpanel2.ColumnCount = 1;
            tablelayoutpanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel2.Controls.Add(label3, 0, 0);
            tablelayoutpanel2.Controls.Add(label4, 0, 1);
            tablelayoutpanel2.Dock = DockStyle.Top;
            tablelayoutpanel2.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel2.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel2.Location = new Point(24, 24);
            tablelayoutpanel2.Margin = new Padding(0, 0, 0, 22);
            tablelayoutpanel2.Name = "tablelayoutpanel2";
            tablelayoutpanel2.RowStyles.Add(new RowStyle());
            tablelayoutpanel2.RowStyles.Add(new RowStyle());
            tablelayoutpanel2.Size = new Size(1192, 66);
            tablelayoutpanel2.TabIndex = 0;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.BackColor = Color.FromArgb(245, 247, 251);
            label3.Dock = DockStyle.Fill;
            label3.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            label3.ForeColor = Color.FromArgb(30, 45, 65);
            label3.Location = new Point(0, 0);
            label3.Margin = new Padding(0, 0, 0, 6);
            label3.Name = "label3";
            label3.Size = new Size(1192, 41);
            label3.TabIndex = 0;
            label3.Tag = "gestion_roles_titulo";
            label3.Text = "Roles y permisos";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.BackColor = Color.FromArgb(245, 247, 251);
            label4.Dock = DockStyle.Fill;
            label4.Font = new Font("Segoe UI", 10F);
            label4.ForeColor = Color.FromArgb(94, 111, 133);
            label4.Location = new Point(0, 47);
            label4.Margin = new Padding(0);
            label4.Name = "label4";
            label4.Size = new Size(1192, 19);
            label4.TabIndex = 1;
            label4.Tag = "gestion_roles_descripcion";
            label4.Text = "Organizá los accesos de cada usuario y los componentes de cada rol.";
            // 
            // layoutRoles
            // 
            layoutRoles.BackColor = Color.FromArgb(245, 247, 251);
            layoutRoles.ColumnCount = 3;
            layoutRoles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34F));
            layoutRoles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
            layoutRoles.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            layoutRoles.Controls.Add(panel6, 0, 0);
            layoutRoles.Controls.Add(panel9, 2, 1);
            layoutRoles.Controls.Add(panelComposicion, 2, 0);
            layoutRoles.Controls.Add(panelAccesos, 1, 0);
            layoutRoles.Dock = DockStyle.Fill;
            layoutRoles.Font = new Font("Segoe UI", 10F);
            layoutRoles.ForeColor = Color.FromArgb(30, 45, 65);
            layoutRoles.Location = new Point(24, 112);
            layoutRoles.Margin = new Padding(0);
            layoutRoles.Name = "layoutRoles";
            layoutRoles.RowCount = 2;
            layoutRoles.RowStyles.Add(new RowStyle(SizeType.Absolute, 496F));
            layoutRoles.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            layoutRoles.Size = new Size(1192, 585);
            layoutRoles.TabIndex = 1;
            // 
            // panel6
            // 
            panel6.BackColor = Color.FromArgb(255, 255, 255);
            panel6.Controls.Add(tablelayoutpanel7);
            panel6.Dock = DockStyle.Fill;
            panel6.Font = new Font("Segoe UI", 10F);
            panel6.ForeColor = Color.FromArgb(30, 45, 65);
            panel6.Location = new Point(0, 0);
            panel6.Margin = new Padding(0, 0, 16, 0);
            panel6.Name = "panel6";
            panel6.Padding = new Padding(14);
            layoutRoles.SetRowSpan(panel6, 2);
            panel6.Size = new Size(389, 585);
            panel6.TabIndex = 0;
            // 
            // tablelayoutpanel7
            // 
            tablelayoutpanel7.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel7.ColumnCount = 1;
            tablelayoutpanel7.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel7.Controls.Add(label8, 0, 0);
            tablelayoutpanel7.Controls.Add(tvRolesJerarquia, 0, 1);
            tablelayoutpanel7.Dock = DockStyle.Fill;
            tablelayoutpanel7.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel7.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel7.Location = new Point(14, 14);
            tablelayoutpanel7.Name = "tablelayoutpanel7";
            tablelayoutpanel7.RowCount = 2;
            tablelayoutpanel7.RowStyles.Add(new RowStyle());
            tablelayoutpanel7.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel7.Size = new Size(361, 557);
            tablelayoutpanel7.TabIndex = 0;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.FromArgb(255, 255, 255);
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label8.ForeColor = Color.FromArgb(30, 45, 65);
            label8.Location = new Point(0, 0);
            label8.Margin = new Padding(0, 0, 0, 14);
            label8.Name = "label8";
            label8.Size = new Size(361, 25);
            label8.TabIndex = 0;
            label8.Tag = "roles_disponibles";
            label8.Text = "Roles disponibles";
            // 
            // tvRolesJerarquia
            // 
            tvRolesJerarquia.BackColor = Color.FromArgb(255, 255, 255);
            tvRolesJerarquia.BorderStyle = BorderStyle.None;
            tvRolesJerarquia.Dock = DockStyle.Fill;
            tvRolesJerarquia.Font = new Font("Segoe UI", 10F);
            tvRolesJerarquia.ForeColor = Color.FromArgb(30, 45, 65);
            tvRolesJerarquia.FullRowSelect = true;
            tvRolesJerarquia.HideSelection = false;
            tvRolesJerarquia.Indent = 22;
            tvRolesJerarquia.ItemHeight = 32;
            tvRolesJerarquia.Location = new Point(3, 42);
            tvRolesJerarquia.Name = "tvRolesJerarquia";
            tvRolesJerarquia.ShowLines = false;
            tvRolesJerarquia.ShowNodeToolTips = true;
            tvRolesJerarquia.Size = new Size(355, 512);
            tvRolesJerarquia.TabIndex = 1;
            // 
            // panel9
            // 
            panel9.BackColor = Color.FromArgb(255, 255, 255);
            panel9.Controls.Add(tablelayoutpanel10);
            panel9.Dock = DockStyle.Fill;
            panel9.Font = new Font("Segoe UI", 10F);
            panel9.ForeColor = Color.FromArgb(30, 45, 65);
            panel9.Location = new Point(834, 512);
            panel9.Margin = new Padding(0, 16, 0, 0);
            panel9.Name = "panel9";
            panel9.Padding = new Padding(14);
            panel9.Size = new Size(358, 73);
            panel9.TabIndex = 1;
            // 
            // tablelayoutpanel10
            // 
            tablelayoutpanel10.BackColor = Color.FromArgb(255, 255, 255);
            tablelayoutpanel10.ColumnCount = 1;
            tablelayoutpanel10.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tablelayoutpanel10.Controls.Add(label11, 0, 0);
            tablelayoutpanel10.Controls.Add(tvCatalogoGeneral, 0, 1);
            tablelayoutpanel10.Dock = DockStyle.Fill;
            tablelayoutpanel10.Font = new Font("Segoe UI", 10F);
            tablelayoutpanel10.ForeColor = Color.FromArgb(30, 45, 65);
            tablelayoutpanel10.Location = new Point(14, 14);
            tablelayoutpanel10.Name = "tablelayoutpanel10";
            tablelayoutpanel10.RowCount = 2;
            tablelayoutpanel10.RowStyles.Add(new RowStyle());
            tablelayoutpanel10.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tablelayoutpanel10.Size = new Size(330, 45);
            tablelayoutpanel10.TabIndex = 0;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.BackColor = Color.FromArgb(255, 255, 255);
            label11.Dock = DockStyle.Fill;
            label11.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label11.ForeColor = Color.FromArgb(30, 45, 65);
            label11.Location = new Point(0, 0);
            label11.Margin = new Padding(0, 0, 0, 14);
            label11.Name = "label11";
            label11.Size = new Size(330, 25);
            label11.TabIndex = 0;
            label11.Tag = "catalogo_permisos";
            label11.Text = "Catálogo de permisos";
            // 
            // tvCatalogoGeneral
            // 
            tvCatalogoGeneral.BackColor = Color.FromArgb(255, 255, 255);
            tvCatalogoGeneral.BorderStyle = BorderStyle.None;
            tvCatalogoGeneral.Dock = DockStyle.Fill;
            tvCatalogoGeneral.Font = new Font("Segoe UI", 10F);
            tvCatalogoGeneral.ForeColor = Color.FromArgb(30, 45, 65);
            tvCatalogoGeneral.FullRowSelect = true;
            tvCatalogoGeneral.HideSelection = false;
            tvCatalogoGeneral.Indent = 22;
            tvCatalogoGeneral.ItemHeight = 32;
            tvCatalogoGeneral.Location = new Point(3, 42);
            tvCatalogoGeneral.Name = "tvCatalogoGeneral";
            tvCatalogoGeneral.ShowLines = false;
            tvCatalogoGeneral.ShowNodeToolTips = true;
            tvCatalogoGeneral.Size = new Size(324, 1);
            tvCatalogoGeneral.TabIndex = 1;
            // 
            // panelComposicion
            // 
            panelComposicion.AutoScroll = true;
            panelComposicion.BackColor = Color.FromArgb(255, 255, 255);
            panelComposicion.Controls.Add(editorComposicion);
            panelComposicion.Dock = DockStyle.Fill;
            panelComposicion.Font = new Font("Segoe UI", 10F);
            panelComposicion.ForeColor = Color.FromArgb(30, 45, 65);
            panelComposicion.Location = new Point(834, 0);
            panelComposicion.Margin = new Padding(0);
            panelComposicion.Name = "panelComposicion";
            panelComposicion.Size = new Size(358, 496);
            panelComposicion.TabIndex = 2;
            // 
            // editorComposicion
            // 
            editorComposicion.BackColor = Color.FromArgb(255, 255, 255);
            editorComposicion.ColumnCount = 1;
            editorComposicion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            editorComposicion.Controls.Add(label14, 0, 0);
            editorComposicion.Controls.Add(label15, 0, 1);
            editorComposicion.Controls.Add(cbRolDestino, 0, 2);
            editorComposicion.Controls.Add(label16, 0, 3);
            editorComposicion.Controls.Add(btnAsignarARol, 0, 4);
            editorComposicion.Controls.Add(btnQuitarDeRol, 0, 5);
            editorComposicion.Controls.Add(btnEditarFamilia, 0, 6);
            editorComposicion.Controls.Add(label17, 0, 7);
            editorComposicion.Dock = DockStyle.Top;
            editorComposicion.Font = new Font("Segoe UI", 10F);
            editorComposicion.ForeColor = Color.FromArgb(30, 45, 65);
            editorComposicion.Location = new Point(0, 0);
            editorComposicion.Name = "editorComposicion";
            editorComposicion.Padding = new Padding(12);
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.RowStyles.Add(new RowStyle());
            editorComposicion.Size = new Size(341, 1076);
            editorComposicion.TabIndex = 0;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.BackColor = Color.FromArgb(255, 255, 255);
            label14.Dock = DockStyle.Fill;
            label14.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label14.ForeColor = Color.FromArgb(30, 45, 65);
            label14.Location = new Point(12, 12);
            label14.Margin = new Padding(0, 0, 0, 8);
            label14.Name = "label14";
            label14.Size = new Size(317, 25);
            label14.TabIndex = 0;
            label14.Tag = "composicion_rol";
            label14.Text = "Composición del rol";
            // 
            // label15
            // 
            label15.AutoSize = true;
            label15.BackColor = Color.FromArgb(255, 255, 255);
            label15.Dock = DockStyle.Fill;
            label15.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            label15.ForeColor = Color.FromArgb(30, 45, 65);
            label15.Location = new Point(12, 45);
            label15.Margin = new Padding(0, 0, 0, 6);
            label15.Name = "label15";
            label15.Size = new Size(317, 19);
            label15.TabIndex = 1;
            label15.Tag = "rol_destino";
            label15.Text = "Rol destino";
            // 
            // cbRolDestino
            // 
            cbRolDestino.BackColor = Color.FromArgb(255, 255, 255);
            cbRolDestino.Dock = DockStyle.Fill;
            cbRolDestino.DropDownStyle = ComboBoxStyle.DropDownList;
            cbRolDestino.Font = new Font("Segoe UI", 10F);
            cbRolDestino.ForeColor = Color.FromArgb(0, 0, 0);
            cbRolDestino.ItemHeight = 17;
            cbRolDestino.Location = new Point(12, 70);
            cbRolDestino.Margin = new Padding(0, 0, 0, 6);
            cbRolDestino.Name = "cbRolDestino";
            cbRolDestino.Size = new Size(317, 25);
            cbRolDestino.TabIndex = 2;
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.BackColor = Color.FromArgb(255, 255, 255);
            label16.Dock = DockStyle.Fill;
            label16.Font = new Font("Segoe UI", 9F);
            label16.ForeColor = Color.FromArgb(94, 111, 133);
            label16.Location = new Point(12, 103);
            label16.Margin = new Padding(0, 0, 0, 6);
            label16.Name = "label16";
            label16.Size = new Size(317, 15);
            label16.TabIndex = 3;
            label16.Tag = "ayuda_composicion_destino";
            label16.Text = "Seleccioná un rol o permiso y elegí el destino.";
            // 
            // btnAsignarARol
            // 
            btnAsignarARol.AutoSize = true;
            btnAsignarARol.BackColor = Color.FromArgb(35, 101, 175);
            btnAsignarARol.Dock = DockStyle.Fill;
            btnAsignarARol.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnAsignarARol.FlatAppearance.BorderSize = 0;
            btnAsignarARol.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnAsignarARol.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnAsignarARol.FlatStyle = FlatStyle.Flat;
            btnAsignarARol.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAsignarARol.ForeColor = Color.FromArgb(255, 255, 255);
            btnAsignarARol.Location = new Point(12, 124);
            btnAsignarARol.Margin = new Padding(0, 0, 0, 4);
            btnAsignarARol.MinimumSize = new Size(96, 34);
            btnAsignarARol.Name = "btnAsignarARol";
            btnAsignarARol.Size = new Size(317, 34);
            btnAsignarARol.TabIndex = 4;
            btnAsignarARol.Tag = "asignar_componente";
            btnAsignarARol.Text = "Asignar componente";
            btnAsignarARol.UseVisualStyleBackColor = false;
            btnAsignarARol.Click += AsignarComponente_Click;
            // 
            // btnQuitarDeRol
            // 
            btnQuitarDeRol.AutoSize = true;
            btnQuitarDeRol.BackColor = Color.FromArgb(255, 255, 255);
            btnQuitarDeRol.Dock = DockStyle.Fill;
            btnQuitarDeRol.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnQuitarDeRol.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnQuitarDeRol.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnQuitarDeRol.FlatStyle = FlatStyle.Flat;
            btnQuitarDeRol.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnQuitarDeRol.ForeColor = Color.FromArgb(30, 45, 65);
            btnQuitarDeRol.Location = new Point(12, 162);
            btnQuitarDeRol.Margin = new Padding(0, 0, 0, 4);
            btnQuitarDeRol.MinimumSize = new Size(96, 34);
            btnQuitarDeRol.Name = "btnQuitarDeRol";
            btnQuitarDeRol.Size = new Size(317, 34);
            btnQuitarDeRol.TabIndex = 5;
            btnQuitarDeRol.Tag = "quitar_componente";
            btnQuitarDeRol.Text = "Quitar componente";
            btnQuitarDeRol.UseVisualStyleBackColor = false;
            btnQuitarDeRol.Click += QuitarComponente_Click;
            // 
            // btnEditarFamilia
            // 
            btnEditarFamilia.AutoSize = true;
            btnEditarFamilia.BackColor = Color.FromArgb(255, 255, 255);
            btnEditarFamilia.Dock = DockStyle.Fill;
            btnEditarFamilia.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnEditarFamilia.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnEditarFamilia.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnEditarFamilia.FlatStyle = FlatStyle.Flat;
            btnEditarFamilia.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEditarFamilia.ForeColor = Color.FromArgb(30, 45, 65);
            btnEditarFamilia.Location = new Point(12, 200);
            btnEditarFamilia.Margin = new Padding(0, 0, 0, 4);
            btnEditarFamilia.MinimumSize = new Size(96, 34);
            btnEditarFamilia.Name = "btnEditarFamilia";
            btnEditarFamilia.Size = new Size(317, 34);
            btnEditarFamilia.TabIndex = 6;
            btnEditarFamilia.Tag = "editar_familia";
            btnEditarFamilia.Text = "Editar familia de permisos";
            btnEditarFamilia.UseVisualStyleBackColor = false;
            btnEditarFamilia.Click += EditarFamilia_Click;
            // 
            // label17
            // 
            label17.AutoSize = true;
            label17.BackColor = Color.FromArgb(255, 255, 255);
            label17.Dock = DockStyle.Fill;
            label17.Font = new Font("Segoe UI", 9F);
            label17.ForeColor = Color.FromArgb(94, 111, 133);
            label17.Location = new Point(12, 238);
            label17.Margin = new Padding(0, 0, 0, 6);
            label17.Name = "label17";
            label17.Size = new Size(317, 820);
            label17.TabIndex = 7;
            label17.Tag = "ayuda_administrador_protegido";
            label17.Text = "El rol Administrador está protegido.";
            // 
            // panelAccesos
            // 
            panelAccesos.BackColor = Color.FromArgb(255, 255, 255);
            panelAccesos.ColumnCount = 1;
            panelAccesos.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelAccesos.Controls.Add(label19, 0, 0);
            panelAccesos.Controls.Add(cbUsuarios, 0, 1);
            panelAccesos.Controls.Add(flowlayoutpanel20, 0, 2);
            panelAccesos.Controls.Add(label21, 0, 3);
            panelAccesos.Controls.Add(tvUsuarioPermisos, 0, 4);
            panelAccesos.Dock = DockStyle.Fill;
            panelAccesos.Font = new Font("Segoe UI", 10F);
            panelAccesos.ForeColor = Color.FromArgb(30, 45, 65);
            panelAccesos.Location = new Point(405, 0);
            panelAccesos.Margin = new Padding(0, 0, 16, 0);
            panelAccesos.Name = "panelAccesos";
            panelAccesos.Padding = new Padding(18);
            panelAccesos.RowCount = 5;
            layoutRoles.SetRowSpan(panelAccesos, 2);
            panelAccesos.RowStyles.Add(new RowStyle());
            panelAccesos.RowStyles.Add(new RowStyle());
            panelAccesos.RowStyles.Add(new RowStyle());
            panelAccesos.RowStyles.Add(new RowStyle());
            panelAccesos.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panelAccesos.Size = new Size(413, 585);
            panelAccesos.TabIndex = 3;
            // 
            // label19
            // 
            label19.AutoSize = true;
            label19.BackColor = Color.FromArgb(255, 255, 255);
            label19.Dock = DockStyle.Fill;
            label19.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            label19.ForeColor = Color.FromArgb(30, 45, 65);
            label19.Location = new Point(18, 18);
            label19.Margin = new Padding(0, 0, 0, 14);
            label19.Name = "label19";
            label19.Size = new Size(377, 25);
            label19.TabIndex = 0;
            label19.Tag = "accesos_usuario";
            label19.Text = "Accesos del usuario";
            // 
            // cbUsuarios
            // 
            cbUsuarios.BackColor = Color.FromArgb(255, 255, 255);
            cbUsuarios.Dock = DockStyle.Fill;
            cbUsuarios.DropDownStyle = ComboBoxStyle.DropDownList;
            cbUsuarios.Font = new Font("Segoe UI", 10F);
            cbUsuarios.ForeColor = Color.FromArgb(0, 0, 0);
            cbUsuarios.FormattingEnabled = true;
            cbUsuarios.ItemHeight = 17;
            cbUsuarios.Location = new Point(18, 57);
            cbUsuarios.Margin = new Padding(0, 0, 0, 14);
            cbUsuarios.Name = "cbUsuarios";
            cbUsuarios.Size = new Size(377, 25);
            cbUsuarios.TabIndex = 1;
            // 
            // flowlayoutpanel20
            // 
            flowlayoutpanel20.AutoSize = true;
            flowlayoutpanel20.BackColor = Color.FromArgb(255, 255, 255);
            flowlayoutpanel20.Controls.Add(btnAsignarAUsuario);
            flowlayoutpanel20.Controls.Add(btnQuitarAUsuario);
            flowlayoutpanel20.Dock = DockStyle.Top;
            flowlayoutpanel20.Font = new Font("Segoe UI", 10F);
            flowlayoutpanel20.ForeColor = Color.FromArgb(30, 45, 65);
            flowlayoutpanel20.Location = new Point(18, 96);
            flowlayoutpanel20.Margin = new Padding(0);
            flowlayoutpanel20.Name = "flowlayoutpanel20";
            flowlayoutpanel20.Size = new Size(377, 38);
            flowlayoutpanel20.TabIndex = 2;
            // 
            // btnAsignarAUsuario
            // 
            btnAsignarAUsuario.AutoSize = true;
            btnAsignarAUsuario.BackColor = Color.FromArgb(35, 101, 175);
            btnAsignarAUsuario.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnAsignarAUsuario.FlatAppearance.BorderSize = 0;
            btnAsignarAUsuario.FlatAppearance.MouseDownBackColor = Color.FromArgb(23, 69, 126);
            btnAsignarAUsuario.FlatAppearance.MouseOverBackColor = Color.FromArgb(28, 82, 146);
            btnAsignarAUsuario.FlatStyle = FlatStyle.Flat;
            btnAsignarAUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnAsignarAUsuario.ForeColor = Color.FromArgb(255, 255, 255);
            btnAsignarAUsuario.Location = new Point(0, 0);
            btnAsignarAUsuario.Margin = new Padding(0, 0, 0, 4);
            btnAsignarAUsuario.MinimumSize = new Size(96, 34);
            btnAsignarAUsuario.Name = "btnAsignarAUsuario";
            btnAsignarAUsuario.Size = new Size(126, 34);
            btnAsignarAUsuario.TabIndex = 0;
            btnAsignarAUsuario.Tag = "asignar_rol_usuario";
            btnAsignarAUsuario.Text = "Asignar rol";
            btnAsignarAUsuario.UseVisualStyleBackColor = false;
            btnAsignarAUsuario.Click += AsignarUsuario_Click;
            // 
            // btnQuitarAUsuario
            // 
            btnQuitarAUsuario.AutoSize = true;
            btnQuitarAUsuario.BackColor = Color.FromArgb(255, 255, 255);
            btnQuitarAUsuario.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnQuitarAUsuario.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnQuitarAUsuario.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnQuitarAUsuario.FlatStyle = FlatStyle.Flat;
            btnQuitarAUsuario.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnQuitarAUsuario.ForeColor = Color.FromArgb(30, 45, 65);
            btnQuitarAUsuario.Location = new Point(126, 0);
            btnQuitarAUsuario.Margin = new Padding(0, 0, 0, 4);
            btnQuitarAUsuario.MinimumSize = new Size(96, 34);
            btnQuitarAUsuario.Name = "btnQuitarAUsuario";
            btnQuitarAUsuario.Size = new Size(126, 34);
            btnQuitarAUsuario.TabIndex = 1;
            btnQuitarAUsuario.Tag = "quitar_rol_usuario";
            btnQuitarAUsuario.Text = "Quitar rol";
            btnQuitarAUsuario.UseVisualStyleBackColor = false;
            btnQuitarAUsuario.Click += QuitarUsuario_Click;
            // 
            // label21
            // 
            label21.AutoSize = true;
            label21.BackColor = Color.FromArgb(255, 255, 255);
            label21.Dock = DockStyle.Fill;
            label21.Font = new Font("Segoe UI", 9F);
            label21.ForeColor = Color.FromArgb(94, 111, 133);
            label21.Location = new Point(18, 134);
            label21.Margin = new Padding(0, 0, 0, 6);
            label21.Name = "label21";
            label21.Size = new Size(377, 30);
            label21.TabIndex = 3;
            label21.Tag = "ayuda_asignacion";
            label21.Text = "Para asignar, seleccioná un rol a la izquierda. Para quitar, seleccioná el rol asignado en este panel.";
            // 
            // tvUsuarioPermisos
            // 
            tvUsuarioPermisos.BackColor = Color.FromArgb(255, 255, 255);
            tvUsuarioPermisos.BorderStyle = BorderStyle.None;
            tvUsuarioPermisos.Dock = DockStyle.Fill;
            tvUsuarioPermisos.Font = new Font("Segoe UI", 10F);
            tvUsuarioPermisos.ForeColor = Color.FromArgb(30, 45, 65);
            tvUsuarioPermisos.FullRowSelect = true;
            tvUsuarioPermisos.HideSelection = false;
            tvUsuarioPermisos.Indent = 22;
            tvUsuarioPermisos.ItemHeight = 32;
            tvUsuarioPermisos.Location = new Point(21, 173);
            tvUsuarioPermisos.Name = "tvUsuarioPermisos";
            tvUsuarioPermisos.ShowLines = false;
            tvUsuarioPermisos.ShowNodeToolTips = true;
            tvUsuarioPermisos.Size = new Size(371, 391);
            tvUsuarioPermisos.TabIndex = 4;
            // 
            // lblEstado
            // 
            lblEstado.AutoSize = true;
            lblEstado.BackColor = Color.FromArgb(231, 240, 252);
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = new Font("Segoe UI", 10F);
            lblEstado.ForeColor = Color.FromArgb(35, 101, 175);
            lblEstado.Location = new Point(24, 713);
            lblEstado.Margin = new Padding(0, 16, 0, 0);
            lblEstado.Name = "lblEstado";
            lblEstado.Padding = new Padding(14, 12, 14, 12);
            lblEstado.Size = new Size(1192, 43);
            lblEstado.TabIndex = 2;
            lblEstado.Text = "Los cambios se guardan automáticamente.";
            // 
            // FormGestionRoles
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1240, 780);
            Controls.Add(layoutPrincipal);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MinimumSize = new Size(1000, 650);
            Name = "FormGestionRoles";
            StartPosition = FormStartPosition.CenterParent;
            Tag = "gestion_roles_titulo";
            Text = "Gestión de roles y permisos";
            layoutPrincipal.ResumeLayout(false);
            layoutPrincipal.PerformLayout();
            tablelayoutpanel2.ResumeLayout(false);
            tablelayoutpanel2.PerformLayout();
            layoutRoles.ResumeLayout(false);
            panel6.ResumeLayout(false);
            tablelayoutpanel7.ResumeLayout(false);
            tablelayoutpanel7.PerformLayout();
            panel9.ResumeLayout(false);
            tablelayoutpanel10.ResumeLayout(false);
            tablelayoutpanel10.PerformLayout();
            panelComposicion.ResumeLayout(false);
            editorComposicion.ResumeLayout(false);
            editorComposicion.PerformLayout();
            panelAccesos.ResumeLayout(false);
            panelAccesos.PerformLayout();
            flowlayoutpanel20.ResumeLayout(false);
            flowlayoutpanel20.PerformLayout();
            ResumeLayout(false);
        }
    }
}
