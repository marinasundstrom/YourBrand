using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Site : AuditableEntity<string>
{
    protected Site() { }

    public Site(string id, string name)
        : base(id)
    {
        Name = name;
    }
    public string Name { get; set; } = null!;
}