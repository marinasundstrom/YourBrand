using YourBrand.Identity;

namespace YourBrand.Domain;

public interface ISoftDelete
{
    UserId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}