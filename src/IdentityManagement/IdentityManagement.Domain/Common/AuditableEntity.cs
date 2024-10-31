using YourBrand.IdentityManagement.Domain.Common.Interfaces;

namespace YourBrand.IdentityManagement.Domain.Common;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTimeOffset Created { get; set; }

    public string? CreatedBy { get; set; } = null!;

    public DateTimeOffset? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}