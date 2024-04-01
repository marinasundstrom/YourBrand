using YourBrand.Inventory.Domain.Common;

namespace YourBrand.Inventory.Domain.Entities;

public class Warehouse : AuditableEntity
{
    protected Warehouse() { }

    public Warehouse(string id, string name, string siteId)
    {
        Id = id;
        Name = name;
        SiteId = siteId;
    }

    public string Id { get; set; }
    public string Name { get; set; } = null!;
    public string SiteId { get; set; } = null!;
    public Site Site { get; set; } = null!;
}