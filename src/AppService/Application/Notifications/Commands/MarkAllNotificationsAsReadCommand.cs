
using MediatR;

using YourBrand.Notifications.Client;

namespace YourBrand.Application.Notifications.Commands;

public sealed record MarkAllNotificationsAsReadCommand : IRequest
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        private readonly INotificationsClient _notificationsClient;

        public MarkAllNotificationsAsReadCommandHandler(YourBrand.Notifications.Client.INotificationsClient notificationsClient)
        {
            _notificationsClient = notificationsClient;
        }

        public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationsClient.MarkAllNotificationsAsReadAsync(cancellationToken);
        }
    }
}