using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Models;

using YourBrand.Sales;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrders(int[]? Status, string? CustomerId, string? SSN, string? AssigneeId, Guid? SubscriptionId, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<Result<PagedResult<OrderDto>>>
{
    public class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrders, Result<PagedResult<OrderDto>>>
    {
        private readonly IOrderRepository orderRepository = orderRepository;

        public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrders request, CancellationToken cancellationToken)
        {
            var query = orderRepository.GetAll();

            if (request.Status?.Any() ?? false)
            {
                var status = request.Status;
                query = query.Where(x => status.Any(z => z == x.Status.Id));
            }

            if (request.CustomerId is not null)
            {
                query = query.Where(x => x.Customer.Id == request.CustomerId);
            }

            if (request.SSN is not null)
            {
                query = query.Where(x => x.BillingDetails.SSN == request.SSN);
            }

            if (request.AssigneeId is not null)
            {
                query = query.Where(x => x.AssigneeId == request.AssigneeId);
            }

            if (request.SubscriptionId is not null)
            {
                query = query.Where(x => x.SubscriptionId == request.SubscriptionId);
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

            var orders = await query
                .Include(i => i.Status)
                .Include(i => i.Items)
                .Include(i => i.Assignee)
                .Include(i => i.CreatedBy)
                .Include(i => i.LastModifiedBy)
                .AsSplitQuery()
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize).AsQueryable()
                .ToArrayAsync(cancellationToken);

            return new PagedResult<OrderDto>(orders.Select(x => x.ToDto()), totalCount);
        }
    }
}