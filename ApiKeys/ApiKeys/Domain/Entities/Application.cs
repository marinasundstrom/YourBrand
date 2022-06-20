
using YourBrand.ApiKeys.Domain.Common;

namespace YourBrand.ApiKeys.Domain.Entities;

public class Application : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public List<ApiKey> ApiKeys { get; } = new List<ApiKey>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
