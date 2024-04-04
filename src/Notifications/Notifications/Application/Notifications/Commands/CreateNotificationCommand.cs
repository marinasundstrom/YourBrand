using MediatR;

using YourBrand.Notifications.Application.Common.Interfaces;
using YourBrand.Notifications.Domain.Entities;
using YourBrand.Notifications.Domain.Events;

namespace YourBrand.Notifications.Application.Notifications.Commands;

public record CreateNotificationCommand(string? Content, string? Link, string? UserId, DateTimeOffset? ScheduledFor) : IRequest
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
    {
        private readonly IWorkerContext context;

        public CreateNotificationCommandHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            if (request.UserId is not null)
            {
                CreateNotification(request);
            }
            else
            {
                await CreateMultipleNotifications(request);
            }

            await context.SaveChangesAsync(cancellationToken);

        }

        private void CreateNotification(CreateNotificationCommand request)
        {
            Notification notification = CreateNotificationDO(request, null);

            context.Notifications.Add(notification);
        }

        private async Task CreateMultipleNotifications(CreateNotificationCommand request)
        {
            var users = await GetUsers();

            foreach (var user in users)
            {
                var userId = user;

                Notification notification = CreateNotificationDO(request, userId);

                context.Notifications.Add(notification);
            }
        }

        private static Task<IEnumerable<string>> GetUsers()
        {
            return Task.FromResult<IEnumerable<string>>(new string[] { "AliceSmith@email.com", "BobSmith@email.com" });
        }

        private static Notification CreateNotificationDO(CreateNotificationCommand request, string? userId)
        {
            var notification = new Notification();
            notification.Id = Guid.NewGuid().ToString();
            notification.Content = request.Content;
            notification.Link = request.Link;
            notification.UserId = userId ?? request.UserId;
            notification.ScheduledFor = request.ScheduledFor;

            notification.AddDomainEvent(new NotificationCreatedEvent(notification.Id));
            return notification;
        }
    }
}