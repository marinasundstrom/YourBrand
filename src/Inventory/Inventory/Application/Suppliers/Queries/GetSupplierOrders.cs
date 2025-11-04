using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Queries;

public record GetSupplierOrders(string SupplierId) : IRequest<IEnumerable<SupplierOrderDto>>;

public class GetSupplierOrdersHandler(IInventoryContext context) : IRequestHandler<GetSupplierOrders, IEnumerable<SupplierOrderDto>>
{
    public async Task<IEnumerable<SupplierOrderDto>> Handle(GetSupplierOrders request, CancellationToken cancellationToken)
    {
        var orders = await context.SupplierOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Lines)
                .ThenInclude(x => x.SupplierItem)
                    .ThenInclude(x => x.Item)
            .Where(x => x.SupplierId == request.SupplierId)
            .OrderByDescending(x => x.OrderedAt)
            .ToListAsync(cancellationToken);

        return orders.Select(x => x.ToDto());
    }
}
