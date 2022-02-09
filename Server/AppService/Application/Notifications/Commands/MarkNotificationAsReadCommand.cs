
using System.Data.Common;

using Catalog.Application.Common.Interfaces;
using Catalog.Domain.Entities;
using Catalog.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Client;

namespace Catalog.Application.Notifications.Commands;

public class MarkNotificationAsReadCommand : IRequest
{
    public MarkNotificationAsReadCommand(string notificationId)
    {
        NotificationId = notificationId;
    }

    public string NotificationId { get; }

    public class MarkNotificationAsReadCommandHandler : IRequestHandler<MarkNotificationAsReadCommand>
    {
        private readonly INotificationsClient _notificationsClient;

        public MarkNotificationAsReadCommandHandler(Worker.Client.INotificationsClient notificationsClient)
        {
            _notificationsClient = notificationsClient;
        }

        public async Task<Unit> Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await _notificationsClient.MarkNotificationAsReadAsync(request.NotificationId);

            return Unit.Value;
        }
    }
}