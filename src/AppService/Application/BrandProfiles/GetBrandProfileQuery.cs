using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Themes;
using YourBrand.Domain.Entities;

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

            if (brandProfile is null)
            {
                brandProfile = new BrandProfile("Brand", null) 
                {
                    Theme = new Theme("Theme", null)
                    {
                        Colors = new ThemeColors
                        {
                            Light = new ThemeColorPalette(),
                            Dark = new ThemeColorPalette()
                        }
                    }
                };
            }

            return brandProfile?.ToDto();
        }
    }
}

public record BrandProfileDto(string Id, string Name, string? Description, ThemeDto? Theme);