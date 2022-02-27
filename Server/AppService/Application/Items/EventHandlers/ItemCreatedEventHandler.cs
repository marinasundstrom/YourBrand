using System;

using Skynet.Application.Common.Interfaces;
using Skynet.Application.Common.Models;
using Skynet.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Skynet.Application.Items.EventHandlers;

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

        var itemDto = new ItemDto(item.Id, item.Name, item.Description, _urlHelper.CreateImageUrl(item.Image), item.CommentCount, item.Created, item.CreatedById, item.LastModified, item.LastModifiedById);

        await _itemsClient.ItemAdded(itemDto);
    }
}