using System;
using System.Collections.Generic;
using System.Linq;

using MediatR;

using Microsoft.EntityFrameworkCore;

using YourBrand.Inventory.Domain;
using YourBrand.Inventory.Domain.Entities;

namespace YourBrand.Inventory.Application.Warehouses.TransferOrders.Commands;

public record CreateTransferOrder(
    string SourceWarehouseId,
    string DestinationWarehouseId,
    IReadOnlyCollection<CreateTransferOrderLineDto> Lines) : IRequest<TransferOrderDto>;

public class CreateTransferOrderHandler(IInventoryContext context) : IRequestHandler<CreateTransferOrder, TransferOrderDto>
{
    public async Task<TransferOrderDto> Handle(CreateTransferOrder request, CancellationToken cancellationToken)
    {
        if (request.SourceWarehouseId == request.DestinationWarehouseId)
        {
            throw new InvalidOperationException("Source and destination warehouses must be different.");
        }

        if (request.Lines is null || !request.Lines.Any())
        {
            throw new InvalidOperationException("A transfer order must contain at least one line.");
        }

        var sourceWarehouse = await context.Warehouses
            .Include(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.SourceWarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Source warehouse not found.");

        var destinationWarehouse = await context.Warehouses
            .Include(x => x.Site)
            .FirstOrDefaultAsync(x => x.Id == request.DestinationWarehouseId, cancellationToken)
            ?? throw new InvalidOperationException("Destination warehouse not found.");

        var itemIds = request.Lines.Select(x => x.ItemId).Distinct().ToList();

        var items = await context.Items
            .Include(x => x.Group)
            .Where(x => itemIds.Contains(x.Id))
            .ToDictionaryAsync(x => x.Id, cancellationToken);

        if (items.Count != itemIds.Count)
        {
            throw new InvalidOperationException("One or more items in the transfer order could not be found.");
        }

        var transferOrder = new TransferOrder(sourceWarehouse, destinationWarehouse);

        foreach (var line in request.Lines)
        {
            if (line.Quantity <= 0)
            {
                throw new InvalidOperationException("Line quantity must be greater than zero.");
            }

            transferOrder.AddLine(items[line.ItemId], line.Quantity);
        }

        context.TransferOrders.Add(transferOrder);

        await context.SaveChangesAsync(cancellationToken);

        var persisted = await context.TransferOrders
            .AsNoTracking()
            .Where(x => x.Id == transferOrder.Id)
            .Include(x => x.SourceWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.DestinationWarehouse)
                .ThenInclude(x => x.Site)
            .Include(x => x.Lines)
                .ThenInclude(x => x.Item)
                    .ThenInclude(x => x.Group)
            .FirstAsync(cancellationToken);

        return persisted.ToDto();
    }
}
