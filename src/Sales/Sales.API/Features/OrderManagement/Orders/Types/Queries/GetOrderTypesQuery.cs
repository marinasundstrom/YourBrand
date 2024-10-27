using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Domain.Entities;
using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types.Queries;

public record GetOrderTypesQuery(string OrganizationId, int Page = 0, int PageSize = 10, string? SearchString = null, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<PagedResult<OrderTypeDto>>
{
    sealed class GetOrderTypesQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetOrderTypesQuery, PagedResult<OrderTypeDto>>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<PagedResult<OrderTypeDto>> Handle(GetOrderTypesQuery request, CancellationToken cancellationToken)
        {
            IQueryable<OrderType> result = _context
                    .OrderTypes
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

            return new PagedResult<OrderTypeDto>(items.Select(cp => cp.ToDto()), totalCount);
        }
    }
}