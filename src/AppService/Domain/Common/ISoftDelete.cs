using YourBrand.Domain.Entities;
using YourBrand.Identity;

namespace YourBrand.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}