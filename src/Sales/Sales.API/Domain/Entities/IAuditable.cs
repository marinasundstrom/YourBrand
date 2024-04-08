namespace YourBrand.Sales.Domain.Entities;

public interface IAuditable
{
    User? CreatedBy { get; set; }
    UserId? CreatedById { get; set; }
    DateTimeOffset Created { get; set; }

    User? LastModifiedBy { get; set; }
    UserId? LastModifiedById { get; set; }
    DateTimeOffset? LastModified { get; set; }
}