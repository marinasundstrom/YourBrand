using System.Diagnostics.Contracts;

using MassTransit;

using YourBrand.Notifications.Contracts;

using MediatR;
using YourBrand.Identity;
using YourBrand.Tenancy;

namespace YourBrand.Application.Notifications.Commands;

public sealed record CreateNotificationCommand(
    string Content,
    string Link,
    string UserId,
    DateTimeOffset? ScheduledFor
    ) : IRequest
{
    public class CreateNotificationCommandHandler(IRequestClient<SendNotification> notificationsClient, ICurrentUserService currentUserService, ITenantService tenantService) : IRequestHandler<CreateNotificationCommand>
    {
        public async Task Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
        {
            await notificationsClient.GetResponse<SendNotificationResponse>(new SendNotification
            {
                TenantId = tenantService.TenantId!,
                Content = request.Content,
                Link = request.Link,
                UserId = request.UserId,
                ScheduledFor = request.ScheduledFor,
                CreatedById = currentUserService.UserId!
            }, cancellationToken);
        }
    }
}