using MudBlazor;

namespace ChatApp.Theming;

public static class Themes
{
    public static MudTheme AppTheme { get; } = new MudTheme()
    {
        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Roboto", "sans-serif" }
            }
        },
        Palette = new Palette
        {
            Background = "rgb(248, 249, 250)",
            AppbarBackground = "#137cdf", 
            Primary = "#4892d7"
            //Secondary = "#00000000"
        }
    };

    public static MudTheme AppTheme2 { get; } = new MudTheme()
    {
        Typography = new Typography()
        {
            Default = new Default()
            {
                FontFamily = new[] { "Roboto", "sans-serif" }
            }
        },
        Palette = new Palette
        {
            Background = "#22FFDD",
            AppbarBackground = "#FF44BB", 
            Primary = "#4892d7"
            //Secondary = "#00000000"
        }
    };
}