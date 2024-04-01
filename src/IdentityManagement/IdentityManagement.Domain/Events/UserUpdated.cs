using YourBrand.IdentityManagement.Domain.Common;

namespace YourBrand.IdentityManagement.Domain.Events;

public record UserUpdated : DomainEvent
{
    public string UserId { get; set; }

    public UserUpdated(string userId)
    {
        UserId = userId;
    }
}