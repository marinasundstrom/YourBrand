using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record UserDeleted : DomainEvent
{
    public string UserId { get; set; }

    public UserDeleted(string userId)
    {
        UserId = userId;
    }
}