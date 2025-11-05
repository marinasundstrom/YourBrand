using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class TransferOrder : AuditableEntity<string>
{
    private readonly HashSet<TransferOrderLine> lines = new();

    protected TransferOrder()
    {
    }

    public TransferOrder(Warehouse sourceWarehouse, Warehouse destinationWarehouse, string? reference = null)
        : base(Guid.NewGuid().ToString())
    {
        if (sourceWarehouse is null)
        {
            throw new ArgumentNullException(nameof(sourceWarehouse));
        }

        if (destinationWarehouse is null)
        {
            throw new ArgumentNullException(nameof(destinationWarehouse));
        }

        if (sourceWarehouse.Id == destinationWarehouse.Id)
        {
            throw new InvalidOperationException("Source and destination warehouses must be different.");
        }

        SourceWarehouse = sourceWarehouse;
        DestinationWarehouse = destinationWarehouse;
        SourceWarehouseId = sourceWarehouse.Id;
        DestinationWarehouseId = destinationWarehouse.Id;
        Reference = string.IsNullOrWhiteSpace(reference) ? null : reference.Trim();
    }

    public string SourceWarehouseId { get; private set; } = null!;

    public Warehouse SourceWarehouse { get; private set; } = null!;

    public string DestinationWarehouseId { get; private set; } = null!;

    public Warehouse DestinationWarehouse { get; private set; } = null!;

    public string? Reference { get; private set; }

    public DateTimeOffset? CompletedAt { get; private set; }

    public bool IsCompleted => CompletedAt.HasValue;

    public IReadOnlyCollection<TransferOrderLine> Lines => lines;

    public TransferOrderLine AddLine(Item item, int quantity)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Cannot modify a completed transfer order.");
        }

        if (item is null)
        {
            throw new ArgumentNullException(nameof(item));
        }

        if (lines.Any(x => x.ItemId == item.Id))
        {
            throw new InvalidOperationException("The transfer order already contains this item.");
        }

        var line = new TransferOrderLine(this, item, quantity);
        AttachLine(line);
        return line;
    }

    public void Complete(DateTimeOffset? completedAt = null)
    {
        if (IsCompleted)
        {
            throw new InvalidOperationException("Transfer order has already been completed.");
        }

        if (!lines.Any())
        {
            throw new InvalidOperationException("Cannot complete a transfer order without any lines.");
        }

        CompletedAt = completedAt ?? DateTimeOffset.UtcNow;
    }

    internal void AttachLine(TransferOrderLine line)
    {
        if (line is null)
        {
            throw new ArgumentNullException(nameof(line));
        }

        lines.Add(line);
    }

    internal void DetachLine(TransferOrderLine line)
    {
        if (line is null)
        {
            throw new ArgumentNullException(nameof(line));
        }

        lines.Remove(line);
    }
}
