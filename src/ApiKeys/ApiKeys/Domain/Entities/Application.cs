
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Application : AuditableEntity, ISoftDeletable
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public List<ApiKey> ApiKeys { get; } = new List<ApiKey>();

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}