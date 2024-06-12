using Blazored.LocalStorage;

using MudBlazor;

namespace ChatApp.Theming;

public interface IThemeManager : IDisposable 
{
    void Initialize();

    MudTheme Theme { get; }

    void SetTheme(MudTheme theme);

    event EventHandler<EventArgs>? ThemeChanged;

    ColorScheme CurrentColorScheme { get; }

    ColorScheme? PreferredColorScheme { get; set;}

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

    public ThemeManager(SystemColorSchemeDetector systemColorSchemeDetector, ISyncLocalStorageService localStorage)
    {
        _systemColorSchemeDetector = systemColorSchemeDetector;
        _systemColorSchemeDetector.ColorSchemeChanged += _systemColorSchemeDetector_ColorSchemeChanged;

        _localStorage = localStorage;
    }

    public void Initialize()
    {
        CurrentColorScheme = PreferredColorScheme ?? _systemColorSchemeDetector.CurrentColorScheme;
    }

    public MudTheme Theme { get; private set; } = new MudTheme();

    public void SetTheme(MudTheme theme) 
    {
        if(theme != Theme) 
        {
            Theme = theme;
            
            RaiseThemeChanged();
        }
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
        _localStorage.SetItem<ColorScheme?>(PreferredColorSchemeKey, null);
        RaiseCurrentColorSchemeChanged();
    }

    public void SetPreferredColorScheme(ColorScheme colorScheme)
    {
        PreferredColorScheme = colorScheme;

        if (CurrentColorScheme != colorScheme)
        {
            CurrentColorScheme = colorScheme;

            RaiseCurrentColorSchemeChanged();
        }

        _localStorage.SetItem<ColorScheme?>(PreferredColorSchemeKey, colorScheme);
    }

    public event EventHandler<ColorSchemeChangedEventArgs>? ColorSchemeChanged;

    public void Dispose()
    {
        _systemColorSchemeDetector.ColorSchemeChanged -= _systemColorSchemeDetector_ColorSchemeChanged;
    }
}