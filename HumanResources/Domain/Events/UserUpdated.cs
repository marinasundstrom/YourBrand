using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class UserUpdated : DomainEvent
{
    public string UserId { get; set; }

    public UserUpdated(string userId)
    {
        UserId = userId;
    }
}
