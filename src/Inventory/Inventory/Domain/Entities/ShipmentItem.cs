using System;

using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class ShipmentItem : AuditableEntity<string>
{
    protected ShipmentItem() { }

    internal ShipmentItem(Shipment shipment, WarehouseItem warehouseItem, int quantity)
        : base(Guid.NewGuid().ToString())
    {
        Shipment = shipment ?? throw new ArgumentNullException(nameof(shipment));
        ShipmentId = shipment.Id;

        WarehouseItem = warehouseItem ?? throw new ArgumentNullException(nameof(warehouseItem));
        WarehouseItemId = warehouseItem.Id;

        ChangeQuantityInternal(quantity);
    }

    public string ShipmentId { get; private set; } = null!;

    public Shipment Shipment { get; private set; } = null!;

    public string WarehouseItemId { get; private set; } = null!;

    public WarehouseItem WarehouseItem { get; private set; } = null!;

    public int Quantity { get; private set; }

    public bool IsShipped { get; private set; }

    public DateTimeOffset? ShippedAt { get; private set; }

    public void ChangeQuantity(int quantity)
    {
        if (Shipment.IsShipped)
        {
            throw new InvalidOperationException("Cannot modify items of a shipment that has already been shipped.");
        }

        ChangeQuantityInternal(quantity);
    }

    public void Cancel()
    {
        if (IsShipped)
        {
            return;
        }

        if (Quantity > 0)
        {
            WarehouseItem.Unpick(Quantity);
        }

        Quantity = 0;
    }

    public void Ship(DateTimeOffset? shippedAt = null)
    {
        if (IsShipped)
        {
            throw new InvalidOperationException("Shipment item has already been shipped.");
        }

        if (Quantity <= 0)
        {
            throw new InvalidOperationException("Cannot ship an item with zero quantity.");
        }

        WarehouseItem.Ship(Quantity, fromPicked: true);

        IsShipped = true;
        ShippedAt = shippedAt ?? DateTimeOffset.UtcNow;
    }

    private void ChangeQuantityInternal(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (Quantity == quantity)
        {
            return;
        }

        if (quantity > Quantity)
        {
            var diff = quantity - Quantity;
            WarehouseItem.Pick(diff);
        }
        else
        {
            var diff = Quantity - quantity;
            WarehouseItem.Unpick(diff);
        }

        Quantity = quantity;
    }
}
