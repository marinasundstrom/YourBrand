using MediatR;

using Microsoft.EntityFrameworkCore;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Commands;

public record UpdateWarehouseCommand(string Id, string Name, string SiteId) : IRequest
{
    public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand>
    {
        private readonly IInventoryContext context;

        public UpdateWarehouseCommandHandler(IInventoryContext context)
        {
            this.context = context;
        }

        public async Task Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await context.Warehouses.FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (warehouse is null) throw new Exception();

            warehouse.Name = request.Name;
            warehouse.SiteId = request.SiteId;

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}
