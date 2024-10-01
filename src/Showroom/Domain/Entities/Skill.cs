using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class Skill : AuditableEntity, ISoftDeletable
{
    public string Id { get; set; } = null!;

    public SkillArea Area { get; set; } = null!;

    public string Name { get; set; } = null!;
    public string Slug { get; set; } = null!;

    public string? Description { get; set; }

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public List<PersonProfile> PersonProfiles { get; set; } = new List<PersonProfile>();

    public List<PersonProfileSkill> PersonProfileSkills { get; set; } = new List<PersonProfileSkill>();
}