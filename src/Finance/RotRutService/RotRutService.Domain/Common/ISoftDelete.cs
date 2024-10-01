using YourBrand.Identity;

namespace YourBrand.RotRutService.Domain.Common;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}