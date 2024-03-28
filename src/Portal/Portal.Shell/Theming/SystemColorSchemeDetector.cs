using Microsoft.JSInterop;

namespace YourBrand.Portal.Theming;

public class SystemColorSchemeDetector : IDisposable
{
    private readonly IJSInProcessRuntime _jsRuntime;

    public SystemColorSchemeDetector(IJSRuntime jSRuntime)
    {
        _jsRuntime = (IJSInProcessRuntime)jSRuntime;

        Internal.ColorSchemeChanged += Internal_ColorSchemeChanged;
    }

    private void Internal_ColorSchemeChanged(object? sender, SystemColorSchemeChangedEventArgs e)
    {
        ColorSchemeChanged?.Invoke(this, e);
    }

    public event EventHandler<SystemColorSchemeChangedEventArgs> ColorSchemeChanged = null!;

    public void Dispose()
    {
        Internal.ColorSchemeChanged -= Internal_ColorSchemeChanged;
    }

    public ColorScheme CurrentColorScheme => _jsRuntime.Invoke<bool>("isDarkMode") ? ColorScheme.Dark : ColorScheme.Light;

    public static class Internal
    {
        [JSInvokable("OnDarkModeChanged")]
        public static void OnDarkModeChanged(bool isDarkMode)
        {
            ColorSchemeChanged?.Invoke(null, new SystemColorSchemeChangedEventArgs(isDarkMode ? ColorScheme.Dark : ColorScheme.Light));
        }

        public static event EventHandler<SystemColorSchemeChangedEventArgs> ColorSchemeChanged = null!;
    }
}