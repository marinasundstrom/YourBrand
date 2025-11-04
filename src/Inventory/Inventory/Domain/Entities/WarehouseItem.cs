using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;
using YourBrand.Inventory.Domain.Events;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItem : AuditableEntity<string>
{
    protected WarehouseItem() { }

    public WarehouseItem(string itemId, string warehouseId, string location, int quantityOnHand, int quantityThreshold = 10)
        : base(Guid.NewGuid().ToString())
    {
        ItemId = itemId;
        WarehouseId = warehouseId;
        Location = location;
        QuantityOnHand = quantityOnHand;
        QuantityThreshold = quantityThreshold;
    }

    public Item Item { get; set; } = null!;

    public string ItemId { get; set; } = null!;

    public string WarehouseId { get; set; } = null!;

    public Warehouse Warehouse { get; set; } = null!;

    public string Location { get; set; }

    public WarehouseItemAvailability Availability { get; set; }

    /// <summary>
    /// This number is the total you have physically available (including Qty Reserved),
    /// minus any items that have already been “picked” in a sales order (i.e. what’s still on your inventory shelves).
    /// </summary>
    public int QuantityOnHand { get; private set; }

    public void AdjustQuantityOnHand(int quantity)
    {
        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand = quantity;

        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
    }

    public void Receive(int quantity)
    {
        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand += quantity;

        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
    }

    /// <summary>
    /// This number is the total that has already been picked in sales orders/work orders and
    /// are awaiting shipment (think of them as sitting in a box waiting to be shipped out).
    /// </summary>
    public int QuantityPicked { get; private set; }

    public void Pick(int quantity, bool fromReserved = false)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityOnHand)
        {
            throw new InvalidOperationException("Cannot pick more items than are on hand.");
        }

        if (fromReserved && quantity > QuantityReserved)
        {
            throw new InvalidOperationException("Cannot pick more items than are reserved.");
        }

        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityPicked += quantity;
        QuantityOnHand -= quantity;

        if (fromReserved)
        {
            QuantityReserved -= quantity;
            //AddDomainEvent(new WarehouseItemQuantityReservedUpdated(Id, WarehouseId, quantity));
        }

        AddDomainEvent(new WarehouseItemsPicked(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
    }

    public void Ship(int quantity, bool fromPicked = false)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        if (fromPicked)
        {
            if (quantity > QuantityPicked)
            {
                throw new InvalidOperationException("Cannot ship more items than have been picked.");
            }

            QuantityPicked -= quantity;
        }
        else
        {
            if (quantity > QuantityOnHand)
            {
                throw new InvalidOperationException("Cannot ship more items than are on hand.");
            }

            QuantityOnHand -= quantity;

            var reservedReduction = Math.Min(quantity, QuantityReserved);

            if (reservedReduction > 0)
            {
                QuantityReserved -= reservedReduction;
            }
        }

        if (QuantityOnHand != oldQuantityOnHand)
        {
            AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        }

        if (QuantityAvailable != oldQuantityAvailable)
        {
            AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
        }
    }

    /// <summary>
    /// This number is the total ordered by your customers (across all open sales orders/work orders).
    /// This number is what you need to be fulfilling to complete sales/production!
    /// </summary>
    public int QuantityReserved { get; private set; }

    public void Reserve(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityAvailable)
        {
            throw new InvalidOperationException("Cannot reserve more items than are available.");
        }

        var oldQuantityAvailable = QuantityAvailable;

        QuantityReserved += quantity;

        AddDomainEvent(new WarehouseItemsReserved(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
    }

    /// <summary>
    /// This number is how many of the item you have left if you fulfill all open sales orders and is therefore equal to Qty on Hand – Qty Reserved.
    /// (i.e. what’s left after you’ve shipped all your current orders).
    /// </summary>
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;

    public int QuantityThreshold { get; set; } = 10;
}