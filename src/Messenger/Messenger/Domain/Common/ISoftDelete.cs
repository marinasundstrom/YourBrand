using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain.Common;

public interface ISoftDelete
{
    DateTime? Deleted { get; set; }

    User? DeletedBy { get; set; }

    string? DeletedById { get; set; }
}