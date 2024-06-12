namespace ChatApp.Theming;

public class ColorSchemeChangedEventArgs : EventArgs
{
    public ColorSchemeChangedEventArgs(ColorScheme colorScheme)
    {
        ColorScheme = colorScheme;
    }

    public ColorScheme ColorScheme { get; }
}