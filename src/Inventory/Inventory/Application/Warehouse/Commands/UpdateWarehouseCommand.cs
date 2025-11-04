using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Warehouses.Commands;

public record UpdateWarehouseCommand(string Id, string Name, string SiteId) : IRequest
{
    public class UpdateWarehouseCommandHandler(IInventoryContext context) : IRequestHandler<UpdateWarehouseCommand>
    {
        public async Task Handle(UpdateWarehouseCommand request, CancellationToken cancellationToken)
        {
            var warehouse = await context.Warehouses
                .Include(x => x.Site)
                .FirstOrDefaultAsync(i => i.Id == request.Id, cancellationToken);

            if (warehouse is null) throw new Exception();

            warehouse.Rename(request.Name);

            if (warehouse.SiteId != request.SiteId)
            {
                var site = await context.Sites.FirstOrDefaultAsync(x => x.Id == request.SiteId, cancellationToken);

                if (site is null)
                {
                    throw new InvalidOperationException($"Site '{request.SiteId}' was not found.");
                }

                warehouse.ChangeSite(site);
            }

            await context.SaveChangesAsync(cancellationToken);

        }
    }
}