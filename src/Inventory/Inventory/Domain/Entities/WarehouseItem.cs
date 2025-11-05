using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;
using YourBrand.Inventory.Domain.Events;
using YourBrand.Inventory.Domain.ValueObjects;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItem : AuditableEntity<string>
{
    protected WarehouseItem() { }

    public WarehouseItem(Item item, Warehouse warehouse, string location, int quantityOnHand, int quantityThreshold = 10)
        : this(item, warehouse, StorageLocation.Create(location), quantityOnHand, quantityThreshold)
    {
    }

    public WarehouseItem(Item item, Warehouse warehouse, StorageLocation location, int quantityOnHand, int quantityThreshold = 10)
        : base(Guid.NewGuid().ToString())
    {
        Item = item ?? throw new ArgumentNullException(nameof(item));
        Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        ItemId = item.Id;
        WarehouseId = warehouse.Id;
        Location = location ?? throw new ArgumentNullException(nameof(location));

        ChangeQuantityThreshold(quantityThreshold);
        SetQuantityOnHand(quantityOnHand);

        Item.AttachWarehouseItem(this);
        Warehouse.AttachItem(this);
        UpdateAvailability();
    }

    public Item Item { get; private set; } = null!;

    public string ItemId { get; private set; } = null!;

    public string WarehouseId { get; private set; } = null!;

    public Warehouse Warehouse { get; private set; } = null!;

    public StorageLocation Location { get; private set; } = null!;

    public WarehouseItemAvailability Availability { get; private set; }

    /// <summary>
    /// This number is the total you have physically available (including Qty Reserved),
    /// minus any items that have already been “picked” in a sales order (i.e. what’s still on your inventory shelves).
    /// </summary>
    public int QuantityOnHand { get; private set; }

    public void AdjustQuantityOnHand(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Quantity on hand cannot be negative.");
        }

        if (quantity < QuantityReserved)
        {
            throw new InvalidOperationException("Quantity on hand cannot fall below the reserved quantity.");
        }

        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand = quantity;

        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();
    }

    public void Receive(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand += quantity;

        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();
    }

    public void Return(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Receive(quantity);
        AddDomainEvent(new WarehouseItemsReturned(Id, WarehouseId, quantity));
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

        if (quantity > QuantityAvailable)
        {
            throw new InvalidOperationException("Cannot pick more items than are available.");
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
            AddDomainEvent(new WarehouseItemsReservationReleased(Id, WarehouseId, quantity));
        }

        AddDomainEvent(new WarehouseItemsPicked(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();
    }

    public void Unpick(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityPicked)
        {
            throw new InvalidOperationException("Cannot unpick more items than have been picked.");
        }

        QuantityPicked -= quantity;
        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand += quantity;

        AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
        AddDomainEvent(new WarehouseItemsUnpicked(Id, WarehouseId, quantity));

        UpdateAvailability();
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
                AddDomainEvent(new WarehouseItemsReservationReleased(Id, WarehouseId, reservedReduction));
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

        AddDomainEvent(new WarehouseItemsShipped(Id, WarehouseId, quantity));

        UpdateAvailability();
    }

    public void TransferTo(WarehouseItem destination, int quantity)
    {
        if (destination is null)
        {
            throw new ArgumentNullException(nameof(destination));
        }

        if (destination.ItemId != ItemId)
        {
            throw new InvalidOperationException("Destination item must represent the same catalog item.");
        }

        if (destination.WarehouseId == WarehouseId)
        {
            throw new InvalidOperationException("Destination warehouse must be different from the source warehouse.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityAvailable)
        {
            throw new InvalidOperationException("Cannot transfer more items than are available.");
        }

        var oldQuantityOnHand = QuantityOnHand;
        var oldQuantityAvailable = QuantityAvailable;

        QuantityOnHand -= quantity;

        if (QuantityOnHand != oldQuantityOnHand)
        {
            AddDomainEvent(new WarehouseItemQuantityOnHandUpdated(Id, WarehouseId, QuantityOnHand, oldQuantityOnHand));
        }

        if (QuantityAvailable != oldQuantityAvailable)
        {
            AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));
        }

        AddDomainEvent(new WarehouseItemsTransferred(Id, WarehouseId, destination.WarehouseId, quantity));

        destination.Receive(quantity);

        UpdateAvailability();
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

        UpdateAvailability();
    }

    public void ReleaseReservation(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityReserved)
        {
            throw new InvalidOperationException("Cannot release more reservations than exist.");
        }

        var oldQuantityAvailable = QuantityAvailable;

        QuantityReserved -= quantity;

        AddDomainEvent(new WarehouseItemsReservationReleased(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();
    }

    /// <summary>
    /// This number is how many of the item you have left if you fulfill all open sales orders and is therefore equal to Qty on Hand – Qty Reserved.
    /// (i.e. what’s left after you’ve shipped all your current orders).
    /// </summary>
    public int QuantityAvailable => QuantityOnHand - QuantityReserved;

    public int QuantityThreshold { get; private set; } = 10;

    public bool IsBelowThreshold => QuantityAvailable <= QuantityThreshold;

    public void ChangeQuantityThreshold(int threshold)
    {
        if (threshold < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(threshold));
        }

        QuantityThreshold = threshold;
    }

    public void ChangeLocation(string location)
    {
        Location = StorageLocation.Create(location);
    }

    internal void ChangeLocation(StorageLocation location)
    {
        Location = location ?? throw new ArgumentNullException(nameof(location));
    }

    internal void AssignWarehouse(Warehouse warehouse)
    {
        Warehouse?.DetachItem(this);

        Warehouse = warehouse ?? throw new ArgumentNullException(nameof(warehouse));
        WarehouseId = warehouse.Id;
        Warehouse.AttachItem(this);
    }

    private void SetQuantityOnHand(int quantity)
    {
        if (quantity < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        QuantityOnHand = quantity;
    }

    private void UpdateAvailability()
    {
        var previousAvailability = Availability;

        if (QuantityAvailable > 0)
        {
            Availability = WarehouseItemAvailability.InStock;
        }
        else if (QuantityOnHand > 0)
        {
            Availability = WarehouseItemAvailability.StockedOnDemand;
        }
        else
        {
            Availability = WarehouseItemAvailability.SupplierOutOfStock;
        }

        if (previousAvailability != Availability)
        {
            Item?.RecalculateAvailability();
        }
    }
}
