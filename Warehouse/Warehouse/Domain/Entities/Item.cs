using System;

using YourBrand.Warehouse.Domain.Common;
using YourBrand.Warehouse.Domain.Events;

namespace YourBrand.Warehouse.Domain.Entities;

public class Item : AuditableEntity
{
    protected Item() { }

    public Item(string sku, string name, int quantityOnHand)
    {
        SKU = sku;
        Name = name;
        QuantityOnHand = quantityOnHand;
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string SKU { get; set; } = null!;

    public string Name { get; set; } = null!;

    /// <summary>
    /// This number is the total you have physically available (including Qty Reserved),
    /// minus any items that have already been “picked” in a sales order (i.e. what’s still on your warehouse shelves).
    /// </summary>
    public int QuantityOnHand { get; private set; }

    public void AdjustQuantityOnHand(int quantity)
    {
        QuantityOnHand = quantity;
    }

    public void Receive(int quantity)
    {
        QuantityOnHand += quantity;
    }

    /// <summary>
    /// This number is the total that has already been picked in sales orders/work orders and
    /// are awaiting shipment (think of them as sitting in a box waiting to be shipped out).
    /// </summary>
    public int QuantityPicked { get; private set; }

    public void Pick(int quantity)
    {
        QuantityPicked += quantity;
        QuantityOnHand -= quantity;
    }

    public void Ship(int quantity)
    {
        QuantityPicked -= quantity;
    }

    /// <summary>
    /// This number is the total ordered by your customers (across all open sales orders/work orders).
    /// This number is what you need to be fulfilling to complete sales/production!
    /// </summary>
    public int QuantityReserved { get; private set; }

    public void Reserve(int quantity)
    {
        QuantityReserved += quantity;
    }

    /// <summary>
    /// This number is how many of the item you have left if you fulfill all open sales orders and is therefore equal to Qty on Hand – Qty Reserved.
    /// (i.e. what’s left after you’ve shipped all your current orders).
    /// </summary>
    public int QuantityAvailable => QuantityOnHand- QuantityReserved;
}
