
using Skynet.Application.Common.Interfaces;
using Skynet.Application.Common.Models;
using Skynet.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Skynet.Application.Items.EventHandlers;

public class ItemDeletedEventHandler : INotificationHandler<DomainEventNotification<ItemDeletedEvent>>
{
    private readonly ICatalogContext _context;
    private readonly IItemsClient _itemsClient;

    public ItemDeletedEventHandler(ICatalogContext context, IItemsClient itemsClient)
    {
        _context = context;
        _itemsClient = itemsClient;
    }

    public async Task Handle(DomainEventNotification<ItemDeletedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        await _itemsClient.ItemDeleted(domainEvent.Id, domainEvent.Name);
    }
}