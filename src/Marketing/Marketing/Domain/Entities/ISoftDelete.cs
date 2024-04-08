using YourBrand.Identity;

namespace YourBrand.Marketing.Domain.Entities;

public interface ISoftDelete
{
    UserId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}