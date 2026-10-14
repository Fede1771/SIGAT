using System.ComponentModel;

namespace SIGAT.UI;

internal static class EntornoDiseno
{
    // En el diseñador mostramos controles sin iniciar sesiones ni consultar la base.
    public static bool Activo => LicenseManager.UsageMode == LicenseUsageMode.Designtime;
}
