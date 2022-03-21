using YourCompany.Showroom.Domain.Entities;

namespace YourCompany.Showroom.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    string? DeletedById { get; set; }

    User? DeletedBy { get; set; }
}