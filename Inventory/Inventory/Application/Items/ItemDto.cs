using System;

namespace YourBrand.Inventory.Application.Items;

public record ItemDto(
    string Id,
    string Name,
    int QuantityOnHand,
    int QuantityPicked,
    int QuantityReserved,
    int QuantityAvailable);
