
using YourBrand.Inventory.Domain;

using MediatR;
using YourBrand.Inventory.Application.Warehouses.Items;
using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Warehouses.Items.Commands;

public record AddWarehouseItem(string ItemId, string WarehouseId, string Location, int QuantityOnHand, int QuantityThreshold) : IRequest<WarehouseItemDto>
{
    public class Handler : IRequestHandler<AddWarehouseItem, WarehouseItemDto>
    {
        private readonly IInventoryContext _context;

        public Handler(IInventoryContext context)
        {
            _context = context;
        }

        public async Task<WarehouseItemDto> Handle(AddWarehouseItem request, CancellationToken cancellationToken)
        {
            var item = await _context.WarehouseItems.FirstOrDefaultAsync(i => i.Id == request.ItemId, cancellationToken);

            if (item is not null) throw new Exception();

            item = new Domain.Entities.WarehouseItem(request.ItemId, request.WarehouseId, request.Location, request.QuantityOnHand, request.QuantityThreshold); ;

            _context.WarehouseItems.Add(item);

            await _context.SaveChangesAsync(cancellationToken);

            item = await _context.WarehouseItems
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
