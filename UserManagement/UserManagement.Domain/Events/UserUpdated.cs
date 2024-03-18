using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Domain.Events;

public record UserUpdated : DomainEvent
{
    public string UserId { get; set; }

    public UserUpdated(string userId)
    {
        UserId = userId;
    }
}
