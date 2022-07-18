using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class TeamUpdated : DomainEvent
{
    public string TeamId { get; set; }

    public TeamUpdated(string teamId)
    {
        TeamId = teamId;
    }
}
