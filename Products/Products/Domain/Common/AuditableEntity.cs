namespace YourBrand.Products.Domain.Common;

public abstract class AuditableEntity: BaseEntity
{
    public DateTime Created { get; set; }

    public string CreatedById { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public string? LastModifiedById { get; set; }
}
