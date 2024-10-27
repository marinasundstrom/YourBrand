using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.SubscriptionManagement.Subscriptions;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.SubscriptionManagement.Subscriptions.Statuses.Queries;

public record GetSubscriptionStatusesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<SubscriptionStatusDto>>
{
    sealed class GetSubscriptionStatusesQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetSubscriptionStatusesQuery, PagedResult<SubscriptionStatusDto>>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<PagedResult<SubscriptionStatusDto>> Handle(GetSubscriptionStatusesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<SubscriptionStatus> result = _context
                    .SubscriptionStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .OrderBy(o => o.Created)
                    .AsNoTracking()
                    .AsQueryable();

            if (request.SearchString is not null)
            {
                result = result.Where(o => o.Name.ToLower().Contains(request.SearchString.ToLower()));
            }

            var totalCount = await result.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                result = result.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                result = result.OrderBy(x => x.Id);
            }

            var items = await result
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<SubscriptionStatusDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}