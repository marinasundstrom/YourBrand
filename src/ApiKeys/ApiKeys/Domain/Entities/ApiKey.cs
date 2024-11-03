
using YourBrand.ApiKeys.Domain.Common;
using YourBrand.ApiKeys.Domain.Enums;
using YourBrand.Domain;
using YourBrand.Identity;

namespace YourBrand.ApiKeys.Domain.Entities;

public class ApiKey : AuditableEntity<string>, ISoftDeletable
{
    protected ApiKey()
    {
    }

    public ApiKey(string key, string? description) : base(Guid.NewGuid().ToString())
    {
        Key = key;
        Description = description;
    }

    public string Key { get; set; } = null!;
    public string? Description { get; set; } = null!;

    public Entities.Application? Application { get; set; }

    public List<ApiKeyService> ApiKeyServices { get; set; } = new List<ApiKeyService>();

    public ApiKeyStatus Status { get; set; }

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}