using YourBrand.Identity;

namespace YourBrand.Inventory.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}