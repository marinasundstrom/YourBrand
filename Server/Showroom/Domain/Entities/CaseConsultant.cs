using Skynet.Showroom.Domain.Common;

namespace Skynet.Showroom.Domain.Entities;

public class CaseConsultant : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public ConsultantProfile ConsultantProfile { get; set; } = null!;

    public string? Presentation { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
