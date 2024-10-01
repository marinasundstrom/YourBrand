using YourBrand.Identity;

namespace YourBrand.Marketing.Domain.Common;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}