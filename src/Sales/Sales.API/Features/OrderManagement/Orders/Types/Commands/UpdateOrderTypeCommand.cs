using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types.Commands;

public record UpdateOrderTypeCommand(string OrganizationId, int Id, string Name, string Handle, string? Description) : IRequest
{
    public class UpdateOrderTypeCommandHandler(ISalesContext context) : IRequestHandler<UpdateOrderTypeCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(UpdateOrderTypeCommand request, CancellationToken cancellationToken)
        {
            var orderType = await context.OrderTypes
                    .Where(x => x.OrganizationId == request.OrganizationId)
                    .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (orderType is null) throw new Exception();

            orderType.Name = request.Name;
            orderType.Handle = request.Handle;
            orderType.Description = request.Description;

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}