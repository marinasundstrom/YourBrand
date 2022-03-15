using Skynet.Showroom.Domain.Common;

namespace Skynet.Showroom.Domain.Entities;

public class Case : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public CaseStatus Status { get; set; }

    public ICollection<CaseConsultant> Consultants { get; set; } = null!;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
