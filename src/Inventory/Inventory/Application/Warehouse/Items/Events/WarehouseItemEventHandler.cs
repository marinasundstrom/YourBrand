using MassTransit;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Inventory.Application.Common.Interfaces;
using YourBrand.Inventory.Contracts;
using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;
using YourBrand.Inventory.Domain.Events;
using YourBrand.Notifications;
using YourBrand.Notifications.Contracts;

namespace YourBrand.Inventory.Application.Warehouses.Items.Events;

public class WarehouseItemEventHandler(
    IInventoryContext context, IPublishEndpoint publishEndpoint, INotificationService notificationService,
    ILogger<WarehouseItemEventHandler> logger, IUserContext userContext)
: IDomainEventHandler<WarehouseItemCreated>,
  IDomainEventHandler<WarehouseItemQuantityOnHandUpdated>,
  IDomainEventHandler<WarehouseItemsPicked>,
  IDomainEventHandler<WarehouseItemsReserved>,
  IDomainEventHandler<WarehouseItemQuantityAvailableUpdated>
{
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
        var item = await context.WarehouseItems
            .Include(x => x.Item)
            .Include(x => x.Warehouse)
            .ThenInclude(x => x.Site)
            .FirstOrDefaultAsync(i => i.Id == notification.ItemId);

        if (item is null)
        {
            return;
        }

        await publishEndpoint.Publish(new QuantityAvailableChanged(item.ItemId, notification.WarehouseId, notification.Quantity));

        if (notification.Quantity <= item.QuantityThreshold
                && notification.OldQuantity > item.QuantityThreshold)
        {
            await SendEmail(item);

            await PostNotification(item);
        }
    }

    private async Task SendEmail(WarehouseItem? item)
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
        await publishEndpoint.Publish(new SendEmail("test@email.com", subject, body));
    }

    private async Task PostNotification(WarehouseItem? item)
    {
        try
        {
            await notificationService.PublishNotificationAsync(new Notification($"Quantity available of {item.Item.Name} is below threshold.")
            {
                UserId = item.CreatedById,
                //Link = Link
            });
        }
        catch (Exception exc)
        {
            logger.LogError(exc, "Failed to post notification.");
        }
    }
}