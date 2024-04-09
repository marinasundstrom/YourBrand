using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Brands.Queries;

public sealed record GetBrandQuery(int Id) : IRequest<BrandDto?>
{
    sealed class GetBrandQueryHandler(
        CatalogContext context,
        IUserContext userContext) : IRequestHandler<GetBrandQuery, BrandDto?>
    {
        public async Task<BrandDto?> Handle(GetBrandQuery request, CancellationToken cancellationToken)
        {
            var brand = await context
               .Brands
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (brand is null)
            {
                return null;
            }

            return brand.ToDto();
        }
    }
}