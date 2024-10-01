using YourBrand.Identity;

namespace YourBrand.Domain;

public interface ISoftDeletable
{
    UserId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}