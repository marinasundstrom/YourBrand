using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Queries;

public record GetSupplier(string Id) : IRequest<SupplierDto?>;

public class GetSupplierHandler(IInventoryContext context) : IRequestHandler<GetSupplier, SupplierDto?>
{
    public async Task<SupplierDto?> Handle(GetSupplier request, CancellationToken cancellationToken)
    {
        var supplier = await context.Suppliers
            .AsNoTracking()
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

        return supplier?.ToDto();
    }
}
