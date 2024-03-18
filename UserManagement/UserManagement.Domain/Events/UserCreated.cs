using YourBrand.UserManagement.Domain.Common;

namespace YourBrand.UserManagement.Domain.Events;

public record UserCreated : DomainEvent
{
    public string UserId { get; set; }

    public UserCreated(string userId)
    {
        UserId = userId;
    }
}
