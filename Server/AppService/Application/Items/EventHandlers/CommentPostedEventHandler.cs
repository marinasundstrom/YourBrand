using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Items.EventHandlers;

public class CommentPostedEventHandler : INotificationHandler<DomainEventNotification<CommentPostedEvent>>
{
    private readonly ICatalogContext context;

    public CommentPostedEventHandler(ICatalogContext context)
    {
        this.context = context;
    }

    public async Task Handle(DomainEventNotification<CommentPostedEvent> notification, CancellationToken cancellationToken)
    {
        var domainEvent = notification.DomainEvent;

        var item = await context.Items
            .FirstOrDefaultAsync(i => i.Id == domainEvent.ItemId, cancellationToken);

        if (item is null) return;

        item.CommentCount = await context.Comments.CountAsync(c => c.Item.Id == domainEvent.ItemId);

        await context.SaveChangesAsync(cancellationToken);
    }
}