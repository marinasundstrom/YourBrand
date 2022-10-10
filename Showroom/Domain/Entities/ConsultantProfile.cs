using System;

using YourBrand.Showroom.Domain.Common;
using YourBrand.Showroom.Domain.Enums;

namespace YourBrand.Showroom.Domain.Entities;

public class ConsultantProfile : AuditableEntity, ISoftDelete
{
    public string Id { get; set; }

    public string FirstName { get; set; }  = null!;

    public string LastName { get; set; }  = null!;

    public string? DisplayName { get; set; }

    public DateTime? BirthDate { get; set; }

    public string? Location { get; set; }

    public User? User { get; set; }

    public string? UserId { get; set; }

    public Industry Industry { get; set; } = null!;

    public int IndustryId { get; set; }

    public Organization Organization { get; set; } = null!;

    public string OrganizationId { get; set; } = null!;

    public CompetenceArea CompetenceArea { get; set; }  = null!;

    public string CompetenceAreaId { get; set; } = null!;
    
    public Availability Availability { get; set; } = Availability.Available;

    public string? ProfileImage { get; set; }

    public string Headline { get; set; }  = null!;

    public string ShortPresentation { get; set; }  = null!;

    public string Presentation { get; set; }  = null!;

    public string? ProfileVideo { get; set; }

    public User? Manager { get; set; }  = null!;

    public string? ManagerId { get; set; } = null!;

    public DateTime? AvailableFromDate { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }

    public List<ConsultantProfileExperience> Experience { get; set; } = new List<ConsultantProfileExperience>();

    public List<Skill> Skills { get; set; } = new List<Skill>();

    public List<ConsultantProfileSkill> ConsultantProfileSkills { get; set; } = new List<ConsultantProfileSkill>();
    
    public List<Employment> Employments { get; set; } = new List<Employment>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
