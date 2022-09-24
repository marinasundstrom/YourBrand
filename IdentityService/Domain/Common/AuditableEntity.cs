using YourBrand.IdentityService.Domain.Common.Interfaces;

namespace YourBrand.IdentityService.Domain.Common;

public abstract class AuditableEntity : Entity, IAuditableEntity
{
    public DateTime Created { get; set; }

    public string? CreatedBy { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public string? LastModifiedBy { get; set; }
}
