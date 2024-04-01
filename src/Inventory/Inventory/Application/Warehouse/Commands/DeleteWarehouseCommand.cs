using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Commands;

public record DeleteWarehouseCommand(string Id) : IRequest
{
    public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand>
    {
        private readonly IInventoryContext context;

        public DeleteWarehouseCommandHandler(IInventoryContext context)
        {
            this.context = context;
        }

        public async Task Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await context.Warehouses
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (warehouse is null) throw new Exception();

            context.Warehouses.Remove(warehouse);

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}