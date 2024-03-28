using System;

using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Items;

namespace YourBrand.Inventory.Application.Warehouses.Items;

public record WarehouseItemDto(
    string Id,
    ItemDto Item,
    WarehouseDto Warehouse,
    string Location,
    int QuantityOnHand,
    int QuantityPicked,
    int QuantityReserved,
    int QuantityAvailable,
    int QuantityThreshold);
