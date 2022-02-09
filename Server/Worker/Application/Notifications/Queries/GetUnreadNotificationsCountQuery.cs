
using MediatR;

using Microsoft.EntityFrameworkCore;

using Worker.Application.Common.Interfaces;

namespace Worker.Application.Notifications.Queries;

public class GetUnreadNotificationsCountQuery : IRequest<int>
{
    public GetUnreadNotificationsCountQuery(string? userId)
    {
        UserId = userId;
    }

    public string? UserId { get; }

    public class GetUnreadNotificationsCountQueryHandler : IRequestHandler<GetUnreadNotificationsCountQuery, int>
    {
        private readonly IWorkerContext context;

        public GetUnreadNotificationsCountQueryHandler(IWorkerContext context, ICurrentUserService currentUserService)
        {
            this.context = context;
        }

        public async Task<int> Handle(GetUnreadNotificationsCountQuery request, CancellationToken cancellationToken)
        {
            var query = context.Notifications
                .Where(n => n.Published != null)
                .AsQueryable();

            if (request.UserId is not null)
            {
                query = query.Where(n => n.UserId == request.UserId);
            }

            var unreadNotificationsCount = await query
                .Where(n => !n.IsRead)
                .CountAsync(cancellationToken);

            return unreadNotificationsCount;
        }
    }
}