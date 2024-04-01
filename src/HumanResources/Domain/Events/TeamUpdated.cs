using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Events;

public record TeamUpdated : DomainEvent
{
    public string TeamId { get; set; }

    public TeamUpdated(string teamId)
    {
        TeamId = teamId;
    }
}