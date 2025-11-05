using System;
using System.Collections.Generic;
using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders.Commands;

public record CompleteTransferOrder(string TransferOrderId, DateTimeOffset? CompletedAt) : IRequest<TransferOrderDto>;

public class CompleteTransferOrderHandler(IInventoryContext context) : IRequestHandler<CompleteTransferOrder, TransferOrderDto>
{
    public async Task<TransferOrderDto> Handle(CompleteTransferOrder request, CancellationToken cancellationToken)
    {
        var transferOrder = await context.TransferOrders
            .Include(x => x.SourceWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.DestinationWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .FirstOrDefaultAsync(x => x.Id == request.TransferOrderId, cancellationToken)
            ?? throw new InvalidOperationException("Transfer order not found.");

        if (transferOrder.IsCompleted)
        {
            throw new InvalidOperationException("Transfer order has already been completed.");
        }

        var itemIds = transferOrder.Lines.Select(x => x.ItemId).Distinct().ToList();

        var sourceItems = await context.WarehouseItems
            .Include(x => x.Item)
                .ThenInclude(x => x.Group)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == transferOrder.SourceWarehouseId && itemIds.Contains(x.ItemId))
            .ToDictionaryAsync(x => x.ItemId, cancellationToken);

        if (sourceItems.Count != itemIds.Count)
        {
            throw new InvalidOperationException("The source warehouse is missing one or more items required for this transfer order.");
        }

        var destinationItems = await context.WarehouseItems
            .Include(x => x.Item)
                .ThenInclude(x => x.Group)
            .Include(x => x.Warehouse)
            .Where(x => x.WarehouseId == transferOrder.DestinationWarehouseId && itemIds.Contains(x.ItemId))
            .ToDictionaryAsync(x => x.ItemId, cancellationToken);

        foreach (var line in transferOrder.Lines)
        {
            var sourceItem = sourceItems[line.ItemId];

            if (line.Quantity > sourceItem.QuantityAvailable)
            {
                throw new InvalidOperationException($"Insufficient quantity available for item '{sourceItem.Item.Name}'.");
            }

            if (!destinationItems.TryGetValue(line.ItemId, out var destinationItem))
            {
                destinationItem = new WarehouseItem(
                    sourceItem.Item,
                    transferOrder.DestinationWarehouse,
                    sourceItem.Location,
                    0,
                    sourceItem.QuantityThreshold);

                context.WarehouseItems.Add(destinationItem);
                destinationItems[line.ItemId] = destinationItem;
            }

            sourceItem.TransferTo(destinationItem, line.Quantity);
        }

        transferOrder.Complete(request.CompletedAt);

        await context.SaveChangesAsync(cancellationToken);

        return transferOrder.ToDto();
    }
}
