using System.Runtime.CompilerServices;

namespace SIGAT.UI;

internal static partial class TemaVisual
{
    public static bool EsOscuro { get; private set; }
    // La bandera evita confundir los colores del tema aplicado con los originales que necesitamos para volver al modo claro.
    private static bool aplicando;
    // Recordamos colores por control sin mantener en memoria una ventana que ya se cerró.
    private static readonly ConditionalWeakTable<Control, ColoresOriginales> originales = new();
    private sealed class ColoresOriginales
    {
        public Color Fondo, Texto;
        public Color BordeBoton, SobreBoton, PulsadoBoton;
        public Color FondoGrilla, Lineas;
        public DataGridViewCellStyle Cabecera, Celdas, Alternas, FilasCabecera;
        public ColoresOriginales(Control control)
        {
            Fondo = ColorOriginal(control.BackColor); Texto = ColorOriginal(control.ForeColor, true);
            control.BackColorChanged += (_, _) => { if (!aplicando) Fondo = ColorOriginal(control.BackColor); };
            control.ForeColorChanged += (_, _) => { if (!aplicando) Texto = ColorOriginal(control.ForeColor, true); };
            if (control is Button boton)
            {
                BordeBoton = boton.FlatAppearance.BorderColor;
                SobreBoton = boton.FlatAppearance.MouseOverBackColor;
                PulsadoBoton = boton.FlatAppearance.MouseDownBackColor;
            }
            if (control is DataGridView grilla)
            {
                FondoGrilla = grilla.BackgroundColor; Lineas = grilla.GridColor;
                Cabecera = grilla.ColumnHeadersDefaultCellStyle.Clone();
                Celdas = grilla.DefaultCellStyle.Clone();
                Alternas = grilla.AlternatingRowsDefaultCellStyle.Clone();
                FilasCabecera = grilla.RowHeadersDefaultCellStyle.Clone();
            }
        }
    }

    // El tema se guarda localmente por ID de cuenta. Esta preferencia no necesita una tabla SQL.
    public static string RutaPreferencia(int idUsuario) => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SIGAT", "preferencias", $"tema-{idUsuario}.txt");

    public static void CargarPreferencia(int idUsuario)
    {
        bool oscuro = false;
        try { oscuro = File.Exists(RutaPreferencia(idUsuario)) && File.ReadAllText(RutaPreferencia(idUsuario)).Trim() == "oscuro"; }
        catch (IOException) { }
        catch (UnauthorizedAccessException) { }
        CambiarModo(oscuro);
    }

    public static void GuardarPreferencia(int idUsuario, bool oscuro)
    {
        if (idUsuario <= 0) throw new InvalidOperationException("Debe iniciar sesión para guardar el tema.");
        string ruta = RutaPreferencia(idUsuario);
        Directory.CreateDirectory(Path.GetDirectoryName(ruta));
        File.WriteAllText(ruta, oscuro ? "oscuro" : "claro");
        CambiarModo(oscuro);
    }

    // Repintamos las ventanas existentes. No las recreamos ni descartamos los datos que se estaban editando.
    public static void CambiarModo(bool oscuro)
    {
        EsOscuro = oscuro;
        foreach (Form form in Application.OpenForms.Cast<Form>().ToArray()) Aplicar(form);
    }

    // Los colores originales se conservan para regresar al modo claro sin recrear controles.
    public static Color ColorActual(Color color, bool texto = false)
    {
        if (!EsOscuro || color.IsEmpty || color.A == 0) return color;
        if (color.ToArgb() == Azul.ToArgb()) return Color.FromArgb(106, 173, 255);
        if (color.ToArgb() == AzulSuave.ToArgb()) return Color.FromArgb(37, 62, 94);
        if (color.ToArgb() == Verde.ToArgb()) return Color.FromArgb(112, 211, 166);
        if (color.ToArgb() == VerdeSuave.ToArgb()) return Color.FromArgb(30, 66, 51);
        if (color.ToArgb() == Ambar.ToArgb()) return Color.FromArgb(244, 196, 112);
        if (color.ToArgb() == AmbarSuave.ToArgb()) return Color.FromArgb(72, 56, 29);
        if (texto)
        {
            if (color.ToArgb() == Color.White.ToArgb()) return Color.White;
            if (color.R > color.G * 1.4 && color.R > color.B * 1.3) return Color.FromArgb(255, 145, 155);
            return color.GetBrightness() > 0.25 ? Color.FromArgb(173, 188, 206) : Color.FromArgb(230, 237, 246);
        }
        if (color.ToArgb() == Fondo.ToArgb() || color.ToArgb() == SystemColors.Control.ToArgb()) return Color.FromArgb(20, 27, 38);
        if (color.GetBrightness() > 0.94) return Color.FromArgb(29, 39, 53);
        if (color.GetBrightness() > 0.75) return Color.FromArgb(43, 56, 74);
        return color;
    }

    public static void Aplicar(Control control)
    {
        if (control.IsDisposed) return;
        Capturar(control);
        // Mientras pintamos, los eventos de cambio de color no deben sobrescribir la referencia del modo claro.
        bool anterior = aplicando;
        aplicando = true;
        try { Pintar(control); }
        finally { aplicando = anterior; }
    }

    private static Color ColorOriginal(Color color, bool texto = false)
    {
        if (!EsOscuro) return color;
        foreach (Color original in new[] { Fondo, Texto, Secundario, Azul, AzulSuave, Borde, Verde, VerdeSuave, Ambar, AmbarSuave, Color.White })
            if (ColorActual(original, texto).ToArgb() == color.ToArgb()) return original;
        return color;
    }

    // Primero recordamos todo el árbol: algunos hijos heredan colores del padre y cambiarlo antes alteraría la captura.
    private static void Capturar(Control control)
    {
        originales.GetValue(control, c => new ColoresOriginales(c));
        foreach (Control hijo in control.Controls) Capturar(hijo);
    }

    private static void Pintar(Control control)
    {
        var original = originales.GetValue(control, c => new ColoresOriginales(c));
        control.BackColor = ColorActual(original.Fondo);
        control.ForeColor = ColorActual(original.Texto, true);
        if (control is Button boton)
        {
            boton.FlatAppearance.BorderColor = ColorActual(original.BordeBoton);
            boton.FlatAppearance.MouseOverBackColor = ColorActual(original.SobreBoton);
            boton.FlatAppearance.MouseDownBackColor = ColorActual(original.PulsadoBoton);
            // El botón principal necesita conservar contraste con su texto blanco.
            if (original.Fondo.ToArgb() == Azul.ToArgb()) boton.BackColor = Azul;
        }
        if (control is DataGridView grilla)
        {
            // Las tablas tienen colores propios para cabecera, filas y selección; cambiar solo el fondo no alcanza.
            grilla.BackgroundColor = ColorActual(original.FondoGrilla);
            grilla.GridColor = ColorActual(original.Lineas);
            grilla.ColumnHeadersDefaultCellStyle = Estilo(original.Cabecera);
            grilla.DefaultCellStyle = Estilo(original.Celdas);
            grilla.AlternatingRowsDefaultCellStyle = Estilo(original.Alternas);
            grilla.RowHeadersDefaultCellStyle = Estilo(original.FilasCabecera);
        }
        if (control is ToolStrip menu)
        {
            menu.Renderer = new ToolStripProfessionalRenderer(new ColoresMenu());
            AplicarItems(menu.Items);
        }
        if (control.ContextMenuStrip != null) Aplicar(control.ContextMenuStrip);
        foreach (Control hijo in control.Controls) Pintar(hijo);
        control.Invalidate();
    }

    // Clonamos el estilo y cambiamos solo colores. Conservamos formato, alineación y espaciado.
    private static DataGridViewCellStyle Estilo(DataGridViewCellStyle original)
    {
        var estilo = original.Clone();
        estilo.BackColor = ColorActual(original.BackColor);
        estilo.ForeColor = ColorActual(original.ForeColor, true);
        estilo.SelectionBackColor = ColorActual(original.SelectionBackColor);
        estilo.SelectionForeColor = ColorActual(original.SelectionForeColor, true);
        return estilo;
    }

    private static void AplicarItems(ToolStripItemCollection items)
    {
        foreach (ToolStripItem item in items)
        {
            item.BackColor = ColorActual(Color.White);
            item.ForeColor = ColorActual(Texto, true);
            if (item is ToolStripDropDownItem desplegable)
            {
                desplegable.DropDown.BackColor = ColorActual(Color.White);
                desplegable.DropDown.Renderer = new ToolStripProfessionalRenderer(new ColoresMenu());
                AplicarItems(desplegable.DropDownItems);
            }
        }
    }

    private sealed class ColoresMenu : ProfessionalColorTable
    {
        public override Color ToolStripDropDownBackground => ColorActual(Color.White);
        public override Color MenuItemSelected => ColorActual(AzulSuave);
        public override Color MenuItemBorder => ColorActual(Borde);
        public override Color MenuBorder => ColorActual(Borde);
        public override Color ImageMarginGradientBegin => ColorActual(Color.White);
        public override Color ImageMarginGradientMiddle => ColorActual(Color.White);
        public override Color ImageMarginGradientEnd => ColorActual(Color.White);
    }
}
