using YourBrand.Identity;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Common.Interfaces;

public interface ISoftDeletable
{
    DateTime? Deleted { get; set; }

    UserId? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}