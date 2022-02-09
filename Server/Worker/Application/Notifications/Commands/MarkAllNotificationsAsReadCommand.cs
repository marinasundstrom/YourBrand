
using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Notifications.Commands;

public class MarkAllNotificationsAsReadCommand : IRequest
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        private readonly IWorkerContext context;

        public MarkAllNotificationsAsReadCommandHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
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

            return Unit.Value;
        }
    }
}