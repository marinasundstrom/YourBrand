using YourBrand.Showroom.Domain.Common;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class Assignment : AuditableEntity<string>, IHasTenant
{
    public Assignment()
        : base(Guid.NewGuid().ToString())
    {

    }
    public TenantId TenantId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string PersonProfileId { get; set; } = null!;

    public Employment Employment { get; set; } = null!;

    public string EmploymentId { get; set; } = null!;

    public Company Company { get; set; } = null!;

    public AssignmentType AssignmentType { get; set; }

    public string? Location { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public List<EmploymentRole> Roles { get; set; } = new List<EmploymentRole>();

    public List<PersonProfileProject> Projects { get; set; } = new List<PersonProfileProject>();

    public List<PersonProfileExperienceSkill> Skills { get; set; } = new List<PersonProfileExperienceSkill>();
}