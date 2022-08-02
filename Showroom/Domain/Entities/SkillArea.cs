using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class SkillArea : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public CompetenceArea? CompetenceArea { get; set; } = null!;

    public string Name { get; set; }  = null!;
    public string Slug { get; set; }  = null!;

    public string? Description { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
