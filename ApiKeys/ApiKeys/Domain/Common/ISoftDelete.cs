using YourBrand.ApiKeys.Domain.Entities;

namespace YourBrand.ApiKeys.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    User? DeletedBy { get; set; }

    string? DeletedById { get; set; }
}