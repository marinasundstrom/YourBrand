using YourBrand.Identity;

namespace YourBrand.RotRutService.Domain.Common;

public interface ISoftDeletable
{
    DateTimeOffset? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}