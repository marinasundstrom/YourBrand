using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Location : AuditableEntity
{
    protected Location() { }

    public Location(string id, string inventoryId, string? part1, string? part2, string? part3, string? part4)
    {
        Id = id;
        WarehouseId = inventoryId;
        Aisle = part1;
        Unit = part2;
        Shelf = part3;
        Bin = part4;
    }

    public string Id { get; set; }
    public string WarehouseId { get; } = null!;
    public string? Aisle { get; set; } // Aisle
    public string? Unit { get; set; } // Unit/Rack
    public string? Shelf { get; set; } // Shelf/Row
    public string? Bin { get; set; } // Bin
}
