using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Themes;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.BrandProfiles;

public record SetBrandProfileTheme(string ThemeId) : IRequest<BrandProfileDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<SetBrandProfileTheme, BrandProfileDto>
    {
        public async Task<BrandProfileDto?> Handle(SetBrandProfileTheme request, CancellationToken cancellationToken)
        {
            BrandProfile? brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            if(brandProfile is null)
            {
                brandProfile = new BrandProfile("Brand", null);
                appServiceContext.BrandProfiles.Add(brandProfile);
            }

            brandProfile.Theme = await appServiceContext.Themes.FirstOrDefaultAsync(t => t.Id == request.ThemeId, cancellationToken);

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