using YourBrand.ApiKeys.Domain.Common;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class ApiKeyService : AuditableEntity<string>, ISoftDeletableWithAudit<User>
{
    public ApiKeyService() : base(Guid.NewGuid().ToString())
    {
    }

    public ApiKey ApiKey { get; set; } = null!;
    public Service Service { get; set; } = null!;

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}