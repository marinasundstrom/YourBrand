namespace YourBrand.IdentityManagement.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    string? DeletedBy { get; set; }
}