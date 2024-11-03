using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Warehouse : AuditableEntity<string>
{
    protected Warehouse() { }

    public Warehouse(string id, string name, string siteId)
        : base(id)
    {
        Name = name;
        SiteId = siteId;
    }
    public string Name { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public Site Site { get; set; } = null!;
}