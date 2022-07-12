using YourBrand.Marketing.Domain.Common;

namespace YourBrand.Marketing.Domain.Events;

public class ProspectCreated : DomainEvent
{
    public ProspectCreated(string personId)
    {
        ProspectId = personId;
    }

    public string ProspectId { get; }
}
