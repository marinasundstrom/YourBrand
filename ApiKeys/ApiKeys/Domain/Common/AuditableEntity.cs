using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }

    public User? CreatedBy { get; set; } = null!;

    public string? CreatedById { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? LastModifiedById { get; set; }
}
