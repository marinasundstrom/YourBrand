
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
            var brandProfile = await appServiceContext.BrandProfiles.FirstOrDefaultAsync(cancellationToken);

            return brandProfile?.ToDto();
        }
    }
}

public record BrandProfileDto(string Id, string? BackgroundColor, string? AppbarBackgroundColor, string? PrimaryColor, string? SecondaryColor);