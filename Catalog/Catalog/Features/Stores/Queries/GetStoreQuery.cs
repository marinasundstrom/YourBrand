using YourBrand.Catalog;
using YourBrand.Catalog.Common;
using YourBrand.Catalog.Features.Stores;
using YourBrand.Catalog.Persistence;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Catalog.Features.Stores.Queries;

public sealed record GetStoreQuery(string IdOrHandle) : IRequest<StoreDto?>
{
    sealed class GetStoreQueryHandler : IRequestHandler<GetStoreQuery, StoreDto?>
    {
        private readonly CatalogContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetStoreQueryHandler(
            CatalogContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<StoreDto?> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            var store = await _context
               .Stores
               .Include(x => x.Currency)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.IdOrHandle || c.Handle == request.IdOrHandle);

            return store?.ToDto();
        }
    }
}