using System;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class TransferOrderLine : Entity<string>
{
    protected TransferOrderLine()
    {
    }

    internal TransferOrderLine(TransferOrder transferOrder, Item item, int quantity)
        : base(Guid.NewGuid().ToString())
    {
        TransferOrder = transferOrder ?? throw new ArgumentNullException(nameof(transferOrder));
        TransferOrderId = transferOrder.Id;
        Item = item ?? throw new ArgumentNullException(nameof(item));
        ItemId = item.Id;

        ChangeQuantity(quantity);

        transferOrder.AttachLine(this);
    }

    public string TransferOrderId { get; private set; } = null!;

    public TransferOrder TransferOrder { get; private set; } = null!;

    public string ItemId { get; private set; } = null!;

    public Item Item { get; private set; } = null!;

    public int Quantity { get; private set; }

    public void ChangeQuantity(int quantity)
    {
        if (TransferOrder.IsCompleted)
        {
            throw new InvalidOperationException("Cannot modify a completed transfer order.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Quantity = quantity;
    }
}
