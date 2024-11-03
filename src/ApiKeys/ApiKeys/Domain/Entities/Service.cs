
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Service : AuditableEntity<string>, ISoftDeletableWithAudit<User>
{
    protected Service()
    {
    }

    public Service(string name, string? description, string url, string secret) : base(Guid.NewGuid().ToString())
    {
        Name = name;
        Description = description;
        Url = url;
        Secret = secret;
    }

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Secret { get; set; } = Guid.NewGuid().ToUrlFriendlyString();

    public List<Resource> Resources { get; set; } = new List<Resource>();

    public bool IsDeleted { get; set;}
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}