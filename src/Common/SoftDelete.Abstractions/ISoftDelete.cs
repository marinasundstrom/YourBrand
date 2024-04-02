namespace YourBrand.Domain;

public interface ISoftDelete
{
    string? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}