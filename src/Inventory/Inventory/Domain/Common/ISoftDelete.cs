using YourBrand.Identity;

namespace YourBrand.Inventory.Domain.Common;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}