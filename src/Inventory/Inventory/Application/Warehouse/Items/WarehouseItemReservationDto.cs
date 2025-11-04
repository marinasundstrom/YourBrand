using System;

namespace YourBrand.Inventory.Application.Warehouses.Items;

public record WarehouseItemReservationDto(
    string Id,
    int Quantity,
    int ConsumedQuantity,
    int ReleasedQuantity,
    int RemainingQuantity,
    WarehouseItemReservationStatusDto Status,
    DateTimeOffset ReservedAt,
    DateTimeOffset ExpiresAt,
    DateTimeOffset? ConfirmedAt,
    DateTimeOffset? ReleasedAt,
    string? Reference);
