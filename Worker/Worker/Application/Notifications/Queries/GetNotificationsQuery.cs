
using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;
using Worker.Application.Common.Models;
using Worker.Domain;

namespace Worker.Application.Notifications.Queries;

public class GetNotificationsQuery : IRequest<NotificationsResults>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public string? SortBy { get; }
    public Application.Common.Models.SortDirection? SortDirection { get; }
    public string? UserId { get; }
    public string? Tag { get; }
    public bool IncludeUnreadNotificationsCount { get; }

    public GetNotificationsQuery(
        string? userId, string? tag,
        bool includeUnreadNotificationsCount,
        int page, int pageSize, string? sortBy = null, Application.Common.Models.SortDirection? sortDirection = null)
    {
        UserId = userId;
        Tag = tag;
        IncludeUnreadNotificationsCount = includeUnreadNotificationsCount;
        Page = page;
        PageSize = pageSize;
        SortBy = sortBy;
        SortDirection = sortDirection;
    }

    public class GetNotificationsQueryHandler : IRequestHandler<GetNotificationsQuery, NotificationsResults>
    {
        private readonly IWorkerContext context;

        public GetNotificationsQueryHandler(IWorkerContext context)
        {
            this.context = context;
        }

        public async Task<NotificationsResults> Handle(GetNotificationsQuery request, CancellationToken cancellationToken)
        {
            var query = context.Notifications
                .Where(n => n.Published != null)
                .AsQueryable();

            if (request.UserId is not null)
            {
                query = query.Where(n => n.UserId == request.UserId);
            }

            if (request.Tag is not null)
            {
                query = query.Where(n => n.Tag == request.Tag);
            }

            query = query.OrderByDescending(n => n.Published);

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(
                    request.SortBy,
                    request.SortDirection == Application.Common.Models.SortDirection.Desc ? Application.SortDirection.Descending : Application.SortDirection.Ascending);
            }

            query = query.Skip(request.Page * request.PageSize)
                .Take(request.PageSize).AsQueryable();

            int? unreadNotificationsCount = null;

            if (request.IncludeUnreadNotificationsCount)
            {
                unreadNotificationsCount = await context.Notifications
                    .OrderByDescending(n => n.Published)
                    .Where(n => n.Published != null)
                    .Where(n => n.UserId == request.UserId)
                    .Where(n => !n.IsRead)
                    .CountAsync(cancellationToken);
            }

            var notifications = await query.ToListAsync(cancellationToken);

            return new NotificationsResults(
                notifications.Select(notification => new NotificationDto(notification.Id, notification.Title, notification.Text, notification.Tag, notification.Link, notification.UserId, notification.IsRead, notification.Read, notification.Published, notification.ScheduledFor, notification.Created, notification.CreatedBy, notification.LastModified, notification.LastModifiedBy)),
                unreadNotificationsCount,
                totalCount);
        }
    }
}