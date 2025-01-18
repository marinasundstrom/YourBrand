using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileProject : AuditableEntity<string>, IHasTenant
{
    public PersonProfileProject()
        : base(Guid.NewGuid().ToString())
    {

    }
    public TenantId TenantId { get; set; } = null!;

    public Project Project { get; set; }

    public PersonProfile PersonProfile { get; set; } = null!;

    public Employment? Employment { get; set; }

    public Assignment? Assignment { get; set; }

    public EmploymentRole? Role { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public List<PersonProfileSkill> Skills { get; set; } = new List<PersonProfileSkill>();
}
