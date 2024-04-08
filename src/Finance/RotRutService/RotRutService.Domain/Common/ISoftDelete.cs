using YourBrand.Identity;

namespace YourBrand.RotRutService.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }
}