using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class ConsultantProfileSkill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public string ConsultantProfileId { get; set; } = null!;

    public Skill Skill { get; set; } = null!;

    public string SkillId { get; set; } = null!;

    public int? Years { get; set; }

    public List<ConsultantProfile> ConsultantProfiles { get; set; } = new List<ConsultantProfile>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
