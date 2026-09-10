using MudBlazor;

namespace SFDocGen.Blazor;

public static class ThemeProvider
{
    public static bool IsDarkMode { get; private set; } = true;
    public static event EventHandler<ThemeChangedEventArgs>? ThemeChanged;

    public static MudTheme Theme { get; } = new()
    {
        Typography = new()
        {
            H1 = new H5Typography() { FontWeight = "900" }
        },

        PaletteLight = new PaletteLight()
        {
            AppbarBackground = new("#43262A"),
            DrawerBackground = new("#F4E0E8"),
            Background = new("#FFF3F5")
        },

        PaletteDark = new PaletteDark()
        {
            
        }
    };

    public static void SwitchTheme()
    {
        IsDarkMode = !IsDarkMode;
        ThemeChanged?.Invoke(null, new(IsDarkMode));
    }

    public static void SetTheme(bool dark)
    {
        if (dark == IsDarkMode) return;
        SwitchTheme();
    }
}

public class ThemeChangedEventArgs(bool isDarkMode) : EventArgs
{
    public bool IsDarkMode { get; } = isDarkMode;
}