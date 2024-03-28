using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class ItemGroup : AuditableEntity
{
    protected ItemGroup() { }

    public ItemGroup(string name)
    {
        Name = name;
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
}