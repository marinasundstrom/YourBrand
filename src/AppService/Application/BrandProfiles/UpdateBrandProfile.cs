using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Themes;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.BrandProfiles;

public record UpdateBrandProfile(string Name, string? Description, ThemeDto Theme) : IRequest<BrandProfileDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<UpdateBrandProfile, BrandProfileDto>
    {
        public async Task<BrandProfileDto?> Handle(UpdateBrandProfile request, CancellationToken cancellationToken)
        {
            BrandProfile? brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            bool @new = false;

            if (brandProfile is null)
            {
                brandProfile = new BrandProfile(request.Name, request.Description);
                @new = true;

                await appServiceContext.SaveChangesAsync(cancellationToken);

            }
            else
            {
                brandProfile.Name = request.Name;
                brandProfile.Description = request.Description;
            }

            if(brandProfile.Theme is null) 
            {
                var theme = new Theme("Theme", null) 
                {
                    ColorSchemes = new ThemeColorSchemes
                    {
                        Light = new ThemeColorScheme(),
                        Dark = new ThemeColorScheme()
                    }
                };

                appServiceContext.Themes.Add(theme);

                await appServiceContext.SaveChangesAsync(cancellationToken);

                brandProfile.Theme = theme;
            }

            Map(brandProfile.Theme.ColorSchemes.Light, request.Theme.ColorSchemes.Light);
            Map(brandProfile.Theme.ColorSchemes.Dark, request.Theme.ColorSchemes.Dark);

            if (@new)
            {
                appServiceContext.BrandProfiles.Add(brandProfile);
            }

            await appServiceContext.SaveChangesAsync(cancellationToken);

            brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstAsync(cancellationToken);

            return brandProfile.ToDto();
        }

        private void Map(ThemeColorScheme target, ThemeColorSchemeDto? from)
        {
            target.BackgroundColor = from.BackgroundColor;
            target.AppbarBackgroundColor = from.AppbarBackgroundColor;
            target.AppbarTextColor = from.AppbarTextColor;
            target.PrimaryColor = from.PrimaryColor;
            target.SecondaryColor = from.SecondaryColor;
        }
    }
}