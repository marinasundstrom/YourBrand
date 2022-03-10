using Skynet.Showroom.Domain.Common;

namespace Skynet.Showroom.Domain.Entities;

public class SkillGroup : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public CompetenceArea CompetenceArea { get; set; } = null!;

    public string Name { get; set; }  = null!;
    public string? Description { get; set; }

    public ICollection<Skill> Skills { get; set; } = new List<Skill>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class Skill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public SkillGroup Group { get; set; } = null!;

    public string Name { get; set; }  = null!;
    public string? Description { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}

public class ConsultantProfileSkill : AuditableEntity, ISoftDelete
{
    public string Id { get; set; }

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public Skill Skill { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
