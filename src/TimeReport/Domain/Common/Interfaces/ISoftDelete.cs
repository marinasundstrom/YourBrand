using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Common.Interfaces;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}