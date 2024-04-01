using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Application.Common.Interfaces;

namespace YourBrand.Notifications.Application.Notifications.Commands;

public record MarkNotificationAsReadCommand(string NotificationId) : IRequest
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly IWorkerContext context;

        public MarkNotificationAsReadCommandHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
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

        }
    }
}