using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Commands;

public record CreateOrderStatusCommand(string OrganizationId, string Name, string Handle, string? Description) : IRequest<OrderStatusDto>
{
    public class CreateOrderStatusCommandHandler(ISalesContext context) : IRequestHandler<CreateOrderStatusCommand, OrderStatusDto>
    {
        private readonly ISalesContext context = context;

        public async Task<OrderStatusDto> Handle(CreateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderStatus = await context.OrderStatuses.FirstOrDefaultAsync(i => i.Name == request.Name, cancellationToken);

            if (orderStatus is not null) throw new Exception();

            int orderStatusNo = 1;

            try
            {
                orderStatusNo = await context.OrderStatuses
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .MaxAsync(x => x.Id, cancellationToken) + 1;
            }
            catch { }

            orderStatus = new Domain.Entities.OrderStatus(orderStatusNo, request.Name, request.Handle, request.Description);
            orderStatus.OrganizationId = request.OrganizationId;

            context.OrderStatuses.Add(orderStatus);

            await context.SaveChangesAsync(cancellationToken);

            return orderStatus.ToDto();
        }
    }
}