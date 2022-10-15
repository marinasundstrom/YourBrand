using System;

using YourBrand.Inventory.Domain.Common;
using YourBrand.Inventory.Domain.Events;

namespace YourBrand.Inventory.Domain.Entities;

public class Item : AuditableEntity
{
    protected Item() { }

    public Item(string id, string name, string groupId, string unit)
    {
        Id = id;
        Name = name;
        GroupId = groupId;
        Unit = unit;
    }

    public string Id { get; set; }

    public string Name { get; set; } = null!;

    public string Barcode { get; set; } = "FOO";

    public ItemGroup Group { get; set; } = null!;

    public string GroupId { get; set; } = null!;

    public string Unit { get; set; } = null!;

    public int QuantityAvailable => WarehouseItems.Sum(x => x.QuantityAvailable);

    public IReadOnlyCollection<WarehouseItem> WarehouseItems { get; } = new HashSet<WarehouseItem>();
}
