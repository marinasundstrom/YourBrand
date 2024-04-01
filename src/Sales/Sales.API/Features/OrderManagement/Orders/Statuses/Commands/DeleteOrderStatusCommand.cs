using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.Services;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Commands;

public record DeleteOrderStatusCommand(int Id) : IRequest
{
    public class DeleteOrderStatusCommandHandler(ISalesContext context) : IRequestHandler<DeleteOrderStatusCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(DeleteOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderStatus = await context.OrderStatuses
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (orderStatus is null) throw new Exception();

            context.OrderStatuses.Remove(orderStatus);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}