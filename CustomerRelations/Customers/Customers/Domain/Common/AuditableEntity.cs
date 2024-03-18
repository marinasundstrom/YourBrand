namespace YourBrand.Customers.Domain.Common;

public abstract class AuditableEntity: Entity
{
    public DateTime? Created { get; set; }

    public string? CreatedById { get; set; } = null!;

    public DateTime? LastModified { get; set; }

    public string? LastModifiedById { get; set; }
}
