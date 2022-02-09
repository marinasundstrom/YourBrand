
using System.Data.Common;

using MediatR;

using Worker.Application.Common.Interfaces;
using Worker.Domain.Entities;
using Worker.Domain.Events;

namespace Worker.Application.Notifications.Commands;

public class CreateNotificationCommand : IRequest
{
    public string Title { get; set; } = null!;

    public string? Text { get; set; } = null!;

    public string? Link { get; set; }

    public string? UserId { get; set; }

    public DateTime? ScheduledFor { get; set; }

    public CreateNotificationCommand(string title, string? text, string? link, string? userId, DateTime? scheduledFor)
    {
        Title = title;
        Text = text;
        Link = link;
        UserId = userId;
        ScheduledFor = scheduledFor;
    }

    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
    {
        private readonly IWorkerContext context;

        public CreateNotificationCommandHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task<Unit> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
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

            return Unit.Value;
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
            notification.Title = request.Title;
            notification.Text = request.Text;
            notification.Link = request.Link;
            notification.UserId = userId ?? request.UserId;
            notification.ScheduledFor = request.ScheduledFor;

            notification.DomainEvents.Add(new NotificationCreatedEvent(notification.Id));
            return notification;
        }
    }
}