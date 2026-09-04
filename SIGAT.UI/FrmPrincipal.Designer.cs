namespace SIGAT.UI
{
    partial class FrmPrincipal
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.menu = new MenuStrip();
            this.itemSistema = new ToolStripMenuItem();
            this.itemUsuarios = new ToolStripMenuItem();
            this.itemBitacora = new ToolStripMenuItem();
            this.itemLogoutSeparator = new ToolStripSeparator();
            this.itemLogout = new ToolStripMenuItem();
            this.itemIdioma = new ToolStripMenuItem();
            this.itemEspañol = new ToolStripMenuItem();
            this.itemIngles = new ToolStripMenuItem();
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
            this.itemSistema.DropDownItems.AddRange(new ToolStripItem[] {
                this.itemUsuarios,
                this.itemBitacora,
                this.itemLogoutSeparator,
                this.itemLogout});
            this.itemSistema.Name = "itemSistema";
            //
            // itemUsuarios
            //
            this.itemUsuarios.Name = "itemUsuarios";
            this.itemUsuarios.Click += new EventHandler(this.ItemUsuarios_Click);
            //
            // itemBitacora
            //
            this.itemBitacora.Name = "itemBitacora";
            this.itemBitacora.Click += new EventHandler(this.ItemBitacora_Click);
            //
            // itemLogout
            //
            this.itemLogout.Name = "itemLogout";
            this.itemLogout.Click += new EventHandler(this.ItemLogout_Click);
            //
            // itemIdioma
            //
            this.itemIdioma.DropDownItems.AddRange(new ToolStripItem[] {
                this.itemEspañol,
                this.itemIngles});
            this.itemIdioma.Name = "itemIdioma";
            //
            // itemEspañol
            //
            this.itemEspañol.Name = "itemEspañol";
            this.itemEspañol.Text = "Español";
            this.itemEspañol.Click += new EventHandler(this.ItemEspañol_Click);
            //
            // itemIngles
            //
            this.itemIngles.Name = "itemIngles";
            this.itemIngles.Text = "English";
            this.itemIngles.Click += new EventHandler(this.ItemIngles_Click);
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
        private ToolStripMenuItem itemEspañol;
        private ToolStripMenuItem itemIngles;
    }
}
