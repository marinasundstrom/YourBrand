using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class ConsultantProfileExperienceSkill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public ConsultantProfileExperience ConsultantProfileExperience { get; set; } = null!;

    public ConsultantProfileSkill ConsultantProfileSkill { get; set; } = null!;

    public string ConsultantProfileSkillId { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
