using System;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Commands;

public record UpdateSupplierOrderExpectedDelivery(string SupplierId, string OrderId, DateTime? ExpectedDelivery) : IRequest<SupplierOrderDto>;

public class UpdateSupplierOrderExpectedDeliveryHandler(IInventoryContext context) : IRequestHandler<UpdateSupplierOrderExpectedDelivery, SupplierOrderDto>
{
    public async Task<SupplierOrderDto> Handle(UpdateSupplierOrderExpectedDelivery request, CancellationToken cancellationToken)
    {
        var order = await context.SupplierOrders
            .Include(x => x.Supplier)
            .Include(x => x.Lines)
                .ThenInclude(x => x.SupplierItem)
                    .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == request.OrderId && x.SupplierId == request.SupplierId, cancellationToken);

        if (order is null)
        {
            throw new InvalidOperationException($"Supplier order '{request.OrderId}' for supplier '{request.SupplierId}' was not found.");
        }

        order.UpdateExpectedDelivery(request.ExpectedDelivery);

        await context.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}
