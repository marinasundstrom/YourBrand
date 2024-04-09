
using MediatR;

using YourBrand.Identity;

namespace YourBrand.Application.Notifications.Queries;

public record GetNotificationsQuery(bool IncludeUnreadNotificationsCount,
        int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<NotificationsResults>
{
    public sealed class GetNotificationsQueryHandler(YourBrand.Notifications.Client.INotificationsClient notificationsClient, IUserContext userContext) : IRequestHandler<GetNotificationsQuery, NotificationsResults>
    {
        public async Task<NotificationsResults> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = userContext.UserId;

            var results = await notificationsClient.GetNotificationsAsync(userId, null, request.IncludeUnreadNotificationsCount, request.Page, request.PageSize, request.SortBy, (YourBrand.Notifications.Client.SortDirection?)request.SortDirection);
            var notifications = results.Items;

            return new NotificationsResults(
                notifications.Select(notification => new NotificationDto(notification.Id, notification.Content, notification.Link, notification.Published.GetValueOrDefault().DateTime, notification.IsRead)),
                results.UnreadNotificationsCount,
                results.TotalCount);
        }
    }
}