using System;

using YourBrand.Inventory.Application.Items.Groups;

namespace YourBrand.Inventory.Application.Items;

public record ItemDto(
    string Id,
    string Name,
    ItemGroupDto Group);
