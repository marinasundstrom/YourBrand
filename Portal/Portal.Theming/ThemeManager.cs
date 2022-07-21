using Blazored.LocalStorage;

namespace YourBrand.Portal.Theming;

public class ThemeManager : IDisposable
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

    public void UseSystemScheme()
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

    public event EventHandler<ColorSchemeChangedEventArgs> ColorSchemeChanged = null!;

    public void Dispose()
    {
        _systemColorSchemeDetector.ColorSchemeChanged -= _systemColorSchemeDetector_ColorSchemeChanged;
    }
}