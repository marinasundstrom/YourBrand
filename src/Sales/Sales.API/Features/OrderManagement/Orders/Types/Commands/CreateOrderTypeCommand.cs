using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types.Commands;

public record CreateOrderTypeCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<OrderTypeDto>
{
    public class CreateOrderTypeCommandHandler(ISalesContext context) : IRequestHandler<CreateOrderTypeCommand, OrderTypeDto>
    {
        private readonly ISalesContext context = context;

        public async Task<OrderTypeDto> Handle(CreateOrderTypeCommand request, CancellationToken cancellationToken)
        {
            var orderType = await context.OrderTypes.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (orderType is not null) throw new Exception();

            int orderTypeNo = 1;

            try
            {
                orderTypeNo = await context.OrderTypes
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            orderType = new Domain.Entities.OrderType(orderTypeNo, request.Name, request.Handle, request.Description);
            orderType.OrganizationId = request.OrganizationId;

            context.OrderTypes.Add(orderType);

            await context.SaveChangesAsync(cancellationToken);

            return orderType.ToDto();
        }
    }
}