
using Hangfire;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Notifications.Application.Common.Interfaces;

namespace YourBrand.Notifications.Application.Notifications.Commands;

public record DeleteNotificationCommand(string NotificationId) : IRequest
{

    public class DeleteNotificationCommandHandler(IWorkerContext context, IBackgroundJobClient backgroundJobClient) : IRequestHandler<DeleteNotificationCommand>
    {
        public async Task Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(i => i.Id == request.NotificationId, cancellationToken);

            if (notification is null)
            {
                throw new Exception();
            }

            if (notification.ScheduledJobId is not null && notification.Published is null)
            {
                backgroundJobClient.Delete(notification.ScheduledJobId);
            }

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}