using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record TeamCreated : DomainEvent
{
    public string TeamId { get; set; }

    public TeamCreated(string teamId)
    {
        TeamId = teamId;
    }
}