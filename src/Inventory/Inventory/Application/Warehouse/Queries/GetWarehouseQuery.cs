using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Queries;

public record GetWarehouseQuery(string Id) : IRequest<WarehouseDto?>
{
    sealed class GetWarehouseQueryHandler(
        IInventoryContext context,
        IUserContext userContext) : IRequestHandler<GetWarehouseQuery, WarehouseDto?>
    {
        public async Task<WarehouseDto?> Handle(GetWarehouseQuery request, CancellationToken cancellationToken)
        {
            var warehouse = await context.Warehouses
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