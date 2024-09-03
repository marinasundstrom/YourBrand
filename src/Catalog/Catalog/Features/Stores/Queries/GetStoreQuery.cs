using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Catalog.Persistence;
using YourBrand.Identity;

namespace YourBrand.Catalog.Features.Stores.Queries;

public sealed record GetStoreQuery(string OrganizationId, string IdOrHandle) : IRequest<StoreDto?>
{
    sealed class GetStoreQueryHandler(
        CatalogContext context) : IRequestHandler<GetStoreQuery, StoreDto?>
    {
        public async Task<StoreDto?> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            var store = await context
               .Stores
               .InOrganization(request.OrganizationId)
               .Include(x => x.Currency)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.IdOrHandle || c.Handle == request.IdOrHandle);

            return store?.ToDto();
        }
    }
}