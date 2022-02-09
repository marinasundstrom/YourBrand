
using System.Data.Common;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;
using Worker.Domain.Entities;
using Worker.Domain.Events;

namespace Worker.Application.Notifications.Commands;

public class MarkNotificationAsReadCommand : IRequest
{
    public MarkNotificationAsReadCommand(string notificationId)
    {
        NotificationId = notificationId;
    }

    public string NotificationId { get; }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly IWorkerContext context;

        public MarkNotificationAsReadCommandHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            var notification = await context.Notifications
                .Where(n => n.Published != null)
                .FirstOrDefaultAsync(i => i.Id == request.NotificationId, cancellationToken);

            if (notification is null)
            {
                throw new Exception();
            }

            notification.IsRead = true;
            notification.Read = DateTime.Now;

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}