namespace YourBrand.IdentityManagement.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    string? DeletedBy { get; set; }
}