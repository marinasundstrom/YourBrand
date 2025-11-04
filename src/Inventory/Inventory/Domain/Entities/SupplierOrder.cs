namespace YourBrand.Inventory.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;

public class SupplierOrder : AuditableEntity<string>
{
    private readonly HashSet<SupplierOrderLine> lines = new();

    protected SupplierOrder() { }

    public SupplierOrder(Supplier supplier, string orderNumber, DateTime orderedAt, DateTime? expectedDelivery = null)
        : base(Guid.NewGuid().ToString())
    {
        Supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        SupplierId = supplier.Id;

        OrderNumber = string.IsNullOrWhiteSpace(orderNumber)
            ? throw new ArgumentException("Order number cannot be empty.", nameof(orderNumber))
            : orderNumber.Trim();

        OrderedAt = orderedAt;
        ExpectedDelivery = expectedDelivery;

        supplier.AttachOrder(this);
    }

    public string SupplierId { get; private set; } = null!;

    public Supplier Supplier { get; private set; } = null!;

    public string OrderNumber { get; private set; } = null!;

    public DateTime OrderedAt { get; private set; }

    public DateTime? ExpectedDelivery { get; private set; }

    public IReadOnlyCollection<SupplierOrderLine> Lines => lines;

    public int TotalQuantityOrdered => lines.Sum(x => x.QuantityOrdered);

    public int TotalQuantityExpected => lines.Sum(x => x.ExpectedQuantity);

    public int TotalQuantityOutstanding => lines.Sum(x => x.QuantityOutstanding);

    public SupplierOrderLine AddLine(SupplierItem supplierItem, int expectedQuantity, DateTime? expectedOn = null)
    {
        if (supplierItem is null)
        {
            throw new ArgumentNullException(nameof(supplierItem));
        }

        if (supplierItem.SupplierId != SupplierId)
        {
            throw new InvalidOperationException("Supplier item does not belong to this supplier.");
        }

        if (expectedQuantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(expectedQuantity));
        }

        var line = new SupplierOrderLine(this, supplierItem, expectedQuantity, expectedOn);
        AttachLine(line);
        return line;
    }

    public void UpdateExpectedDelivery(DateTime? expectedDelivery)
    {
        ExpectedDelivery = expectedDelivery;
    }

    internal void AttachLine(SupplierOrderLine line)
    {
        if (!lines.Contains(line))
        {
            lines.Add(line);
        }
    }

    internal void DetachLine(SupplierOrderLine line)
    {
        lines.Remove(line);
    }
}
