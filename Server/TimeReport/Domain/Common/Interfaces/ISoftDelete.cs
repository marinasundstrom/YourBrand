using YourCompany.TimeReport.Domain.Entities;

namespace YourCompany.TimeReport.Domain.Common.Interfaces;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}