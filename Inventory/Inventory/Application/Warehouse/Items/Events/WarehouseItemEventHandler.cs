using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Events;
using YourBrand.Inventory.Application.Common.Interfaces;
using YourBrand.Notifications.Contracts;
using MassTransit;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Inventory.Application.Warehouses.Items.Events;

public class WarehouseItemEventHandler 
: IDomainEventHandler<WarehouseItemCreated>, 
  IDomainEventHandler<WarehouseItemQuantityOnHandUpdated>, 
  IDomainEventHandler<WarehouseItemsPicked>,
  IDomainEventHandler<WarehouseItemsReserved>,
  IDomainEventHandler<WarehouseItemQuantityAvailableUpdated>
{
    private readonly IInventoryContext _context;
    private readonly IPublishEndpoint _publishEndpoint;

    public WarehouseItemEventHandler(IInventoryContext context, IPublishEndpoint publishEndpoint)
    {
        _context = context;
        _publishEndpoint = publishEndpoint;
    }

    public async Task Handle(WarehouseItemCreated notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemQuantityOnHandUpdated notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemsPicked notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemsReserved notification, CancellationToken cancellationToken)
    {
    }

    public async Task Handle(WarehouseItemQuantityAvailableUpdated notification, CancellationToken cancellationToken)
    {
        var item = await _context.WarehouseItems
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .ThenInclude(x => x.Site)
            .FirstOrDefaultAsync(i => i.Id == notification.ItemId);

        if(item is not null) 
        {
            if(notification.Quantity <= item.QuantityThreshold  
                && notification.OldQuantity > item.QuantityThreshold) 
            {
                var subject = $"Quantity available is below threshold for {item.Item.Name} ({item.Item.Id})";

                var body = @$"
                Quantity available is below threshold.<br />
                <br />
                <b>Item:</b> {item.Item.Name} ({item.Item.Id})<br />
                <b>Warehouse:</b>  {item.Warehouse.Name}<br />
                <b>Site:</b>  {item.Warehouse.Site.Name}<br />
                <b>Quantity available:</b>  {item.QuantityAvailable}<br />
                ";
                await _publishEndpoint.Publish(new SendEmail("test@email.com", subject, body));
            }
        }
    }
}