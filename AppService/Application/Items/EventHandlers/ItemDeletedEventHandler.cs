
using YourBrand.Application.Common.Interfaces;
using YourBrand.Application.Common.Models;
using YourBrand.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Application.Items.EventHandlers;

public class ItemDeletedEventHandler : IDomainEventHandler<ItemDeletedEvent>
{
    private readonly ICatalogContext _context;
    private readonly IItemsClient _itemsClient;

    public ItemDeletedEventHandler(ICatalogContext context, IItemsClient itemsClient)
    {
        _context = context;
        _itemsClient = itemsClient;
    }

    public async Task Handle(ItemDeletedEvent notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification;

        await _itemsClient.ItemDeleted(domainEvent.Id, domainEvent.Name);
    }
}