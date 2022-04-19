
using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;
using Worker.Application.Common.Models;
using Worker.Domain;

namespace Worker.Application.Notifications.Queries;

public record GetNotificationsQuery(string? UserId, string? Tag,
        bool IncludeUnreadNotificationsCount,
        int Page, int PageSize, string? SortBy = null, Application.Common.Models.SortDirection? SortDirection = null) : IRequest<NotificationsResults>
{
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