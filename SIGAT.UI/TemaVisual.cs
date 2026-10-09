using System.Drawing.Drawing2D;

namespace SIGAT.UI;

internal enum IconoVisual { Inicio, Usuarios, Roles, Idiomas, Bitacora, Salir, Familia, Permiso }

internal static class TemaVisual
{
    public static readonly Color Fondo = Color.FromArgb(245, 247, 251);
    public static readonly Color Texto = Color.FromArgb(30, 45, 65);
    public static readonly Color Secundario = Color.FromArgb(94, 111, 133);
    public static readonly Color Azul = Color.FromArgb(35, 101, 175);
    public static readonly Color AzulSuave = Color.FromArgb(231, 240, 252);
    public static readonly Color Borde = Color.FromArgb(223, 230, 239);
    public static readonly Color Verde = Color.FromArgb(27, 112, 79);
    public static readonly Color VerdeSuave = Color.FromArgb(231, 245, 237);
    public static readonly Color Ambar = Color.FromArgb(132, 84, 17);
    public static readonly Color AmbarSuave = Color.FromArgb(255, 245, 219);
    public static readonly Font FuenteEstado = new Font("Segoe UI", 9F, FontStyle.Bold);

    public static void Formulario(Form form)
    {
        form.Font = new Font("Segoe UI", 10F);
        form.BackColor = Fondo;
        form.ForeColor = Texto;
        form.AutoScaleMode = AutoScaleMode.Font;
        form.AutoScaleDimensions = form.CurrentAutoScaleDimensions;
    }

    public static Label Etiqueta(string text, string key = null, bool title = false)
        => new Label
        {
            Text = text, Tag = key, AutoSize = true, Dock = DockStyle.Fill,
            Font = new Font("Segoe UI", title ? 13F : 10F, FontStyle.Bold),
            ForeColor = Texto, Margin = new Padding(0, 0, 0, title ? 14 : 6)
        };

    public static TableLayoutPanel Encabezado(string title, string subtitle, string titleKey, string subtitleKey)
    {
        var panel = new TableLayoutPanel { Dock = DockStyle.Top, AutoSize = true, ColumnCount = 1, Margin = new Padding(0, 0, 0, 22) };
        panel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        var heading = Etiqueta(title, titleKey);
        heading.Font = new Font("Segoe UI", 22F, FontStyle.Bold);
        var description = new Label { Text = subtitle, Tag = subtitleKey, AutoSize = true, Dock = DockStyle.Fill, ForeColor = Secundario, Margin = Padding.Empty };
        panel.Controls.Add(heading);
        panel.Controls.Add(description);
        return panel;
    }

    public static Panel Tarjeta(Control content, int padding = 18)
    {
        var card = new Panel { Dock = DockStyle.Fill, BackColor = Color.White, Padding = new Padding(padding), Margin = Padding.Empty };
        content.Dock = DockStyle.Fill;
        card.Controls.Add(content);
        return card;
    }

    public static Button Boton(string name, string text, bool primary = false, bool danger = false)
    {
        var button = new Button { Name = name, Text = text, AutoSize = true, MinimumSize = new Size(96, 40), Height = 40, Margin = new Padding(0, 0, 8, 8) };
        EstilizarBoton(button, primary, danger);
        return button;
    }

    public static void EstilizarBoton(Button button, bool primary = false, bool danger = false)
    {
        button.FlatStyle = FlatStyle.Flat;
        button.Cursor = Cursors.Hand;
        button.UseVisualStyleBackColor = false;
        button.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        button.BackColor = primary ? Azul : Color.White;
        button.ForeColor = primary ? Color.White : danger ? Color.FromArgb(164, 48, 59) : Texto;
        button.FlatAppearance.BorderSize = primary ? 0 : 1;
        button.FlatAppearance.BorderColor = Borde;
        button.FlatAppearance.MouseOverBackColor = primary ? Color.FromArgb(28, 82, 146) : AzulSuave;
        button.FlatAppearance.MouseDownBackColor = primary ? Color.FromArgb(23, 69, 126) : Color.FromArgb(215, 230, 250);
    }

    public static void Grilla(DataGridView grid)
    {
        grid.Font = new Font("Segoe UI", 10F);
        grid.BackgroundColor = Color.White;
        grid.BorderStyle = BorderStyle.None;
        grid.AllowUserToAddRows = false;
        grid.AllowUserToDeleteRows = false;
        grid.AllowUserToResizeRows = false;
        grid.RowHeadersVisible = false;
        grid.MultiSelect = false;
        grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
        grid.GridColor = Color.FromArgb(234, 239, 246);
        grid.EnableHeadersVisualStyles = false;
        grid.ColumnHeadersHeight = 44;
        grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.FromArgb(236, 241, 248), ForeColor = Texto,
            SelectionBackColor = Color.FromArgb(236, 241, 248), SelectionForeColor = Texto,
            Font = new Font("Segoe UI", 10F, FontStyle.Bold), Padding = new Padding(10, 0, 10, 0)
        };
        grid.DefaultCellStyle = new DataGridViewCellStyle
        {
            BackColor = Color.White, ForeColor = Texto, Padding = new Padding(10, 5, 10, 5),
            SelectionBackColor = AzulSuave, SelectionForeColor = Texto
        };
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(249, 251, 254);
        grid.RowTemplate.Height = 40;
    }

    public static void Arbol(TreeView tree)
    {
        tree.BorderStyle = BorderStyle.None;
        tree.BackColor = Color.White;
        tree.ForeColor = Texto;
        tree.Font = new Font("Segoe UI", 10F);
        tree.ItemHeight = 32;
        tree.Indent = 22;
        tree.HideSelection = false;
        tree.FullRowSelect = true;
        tree.ShowLines = false;
        tree.ShowNodeToolTips = true;
        var images = new ImageList { ImageSize = new Size(20, 20), ColorDepth = ColorDepth.Depth32Bit };
        foreach (var icon in new[] { IconoVisual.Roles, IconoVisual.Familia, IconoVisual.Permiso })
            images.Images.Add(icon.ToString(), Icono(icon, Azul, 20));
        tree.ImageList = images;
        tree.Disposed += (_, _) => images.Dispose();
    }

    public static Bitmap Icono(IconoVisual icon, Color color, int size = 24)
    {
        var image = new Bitmap(size, size);
        using var g = Graphics.FromImage(image);
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.ScaleTransform(size / 24F, size / 24F);
        using var p = new Pen(color, 1.7F) { StartCap = LineCap.Round, EndCap = LineCap.Round, LineJoin = LineJoin.Round };
        switch (icon)
        {
            case IconoVisual.Usuarios:
                g.DrawEllipse(p, 8, 3, 8, 8); g.DrawArc(p, 4, 13, 16, 14, 180, 180); break;
            case IconoVisual.Roles:
                g.DrawPolygon(p, new PointF[] { new(12, 2), new(21, 6), new(19, 16), new(12, 22), new(5, 16), new(3, 6) });
                g.DrawLines(p, new PointF[] { new(8, 12), new(11, 15), new(16, 9) }); break;
            case IconoVisual.Idiomas:
                g.DrawEllipse(p, 3, 3, 18, 18); g.DrawEllipse(p, 8, 3, 8, 18); g.DrawLine(p, 3, 12, 21, 12); break;
            case IconoVisual.Bitacora:
                g.DrawRectangle(p, 5, 3, 14, 18); g.DrawLine(p, 9, 8, 15, 8); g.DrawLine(p, 9, 12, 15, 12); g.DrawLine(p, 9, 16, 13, 16); break;
            case IconoVisual.Salir:
                g.DrawLines(p, new PointF[] { new(11, 4), new(4, 4), new(4, 20), new(11, 20) });
                g.DrawLine(p, 10, 12, 21, 12); g.DrawLines(p, new PointF[] { new(17, 8), new(21, 12), new(17, 16) }); break;
            case IconoVisual.Familia:
                g.DrawRectangle(p, 9, 2, 6, 6); g.DrawLine(p, 12, 8, 12, 12); g.DrawLine(p, 5, 12, 19, 12);
                g.DrawLine(p, 5, 12, 5, 16); g.DrawLine(p, 19, 12, 19, 16); g.DrawRectangle(p, 2, 16, 6, 6); g.DrawRectangle(p, 16, 16, 6, 6); break;
            case IconoVisual.Permiso:
                g.DrawEllipse(p, 3, 4, 9, 9); g.DrawLine(p, 11, 12, 20, 21); g.DrawLine(p, 15, 16, 18, 13); break;
            default:
                g.DrawLines(p, new PointF[] { new(3, 11), new(12, 3), new(21, 11) }); g.DrawRectangle(p, 6, 11, 12, 10); break;
        }
        return image;
    }
}

internal sealed class BotonNavegacion : Button
{
    [System.ComponentModel.DefaultValue(false)]
    public bool Seleccionado { get; set; }
    private readonly Bitmap icon;

    public BotonNavegacion(IconoVisual tipo)
    {
        icon = TemaVisual.Icono(tipo, TemaVisual.Azul);
        FlatStyle = FlatStyle.Flat;
        FlatAppearance.BorderSize = 0;
        FlatAppearance.MouseOverBackColor = TemaVisual.AzulSuave;
        Cursor = Cursors.Hand;
        TextAlign = ContentAlignment.MiddleLeft;
        Padding = new Padding(44, 0, 10, 0);
        Height = 54;
        Dock = DockStyle.Top;
        Font = new Font("Segoe UI", 10F);
        UseVisualStyleBackColor = false;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        BackColor = Seleccionado ? TemaVisual.AzulSuave : Color.White;
        ForeColor = Seleccionado ? TemaVisual.Azul : TemaVisual.Texto;
        base.OnPaint(e);
        if (Seleccionado) using (var brush = new SolidBrush(TemaVisual.Azul)) e.Graphics.FillRectangle(brush, 0, 10, 3, Height - 20);
        e.Graphics.DrawImage(icon, 12, (Height - 24) / 2, 24, 24);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) icon.Dispose();
        base.Dispose(disposing);
    }
}
