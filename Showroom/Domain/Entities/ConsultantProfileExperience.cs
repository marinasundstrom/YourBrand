using YourBrand.Showroom.Domain.Common;

namespace YourBrand.Showroom.Domain.Entities;

public class ConsultantProfileExperience : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public string Title { get; set; } = null!;
    public string CompanyName { get; set; } = null!;
    public string? Location { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }

    public string? Description { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
