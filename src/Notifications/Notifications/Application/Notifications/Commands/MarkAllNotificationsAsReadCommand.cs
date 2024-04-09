
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Application.Common.Interfaces;

namespace YourBrand.Notifications.Application.Notifications.Commands;

public record MarkAllNotificationsAsReadCommand : IRequest
{
    public class MarkAllNotificationsAsReadCommandHandler(IWorkerContext context) : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            var notifications = await context.Notifications
                .Where(n => n.Published != null)
                .ToListAsync(cancellationToken);

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
                notification.Read = DateTime.Now;
            }

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}