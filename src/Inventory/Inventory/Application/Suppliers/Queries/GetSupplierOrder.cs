using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Queries;

public record GetSupplierOrder(string SupplierId, string OrderId) : IRequest<SupplierOrderDto?>;

public class GetSupplierOrderHandler(IInventoryContext context) : IRequestHandler<GetSupplierOrder, SupplierOrderDto?>
{
    public async Task<SupplierOrderDto?> Handle(GetSupplierOrder request, CancellationToken cancellationToken)
    {
        var order = await context.SupplierOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Lines)
                .ThenInclude(x => x.SupplierItem)
                    .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.SupplierId == request.SupplierId, cancellationToken);

        return order?.ToDto();
    }
}
