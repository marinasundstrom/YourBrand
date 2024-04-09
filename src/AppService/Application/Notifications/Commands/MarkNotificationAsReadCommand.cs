using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Notifications.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Application.Notifications.Commands;


public sealed record MarkNotificationAsReadCommand(string NotificationId) : IRequest
{
    public sealed class MarkNotificationAsReadCommandHandler(IRequestClient<MarkNotificationAsRead> notificationsClient, ITenantContext tenantContext) : IRequestHandler<MarkNotificationAsReadCommand>
    {
        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await notificationsClient.GetResponse<MarkNotificationAsReadResponse>(new MarkNotificationAsRead
            {
                TenantId = tenantContext.TenantId!,
                NotificationId = request.NotificationId
            }, cancellationToken);
        }
    }
}