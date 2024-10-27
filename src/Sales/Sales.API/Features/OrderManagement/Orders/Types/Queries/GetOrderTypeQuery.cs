using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types.Queries;

public record GetOrderTypeQuery(string OrganizationId, int Id) : IRequest<OrderTypeDto?>
{
    sealed class GetOrderTypeQueryHandler(
        ISalesContext context,
        IUserContext userContext) : IRequestHandler<GetOrderTypeQuery, OrderTypeDto?>
    {
        private readonly ISalesContext _context = context;
        private readonly IUserContext userContext = userContext;

        public async Task<OrderTypeDto?> Handle(GetOrderTypeQuery request, CancellationToken cancellationToken)
        {
            var orderType = await _context
               .OrderTypes
               .Where(x => x.OrganizationId == request.OrganizationId)
               .AsNoTracking()
               .FirstAsync(c => c.Id == request.Id);

            if (orderType is null)
            {
                return null;
            }

            return orderType.ToDto();
        }
    }
}