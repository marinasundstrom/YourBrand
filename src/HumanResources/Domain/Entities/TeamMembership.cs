using YourBrand.Domain;
using YourBrand.HumanResources.Domain.Common;

namespace YourBrand.HumanResources.Domain.Entities;

public class TeamMembership : AuditableEntity<string>, ISoftDeletable
{
    private TeamMembership()
    {
    }

    public TeamMembership(Person person)
        : base(Guid.NewGuid().ToString())
    {
        Person = person;
    }

    public Team Team { get; set; } = null!;

    public string TeamId { get; set; } = null!;

    public Person Person { get; set; } = null!;

    public string PersonId { get; set; } = null!;

    public bool IsDeleted { get; set; }

    public DateTimeOffset? Deleted { get; set; }

    public string? DeletedBy { get; set; }
}