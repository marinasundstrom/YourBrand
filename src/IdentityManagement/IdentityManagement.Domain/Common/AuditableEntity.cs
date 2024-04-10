using YourBrand.IdentityManagement.Domain.Common.Interfaces;

namespace YourBrand.IdentityManagement.Domain.Common;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}