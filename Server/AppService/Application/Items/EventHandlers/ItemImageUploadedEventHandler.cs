
using YourCompany.Application.Common.Interfaces;
using YourCompany.Application.Common.Models;
using YourCompany.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourCompany.Application.Items.EventHandlers;

public class ItemImageUploadedEventHandler : INotificationHandler<DomainEventNotification<ItemImageUploadedEvent>>
{
    private readonly ICatalogContext _context;
    private readonly IUrlHelper _urlHelper;
    private readonly IItemsClient _itemsClient;

    public ItemImageUploadedEventHandler(ICatalogContext context, IUrlHelper urlHelper, IItemsClient itemsClient)
    {
        _context = context;
        _urlHelper = urlHelper;
        _itemsClient = itemsClient;
    }

    public async Task Handle(DomainEventNotification<ItemImageUploadedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var item = await _context.Items.FirstAsync(i => i.Id == domainEvent.Id, cancellationToken);

        await _itemsClient.ImageUploaded(domainEvent.Id, _urlHelper.CreateImageUrl(item.Image)!);
    }
}