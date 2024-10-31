using YourBrand.Identity;

namespace YourBrand.Inventory.Domain.Common;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}