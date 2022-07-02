using YourBrand.HumanResources.Domain.Common;
using YourBrand.HumanResources.Domain.Common.Interfaces;

namespace YourBrand.HumanResources.Domain.Entities;

public class TeamMembership : AuditableEntity, ISoftDelete
{
    private TeamMembership()
    {
    }

    public TeamMembership(Person user)
    {
        Id = Guid.NewGuid().ToString();
        Person = user;
    }

    public string Id { get; private set; }

    public Team Team { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public Person Person { get; set; } = null!;

    public string PersonId { get; set; } = null!;

    public DateTime? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}
