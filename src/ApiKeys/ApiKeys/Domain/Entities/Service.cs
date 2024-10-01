
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Service : AuditableEntity, ISoftDeletable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public string Url { get; set; } = null!;

    public string Secret { get; set; } = Guid.NewGuid().ToUrlFriendlyString();

    public List<Resource> Resources { get; set; } = new List<Resource>();

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}