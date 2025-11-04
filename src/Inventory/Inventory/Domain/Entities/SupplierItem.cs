namespace YourBrand.Inventory.Domain.Entities;

using System;
using System.Collections.Generic;

using YourBrand.Inventory.Domain.Common;

public class SupplierItem : AuditableEntity<string>
{
    private readonly HashSet<SupplierOrderLine> orderLines = new();

    protected SupplierItem() { }

    public SupplierItem(Supplier supplier, Item item, string? supplierItemId, decimal? unitCost, int? leadTimeDays)
        : base(Guid.NewGuid().ToString())
    {
        Supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
        SupplierId = supplier.Id;

        Item = item ?? throw new ArgumentNullException(nameof(item));
        ItemId = item.Id;

        UpdateDetails(supplierItemId, unitCost, leadTimeDays);

        supplier.AttachItem(this);
        item.AttachSupplierItem(this);
    }

    public string SupplierId { get; private set; } = null!;

    public Supplier Supplier { get; private set; } = null!;

    public string ItemId { get; private set; } = null!;

    public Item Item { get; private set; } = null!;

    public string? SupplierItemId { get; private set; }

    public decimal? UnitCost { get; private set; }

    public int? LeadTimeDays { get; private set; }

    public IReadOnlyCollection<SupplierOrderLine> OrderLines => orderLines;

    public void UpdateDetails(string? supplierItemId, decimal? unitCost, int? leadTimeDays)
    {
        SupplierItemId = string.IsNullOrWhiteSpace(supplierItemId) ? null : supplierItemId.Trim();
        UnitCost = unitCost;

        if (leadTimeDays is < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(leadTimeDays));
        }

        LeadTimeDays = leadTimeDays;
    }

    internal void AttachOrderLine(SupplierOrderLine line)
    {
        if (!orderLines.Contains(line))
        {
            orderLines.Add(line);
        }
    }

    internal void DetachOrderLine(SupplierOrderLine line)
    {
        orderLines.Remove(line);
    }
}
