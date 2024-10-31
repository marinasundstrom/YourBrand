namespace YourBrand.HumanResources.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    string? DeletedBy { get; set; }
}