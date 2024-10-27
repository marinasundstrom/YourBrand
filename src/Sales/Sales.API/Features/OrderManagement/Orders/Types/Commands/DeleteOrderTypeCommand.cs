using MediatR;

using Microsoft.EntityFrameworkCore;

namespace YourBrand.Sales.Features.OrderManagement.Orders.Types.Commands;

public record DeleteOrderTypeCommand(string OrganizationId, int Id) : IRequest
{
    public class DeleteOrderTypeCommandHandler(ISalesContext context) : IRequestHandler<DeleteOrderTypeCommand>
    {
        private readonly ISalesContext context = context;

        public async Task Handle(DeleteOrderTypeCommand request, CancellationToken cancellationToken)
        {
            var orderType = await context.OrderTypes
                .Where(x => x.OrganizationId == request.OrganizationId)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (orderType is null) throw new Exception();

            context.OrderTypes.Remove(orderType);

            await context.SaveChangesAsync(cancellationToken);
        }
    }
}