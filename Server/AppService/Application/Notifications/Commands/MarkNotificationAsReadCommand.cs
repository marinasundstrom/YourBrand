
using System.Data.Common;

using YourBrand.Application.Common.Interfaces;
using YourBrand.Domain.Entities;
using YourBrand.Domain.Events;

using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Client;

namespace YourBrand.Application.Notifications.Commands;

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