
using YourCompany.TimeReport.Domain.Common;
using YourCompany.TimeReport.Domain.Common.Interfaces;

namespace YourCompany.TimeReport.Domain.Entities;

public class ProjectMembership : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public User User { get; set; } = null!;

    public DateTime? From { get; set; }

    public DateTime? Thru { get; set; }

    /// <summary>
    /// Expected hours per week / timesheet
    /// </summary>
    public double? ExpectedHoursWeekly { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}