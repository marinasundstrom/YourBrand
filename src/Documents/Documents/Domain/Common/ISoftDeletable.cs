using YourBrand.Identity;

namespace YourBrand.Documents.Domain.Common;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}