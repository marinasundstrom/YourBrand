using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Enums;

namespace YourBrand.Inventory.Domain.Entities;

public class Item : AuditableEntity
{
    protected Item() { }

    public Item(string id, string name, ItemType type, string gtin, string groupId, string unit)
    {
        Id = id;
        Name = name;
        Type = type;
        GTIN = gtin;
        GroupId = groupId;
        Unit = unit;
    }

    public string Id { get; set; }

    public string Name { get; set; } = null!;
    
    public ItemType Type { get; set; }

    public string? GTIN { get; set; }

    public ItemGroup Group { get; set; } = null!;

    public string GroupId { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public ItemAvailability Availability { get; set; }

    public bool? Discontinued { get; set; }

    public int QuantityAvailable => WarehouseItems.Sum(x => x.QuantityAvailable);

    public IReadOnlyCollection<WarehouseItem> WarehouseItems { get; } = new HashSet<WarehouseItem>();
}
