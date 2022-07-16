using System;

namespace YourBrand.Warehouse.Application.Items;

public record ItemDto(
    string Id,
    string SKU,
    string Name,
    int QuantityOnHand,
    int QuantityPicked,
    int QuantityReserved,
    int QuantityAvailable);
