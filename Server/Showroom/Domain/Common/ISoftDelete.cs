using Skynet.Showroom.Domain.Entities;

namespace Skynet.Showroom.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}