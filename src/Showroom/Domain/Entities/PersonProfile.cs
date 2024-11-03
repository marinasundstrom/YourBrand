using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;
using YourBrand.Tenancy;

namespace YourBrand.Showroom.Domain.Entities;

public class PersonProfile : AuditableEntity<string>, IHasTenant, ISoftDeletable
{
    public PersonProfile()
        : base(Guid.NewGuid().ToString())
    {

    }

    public TenantId TenantId { get; set; } = null!;

    public Organization Organization { get; set; } = null!;

    public OrganizationId OrganizationId { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? DisplayName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Location { get; set; }

    public User? User { get; set; }

    public UserId? UserId { get; set; }

    public Industry Industry { get; set; } = null!;

    public int IndustryId { get; set; }

    public CompetenceArea CompetenceArea { get; set; } = null!;

    public string CompetenceAreaId { get; set; } = null!;

    public Availability Availability { get; set; } = Availability.Available;

    public string? ProfileImage { get; set; }

    public string Headline { get; set; } = null!;

    public string ShortPresentation { get; set; } = null!;

    public string Presentation { get; set; } = null!;

    public string? ProfileVideo { get; set; }

    public User? Manager { get; set; } = null!;

    public UserId? ManagerId { get; set; } = null!;

    public DateTime? AvailableFromDate { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public List<PersonProfileExperience> Experience { get; set; } = new List<PersonProfileExperience>();

    public List<PersonProfileIndustryExperiences> IndustryExperience { get; set; } = new List<PersonProfileIndustryExperiences>();

    public List<Skill> Skills { get; set; } = new List<Skill>();

    public List<PersonProfileSkill> PersonProfileSkills { get; set; } = new List<PersonProfileSkill>();

    public List<Employment> Employments { get; set; } = new List<Employment>();

    public bool IsDeleted { get; set; }
    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}