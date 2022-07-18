using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class TeamDeleted : DomainEvent
{
    public string TeamId { get; set; }

    public TeamDeleted(string teamId)
    {
        TeamId = teamId;
    }
}