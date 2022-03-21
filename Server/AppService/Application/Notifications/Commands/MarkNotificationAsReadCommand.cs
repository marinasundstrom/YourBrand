
using System.Data.Common;

using YourCompany.Application.Common.Interfaces;
using YourCompany.Domain.Entities;
using YourCompany.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Client;

namespace YourCompany.Application.Notifications.Commands;

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