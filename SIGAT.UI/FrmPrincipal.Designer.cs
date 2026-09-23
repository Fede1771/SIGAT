namespace SIGAT.UI
{
    partial class FrmPrincipal
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.menu = new MenuStrip();
            this.itemSistema = new ToolStripMenuItem();
            this.itemUsuarios = new ToolStripMenuItem();
            this.itemBitacora = new ToolStripMenuItem();
            this.itemLogoutSeparator = new ToolStripSeparator();
            this.itemLogout = new ToolStripMenuItem();
            this.itemIdioma = new ToolStripMenuItem();
            this.itemGestionIdiomas = new ToolStripMenuItem();
            this.menu.SuspendLayout();
            this.SuspendLayout();
            //
            // menu
            //
            this.menu.BackColor = Color.White;
            this.menu.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            this.menu.Items.AddRange(new ToolStripItem[] {
                this.itemSistema,
                this.itemIdioma});
            this.menu.Name = "menu";
            //
            // itemSistema
            //
            this.itemSistema.Text = "Sistema";
            this.itemSistema.DropDownItems.AddRange(new ToolStripItem[] {
                this.itemUsuarios,
                this.itemBitacora,
                this.itemLogoutSeparator,
                this.itemLogout});
            this.itemSistema.Name = "itemSistema";
            //
            // itemUsuarios
            //
            this.itemUsuarios.Text = "Gestión de Usuarios";
            this.itemUsuarios.Name = "itemUsuarios";
            this.itemUsuarios.Click += new EventHandler(this.ItemUsuarios_Click);
            //
            // itemBitacora
            //
            this.itemBitacora.Text = "Bitácora";
            this.itemBitacora.Name = "itemBitacora";
            this.itemBitacora.Click += new EventHandler(this.ItemBitacora_Click);
            //
            // itemLogout
            //
            this.itemLogout.Text = "Cerrar Sesión";
            this.itemLogout.Name = "itemLogout";
            this.itemLogout.Click += new EventHandler(this.ItemLogout_Click);
            //
            // itemIdioma
            //
            this.itemIdioma.Text = "Idioma";
            this.itemIdioma.DropDownItems.AddRange(new ToolStripItem[] {
                this.itemGestionIdiomas});
            this.itemIdioma.Name = "itemIdioma";
            //
            // itemGestionIdiomas
            //
            this.itemGestionIdiomas.Name = "itemGestionIdiomas";
            this.itemGestionIdiomas.Text = "Gestionar idiomas...";
            this.itemGestionIdiomas.Click += new EventHandler(this.ItemGestionIdiomas_Click);
            //
            // FrmPrincipal
            //
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(1000, 600);
            this.Controls.Add(this.menu);
            this.IsMdiContainer = true;
            this.MainMenuStrip = this.menu;
            this.Name = "FrmPrincipal";
            this.Text = "FrmPrincipal";
            this.WindowState = FormWindowState.Maximized;
            this.menu.ResumeLayout(false);
            this.menu.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        private MenuStrip menu;
        private ToolStripMenuItem itemSistema;
        private ToolStripMenuItem itemUsuarios;
        private ToolStripMenuItem itemBitacora;
        private ToolStripSeparator itemLogoutSeparator;
        private ToolStripMenuItem itemLogout;
        private ToolStripMenuItem itemIdioma;
        private ToolStripMenuItem itemGestionIdiomas;
    }
}