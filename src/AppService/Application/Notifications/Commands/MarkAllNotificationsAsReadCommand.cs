
using MassTransit;

using MediatR;

using YourBrand.Identity;
using YourBrand.Notifications.Contracts;
using YourBrand.Tenancy;

namespace YourBrand.Application.Notifications.Commands;

public sealed record MarkAllNotificationsAsReadCommand() : IRequest
{
    public class MarkAllNotificationsAsReadCommandHandler(IRequestClient<MarkAllNotificationsAsRead> notificationsClient, ICurrentUserService currentUserService, ITenantService tenantService) : IRequestHandler<MarkAllNotificationsAsReadCommand>
    {
        public async Task Handle(MarkAllNotificationsAsReadCommand request, CancellationToken cancellationToken)
        {
            await notificationsClient.GetResponse<MarkAllNotificationsAsReadResponse>(new MarkAllNotificationsAsRead
            {
                TenantId = tenantService.TenantId!,
                CreatedById = currentUserService.UserId!
            }, cancellationToken);
        }
    }
}