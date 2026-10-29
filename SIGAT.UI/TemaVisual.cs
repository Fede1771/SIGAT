using System.Drawing.Drawing2D;

namespace SIGAT.UI;

public enum IconoVisual { Inicio, Usuarios, Roles, Idiomas, Bitacora, Salir, Familia, Permiso }

internal static partial class TemaVisual
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
        DisenoAdaptativo.Configurar(form);
        // Preparamos los colores antes de mostrar la ventana: Shown llega después del primer dibujo.
        form.HandleCreated += (_, _) => Aplicar(form);
        form.Load += (_, _) => Aplicar(form);
        // Fuente, colores base y escalado están definidos en el diseñador.
    }

    // Los iconos se generan al ejecutar; la estructura del árbol está en el diseñador.
    public static void CargarIconosArbol(TreeView tree)
    {
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

public sealed class BotonNavegacion : Button
{
    [System.ComponentModel.DefaultValue(false)]
    public bool Seleccionado { get; set; }
    [System.ComponentModel.DefaultValue(false)]
    public bool MostrarSoloIcono { get; set; }
    private Bitmap icon;
    private IconoVisual tipo;
    private Color colorIcono;

    // Constructor y propiedad públicos para que Visual Studio pueda editar el control.
    public BotonNavegacion() : this(IconoVisual.Inicio) { }

    [System.ComponentModel.DefaultValue(IconoVisual.Inicio)]
    public IconoVisual Icono
    {
        get => tipo;
        set
        {
            tipo = value;
            icon?.Dispose();
            icon = TemaVisual.Icono(tipo, colorIcono);
            Invalidate();
        }
    }
    public BotonNavegacion(IconoVisual tipo)
    {
        this.tipo = tipo;
        colorIcono = TemaVisual.ColorActual(TemaVisual.Azul);
        icon = TemaVisual.Icono(tipo, colorIcono);
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

    // Dibujamos selección e icono según el tema actual. Esto cambia la presentación, no la acción del botón.
    protected override void OnPaint(PaintEventArgs e)
    {
        Color actual = TemaVisual.ColorActual(TemaVisual.Azul);
        if (actual != colorIcono)
        {
            icon.Dispose();
            colorIcono = actual;
            icon = TemaVisual.Icono(tipo, colorIcono);
        }
        BackColor = TemaVisual.ColorActual(Seleccionado ? TemaVisual.AzulSuave : Color.White);
        ForeColor = TemaVisual.ColorActual(Seleccionado ? TemaVisual.Azul : TemaVisual.Texto, true);
        if (MostrarSoloIcono)
        {
            using var fondo = new SolidBrush(BackColor);
            e.Graphics.FillRectangle(fondo, ClientRectangle);
            if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(e.Graphics, Rectangle.Inflate(ClientRectangle, -4, -4));
        }
        else base.OnPaint(e);
        if (Seleccionado) using (var brush = new SolidBrush(actual)) e.Graphics.FillRectangle(brush, 0, 10, 3, Height - 20);
        e.Graphics.DrawImage(icon, MostrarSoloIcono ? (Width - 24) / 2 : 12, (Height - 24) / 2, 24, 24);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing) icon.Dispose();
        base.Dispose(disposing);
    }
}

