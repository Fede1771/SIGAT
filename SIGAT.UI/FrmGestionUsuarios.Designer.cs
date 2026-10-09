namespace SIGAT.UI
{
    partial class FrmGestionUsuarios
    {
        private Label lblUsuario, lblClave, lblNombre, lblApellido, lblAyuda;
        private TextBox txtUsername, txtPass, txtNombre, txtApellido;
        private CheckBox chkActivo;
        private Button btnGuardar, btnEliminar, btnLimpiar;
        private DataGridView dgvUsuarios;

        private void InitializeComponent()
        {
            SuspendLayout();
            Name = "FrmGestionUsuarios";
            Text = "Gestión de usuarios";
            TemaVisual.Formulario(this);
            ClientSize = new Size(1160, 620);
            MinimumSize = new Size(940, 540);
            // El marco estándar permite que MDI calcule correctamente el área maximizada.
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Maximized;
            AutoScaleMode = AutoScaleMode.Font;

            TableLayoutPanel layout = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 2, Padding = new Padding(24)
            };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360));
            layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            Controls.Add(layout);

            TableLayoutPanel encabezado = new TableLayoutPanel
            {
                Dock = DockStyle.Fill, AutoSize = true, ColumnCount = 1,
                Margin = new Padding(0, 0, 0, 22)
            };
            encabezado.Controls.Add(new Label
            {
                Text = "Gestión de usuarios", Tag = "frmgestionusuarios_titulo", AutoSize = true,
                Font = new Font("Segoe UI", 22F, FontStyle.Bold),
                ForeColor = TemaVisual.Texto, Margin = new Padding(0, 0, 0, 6)
            });
            encabezado.Controls.Add(new Label
            {
                Text = "Seleccioná una cuenta para editarla o completá el formulario para crear un usuario.",
                Tag = "lbl_descripcion_usuarios", AutoSize = true,
                ForeColor = Color.FromArgb(85, 100, 115), Margin = Padding.Empty
            });
            layout.Controls.Add(encabezado, 0, 0);
            layout.SetColumnSpan(encabezado, 2);

            dgvUsuarios = new DataGridView
            {
                Name = "dgvUsuarios", Dock = DockStyle.Fill, Margin = new Padding(0, 0, 18, 0),
                ReadOnly = true, AllowUserToAddRows = false, AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false, MultiSelect = false, RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                BackgroundColor = Color.White, BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal,
                GridColor = Color.FromArgb(230, 235, 241), EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 42,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
            };
            dgvUsuarios.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = Color.FromArgb(231, 237, 245), ForeColor = Color.FromArgb(35, 55, 78),
                Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(8, 0, 8, 0),
                SelectionBackColor = Color.FromArgb(231, 237, 245), SelectionForeColor = Color.FromArgb(35, 55, 78)
            };
            dgvUsuarios.DefaultCellStyle = new DataGridViewCellStyle
            {
                Padding = new Padding(8, 4, 8, 4), ForeColor = Color.FromArgb(35, 45, 60),
                SelectionBackColor = Color.FromArgb(219, 234, 252), SelectionForeColor = Color.FromArgb(20, 45, 80)
            };
            dgvUsuarios.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
            dgvUsuarios.RowTemplate.Height = 38;
            TemaVisual.Grilla(dgvUsuarios);
            layout.Controls.Add(dgvUsuarios, 0, 1);

            Panel contenedor = new Panel { Dock = DockStyle.Fill, AutoScroll = true, BackColor = Color.White, Margin = Padding.Empty };
            TableLayoutPanel editor = new TableLayoutPanel
            {
                Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Padding = new Padding(16)
            };
            editor.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            contenedor.Controls.Add(editor);
            layout.Controls.Add(contenedor, 1, 1);
            editor.Controls.Add(new Label
            {
                Text = "Datos de la cuenta", Tag = "lbl_datos_cuenta", AutoSize = true,
                Font = new Font("Segoe UI", 13F, FontStyle.Bold),
                ForeColor = Color.FromArgb(35, 55, 78), Margin = new Padding(0, 0, 0, 14)
            });

            lblUsuario = Etiqueta("Usuario:");
            txtUsername = Campo("txtUsername", 50);
            lblClave = Etiqueta("Clave (vacío no cambia):");
            txtPass = Campo("txtPass", 100);
            txtPass.UseSystemPasswordChar = true;
            lblNombre = Etiqueta("Nombre:");
            txtNombre = Campo("txtNombre", 100);
            lblApellido = Etiqueta("Apellido:");
            txtApellido = Campo("txtApellido", 100);
            chkActivo = new CheckBox { Name = "chkActivo", Text = "Activo", Checked = true, AutoSize = true, Margin = new Padding(0, 10, 0, 12) };
            foreach (Control control in new Control[] { lblUsuario, txtUsername, lblClave, txtPass, lblNombre, txtNombre, lblApellido, txtApellido, chkActivo })
                editor.Controls.Add(control);

            TableLayoutPanel botones = new TableLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, ColumnCount = 3, Margin = new Padding(0, 4, 0, 12) };
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            botones.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
            btnGuardar = BotonUsuario("btnGuardar", "Guardar");
            btnEliminar = BotonUsuario("btnEliminar", "Baja");
            btnLimpiar = BotonUsuario("btnLimpiar", "Limpiar");
            TemaVisual.EstilizarBoton(btnGuardar, primary: true);
            TemaVisual.EstilizarBoton(btnEliminar, danger: true);
            btnGuardar.Click += BtnGuardar_Click;
            btnEliminar.Click += BtnEliminar_Click;
            btnLimpiar.Click += BtnLimpiar_Click;
            botones.Controls.Add(btnGuardar, 0, 0);
            botones.Controls.Add(btnEliminar, 1, 0);
            botones.Controls.Add(btnLimpiar, 2, 0);
            editor.Controls.Add(botones);
            lblAyuda = new Label
            {
                AutoSize = true, Dock = DockStyle.Fill, ForeColor = Color.FromArgb(85, 100, 115),
                MaximumSize = new Size(300, 0),
                BackColor = Color.FromArgb(245, 247, 250), Padding = new Padding(12),
                Font = new Font("Segoe UI", 9F),
                Text = "Las cuentas nuevas se crean sin roles. Asigná sus accesos desde Gestión de roles y permisos.\n\nPara reactivar una cuenta, seleccionála, marcá Activo y guardá."
            };
            editor.Controls.Add(lblAyuda);
            AcceptButton = btnGuardar;
            ResumeLayout(true);
        }

        private static Label Etiqueta(string texto)
        {
            return new Label { Text = texto, AutoSize = true, Font = new Font("Segoe UI", 10F, FontStyle.Bold), Margin = new Padding(0, 8, 0, 5) };
        }

        private static TextBox Campo(string nombre, int longitud)
        {
            return new TextBox { Name = nombre, Dock = DockStyle.Fill, MaxLength = longitud, Margin = new Padding(0, 0, 0, 8) };
        }

        private static Button BotonUsuario(string nombre, string texto)
        {
            var boton = TemaVisual.Boton(nombre, texto);
            boton.Dock = DockStyle.Fill;
            boton.AutoSize = false;
            boton.MinimumSize = new Size(0, 40);
            boton.Margin = new Padding(0, 0, 6, 0);
            return boton;
        }
    }
}
