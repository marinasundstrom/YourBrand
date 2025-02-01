using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;
using YourBrand.Sales.Features.OrderManagement.Repositories;
using YourBrand.Sales.Models;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Queries;

public record GetOrders(string OrganizationId, int[]? Types, int[]? Status, string? CustomerId, string? SSN, string? AssigneeId, Guid? SubscriptionId, DateTimeOffset? FromDate = null, DateTimeOffset? ToDate = null, DateTimeOffset? PlannedFromDate = null, DateTimeOffset? PlannedToDate = null, int Page = 1, int PageSize = 10, string? SortBy = null, SortDirection? SortDirection = null) : IRequest<Result<PagedResult<OrderDto>>>
{
    public class Handler(IOrderRepository orderRepository) : IRequestHandler<GetOrders, Result<PagedResult<OrderDto>>>
    {
        private readonly IOrderRepository orderRepository = orderRepository;

        public async Task<Result<PagedResult<OrderDto>>> Handle(GetOrders request, CancellationToken cancellationToken)
        {
            var query = orderRepository
                .GetAll()
                .InOrganization(request.OrganizationId);

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
            else
            {
                query = query.Where(x => x.StatusId != 1);
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

            if (request.FromDate is not null)
            {
                query = query.Where(x => x.Date >= request.FromDate);
            }

            if (request.ToDate is not null)
            {
                query = query.Where(x => x.Date <= request.ToDate);
            }

            if (request.PlannedFromDate is not null)
            {
                query = query.Where(x => x.Schedule!.PlannedStartDate == null || x.Schedule!.PlannedStartDate >= request.PlannedFromDate);
            }

            if (request.PlannedToDate is not null)
            {
                query = query.Where(x => x.Schedule!.PlannedEndDate == null || x.Schedule!.PlannedEndDate <= request.PlannedToDate);
            }

            var totalCount = await query.CountAsync(cancellationToken);

            if (request.SortBy is not null)
            {
                query = query.OrderBy(request.SortBy, request.SortDirection);
            }
            else
            {
                if (!request.Status?.Any() ?? true)
                {
                    // For drafts
                    query = query.OrderByDescending(x => x.Created);
                }
                else
                {
                    query = query.OrderByDescending(x => x.Date);
                }
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