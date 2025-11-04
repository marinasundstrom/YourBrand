using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Domain.Entities;

public class WarehouseItemReservation : AuditableEntity<string>
{
    private WarehouseItemReservation()
    {
    }

    internal WarehouseItemReservation(WarehouseItem warehouseItem, int quantity, DateTimeOffset reservedAt, DateTimeOffset expiresAt, string? reference)
        : base(Guid.NewGuid().ToString())
    {
        WarehouseItem = warehouseItem ?? throw new ArgumentNullException(nameof(warehouseItem));
        WarehouseItemId = warehouseItem.Id;

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        Quantity = quantity;
        ReservedAt = reservedAt;
        ExpiresAt = expiresAt;
        Reference = reference;
        Status = WarehouseItemReservationStatus.Pending;
    }

    public string WarehouseItemId { get; private set; } = null!;

    public WarehouseItem WarehouseItem { get; private set; } = null!;

    public int Quantity { get; private set; }

    public int ConsumedQuantity { get; private set; }

    public int ReleasedQuantity { get; private set; }

    public WarehouseItemReservationStatus Status { get; private set; }

    public DateTimeOffset ReservedAt { get; private set; }

    public DateTimeOffset ExpiresAt { get; private set; }

    public DateTimeOffset? ConfirmedAt { get; private set; }

    public DateTimeOffset? ReleasedAt { get; private set; }

    public string? Reference { get; private set; }

    public int RemainingQuantity => Quantity - ConsumedQuantity - ReleasedQuantity;

    public bool IsActive => Status is WarehouseItemReservationStatus.Pending or WarehouseItemReservationStatus.Confirmed;

    public void Confirm()
    {
        if (Status == WarehouseItemReservationStatus.Confirmed)
        {
            return;
        }

        if (Status != WarehouseItemReservationStatus.Pending)
        {
            throw new InvalidOperationException("Only pending reservations can be confirmed.");
        }

        Status = WarehouseItemReservationStatus.Confirmed;
        ConfirmedAt = DateTimeOffset.UtcNow;
    }

    public int Consume(int quantity)
    {
        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > RemainingQuantity)
        {
            throw new InvalidOperationException("Cannot consume more than the remaining reserved quantity.");
        }

        ConsumedQuantity += quantity;

        if (RemainingQuantity == 0)
        {
            Status = WarehouseItemReservationStatus.Completed;
        }

        return quantity;
    }

    public int ReleaseRemaining(WarehouseItemReservationStatus status)
    {
        if (status is not WarehouseItemReservationStatus.Released and not WarehouseItemReservationStatus.Expired)
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (Status is WarehouseItemReservationStatus.Completed or WarehouseItemReservationStatus.Released or WarehouseItemReservationStatus.Expired)
        {
            return 0;
        }

        var released = RemainingQuantity;

        if (released == 0)
        {
            Status = WarehouseItemReservationStatus.Completed;
            return 0;
        }

        ReleasedQuantity += released;
        Status = status;
        ReleasedAt = DateTimeOffset.UtcNow;

        return released;
    }

    public int Release(int quantity, WarehouseItemReservationStatus status)
    {
        if (status is not WarehouseItemReservationStatus.Released and not WarehouseItemReservationStatus.Expired)
        {
            throw new ArgumentOutOfRangeException(nameof(status));
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity));
        }

        if (quantity > RemainingQuantity)
        {
            throw new InvalidOperationException("Cannot release more than the remaining reserved quantity.");
        }

        ReleasedQuantity += quantity;

        if (RemainingQuantity == 0)
        {
            Status = status;
            ReleasedAt = DateTimeOffset.UtcNow;
        }

        return quantity;
    }

    public void Extend(TimeSpan extension)
    {
        if (extension <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(extension));
        }

        if (Status is WarehouseItemReservationStatus.Released or WarehouseItemReservationStatus.Expired)
        {
            throw new InvalidOperationException("Cannot extend a released or expired reservation.");
        }

        ExpiresAt = ExpiresAt.Add(extension);
    }
}
