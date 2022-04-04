using YourBrand.Showroom.Domain.Entities;

namespace YourBrand.Showroom.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}