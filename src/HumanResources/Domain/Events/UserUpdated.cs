using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record UserUpdated : DomainEvent
{
    public string UserId { get; set; }

    public UserUpdated(string userId)
    {
        UserId = userId;
    }
}