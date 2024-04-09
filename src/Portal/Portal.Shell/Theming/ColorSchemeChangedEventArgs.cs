namespace YourBrand.Portal.Theming;

public class ColorSchemeChangedEventArgs(ColorScheme colorScheme) : EventArgs
{
    public ColorScheme ColorScheme { get; } = colorScheme;
}