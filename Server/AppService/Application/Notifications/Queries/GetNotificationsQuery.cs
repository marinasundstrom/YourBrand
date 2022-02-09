
using Catalog.Application.Common.Interfaces;
using Catalog.Application.Common.Models;
using Catalog.Domain;

using MediatR;

using Microsoft.EntityFrameworkCore;

namespace Catalog.Application.Notifications.Queries;

public class GetNotificationsQuery : IRequest<NotificationsResults>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; }
    public Application.Common.Models.SortDirection? SortDirection { get; }
    public bool IncludeUnreadNotificationsCount { get; }

    public GetNotificationsQuery(
        bool includeUnreadNotificationsCount,
        int page, int pageSize, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        IncludeUnreadNotificationsCount = includeUnreadNotificationsCount;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, NotificationsResults>
    {
        private readonly Worker.Client.INotificationsClient _notificationsClient;
        private readonly ICurrentUserService _currentUserService;

        public GetNotificationsQueryHandler(Worker.Client.INotificationsClient notificationsClient, ICurrentUserService currentUserService)
        {
            _notificationsClient = notificationsClient;
            _currentUserService = currentUserService;
        }

        public async Task<NotificationsResults> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var userId = _currentUserService.UserId;

            var results = await _notificationsClient.GetNotificationsAsync(userId, null, request.IncludeUnreadNotificationsCount, request.Page, request.PageSize, request.SortBy, (Worker.Client.SortDirection?)request.SortDirection);
            var notifications = results.Items;

            return new NotificationsResults(
                notifications.Select(notification => new NotificationDto(notification.Id, notification.Title, notification.Text, notification.Link, notification.Published.GetValueOrDefault().DateTime, notification.IsRead)),
                results.UnreadNotificationsCount,
                results.TotalCount);
        }
    }
}