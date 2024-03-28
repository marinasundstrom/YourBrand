using System;

using YourBrand.Inventory.Application.Warehouses;
using YourBrand.Inventory.Application.Items;

namespace YourBrand.Inventory.Application.Items.Groups;

public record ItemGroupDto(
    string Id,
    string Name);
