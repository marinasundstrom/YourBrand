using YourBrand.Inventory.Application.Common.Models;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Application.Common.Interfaces;

namespace YourBrand.Inventory.Application.Warehouses.Items.Events;

public class WarehouseItemEventHandler 
: IDomainEventHandler<WarehouseItemCreated>, 
  IDomainEventHandler<WarehouseItemQuantityOnHandUpdated>, 
  IDomainEventHandler<WarehouseItemsPicked>,
  IDomainEventHandler<WarehouseItemsReserved>,
  IDomainEventHandler<WarehouseItemQuantityAvailableUpdated>
{
    private readonly IInventoryContext _context;

    public WarehouseItemEventHandler(IInventoryContext context)
    {
        _context = context;
    }

    public async Task Handle(WarehouseItemCreated notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemQuantityOnHandUpdated notification, CancellationToken cancellationToken)
    {
        var item = await _context.WarehouseItems
            .FirstOrDefaultAsync(i => i.Id == notification.ItemId);

        if(item is not null) 
        {
            if(notification.Quantity <= item.QuantityThreshold  
                && notification.OldQuantity > item.QuantityThreshold) 
            {
                // Ha fallen below threshold
            }
        }
    }

    public async Task Handle(WarehouseItemsPicked notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemsReserved notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemQuantityAvailableUpdated notification, CancellationToken cancellationToken)
    {
    }
}