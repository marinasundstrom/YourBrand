
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class ActivityType: AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; } = null!;

    public Project? Project { get; set; } = null!;

    public bool ExcludeHours { get; set; }

    public List<Activity> Activities { get; set; } = new List<Activity>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}