using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class UserDeleted : DomainEvent
{
    public string UserId { get; set; }

    public UserDeleted(string userId)
    {
        UserId = userId;
    }
}
