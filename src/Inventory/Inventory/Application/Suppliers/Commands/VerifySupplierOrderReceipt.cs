using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Application;
using YourBrand.Inventory.Domain;

namespace YourBrand.Inventory.Application.Suppliers.Commands;

public record VerifySupplierOrderReceipt(string SupplierId, string OrderId, IReadOnlyCollection<VerifySupplierOrderReceiptLine> Lines) : IRequest<SupplierOrderDto>;

public record VerifySupplierOrderReceiptLine(string LineId, int QuantityReceived);

public class VerifySupplierOrderReceiptHandler(IInventoryContext context) : IRequestHandler<VerifySupplierOrderReceipt, SupplierOrderDto>
{
    public async Task<SupplierOrderDto> Handle(VerifySupplierOrderReceipt request, CancellationToken cancellationToken)
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

        if (!order.IsReceived)
        {
            throw new InvalidOperationException($"Supplier order '{request.OrderId}' must be marked as received before it can be verified.");
        }

        foreach (var lineRequest in request.Lines)
        {
            var line = order.Lines.FirstOrDefault(x => x.Id == lineRequest.LineId);

            if (line is null)
            {
                throw new InvalidOperationException($"Order line '{lineRequest.LineId}' was not found on supplier order '{request.OrderId}'.");
            }

            line.UpdateQuantityReceived(lineRequest.QuantityReceived);
        }

        await context.SaveChangesAsync(cancellationToken);

        return order.ToDto();
    }
}
