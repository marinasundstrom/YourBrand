
using YourBrand.Messenger.Domain.Common;

namespace YourBrand.Messenger.Domain.Entities;

public class User : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? DisplayName { get; set; }

    public string Email { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public bool Hidden { get; set; }
}