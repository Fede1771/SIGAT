namespace SIGAT.UI;

// Conserva selección y navegación por teclado del TabControl nativo, pero pinta
// el marco y el espacio libre con el tema de SIGAT, también en modo oscuro.
public sealed class PestanasRoles : TabControl
{
    public PestanasRoles()
    {
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(TemaVisual.ColorActual(TemaVisual.Fondo));
        using var borde = new Pen(TemaVisual.ColorActual(TemaVisual.Borde));
        var cuerpo = DisplayRectangle;
        cuerpo.Inflate(1, 1);
        if (cuerpo.Width > 0 && cuerpo.Height > 0) e.Graphics.DrawRectangle(borde, cuerpo);
        for (int i = 0; i < TabCount; i++)
        {
            var rect = GetTabRect(i);
            OnDrawItem(new DrawItemEventArgs(e.Graphics, Font, rect, i,
                i == SelectedIndex ? DrawItemState.Selected | (Focused ? DrawItemState.Focus : DrawItemState.Default) : DrawItemState.Default));
        }
        base.OnPaint(e);
    }

    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        Invalidate();
    }
}
