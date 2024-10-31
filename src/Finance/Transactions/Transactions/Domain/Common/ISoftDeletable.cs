using YourBrand.Identity;

namespace YourBrand.Transactions.Domain.Common;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}