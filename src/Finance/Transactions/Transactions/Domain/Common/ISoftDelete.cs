using YourBrand.Identity;

namespace YourBrand.Transactions.Domain.Common;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}