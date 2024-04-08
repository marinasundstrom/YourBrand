using YourBrand.Identity;

namespace YourBrand.Transactions.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}