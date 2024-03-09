using MediatR;

using YourBrand.Notifications.Client;

namespace YourBrand.Application.Notifications.Commands;

public sealed record CreateNotificationCommand(
    string Title,
    string Text,
    string Link,
    string UserId,
    DateTimeOffset? ScheduledFor
    ) : IRequest
{
    public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand>
    {
        private readonly INotificationsClient _notificationsClient;

        public CreateNotificationCommandHandler(YourBrand.Notifications.Client.INotificationsClient notificationsClient)
        {
            _notificationsClient = notificationsClient;
        }

        public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            await _notificationsClient.CreateNotificationAsync(new CreateNotificationDto {
                Title = request.Title,
                Text = request.Text,
                Link = request.Link,
                UserId = request.UserId,
                ScheduledFor = request.ScheduledFor
            }, cancellationToken);
        }
    }
}