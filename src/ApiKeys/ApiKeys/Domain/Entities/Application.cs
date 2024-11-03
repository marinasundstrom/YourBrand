
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Application : AuditableEntity<string>, ISoftDeletable
{
    protected Application()
    {
    }

    public Application(string name, string? description) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
    }

    public string Name { get; private set; } = null!;
    public string? Description { get; private set; } = null!;

    public List<ApiKey> ApiKeys { get; } = new List<ApiKey>();

    public bool IsDeleted { get; set; } = false;
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}