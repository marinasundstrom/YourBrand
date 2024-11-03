using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record TeamMemberAdded : DomainEvent
{
    public string TeamId { get; set; }

    public string PersonId { get; set; }

    public TeamMemberAdded(string teamId, string personId)
    {
        TeamId = teamId;
        PersonId = personId;
    }
}