using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record TeamDeleted : DomainEvent
{
    public string TeamId { get; set; }

    public TeamDeleted(string teamId)
    {
        TeamId = teamId;
    }
}