using YourBrand.Identity;

namespace YourBrand.Documents.Domain.Common;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}