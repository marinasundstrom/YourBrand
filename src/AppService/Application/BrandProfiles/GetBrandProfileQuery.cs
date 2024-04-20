
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.Application.BrandProfiles;

public record GetBrandProfileQuery() : IRequest<BrandProfileDto?>
{
    public class Handler(IAppServiceContext appServiceContext) : IRequestHandler<GetBrandProfileQuery, BrandProfileDto?>
    {
        public async Task<BrandProfileDto?> Handle(GetBrandProfileQuery request, CancellationToken cancellationToken)
        {
            var brandProfile = await appServiceContext.BrandProfiles
                .OrderBy(x => x.Created)
                .FirstOrDefaultAsync(cancellationToken);

            return brandProfile?.ToDto();
        }
    }
}

public record BrandProfileDto(string Id, string Name, string? Description, BrandColorsDto Colors);

public record BrandColorsDto(BrandColorPaletteDto? Light, BrandColorPaletteDto? Dark);

public record BrandColorPaletteDto(string? BackgroundColor, string? AppbarBackgroundColor, string? PrimaryColor, string? SecondaryColor);