using YourBrand.Domain;
using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record UserCreated : DomainEvent
{
    public string UserId { get; set; }

    public UserCreated(string userId)
    {
        UserId = userId;
    }
}