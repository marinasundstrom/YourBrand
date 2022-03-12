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

    public List<ConsultantProfile> ConsultantProfiles { get; set; } = new List<ConsultantProfile>();

    public List<ConsultantProfileSkill> ConsultantProfileSkills { get; set; } = new List<ConsultantProfileSkill>();
}

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
