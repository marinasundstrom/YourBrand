
using YourBrand.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Client;

namespace YourBrand.Application.Notifications.Commands;

public record MarkAllNotificationsAsReadCommand : IRequest
{
    public class MarkAllNotificationsAsReadCommandHandler : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        private readonly INotificationsClient _notificationsClient;

        public MarkAllNotificationsAsReadCommandHandler(Worker.Client.INotificationsClient notificationsClient)
        {
            _notificationsClient = notificationsClient;
        }

        public async Task<Unit> Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationsClient.MarkAllNotificationsAsReadAsync();

            return Unit.Value;
        }
    }
}