using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Queries;

public record GetOrderStatusesQuery(int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<OrderStatusDto>>
{
    class GetOrderStatusesQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetOrderStatusesQuery, PagedResult<OrderStatusDto>>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<PagedResult<OrderStatusDto>> Handle(GetOrderStatusesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<OrderStatus> result = _context
                    .OrderStatuses
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
                result = result.OrderBy(x => x.Name);
            }

            var items = await result
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToArrayAsync(cancellationToken);

            return new PagedResult<OrderStatusDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}