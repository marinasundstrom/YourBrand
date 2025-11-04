namespace YourBrand.Inventory.Domain.Entities;

using System;

using YourBrand.Inventory.Domain.Common;

public class SupplierOrderLine : AuditableEntity<string>
{
    protected SupplierOrderLine() { }

    internal SupplierOrderLine(SupplierOrder order, SupplierItem supplierItem, int expectedQuantity, DateTime? expectedOn)
        : base(Guid.NewGuid().ToString())
    {
        Order = order ?? throw new ArgumentNullException(nameof(order));
        OrderId = order.Id;

        SupplierItem = supplierItem ?? throw new ArgumentNullException(nameof(supplierItem));
        SupplierItemId = supplierItem.Id;

        QuantityOrdered = expectedQuantity;
        ExpectedQuantity = expectedQuantity;
        ExpectedOn = expectedOn;

        order.AttachLine(this);
        supplierItem.AttachOrderLine(this);
    }

    public string OrderId { get; private set; } = null!;

    public SupplierOrder Order { get; private set; } = null!;

    public string SupplierItemId { get; private set; } = null!;

    public SupplierItem SupplierItem { get; private set; } = null!;

    public int QuantityOrdered { get; private set; }

    public int ExpectedQuantity { get; private set; }

    public int QuantityReceived { get; private set; }

    public int QuantityOutstanding => ExpectedQuantity - QuantityReceived;

    public DateTime? ExpectedOn { get; private set; }

    public void ChangeExpectedQuantity(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity < QuantityReceived)
        {
            throw new InvalidOperationException("Expected quantity cannot be less than the quantity already received.");
        }

        ExpectedQuantity = quantity;
    }

    public void RegisterReceipt(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (QuantityReceived + quantity > ExpectedQuantity)
        {
            throw new InvalidOperationException("Cannot receive more items than expected.");
        }

        QuantityReceived += quantity;
    }

    public void UpdateExpectedOn(DateTime? expectedOn)
    {
        ExpectedOn = expectedOn;
    }

    public void UpdateQuantityReceived(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > ExpectedQuantity)
        {
            throw new InvalidOperationException("Cannot receive more items than expected.");
        }

        QuantityReceived = quantity;
    }
}
