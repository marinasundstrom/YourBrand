using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.Themes;

public record GetThemeQuery() : IRequest<ThemeDto?>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<GetThemeQuery, ThemeDto?>
    {
        public async Task<ThemeDto?> Handle(GetThemeQuery request, CancellationToken cancellationToken)
        {
            var theme = await appServiceContext.Themes
                .OrderBy(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if (theme is null)
            {
                theme = new Theme("Theme", null);
            }

            theme.ColorSchemes ??= new ThemeColorSchemes
            {
                Light = new ThemeColorScheme(),
                Dark = new ThemeColorScheme()
            };

            return theme?.ToDto();
        }
    }
}

public record ThemeDto(string Id, string Name, string? Description, string? Title, string? Logo, bool Dense, ThemeColorSchemesDto ColorSchemes);

public record ThemeColorSchemesDto(ThemeColorSchemeDto? Light, ThemeColorSchemeDto? Dark);

public record ThemeColorSchemeDto(string? Logo, string? BackgroundColor, string? AppbarBackgroundColor, string? AppbarTextColor, string? PrimaryColor, string? SecondaryColor);