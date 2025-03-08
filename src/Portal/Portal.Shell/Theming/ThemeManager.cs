using Blazored.LocalStorage;

using MudBlazor;

using YourBrand.AppService.Client;

namespace YourBrand.Portal.Theming;

public interface IThemeManager : IDisposable
{
    void Initialize();

    Theme Theme { get; }

    void SetTheme(Theme theme);

    void RefreshTheme();

    event EventHandler<EventArgs>? ThemeChanged;

    ColorScheme CurrentColorScheme { get; }

    ColorScheme? PreferredColorScheme { get; set; }

    bool IsAutoColorScheme => PreferredColorScheme is null;

    void UseSystemColorScheme();

    void SetPreferredColorScheme(ColorScheme colorScheme);

    event EventHandler<ColorSchemeChangedEventArgs>? ColorSchemeChanged;

}

public sealed class ThemeManager : IThemeManager
{
    private const string PreferredColorSchemeKey = "preferredColorScheme";
    private readonly SystemColorSchemeDetector _systemColorSchemeDetector;
    private readonly ISyncLocalStorageService _localStorage;
    private bool initialized = false;
    private static Theme s_mudTheme = new Theme();

    public ThemeManager(SystemColorSchemeDetector systemColorSchemeDetector, ISyncLocalStorageService localStorage)
    {
        _systemColorSchemeDetector = systemColorSchemeDetector;
        _systemColorSchemeDetector.ColorSchemeChanged += _systemColorSchemeDetector_ColorSchemeChanged;

        _localStorage = localStorage;
    }

    public void Initialize()
    {
        if (initialized)
        {
            return;
        }

        CurrentColorScheme = PreferredColorScheme ?? _systemColorSchemeDetector.CurrentColorScheme;

        RaiseCurrentColorSchemeChanged();
        RaiseThemeChanged();

        initialized = true;
    }

    public Theme Theme
    {
        get => s_mudTheme;
        private set => s_mudTheme = value;
    }

    public void SetTheme(Theme theme)
    {
        if (theme != Theme)
        {
            Theme = theme;

            RaiseThemeChanged();
        }
    }

    public void RefreshTheme()
    {
        RaiseThemeChanged();
    }

    private void RaiseThemeChanged()
    {
        ThemeChanged?.Invoke(this, EventArgs.Empty);
    }

    public event EventHandler<EventArgs>? ThemeChanged;

    private void _systemColorSchemeDetector_ColorSchemeChanged(object? sender, SystemColorSchemeChangedEventArgs e)
    {
        if (PreferredColorScheme == null)
        {
            CurrentColorScheme = e.ColorScheme;

            RaiseCurrentColorSchemeChanged();
        }
    }

    private void RaiseCurrentColorSchemeChanged()
    {
        ColorSchemeChanged?.Invoke(this, new ColorSchemeChangedEventArgs(CurrentColorScheme));
    }

    public ColorScheme CurrentColorScheme { get; private set; }

    public ColorScheme? PreferredColorScheme
    {
        get => _localStorage.GetItem<ColorScheme?>(PreferredColorSchemeKey);

        set => _localStorage.SetItem(PreferredColorSchemeKey, value);
    }

    public void UseSystemColorScheme()
    {
        PreferredColorScheme = null;
        CurrentColorScheme = _systemColorSchemeDetector.CurrentColorScheme;
        PreferredColorScheme = null;
        RaiseCurrentColorSchemeChanged();
    }

    public void SetPreferredColorScheme(ColorScheme colorScheme)
    {
        if (PreferredColorScheme != colorScheme)
        {
            PreferredColorScheme = colorScheme;
            CurrentColorScheme = colorScheme;

            RaiseCurrentColorSchemeChanged();
        }
    }

    public event EventHandler<ColorSchemeChangedEventArgs>? ColorSchemeChanged;

    public void Dispose()
    {
        _systemColorSchemeDetector.ColorSchemeChanged -= _systemColorSchemeDetector_ColorSchemeChanged;
    }
}