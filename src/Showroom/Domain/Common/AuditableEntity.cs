using YourBrand.Identity;
using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Domain.Common;

public abstract class AuditableEntity : Entity
{
    public DateTime Created { get; set; }

    public UserId? CreatedById { get; set; }

    public User? CreatedBy { get; set; }

    public DateTime? LastModified { get; set; }

    public UserId? LastModifiedById { get; set; }

    public User? LastModifiedBy { get; set; }

    public string? ApplicationId { get; set; }
}