using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Queries;

public record GetWarehouseQuery(string Id) : IRequest<WarehouseDto?>
{
    class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQuery, WarehouseDto?>
    {
        private readonly IInventoryContext _context;
        private readonly ICurrentUserService currentUserService;

        public GetWarehouseQueryHandler(
            IInventoryContext context,
            ICurrentUserService currentUserService)
        {
            _context = context;
            this.currentUserService = currentUserService;
        }

        public async Task<WarehouseDto?> Handle(GetWarehouseQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await _context.Warehouses
                .Include(x => x.Site)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (warehouse is null)
            {
                return null;
            }

            return warehouse.ToDto();
        }
    }
}