using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Queries;

public record GetOrderStatusQuery(string OrganizationId, int Id) : IRequest<OrderStatusDto?>
{
    sealed class GetOrderStatusQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetOrderStatusQuery, OrderStatusDto?>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<OrderStatusDto?> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
        {
            var orderStatus = await _context
               .OrderStatuses
               .Where(x => x.OrganizationId == request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (orderStatus is null)
            {
                return null;
            }

            return orderStatus.ToDto();
        }
    }
}