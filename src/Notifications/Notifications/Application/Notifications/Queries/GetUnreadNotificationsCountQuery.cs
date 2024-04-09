
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Identity;
using YourBrand.Notifications.Application.Common.Interfaces;

namespace YourBrand.Notifications.Application.Notifications.Queries;

public record GetUnreadNotificationsCountQuery(string? UserId) : IRequest<int>
{
    public class GetUnreadNotificationsCountQueryHandler(IWorkerContext context) : IRequestHandler<GetUnreadNotificationsCountQuery, int>
    {
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