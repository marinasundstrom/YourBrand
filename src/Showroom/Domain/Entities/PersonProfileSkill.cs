using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;
using YourBrand.Showroom.Domain.ValueObjects;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfileSkill : AuditableEntity<string>, IHasTenant, ISoftDeletableWithAudit<User>
{
    public PersonProfileSkill() : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    //public OrganizationId OrganizationId { get; set; } = null!;

    public PersonProfile PersonProfile { get; set; } = null!;

    public string PersonProfileId { get; set; } = null!;

    public Skill Skill { get; set; } = null!;

    public string SkillId { get; set; } = null!;

    public int? Years { get; set; }

    public SkillLevel? Level { get; set; }

    public string? Comment { get; set; }

    public Link? Link { get; set; }

    public List<PersonProfile> PersonProfiles { get; set; } = new List<PersonProfile>();

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}