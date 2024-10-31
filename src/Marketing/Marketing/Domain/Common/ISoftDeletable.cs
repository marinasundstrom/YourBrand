using YourBrand.Identity;

namespace YourBrand.Marketing.Domain.Common;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}