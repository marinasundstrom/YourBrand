using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Queries;

public record GetSuppliers() : IRequest<IEnumerable<SupplierDto>>;

public class GetSuppliersHandler(IInventoryContext context) : IRequestHandler<GetSuppliers, IEnumerable<SupplierDto>>
{
    public async Task<IEnumerable<SupplierDto>> Handle(GetSuppliers request, CancellationToken cancellationToken)
    {
        var suppliers = await context.Suppliers
            .AsNoTracking()
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .ToListAsync(cancellationToken);

        return suppliers.Select(x => x.ToDto());
    }
}
