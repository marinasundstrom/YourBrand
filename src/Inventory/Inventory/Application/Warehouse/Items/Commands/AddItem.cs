
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record AddWarehouseItem(string ItemId, string WarehouseId, string Location, int QuantityOnHand, int QuantityThreshold) : IRequest<WarehouseItemDto>
{
    public class Handler(IInventoryContext context) : IRequestHandler<AddWarehouseItem, WarehouseItemDto>
    {
        public async Task<WarehouseItemDto> Handle(AddWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await context.WarehouseItems.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item is not null) throw new Exception();

            item = new Domain.Entities.WarehouseItem(request.ItemId, request.WarehouseId, request.Location, request.QuantityOnHand, request.QuantityThreshold); ;

            context.WarehouseItems.Add(item);

            await context.SaveChangesAsync(cancellationToken);

            item = await context.WarehouseItems
               .Include(x => x.Item)
               .ThenInclude(x => x.Group)
               .Include(x => x.Warehouse)
               .ThenInclude(x => x.Site)
               .AsNoTracking()
               .FirstAsync(c => c.Id == item.Id);

            return item.ToDto();
        }
    }
}