using System;

using Skynet.Showroom.Domain.Common;

namespace Skynet.Showroom.Domain.Entities;

public class ConsultantProfile : AuditableEntity, ISoftDelete
{
    public string Id { get; set; }

    public string FirstName { get; set; }  = null!;

    public string LastName { get; set; }  = null!;

    public string? DisplayName { get; set; }

    public User? User { get; set; }

    public string? UserId { get; set; }

    public Organization Organization { get; set; } = null!;

    public string OrganizationId { get; set; } = null!;

    public CompetenceArea CompetenceArea { get; set; }  = null!;

    public string CompetenceAreaId { get; set; } = null!;

    public string? ProfileImage { get; set; }

    public string Headline { get; set; }  = null!;

    public string ShortPresentation { get; set; }  = null!;

    public string Presentation { get; set; }  = null!;

    public string? ProfileVideo { get; set; }

    public User Manager { get; set; }  = null!;

    public string ManagerId { get; set; } = null!;

    public DateTime? AvailableFromDate { get; set; }

    public string? Email { get; set; }

    public string? PhoneNumber { get; set; }


    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
