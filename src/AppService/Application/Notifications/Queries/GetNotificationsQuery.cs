
using MediatR;

using YourBrand.Identity;

namespace YourBrand.Application.Notifications.Queries;

public record GetNotificationsQuery(bool IncludeUnreadNotificationsCount,
        int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<NotificationsResults>
{
    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, NotificationsResults>
    {
        private readonly YourBrand.Notifications.Client.INotificationsClient _notificationsClient;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationsQueryHandler(YourBrand.Notifications.Client.INotificationsClient notificationsClient, ICurrentUserService currentUserService)
        {
            _notificationsClient = notificationsClient;
            _currentUserService = currentUserService;
        }

        public async Task<NotificationsResults> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var results = await _notificationsClient.GetNotificationsAsync(userId, null, request.IncludeUnreadNotificationsCount, request.Page, request.PageSize, request.SortBy, (YourBrand.Notifications.Client.SortDirection?)request.SortDirection);
            var notifications = results.Items;

            return new NotificationsResults(
                notifications.Select(notification => new NotificationDto(notification.Id, notification.Title, notification.Text, notification.Link, notification.Published.GetValueOrDefault().DateTime, notification.IsRead)),
                results.UnreadNotificationsCount,
                results.TotalCount);
        }
    }
}