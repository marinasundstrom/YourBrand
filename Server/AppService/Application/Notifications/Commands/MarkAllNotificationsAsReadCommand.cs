
using Catalog.Application.Common.Interfaces;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Client;

namespace Catalog.Application.Notifications.Commands;

public class MarkAllNotificationsAsReadCommand : IRequest
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