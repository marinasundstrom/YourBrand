
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Project : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    /// <summary>
    /// Expected hours per week / timesheet
    /// </summary>
    public double? ExpectedHoursWeekly { get; set; }

    /// <summary>
    /// Required hours per week / timesheet
    /// </summary>
    public double? RequiredHoursWeekly { get; set; }

    public List<Expense> Expenses { get; set; } = new List<Expense>();

    public List<Activity> Activities { get; set; } = new List<Activity>();

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public List<ProjectMembership> Memberships { get; set; } = new List<ProjectMembership>();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}