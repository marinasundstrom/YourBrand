namespace YourBrand.Portal.Theming;

public class SystemColorSchemeChangedEventArgs(ColorScheme colorScheme) : EventArgs
{
    public ColorScheme ColorScheme { get; } = colorScheme;
}