namespace YourBrand.Ticketing.Domain.Entities;

public interface IAuditable
{
    User? CreatedBy { get; set; }
    string? CreatedById { get; set; }
    DateTimeOffset Created { get; set; }

    User? LastModifiedBy { get; set; }
    string? LastModifiedById { get; set; }
    DateTimeOffset? LastModified { get; set; }
}
