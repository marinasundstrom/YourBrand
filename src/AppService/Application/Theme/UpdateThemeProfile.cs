using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.Themes;

public record UpdateTheme(string Name, string? Description, ThemeColorSchemesDto Colors) : IRequest<ThemeDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<UpdateTheme, ThemeDto>
    {
        public async Task<ThemeDto?> Handle(UpdateTheme request, CancellationToken cancellationToken)
        {
            Theme? theme = await appServiceContext.Themes
                .OrderBy(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            bool @new = false;

            if (theme is null)
            {
                theme = new Theme(request.Name, request.Description);
                @new = true;
            }
            else
            {
                theme.Name = request.Name;
                theme.Description = request.Description;
            }

            theme.ColorSchemes ??= new ThemeColorSchemes
            {
                Light = new ThemeColorScheme(),
                Dark = new ThemeColorScheme()
            };

            Map(theme.ColorSchemes.Light, request.Colors.Light);
            Map(theme.ColorSchemes.Dark, request.Colors.Dark);

            if (@new)
            {
                appServiceContext.Themes.Add(theme);
            }

            await appServiceContext.SaveChangesAsync(cancellationToken);

            theme = await appServiceContext.Themes
                .OrderBy(x => x.Created)
                .FirstAsync(cancellationToken);

            return theme.ToDto();
        }

        private void Map(ThemeColorScheme target, ThemeColorSchemeDto? from)
        {
            target.BackgroundColor = from.BackgroundColor;
            target.AppbarBackgroundColor = from.AppbarBackgroundColor;
            target.PrimaryColor = from.PrimaryColor;
            target.SecondaryColor = from.SecondaryColor;
        }
    }
}