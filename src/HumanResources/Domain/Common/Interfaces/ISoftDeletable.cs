namespace YourBrand.HumanResources.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    string? DeletedBy { get; set; }
}