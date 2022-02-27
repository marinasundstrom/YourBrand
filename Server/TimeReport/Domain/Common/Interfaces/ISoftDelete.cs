using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Domain.Common.Interfaces;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}