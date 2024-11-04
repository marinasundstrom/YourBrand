namespace YourBrand.Domain;

public interface ISoftDeletable
{
    bool IsDeleted { get; set; }
}