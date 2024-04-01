using MediatR;

using YourBrand.Notifications.Client;

namespace YourBrand.Application.Notifications.Commands;

public record MarkNotificationAsReadCommand(string NotificationId) : IRequest
{
    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly INotificationsClient _notificationsClient;

        public MarkNotificationAsReadCommandHandler(YourBrand.Notifications.Client.INotificationsClient notificationsClient)
        {
            _notificationsClient = notificationsClient;
        }

        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationsClient.MarkNotificationAsReadAsync(request.NotificationId);

        }
    }
}