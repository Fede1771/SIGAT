namespace SIGAT.UI
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components;
        private TableLayoutPanel marcoPrincipal;
        private Label label2;
        private TableLayoutPanel encabezadoPrincipal;
        private Label lblSeccion;
        private Label lblCuenta;
        private Button btnTemaVisual;
        private Button btnIdiomaVisual;
        private TableLayoutPanel barraLateral;
        private TableLayoutPanel cabeceraMenu;
        private Button btnAlternarMenu;
        private ToolTip ayudaMenu;
        private TableLayoutPanel panelNavegacion;
        private BotonNavegacion navInicio;
        private BotonNavegacion navFrmGestionUsuarios;
        private BotonNavegacion navFormGestionRoles;
        private BotonNavegacion navFrmGestionIdiomas;
        private BotonNavegacion navFrmBitacora;
        private BotonNavegacion navFrmIntegridad;
        private Label lblRoles;
        private BotonNavegacion navSalir;
        private Panel panelContenido;
        private TableLayoutPanel panelInicio;
        private TableLayoutPanel encabezadoInicio;
        private Label label8;
        private Label label9;
        private FlowLayoutPanel tarjetasInicio;
        private BotonNavegacion inicioFrmGestionUsuarios;
        private BotonNavegacion inicioFormGestionRoles;
        private BotonNavegacion inicioFrmGestionIdiomas;
        private BotonNavegacion inicioFrmBitacora;
        private BotonNavegacion inicioFrmIntegridad;
        private MenuStrip menu;
        private ContextMenuStrip selectorIdioma;
        private ToolStripMenuItem itemSistema;
        private ToolStripMenuItem itemUsuarios;
        private ToolStripMenuItem itemBitacora;
        private ToolStripMenuItem itemLogout;
        private ToolStripMenuItem itemIdioma;
        private ToolStripMenuItem itemGestionIdiomas;
        private ToolStripMenuItem itemRoles;
        private ToolStripMenuItem itemIntegridad;

        protected override void Dispose(bool disposing)
        {
            if (disposing) components?.Dispose();
            base.Dispose(disposing);
        }

        // Estructura visual editable desde el diseñador; el comportamiento queda en el .cs.
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            menu = new MenuStrip();
            itemSistema = new ToolStripMenuItem();
            itemUsuarios = new ToolStripMenuItem();
            itemBitacora = new ToolStripMenuItem();
            itemLogout = new ToolStripMenuItem();
            itemRoles = new ToolStripMenuItem();
            itemIntegridad = new ToolStripMenuItem();
            itemIdioma = new ToolStripMenuItem();
            itemGestionIdiomas = new ToolStripMenuItem();
            marcoPrincipal = new TableLayoutPanel();
            label2 = new Label();
            encabezadoPrincipal = new TableLayoutPanel();
            lblSeccion = new Label();
            lblCuenta = new Label();
            btnTemaVisual = new Button();
            btnIdiomaVisual = new Button();
            barraLateral = new TableLayoutPanel();
            cabeceraMenu = new TableLayoutPanel { Name = "cabeceraMenu", Dock = DockStyle.Fill, Margin = Padding.Empty, ColumnCount = 2, RowCount = 1, BackColor = Color.White };
            cabeceraMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            cabeceraMenu.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 44F));
            cabeceraMenu.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            btnAlternarMenu = new Button { Name = "btnAlternarMenu", Text = "☰", Dock = DockStyle.Fill, FlatStyle = FlatStyle.Flat,
                Margin = new Padding(2,12,4,12), Font = new Font("Segoe UI", 15F), AccessibleName = "Contraer menú", BackColor = Color.White };
            btnAlternarMenu.FlatAppearance.BorderSize = 0;
            ayudaMenu = new ToolTip(components);
            cabeceraMenu.Controls.Add(label2, 0, 0);
            cabeceraMenu.Controls.Add(btnAlternarMenu, 1, 0);
            panelNavegacion = new TableLayoutPanel();
            navInicio = new BotonNavegacion();
            navFrmGestionUsuarios = new BotonNavegacion();
            navFormGestionRoles = new BotonNavegacion();
            navFrmGestionIdiomas = new BotonNavegacion();
            navFrmBitacora = new BotonNavegacion();
            navFrmIntegridad = new BotonNavegacion();
            lblRoles = new Label();
            navSalir = new BotonNavegacion();
            panelContenido = new Panel();
            panelInicio = new TableLayoutPanel();
            encabezadoInicio = new TableLayoutPanel();
            label8 = new Label();
            label9 = new Label();
            tarjetasInicio = new FlowLayoutPanel();
            inicioFrmGestionUsuarios = new BotonNavegacion();
            inicioFormGestionRoles = new BotonNavegacion();
            inicioFrmGestionIdiomas = new BotonNavegacion();
            inicioFrmBitacora = new BotonNavegacion();
            inicioFrmIntegridad = new BotonNavegacion();
            selectorIdioma = new ContextMenuStrip(components);
            menu.SuspendLayout();
            marcoPrincipal.SuspendLayout();
            encabezadoPrincipal.SuspendLayout();
            barraLateral.SuspendLayout();
            panelNavegacion.SuspendLayout();
            panelContenido.SuspendLayout();
            panelInicio.SuspendLayout();
            encabezadoInicio.SuspendLayout();
            tarjetasInicio.SuspendLayout();
            SuspendLayout();
            // 
            // menu
            // 
            menu.Items.AddRange(new ToolStripItem[] { itemSistema, itemIdioma });
            menu.Location = new Point(0, 0);
            menu.Name = "menu";
            menu.Size = new Size(200, 24);
            menu.TabIndex = 0;
            menu.Visible = false;
            // 
            // itemSistema
            // 
            itemSistema.DropDownItems.AddRange(new ToolStripItem[] { itemUsuarios, itemBitacora, itemLogout, itemRoles, itemIntegridad });
            itemSistema.Name = "itemSistema";
            itemSistema.Size = new Size(60, 20);
            itemSistema.Tag = "menu_sistema";
            itemSistema.Text = "Sistema";
            // 
            // itemUsuarios
            // 
            itemUsuarios.Name = "itemUsuarios";
            itemUsuarios.Size = new Size(223, 22);
            itemUsuarios.Tag = "menu_usuarios";
            itemUsuarios.Text = "Gestión de Usuarios";
            itemUsuarios.Click += ItemUsuarios_Click;
            // 
            // itemBitacora
            // 
            itemBitacora.Name = "itemBitacora";
            itemBitacora.Size = new Size(223, 22);
            itemBitacora.Tag = "menu_bitacora";
            itemBitacora.Text = "Bitácora";
            itemBitacora.Click += ItemBitacora_Click;
            // 
            // itemLogout
            // 
            itemLogout.Name = "itemLogout";
            itemLogout.Size = new Size(223, 22);
            itemLogout.Tag = "menu_logout";
            itemLogout.Text = "Cerrar Sesión";
            itemLogout.Click += ItemLogout_Click;
            // 
            // itemRoles
            // 
            itemRoles.Name = "itemRoles";
            itemRoles.Size = new Size(223, 22);
            itemRoles.Text = "Gestión de roles y permisos";
            itemRoles.Click += ItemRoles_Click;
            // 
            // itemIntegridad
            // 
            itemIntegridad.Name = "itemIntegridad";
            itemIntegridad.Size = new Size(223, 22);
            itemIntegridad.Text = "Integridad y recuperación";
            itemIntegridad.Click += ItemIntegridad_Click;
            // 
            // itemIdioma
            // 
            itemIdioma.DropDownItems.AddRange(new ToolStripItem[] { itemGestionIdiomas });
            itemIdioma.Name = "itemIdioma";
            itemIdioma.Size = new Size(56, 20);
            itemIdioma.Tag = "menu_idioma";
            itemIdioma.Text = "Idioma";
            // 
            // itemGestionIdiomas
            // 
            itemGestionIdiomas.Name = "itemGestionIdiomas";
            itemGestionIdiomas.Size = new Size(178, 22);
            itemGestionIdiomas.Text = "Gestionar idiomas...";
            itemGestionIdiomas.Click += ItemGestionIdiomas_Click;
            // 
            // marcoPrincipal
            // 
            marcoPrincipal.BackColor = Color.FromArgb(245, 247, 251);
            marcoPrincipal.ColumnCount = 2;
            marcoPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 236F));
            marcoPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            marcoPrincipal.Controls.Add(cabeceraMenu, 0, 0);
            marcoPrincipal.Controls.Add(encabezadoPrincipal, 1, 0);
            marcoPrincipal.Controls.Add(barraLateral, 0, 1);
            marcoPrincipal.Controls.Add(panelContenido, 1, 1);
            marcoPrincipal.Dock = DockStyle.Fill;
            marcoPrincipal.Font = new Font("Segoe UI", 10F);
            marcoPrincipal.ForeColor = Color.FromArgb(30, 45, 65);
            marcoPrincipal.Location = new Point(0, 0);
            marcoPrincipal.Margin = new Padding(0);
            marcoPrincipal.Name = "marcoPrincipal";
            marcoPrincipal.RowCount = 2;
            marcoPrincipal.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            marcoPrincipal.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            marcoPrincipal.Size = new Size(1084, 681);
            marcoPrincipal.TabIndex = 0;
            // 
            // label2
            // 
            label2.BackColor = Color.FromArgb(255, 255, 255);
            label2.Dock = DockStyle.Fill;
            label2.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            label2.ForeColor = Color.FromArgb(35, 101, 175);
            label2.Location = new Point(0, 0);
            label2.Margin = new Padding(0);
            label2.Name = "label2";
            label2.Padding = new Padding(26, 0, 0, 0);
            label2.Size = new Size(236, 76);
            label2.TabIndex = 0;
            label2.Text = "SIGAT";
            label2.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // encabezadoPrincipal
            // 
            encabezadoPrincipal.BackColor = Color.FromArgb(255, 255, 255);
            encabezadoPrincipal.ColumnCount = 4;
            encabezadoPrincipal.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            encabezadoPrincipal.ColumnStyles.Add(new ColumnStyle());
            encabezadoPrincipal.ColumnStyles.Add(new ColumnStyle());
            encabezadoPrincipal.ColumnStyles.Add(new ColumnStyle());
            encabezadoPrincipal.Controls.Add(lblSeccion, 0, 0);
            encabezadoPrincipal.Controls.Add(lblCuenta, 1, 0);
            encabezadoPrincipal.Controls.Add(btnTemaVisual, 2, 0);
            encabezadoPrincipal.Controls.Add(btnIdiomaVisual, 3, 0);
            encabezadoPrincipal.Dock = DockStyle.Fill;
            encabezadoPrincipal.Font = new Font("Segoe UI", 10F);
            encabezadoPrincipal.ForeColor = Color.FromArgb(30, 45, 65);
            encabezadoPrincipal.Location = new Point(236, 0);
            encabezadoPrincipal.Margin = new Padding(0);
            encabezadoPrincipal.Name = "encabezadoPrincipal";
            encabezadoPrincipal.Padding = new Padding(24, 12, 24, 12);
            encabezadoPrincipal.RowStyles.Add(new RowStyle());
            encabezadoPrincipal.Size = new Size(848, 76);
            encabezadoPrincipal.TabIndex = 1;
            // 
            // lblSeccion
            // 
            lblSeccion.AutoEllipsis = true;
            lblSeccion.BackColor = Color.FromArgb(255, 255, 255);
            lblSeccion.Dock = DockStyle.Fill;
            lblSeccion.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lblSeccion.ForeColor = Color.FromArgb(30, 45, 65);
            lblSeccion.Location = new Point(27, 12);
            lblSeccion.Name = "lblSeccion";
            lblSeccion.Size = new Size(554, 52);
            lblSeccion.TabIndex = 0;
            lblSeccion.Text = "Inicio";
            lblSeccion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblCuenta
            // 
            lblCuenta.AutoSize = true;
            lblCuenta.BackColor = Color.FromArgb(255, 255, 255);
            lblCuenta.Dock = DockStyle.Fill;
            lblCuenta.Font = new Font("Segoe UI", 10F);
            lblCuenta.ForeColor = Color.FromArgb(94, 111, 133);
            lblCuenta.Location = new Point(587, 12);
            lblCuenta.Name = "lblCuenta";
            lblCuenta.Padding = new Padding(0, 0, 24, 0);
            lblCuenta.Size = new Size(24, 52);
            lblCuenta.TabIndex = 1;
            lblCuenta.TextAlign = ContentAlignment.MiddleRight;
            // 
            // btnTemaVisual
            // 
            btnTemaVisual.AutoSize = true;
            btnTemaVisual.BackColor = Color.FromArgb(255, 255, 255);
            btnTemaVisual.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnTemaVisual.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnTemaVisual.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnTemaVisual.FlatStyle = FlatStyle.Flat;
            btnTemaVisual.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnTemaVisual.ForeColor = Color.FromArgb(30, 45, 65);
            btnTemaVisual.Location = new Point(614, 12);
            btnTemaVisual.Margin = new Padding(0, 0, 12, 0);
            btnTemaVisual.MinimumSize = new Size(96, 40);
            btnTemaVisual.Name = "btnTemaVisual";
            btnTemaVisual.Size = new Size(102, 40);
            btnTemaVisual.TabIndex = 2;
            btnTemaVisual.Text = "Modo claro";
            btnTemaVisual.UseVisualStyleBackColor = false;
            // 
            // btnIdiomaVisual
            // 
            btnIdiomaVisual.AutoSize = true;
            btnIdiomaVisual.BackColor = Color.FromArgb(255, 255, 255);
            btnIdiomaVisual.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            btnIdiomaVisual.FlatAppearance.MouseDownBackColor = Color.FromArgb(215, 230, 250);
            btnIdiomaVisual.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            btnIdiomaVisual.FlatStyle = FlatStyle.Flat;
            btnIdiomaVisual.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnIdiomaVisual.ForeColor = Color.FromArgb(30, 45, 65);
            btnIdiomaVisual.Location = new Point(728, 12);
            btnIdiomaVisual.Margin = new Padding(0);
            btnIdiomaVisual.MinimumSize = new Size(96, 40);
            btnIdiomaVisual.Name = "btnIdiomaVisual";
            btnIdiomaVisual.Size = new Size(96, 40);
            btnIdiomaVisual.TabIndex = 3;
            btnIdiomaVisual.Tag = "menu_idioma";
            btnIdiomaVisual.Text = "Idioma";
            btnIdiomaVisual.UseVisualStyleBackColor = false;
            // 
            // barraLateral
            // 
            barraLateral.BackColor = Color.FromArgb(255, 255, 255);
            barraLateral.ColumnCount = 1;
            barraLateral.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            barraLateral.Controls.Add(panelNavegacion, 0, 0);
            barraLateral.Controls.Add(lblRoles, 0, 1);
            barraLateral.Controls.Add(navSalir, 0, 2);
            barraLateral.Dock = DockStyle.Fill;
            barraLateral.Font = new Font("Segoe UI", 10F);
            barraLateral.ForeColor = Color.FromArgb(30, 45, 65);
            barraLateral.Location = new Point(0, 76);
            barraLateral.Margin = new Padding(0);
            barraLateral.Name = "barraLateral";
            barraLateral.Padding = new Padding(12, 20, 12, 16);
            barraLateral.RowCount = 3;
            barraLateral.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            barraLateral.RowStyles.Add(new RowStyle());
            barraLateral.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
            barraLateral.Size = new Size(236, 605);
            barraLateral.TabIndex = 2;
            // 
            // panelNavegacion
            // 
            panelNavegacion.AutoSize = true;
            panelNavegacion.BackColor = Color.FromArgb(255, 255, 255);
            panelNavegacion.ColumnCount = 1;
            panelNavegacion.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelNavegacion.Controls.Add(navInicio, 0, 0);
            panelNavegacion.Controls.Add(navFrmGestionUsuarios, 0, 1);
            panelNavegacion.Controls.Add(navFormGestionRoles, 0, 2);
            panelNavegacion.Controls.Add(navFrmGestionIdiomas, 0, 3);
            panelNavegacion.Controls.Add(navFrmBitacora, 0, 4);
            panelNavegacion.Controls.Add(navFrmIntegridad, 0, 5);
            panelNavegacion.Dock = DockStyle.Top;
            panelNavegacion.Font = new Font("Segoe UI", 10F);
            panelNavegacion.ForeColor = Color.FromArgb(30, 45, 65);
            panelNavegacion.Location = new Point(12, 20);
            panelNavegacion.Margin = new Padding(0);
            panelNavegacion.Name = "panelNavegacion";
            panelNavegacion.RowCount = 6;
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            panelNavegacion.Size = new Size(212, 360);
            panelNavegacion.TabIndex = 0;
            // 
            // navInicio
            // 
            navInicio.BackColor = Color.White;
            navInicio.Dock = DockStyle.Fill;
            navInicio.FlatAppearance.BorderSize = 0;
            navInicio.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navInicio.FlatStyle = FlatStyle.Flat;
            navInicio.Font = new Font("Segoe UI", 10F);
            navInicio.ForeColor = Color.FromArgb(30, 45, 65);
            navInicio.Location = new Point(0, 0);
            navInicio.Margin = new Padding(0, 0, 0, 6);
            navInicio.Name = "navInicio";
            navInicio.Padding = new Padding(44, 0, 10, 0);
            navInicio.Size = new Size(212, 54);
            navInicio.TabIndex = 0;
            navInicio.Tag = "menu_inicio";
            navInicio.Text = "Inicio";
            navInicio.TextAlign = ContentAlignment.MiddleLeft;
            navInicio.UseVisualStyleBackColor = false;
            // 
            // navFrmGestionUsuarios
            // 
            navFrmGestionUsuarios.BackColor = Color.White;
            navFrmGestionUsuarios.Dock = DockStyle.Fill;
            navFrmGestionUsuarios.FlatAppearance.BorderSize = 0;
            navFrmGestionUsuarios.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navFrmGestionUsuarios.FlatStyle = FlatStyle.Flat;
            navFrmGestionUsuarios.Font = new Font("Segoe UI", 10F);
            navFrmGestionUsuarios.ForeColor = Color.FromArgb(30, 45, 65);
            navFrmGestionUsuarios.Icono = IconoVisual.Usuarios;
            navFrmGestionUsuarios.Location = new Point(0, 60);
            navFrmGestionUsuarios.Margin = new Padding(0, 0, 0, 6);
            navFrmGestionUsuarios.Name = "navFrmGestionUsuarios";
            navFrmGestionUsuarios.Padding = new Padding(44, 0, 10, 0);
            navFrmGestionUsuarios.Size = new Size(212, 54);
            navFrmGestionUsuarios.TabIndex = 1;
            navFrmGestionUsuarios.Tag = "menu_usuarios";
            navFrmGestionUsuarios.Text = "Gestión de Usuarios";
            navFrmGestionUsuarios.TextAlign = ContentAlignment.MiddleLeft;
            navFrmGestionUsuarios.UseVisualStyleBackColor = false;
            // 
            // navFormGestionRoles
            // 
            navFormGestionRoles.BackColor = Color.White;
            navFormGestionRoles.Dock = DockStyle.Fill;
            navFormGestionRoles.FlatAppearance.BorderSize = 0;
            navFormGestionRoles.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navFormGestionRoles.FlatStyle = FlatStyle.Flat;
            navFormGestionRoles.Font = new Font("Segoe UI", 10F);
            navFormGestionRoles.ForeColor = Color.FromArgb(30, 45, 65);
            navFormGestionRoles.Icono = IconoVisual.Roles;
            navFormGestionRoles.Location = new Point(0, 120);
            navFormGestionRoles.Margin = new Padding(0, 0, 0, 6);
            navFormGestionRoles.Name = "navFormGestionRoles";
            navFormGestionRoles.Padding = new Padding(44, 0, 10, 0);
            navFormGestionRoles.Size = new Size(212, 54);
            navFormGestionRoles.TabIndex = 2;
            navFormGestionRoles.Tag = "menu_roles";
            navFormGestionRoles.Text = "Gestión de roles y permisos";
            navFormGestionRoles.TextAlign = ContentAlignment.MiddleLeft;
            navFormGestionRoles.UseVisualStyleBackColor = false;
            // 
            // navFrmGestionIdiomas
            // 
            navFrmGestionIdiomas.BackColor = Color.White;
            navFrmGestionIdiomas.Dock = DockStyle.Fill;
            navFrmGestionIdiomas.FlatAppearance.BorderSize = 0;
            navFrmGestionIdiomas.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navFrmGestionIdiomas.FlatStyle = FlatStyle.Flat;
            navFrmGestionIdiomas.Font = new Font("Segoe UI", 10F);
            navFrmGestionIdiomas.ForeColor = Color.FromArgb(30, 45, 65);
            navFrmGestionIdiomas.Icono = IconoVisual.Idiomas;
            navFrmGestionIdiomas.Location = new Point(0, 180);
            navFrmGestionIdiomas.Margin = new Padding(0, 0, 0, 6);
            navFrmGestionIdiomas.Name = "navFrmGestionIdiomas";
            navFrmGestionIdiomas.Padding = new Padding(44, 0, 10, 0);
            navFrmGestionIdiomas.Size = new Size(212, 54);
            navFrmGestionIdiomas.TabIndex = 3;
            navFrmGestionIdiomas.Tag = "menu_gestion_idiomas";
            navFrmGestionIdiomas.Text = "Gestionar idiomas...";
            navFrmGestionIdiomas.TextAlign = ContentAlignment.MiddleLeft;
            navFrmGestionIdiomas.UseVisualStyleBackColor = false;
            // 
            // navFrmBitacora
            // 
            navFrmBitacora.BackColor = Color.White;
            navFrmBitacora.Dock = DockStyle.Fill;
            navFrmBitacora.FlatAppearance.BorderSize = 0;
            navFrmBitacora.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navFrmBitacora.FlatStyle = FlatStyle.Flat;
            navFrmBitacora.Font = new Font("Segoe UI", 10F);
            navFrmBitacora.ForeColor = Color.FromArgb(30, 45, 65);
            navFrmBitacora.Icono = IconoVisual.Bitacora;
            navFrmBitacora.Location = new Point(0, 240);
            navFrmBitacora.Margin = new Padding(0, 0, 0, 6);
            navFrmBitacora.Name = "navFrmBitacora";
            navFrmBitacora.Padding = new Padding(44, 0, 10, 0);
            navFrmBitacora.Size = new Size(212, 54);
            navFrmBitacora.TabIndex = 4;
            navFrmBitacora.Tag = "menu_bitacora";
            navFrmBitacora.Text = "Bitácora";
            navFrmBitacora.TextAlign = ContentAlignment.MiddleLeft;
            navFrmBitacora.UseVisualStyleBackColor = false;
            // 
            // navFrmIntegridad
            // 
            navFrmIntegridad.BackColor = Color.White;
            navFrmIntegridad.Dock = DockStyle.Fill;
            navFrmIntegridad.FlatAppearance.BorderSize = 0;
            navFrmIntegridad.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navFrmIntegridad.FlatStyle = FlatStyle.Flat;
            navFrmIntegridad.Font = new Font("Segoe UI", 10F);
            navFrmIntegridad.ForeColor = Color.FromArgb(30, 45, 65);
            navFrmIntegridad.Icono = IconoVisual.Roles;
            navFrmIntegridad.Location = new Point(0, 300);
            navFrmIntegridad.Margin = new Padding(0, 0, 0, 6);
            navFrmIntegridad.Name = "navFrmIntegridad";
            navFrmIntegridad.Padding = new Padding(44, 0, 10, 0);
            navFrmIntegridad.Size = new Size(212, 54);
            navFrmIntegridad.TabIndex = 5;
            navFrmIntegridad.Tag = "menu_integridad";
            navFrmIntegridad.Text = "Integridad y recuperación";
            navFrmIntegridad.TextAlign = ContentAlignment.MiddleLeft;
            navFrmIntegridad.UseVisualStyleBackColor = false;
            // 
            // lblRoles
            // 
            lblRoles.AutoEllipsis = true;
            lblRoles.AutoSize = true;
            lblRoles.BackColor = Color.FromArgb(255, 255, 255);
            lblRoles.Dock = DockStyle.Fill;
            lblRoles.Font = new Font("Segoe UI", 10F);
            lblRoles.ForeColor = Color.FromArgb(94, 111, 133);
            lblRoles.Location = new Point(12, 476);
            lblRoles.Margin = new Padding(0);
            lblRoles.MaximumSize = new Size(212, 96);
            lblRoles.Name = "lblRoles";
            lblRoles.Padding = new Padding(12, 18, 12, 18);
            lblRoles.Size = new Size(212, 55);
            lblRoles.TabIndex = 1;
            // 
            // navSalir
            // 
            navSalir.BackColor = Color.FromArgb(255, 255, 255);
            navSalir.Dock = DockStyle.Fill;
            navSalir.FlatAppearance.BorderSize = 0;
            navSalir.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            navSalir.FlatStyle = FlatStyle.Flat;
            navSalir.Font = new Font("Segoe UI", 10F);
            navSalir.ForeColor = Color.FromArgb(30, 45, 65);
            navSalir.Icono = IconoVisual.Salir;
            navSalir.Location = new Point(12, 531);
            navSalir.Margin = new Padding(0);
            navSalir.Name = "navSalir";
            navSalir.Padding = new Padding(44, 0, 10, 0);
            navSalir.Size = new Size(212, 58);
            navSalir.TabIndex = 2;
            navSalir.Tag = "menu_logout";
            navSalir.Text = "Cerrar sesión";
            navSalir.TextAlign = ContentAlignment.MiddleLeft;
            navSalir.UseVisualStyleBackColor = false;
            // 
            // panelContenido
            // 
            panelContenido.BackColor = Color.FromArgb(245, 247, 251);
            panelContenido.Controls.Add(panelInicio);
            panelContenido.Dock = DockStyle.Fill;
            panelContenido.Font = new Font("Segoe UI", 10F);
            panelContenido.ForeColor = Color.FromArgb(30, 45, 65);
            panelContenido.Location = new Point(236, 76);
            panelContenido.Margin = new Padding(0);
            panelContenido.Name = "panelContenido";
            panelContenido.Size = new Size(848, 605);
            panelContenido.TabIndex = 3;
            // 
            // panelInicio
            // 
            panelInicio.BackColor = Color.FromArgb(240, 240, 240);
            panelInicio.ColumnCount = 1;
            panelInicio.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            panelInicio.Controls.Add(encabezadoInicio, 0, 0);
            panelInicio.Controls.Add(tarjetasInicio, 0, 1);
            panelInicio.Dock = DockStyle.Fill;
            panelInicio.Font = new Font("Segoe UI", 10F);
            panelInicio.ForeColor = Color.FromArgb(0, 0, 0);
            panelInicio.Location = new Point(0, 0);
            panelInicio.Name = "panelInicio";
            panelInicio.Padding = new Padding(32);
            panelInicio.RowCount = 3;
            panelInicio.RowStyles.Add(new RowStyle());
            panelInicio.RowStyles.Add(new RowStyle());
            panelInicio.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            panelInicio.Size = new Size(848, 605);
            panelInicio.TabIndex = 0;
            // 
            // encabezadoInicio
            // 
            encabezadoInicio.AutoSize = true;
            encabezadoInicio.BackColor = Color.FromArgb(240, 240, 240);
            encabezadoInicio.ColumnCount = 1;
            encabezadoInicio.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            encabezadoInicio.Controls.Add(label8, 0, 0);
            encabezadoInicio.Controls.Add(label9, 0, 1);
            encabezadoInicio.Dock = DockStyle.Top;
            encabezadoInicio.Font = new Font("Segoe UI", 10F);
            encabezadoInicio.ForeColor = Color.FromArgb(0, 0, 0);
            encabezadoInicio.Location = new Point(32, 32);
            encabezadoInicio.Margin = new Padding(0, 0, 0, 22);
            encabezadoInicio.Name = "encabezadoInicio";
            encabezadoInicio.RowStyles.Add(new RowStyle());
            encabezadoInicio.RowStyles.Add(new RowStyle());
            encabezadoInicio.Size = new Size(784, 66);
            encabezadoInicio.TabIndex = 0;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.BackColor = Color.FromArgb(240, 240, 240);
            label8.Dock = DockStyle.Fill;
            label8.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
            label8.ForeColor = Color.FromArgb(30, 45, 65);
            label8.Location = new Point(0, 0);
            label8.Margin = new Padding(0, 0, 0, 6);
            label8.Name = "label8";
            label8.Size = new Size(784, 41);
            label8.TabIndex = 0;
            label8.Tag = "inicio_titulo";
            label8.Text = "Tu espacio de trabajo";
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.BackColor = Color.FromArgb(240, 240, 240);
            label9.Dock = DockStyle.Fill;
            label9.Font = new Font("Segoe UI", 10F);
            label9.ForeColor = Color.FromArgb(94, 111, 133);
            label9.Location = new Point(0, 47);
            label9.Margin = new Padding(0);
            label9.Name = "label9";
            label9.Size = new Size(784, 19);
            label9.TabIndex = 1;
            label9.Tag = "inicio_descripcion";
            label9.Text = "Seleccioná una sección para comenzar.";
            // 
            // tarjetasInicio
            // 
            tarjetasInicio.AutoSize = true;
            tarjetasInicio.BackColor = Color.FromArgb(240, 240, 240);
            tarjetasInicio.Controls.Add(inicioFrmGestionUsuarios);
            tarjetasInicio.Controls.Add(inicioFormGestionRoles);
            tarjetasInicio.Controls.Add(inicioFrmGestionIdiomas);
            tarjetasInicio.Controls.Add(inicioFrmBitacora);
            tarjetasInicio.Controls.Add(inicioFrmIntegridad);
            tarjetasInicio.Dock = DockStyle.Top;
            tarjetasInicio.Font = new Font("Segoe UI", 10F);
            tarjetasInicio.ForeColor = Color.FromArgb(0, 0, 0);
            tarjetasInicio.Location = new Point(32, 120);
            tarjetasInicio.Margin = new Padding(0);
            tarjetasInicio.Name = "tarjetasInicio";
            tarjetasInicio.Size = new Size(784, 312);
            tarjetasInicio.TabIndex = 1;
            // 
            // inicioFrmGestionUsuarios
            // 
            inicioFrmGestionUsuarios.BackColor = Color.White;
            inicioFrmGestionUsuarios.Dock = DockStyle.Top;
            inicioFrmGestionUsuarios.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            inicioFrmGestionUsuarios.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            inicioFrmGestionUsuarios.FlatStyle = FlatStyle.Flat;
            inicioFrmGestionUsuarios.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            inicioFrmGestionUsuarios.ForeColor = Color.FromArgb(30, 45, 65);
            inicioFrmGestionUsuarios.Icono = IconoVisual.Usuarios;
            inicioFrmGestionUsuarios.Location = new Point(0, 0);
            inicioFrmGestionUsuarios.Margin = new Padding(0, 0, 16, 16);
            inicioFrmGestionUsuarios.MinimumSize = new Size(250, 88);
            inicioFrmGestionUsuarios.Name = "inicioFrmGestionUsuarios";
            inicioFrmGestionUsuarios.Padding = new Padding(48, 0, 16, 0);
            inicioFrmGestionUsuarios.Size = new Size(250, 88);
            inicioFrmGestionUsuarios.TabIndex = 0;
            inicioFrmGestionUsuarios.Tag = "menu_usuarios";
            inicioFrmGestionUsuarios.Text = "Gestión de Usuarios";
            inicioFrmGestionUsuarios.TextAlign = ContentAlignment.MiddleLeft;
            inicioFrmGestionUsuarios.UseVisualStyleBackColor = false;
            // 
            // inicioFormGestionRoles
            // 
            inicioFormGestionRoles.BackColor = Color.White;
            inicioFormGestionRoles.Dock = DockStyle.Top;
            inicioFormGestionRoles.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            inicioFormGestionRoles.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            inicioFormGestionRoles.FlatStyle = FlatStyle.Flat;
            inicioFormGestionRoles.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            inicioFormGestionRoles.ForeColor = Color.FromArgb(30, 45, 65);
            inicioFormGestionRoles.Icono = IconoVisual.Roles;
            inicioFormGestionRoles.Location = new Point(266, 0);
            inicioFormGestionRoles.Margin = new Padding(0, 0, 16, 16);
            inicioFormGestionRoles.MinimumSize = new Size(250, 88);
            inicioFormGestionRoles.Name = "inicioFormGestionRoles";
            inicioFormGestionRoles.Padding = new Padding(48, 0, 16, 0);
            inicioFormGestionRoles.Size = new Size(250, 88);
            inicioFormGestionRoles.TabIndex = 1;
            inicioFormGestionRoles.Tag = "menu_roles";
            inicioFormGestionRoles.Text = "Gestión de roles y permisos";
            inicioFormGestionRoles.TextAlign = ContentAlignment.MiddleLeft;
            inicioFormGestionRoles.UseVisualStyleBackColor = false;
            // 
            // inicioFrmGestionIdiomas
            // 
            inicioFrmGestionIdiomas.BackColor = Color.White;
            inicioFrmGestionIdiomas.Dock = DockStyle.Top;
            inicioFrmGestionIdiomas.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            inicioFrmGestionIdiomas.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            inicioFrmGestionIdiomas.FlatStyle = FlatStyle.Flat;
            inicioFrmGestionIdiomas.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            inicioFrmGestionIdiomas.ForeColor = Color.FromArgb(30, 45, 65);
            inicioFrmGestionIdiomas.Icono = IconoVisual.Idiomas;
            inicioFrmGestionIdiomas.Location = new Point(0, 104);
            inicioFrmGestionIdiomas.Margin = new Padding(0, 0, 16, 16);
            inicioFrmGestionIdiomas.MinimumSize = new Size(250, 88);
            inicioFrmGestionIdiomas.Name = "inicioFrmGestionIdiomas";
            inicioFrmGestionIdiomas.Padding = new Padding(48, 0, 16, 0);
            inicioFrmGestionIdiomas.Size = new Size(250, 88);
            inicioFrmGestionIdiomas.TabIndex = 2;
            inicioFrmGestionIdiomas.Tag = "menu_gestion_idiomas";
            inicioFrmGestionIdiomas.Text = "Gestionar idiomas...";
            inicioFrmGestionIdiomas.TextAlign = ContentAlignment.MiddleLeft;
            inicioFrmGestionIdiomas.UseVisualStyleBackColor = false;
            // 
            // inicioFrmBitacora
            // 
            inicioFrmBitacora.BackColor = Color.White;
            inicioFrmBitacora.Dock = DockStyle.Top;
            inicioFrmBitacora.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            inicioFrmBitacora.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            inicioFrmBitacora.FlatStyle = FlatStyle.Flat;
            inicioFrmBitacora.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            inicioFrmBitacora.ForeColor = Color.FromArgb(30, 45, 65);
            inicioFrmBitacora.Icono = IconoVisual.Bitacora;
            inicioFrmBitacora.Location = new Point(266, 104);
            inicioFrmBitacora.Margin = new Padding(0, 0, 16, 16);
            inicioFrmBitacora.MinimumSize = new Size(250, 88);
            inicioFrmBitacora.Name = "inicioFrmBitacora";
            inicioFrmBitacora.Padding = new Padding(48, 0, 16, 0);
            inicioFrmBitacora.Size = new Size(250, 88);
            inicioFrmBitacora.TabIndex = 3;
            inicioFrmBitacora.Tag = "menu_bitacora";
            inicioFrmBitacora.Text = "Bitácora";
            inicioFrmBitacora.TextAlign = ContentAlignment.MiddleLeft;
            inicioFrmBitacora.UseVisualStyleBackColor = false;
            // 
            // inicioFrmIntegridad
            // 
            inicioFrmIntegridad.BackColor = Color.White;
            inicioFrmIntegridad.Dock = DockStyle.Top;
            inicioFrmIntegridad.FlatAppearance.BorderColor = Color.FromArgb(223, 230, 239);
            inicioFrmIntegridad.FlatAppearance.MouseOverBackColor = Color.FromArgb(231, 240, 252);
            inicioFrmIntegridad.FlatStyle = FlatStyle.Flat;
            inicioFrmIntegridad.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            inicioFrmIntegridad.ForeColor = Color.FromArgb(30, 45, 65);
            inicioFrmIntegridad.Icono = IconoVisual.Roles;
            inicioFrmIntegridad.Location = new Point(0, 208);
            inicioFrmIntegridad.Margin = new Padding(0, 0, 16, 16);
            inicioFrmIntegridad.MinimumSize = new Size(250, 88);
            inicioFrmIntegridad.Name = "inicioFrmIntegridad";
            inicioFrmIntegridad.Padding = new Padding(48, 0, 16, 0);
            inicioFrmIntegridad.Size = new Size(250, 88);
            inicioFrmIntegridad.TabIndex = 4;
            inicioFrmIntegridad.Tag = "menu_integridad";
            inicioFrmIntegridad.Text = "Integridad y recuperación";
            inicioFrmIntegridad.TextAlign = ContentAlignment.MiddleLeft;
            inicioFrmIntegridad.UseVisualStyleBackColor = false;
            // 
            // selectorIdioma
            // 
            selectorIdioma.BackColor = Color.White;
            selectorIdioma.Font = new Font("Segoe UI", 10F);
            selectorIdioma.ForeColor = Color.FromArgb(30, 45, 65);
            selectorIdioma.Name = "selectorIdioma";
            selectorIdioma.ShowCheckMargin = true;
            selectorIdioma.ShowImageMargin = false;
            selectorIdioma.Size = new Size(61, 4);
            // 
            // FrmPrincipal
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(245, 247, 251);
            ClientSize = new Size(1084, 681);
            Controls.Add(marcoPrincipal);
            Font = new Font("Segoe UI", 10F);
            ForeColor = Color.FromArgb(30, 45, 65);
            MainMenuStrip = menu;
            MinimumSize = new Size(1100, 720);
            Name = "FrmPrincipal";
            Tag = "titulo_ventana";
            Text = "SIGAT";
            menu.ResumeLayout(false);
            menu.PerformLayout();
            marcoPrincipal.ResumeLayout(false);
            encabezadoPrincipal.ResumeLayout(false);
            encabezadoPrincipal.PerformLayout();
            barraLateral.ResumeLayout(false);
            barraLateral.PerformLayout();
            panelNavegacion.ResumeLayout(false);
            panelContenido.ResumeLayout(false);
            panelInicio.ResumeLayout(false);
            panelInicio.PerformLayout();
            encabezadoInicio.ResumeLayout(false);
            encabezadoInicio.PerformLayout();
            tarjetasInicio.ResumeLayout(false);
            ResumeLayout(false);
        }
    }
}
