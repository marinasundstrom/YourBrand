using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record TeamMemberRemoved : DomainEvent
{
    public string TeamId { get; set; }

    public string PersonId { get; set; }

    public TeamMemberRemoved(string teamId, string personId)
    {
        TeamId = teamId;
        PersonId = personId;
    }
}