using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public class UserDeleted : DomainEvent
{
    public string UserId { get; set; }

    public UserDeleted(string userId)
    {
        UserId = userId;
    }
}
