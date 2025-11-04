using System;
using System.Collections.Generic;
using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application.Suppliers;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Commands;

public record CreateSupplierOrder(
    string SupplierId,
    string OrderNumber,
    DateTime OrderedAt,
    DateTime? ExpectedDelivery,
    IReadOnlyCollection<CreateSupplierOrderLine> Lines) : IRequest<SupplierOrderDto>;

public record CreateSupplierOrderLine(string SupplierItemId, int ExpectedQuantity, DateTime? ExpectedOn);

public class CreateSupplierOrderHandler(IInventoryContext context) : IRequestHandler<CreateSupplierOrder, SupplierOrderDto>
{
    public async Task<SupplierOrderDto> Handle(CreateSupplierOrder request, CancellationToken cancellationToken)
    {
        var supplier = await context.Suppliers
            .Include(x => x.Items)
                .ThenInclude(x => x.Item)
            .FirstOrDefaultAsync(x => x.Id == request.SupplierId, cancellationToken);

        if (supplier is null)
        {
            throw new InvalidOperationException($"Supplier '{request.SupplierId}' was not found.");
        }

        if (request.Lines is null || request.Lines.Count == 0)
        {
            throw new InvalidOperationException("At least one order line is required.");
        }

        var order = supplier.PlaceOrder(request.OrderNumber, request.OrderedAt, request.ExpectedDelivery);

        foreach (var line in request.Lines)
        {
            var supplierItem = supplier.Items.FirstOrDefault(x => x.Id == line.SupplierItemId);

            if (supplierItem is null)
            {
                throw new InvalidOperationException($"Supplier item '{line.SupplierItemId}' does not belong to supplier '{supplier.Id}'.");
            }

            order.AddLine(supplierItem, line.ExpectedQuantity, line.ExpectedOn);
        }

        context.SupplierOrders.Add(order);

        await context.SaveChangesAsync(cancellationToken);

        var persistedOrder = await context.SupplierOrders
            .AsNoTracking()
            .Include(x => x.Supplier)
            .Include(x => x.Lines)
                .ThenInclude(x => x.SupplierItem)
                    .ThenInclude(x => x.Item)
            .FirstAsync(x => x.Id == order.Id, cancellationToken);

        return persistedOrder.ToDto();
    }
}
