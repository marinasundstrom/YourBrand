
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.ApiKeys.Domain.Enums;

namespace YourBrand.ApiKeys.Domain.Entities;

public class ApiKey : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Key { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public Entities.Application? Application { get; set; }

    public List<ApiKeyService> ApiKeyServices { get; set; } = new List<ApiKeyService>();

    public ApiKeyStatus Status { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
