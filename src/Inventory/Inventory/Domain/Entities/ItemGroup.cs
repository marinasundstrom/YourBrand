using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class ItemGroup : AuditableEntity<string>
{
    protected ItemGroup() { }

    public ItemGroup(string name) : base(Guid.NewGuid().ToString())
    {
        Name = name;
    }

    public string Name { get; set; } = null!;
}