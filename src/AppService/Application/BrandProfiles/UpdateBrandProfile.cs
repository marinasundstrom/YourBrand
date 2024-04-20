using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;

namespace YourBrand.Application.BrandProfiles;

public record UpdateBrandProfile(string Name, string? Description, BrandColorsDto Colors) : IRequest<BrandProfileDto>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<UpdateBrandProfile, BrandProfileDto>
    {
        public async Task<BrandProfileDto?> Handle(UpdateBrandProfile request, CancellationToken cancellationToken)
        {
            BrandProfile brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstAsync(cancellationToken);

            bool @new = false;

            if(brandProfile is null) 
            {
                brandProfile = new BrandProfile(request.Name, request.Description);
                @new = true;
            }
            else 
            {
                brandProfile.Name = request.Name;
                brandProfile.Description = request.Description;
            }

            brandProfile.Colors ??= new BrandColors();

            Map(brandProfile.Colors.Light, request.Colors.Light);
            Map(brandProfile.Colors.Dark, request.Colors.Dark);

            if(@new)
            {
                appServiceContext.BrandProfiles.Add(brandProfile);
            }

            await appServiceContext.SaveChangesAsync(cancellationToken);

            brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstAsync(cancellationToken);

            return brandProfile.ToDto();
        }

        private void Map(BrandColorPalette target, BrandColorPaletteDto? from)
        {
            target.BackgroundColor = from.BackgroundColor;
            target.AppbarBackgroundColor = from.AppbarBackgroundColor;
            target.PrimaryColor = from.PrimaryColor;
            target.SecondaryColor = from.SecondaryColor;
        }
    }
}
