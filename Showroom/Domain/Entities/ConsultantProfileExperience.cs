using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class ConsultantProfileExperience : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public string Title { get; set; } = null!;

    public Company Company { get; set; }

    public string CompanyId { get; set; }

    public string? Location { get; set; }

    public Employment? Employment { get; set; }

    public string? EmploymentType { get; set; } = null!;

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public bool Current { get; set; }
    
    public bool Highlight { get; set; }

    public string? Description { get; set; }

    public HashSet<ConsultantProfileExperienceSkill> Skills { get; private set; } = new HashSet<ConsultantProfileExperienceSkill>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
