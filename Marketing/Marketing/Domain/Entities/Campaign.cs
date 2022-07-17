using YourBrand.Marketing.Domain.Common;
using YourBrand.Marketing.Domain.Events;

namespace YourBrand.Marketing.Domain.Entities;

public class Campaign : AuditableEntity
{
    readonly HashSet<Address> _addresses = new HashSet<Address>();

    protected Campaign() { }

    public Campaign(string name)
    {
        Name = name;
    }

    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; }

    public string? Description { get; set; }
}
