using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Models;
using YourBrand.Sales.Persistence;

namespace YourBrand.Sales.Features.SubscriptionManagement;

public record GetSubscriptionsQuery(string OrganizationId, int[]? Types, int[]? Status, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<SubscriptionDto>>
{
    public class GetSubscriptionsQueryHandler(SalesContext salesContext) : IRequestHandler<GetSubscriptionsQuery, PagedResult<SubscriptionDto>>
    {
        public async Task<PagedResult<SubscriptionDto>> Handle(GetSubscriptionsQuery request, CancellationToken cancellationToken)
        {
            var query = salesContext.Subscriptions
                .AsNoTracking()
                .AsQueryable();

            if (request.Types?.Any() ?? false)
            {
                var status = request.Types;
                query = query.Where(x => status.Any(z => z == x.TypeId));
            }

            if (request.Status?.Any() ?? false)
            {
                var status = request.Status;
                query = query.Where(x => status.Any(z => z == x.StatusId));
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                query = query.OrderByDescending(x => x.Created);
            }

            var subscriptions = await query.Include(x => x.Type)
                .Include(x => x.Status)
                .Include(x => x.SubscriptionPlan)
                .Include(x => x.Order)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<SubscriptionDto>(subscriptions.Select(Mappings.ToDto), totalCount);
        }
    }
}