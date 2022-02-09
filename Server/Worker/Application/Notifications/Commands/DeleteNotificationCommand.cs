
using Hangfire;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Notifications.Commands;

public class DeleteNotificationCommand : IRequest
{
    public DeleteNotificationCommand(string notificationId)
    {
        NotificationId = notificationId;
    }

    public string NotificationId { get; }

    public class DeleteNotificationCommandHandler : IRequestHandler<DeleteNotificationCommand>
    {
        private readonly IWorkerContext context;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public DeleteNotificationCommandHandler(IWorkerContext context, IBackgroundJobClient backgroundJobClient)
        {
            this.context = context;
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task<Unit> Handle(DeleteNotificationCommand request, CancellationToken cancellationToken)
        {
            var notification = await context.Notifications
                .FirstOrDefaultAsync(i => i.Id == request.NotificationId, cancellationToken);

            if (notification is null)
            {
                throw new Exception();
            }

            if (notification.ScheduledJobId is not null && notification.Published is null)
            {
                _backgroundJobClient.Delete(notification.ScheduledJobId);
            }

            context.Notifications.Remove(notification);

            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}