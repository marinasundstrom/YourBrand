using System;

using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Warehouses.Items;

namespace YourBrand.Inventory.Application.Warehouses.Shipments;

public record ShipmentDto(
    string Id,
    string OrderNo,
    WarehouseDto Warehouse,
    string Destination,
    string Service,
    DateTimeOffset Created,
    DateTimeOffset? ShippedAt,
    IReadOnlyCollection<ShipmentItemDto> Items);

public record ShipmentItemDto(
    string Id,
    WarehouseItemDto WarehouseItem,
    int Quantity,
    bool IsShipped,
    DateTimeOffset? ShippedAt);

public record CreateShipmentDto(string OrderNo, string Destination, string Service);

public record UpdateShipmentDto(string Destination, string Service);

public record AddShipmentItemDto(string WarehouseItemId, int Quantity);

public record UpdateShipmentItemDto(int Quantity);

public record ShipShipmentDto(DateTimeOffset? ShippedAt);
