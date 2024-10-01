using YourBrand.Identity;

namespace YourBrand.Marketing.Domain.Entities;

public interface ISoftDeletable
{
    UserId? DeletedById { get; set; }
    DateTimeOffset? Deleted { get; set; }
}