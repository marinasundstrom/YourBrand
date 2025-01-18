using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class EmploymentRole : AuditableEntity<string>, IHasTenant
{
    public EmploymentRole()
        : base(Guid.NewGuid().ToString())
    {

    }
    public TenantId TenantId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public Employment? Employment { get; set; }

    public string? EmploymentId { get; set; }

    public Assignment? Assignment { get; set; }

    public string? AssignmentId { get; set; }

    public string Title { get; set; } = null!;

    public string? Location { get; set; }

    public bool IsRemote { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public List<PersonProfileSkill> Skills { get; set; } = new List<PersonProfileSkill>();
}