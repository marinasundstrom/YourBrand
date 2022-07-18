using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class UserCreated : DomainEvent
{
    public string UserId { get; set; }

    public UserCreated(string userId)
    {
        UserId = userId;
    }
}
