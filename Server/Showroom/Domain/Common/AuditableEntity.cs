using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Domain.Common;

public abstract class AuditableEntity
{
    public DateTime Created { get; set; }

    public string? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public string? LastModifiedById { get; set; }

    public User? LastModifiedBy { get; set; }
}