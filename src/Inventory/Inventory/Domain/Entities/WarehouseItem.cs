using System;
using System.Collections.Generic;
using System.Linq;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;
using YourBrand.Inventory.Domain.Events;
using YourBrand.Inventory.Domain.ValueObjects;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItem : AuditableEntity<string>
{
    private static readonly TimeSpan DefaultReservationDuration = TimeSpan.FromMinutes(15);

    private readonly List<WarehouseItemReservation> _reservations = new();

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

    public IReadOnlyCollection<WarehouseItemReservation> Reservations => _reservations.AsReadOnly();

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
            ConsumeReservedQuantity(quantity);
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
                ConsumeReservedQuantity(reservedReduction);
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

    /// <summary>
    /// This number is the total ordered by your customers (across all open sales orders/work orders).
    /// This number is what you need to be fulfilling to complete sales/production!
    /// </summary>
    public int QuantityReserved { get; private set; }

    public void Reserve(int quantity)
    {
        Reserve(quantity, DefaultReservationDuration, null);
    }

    public WarehouseItemReservation Reserve(int quantity, TimeSpan duration, string? reference = null)
    {
        if (duration <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(duration));
        }

        return Reserve(quantity, DateTimeOffset.UtcNow.Add(duration), reference);
    }

    public WarehouseItemReservation Reserve(int quantity, DateTimeOffset expiresAt, string? reference = null)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > QuantityAvailable)
        {
            throw new InvalidOperationException("Cannot reserve more items than are available.");
        }

        if (expiresAt <= DateTimeOffset.UtcNow)
        {
            throw new ArgumentOutOfRangeException(nameof(expiresAt), "Expiration must be in the future.");
        }

        var oldQuantityAvailable = QuantityAvailable;

        var reservation = new WarehouseItemReservation(this, quantity, DateTimeOffset.UtcNow, expiresAt, reference);
        _reservations.Add(reservation);

        QuantityReserved += quantity;

        AddDomainEvent(new WarehouseItemsReserved(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();

        return reservation;
    }

    public void ConfirmReservation(string reservationId)
    {
        if (string.IsNullOrWhiteSpace(reservationId))
        {
            throw new ArgumentNullException(nameof(reservationId));
        }

        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
            ?? throw new InvalidOperationException($"Reservation '{reservationId}' does not exist.");

        reservation.Confirm();
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

        var remaining = quantity;

        foreach (var reservation in _reservations
                     .Where(r => r.RemainingQuantity > 0)
                     .OrderBy(r => r.ReservedAt))
        {
            if (remaining == 0)
            {
                break;
            }

            var releaseAmount = Math.Min(remaining, reservation.RemainingQuantity);
            reservation.Release(releaseAmount, WarehouseItemReservationStatus.Released);
            remaining -= releaseAmount;
        }

        if (remaining > 0)
        {
            throw new InvalidOperationException("Unable to release the requested reservation quantity.");
        }

        QuantityReserved -= quantity;

        AddDomainEvent(new WarehouseItemsReservationReleased(Id, WarehouseId, quantity));
        AddDomainEvent(new WarehouseItemQuantityAvailableUpdated(Id, WarehouseId, QuantityAvailable, oldQuantityAvailable));

        UpdateAvailability();
    }

    public void ReleaseReservation(string reservationId, WarehouseItemReservationStatus status)
    {
        if (string.IsNullOrWhiteSpace(reservationId))
        {
            throw new ArgumentNullException(nameof(reservationId));
        }

        var reservation = _reservations.FirstOrDefault(r => r.Id == reservationId)
            ?? throw new InvalidOperationException($"Reservation '{reservationId}' does not exist.");

        var oldQuantityAvailable = QuantityAvailable;

        var releasedQuantity = reservation.ReleaseRemaining(status);

        if (releasedQuantity == 0)
        {
            return;
        }

        QuantityReserved -= releasedQuantity;

        AddDomainEvent(new WarehouseItemsReservationReleased(Id, WarehouseId, releasedQuantity));
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

    private void ConsumeReservedQuantity(int quantity)
    {
        var remaining = quantity;

        foreach (var reservation in _reservations
                     .Where(r => r.RemainingQuantity > 0
                         && r.Status is WarehouseItemReservationStatus.Pending or WarehouseItemReservationStatus.Confirmed)
                     .OrderBy(r => r.Status == WarehouseItemReservationStatus.Confirmed ? 0 : 1)
                     .ThenBy(r => r.ReservedAt))
        {
            if (remaining == 0)
            {
                break;
            }

            var consumeAmount = Math.Min(remaining, reservation.RemainingQuantity);
            reservation.Consume(consumeAmount);
            remaining -= consumeAmount;
        }

        if (remaining > 0)
        {
            throw new InvalidOperationException("Unable to consume the requested reserved quantity.");
        }
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
