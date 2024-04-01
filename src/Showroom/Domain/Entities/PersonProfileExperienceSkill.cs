using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileExperienceSkill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public PersonProfileExperience PersonProfileExperience { get; set; } = null!;

    public PersonProfileSkill PersonProfileSkill { get; set; } = null!;

    public string PersonProfileSkillId { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}