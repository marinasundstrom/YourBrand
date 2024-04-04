using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Notifications.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Application.Notifications.Commands;


public sealed record MarkNotificationAsReadCommand(string NotificationId) : IRequest
{
    public class MarkNotificationAsReadCommandHandler(IRequestClient<MarkNotificationAsRead> notificationsClient, ICurrentUserService currentUserService, ITenantService tenantService) : IRequestHandler<MarkNotificationAsReadCommand>
    {
        public async Task Handle(MarkNotificationAsReadCommand request, CancellationToken cancellationToken)
        {
            await notificationsClient.GetResponse<MarkNotificationAsReadResponse>(new MarkNotificationAsRead
            {
                TenantId = tenantService.TenantId!,
                NotificationId = request.NotificationId,
                CreatedById = currentUserService.UserId!
            }, cancellationToken);
        }
    }
}