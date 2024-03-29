using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Sales.Features.OrderManagement.Orders.Dtos;

using YourBrand.Sales.Features.Services;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Statuses.Commands;

public record UpdateOrderStatusCommand(int Id, string Name, string Handle, string? Description) : IRequest
{
    public class UpdateOrderStatusCommandHandler(ISalesContext context) : IRequestHandler<UpdateOrderStatusCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
        {
            var orderStatus = await context.OrderStatuses.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (orderStatus is null) throw new Exception();

            orderStatus.Name = request.Name;
            orderStatus.Handle = request.Handle;
            orderStatus.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}