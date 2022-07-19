using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public class TeamCreated : DomainEvent
{
    public string TeamId { get; set; }

    public TeamCreated(string teamId)
    {
        TeamId = teamId;
    }
}
