namespace YourBrand.Analytics.Domain.Entities;

public interface IAuditable
{
    string? CreatedById { get; set; }
    DateTimeOffset Created { get; set; }

    string? LastModifiedById { get; set; }
    DateTimeOffset? LastModified { get; set; }
}
