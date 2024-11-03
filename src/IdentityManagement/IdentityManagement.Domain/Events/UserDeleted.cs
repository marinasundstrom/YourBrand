using YourBrand.Domain;
using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record UserDeleted : DomainEvent
{
    public string UserId { get; set; }

    public UserDeleted(string userId)
    {
        UserId = userId;
    }
}