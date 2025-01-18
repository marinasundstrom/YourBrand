using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class Employment : AuditableEntity<string>, IHasTenant
{
    public Employment()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string PersonProfileId { get; set; } = null!;

    public Company Employer { get; set; } = null!;

    public EmploymentType EmploymentType { get; set; }

    public string? Location { get; set; }

    public string? Description { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public List<EmploymentRole> Roles { get; set; } = new List<EmploymentRole>();

    public List<Assignment> Assignments { get; set; } = new List<Assignment>();

    public List<PersonProfileProject> Projects { get; set; } = new List<PersonProfileProject>();

    public List<PersonProfileExperienceSkill> Skills { get; set; } = new List<PersonProfileExperienceSkill>();
}
