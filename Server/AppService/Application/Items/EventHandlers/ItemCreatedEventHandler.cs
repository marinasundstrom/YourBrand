using System;

using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.Items.EventHandlers;

public class ItemCreatedEventHandler : INotificationHandler<DomainEventNotification<ItemCreatedEvent>>
{
    private readonly ICatalogContext _context;
    private readonly IUrlHelper _urlHelper;
    private readonly IItemsClient _itemsClient;

    public ItemCreatedEventHandler(ICatalogContext context, IUrlHelper urlHelper, IItemsClient itemsClient)
    {
        _context = context;
        _urlHelper = urlHelper;
        _itemsClient = itemsClient;
    }

    public async Task Handle(DomainEventNotification<ItemCreatedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var item = await _context.Items.FirstAsync(i => i.Id == domainEvent.ItemId, cancellationToken);

        var itemDto = new ItemDto(item.Id, item.Name, item.Description, _urlHelper.CreateImageUrl(item.Image), item.CommentCount, item.Created, item.CreatedBy, item.LastModified, item.LastModifiedBy);

        await _itemsClient.ItemAdded(itemDto);
    }
}