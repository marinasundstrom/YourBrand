
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Project : AuditableEntity, ISoftDelete
{
    readonly List<Expense> _expenses = new List<Expense>();
    readonly List<Activity> _activities = new List<Activity>();
    readonly List<Entry> _entries = new List<Entry>();
    readonly List<ProjectMembership> _memberships = new List<ProjectMembership>();

    protected Project()
    {
        
    }

    public Project(string name, string? description)
    {
        Id = Guid.NewGuid().ToString();
        Name = name;
        Description = description;
    }

    public string Id { get; set; } = null!;

    public void AddActivity(Activity activity)
    {
        activity.Project = this;
        _activities.Add(activity);
    }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public Organization Organization { get; set; } = null!;

    /// <summary>
    /// Expected hours per week / timesheet
    /// </summary>
    public double? ExpectedHoursWeekly { get; set; }

    /// <summary>
    /// Required hours per week / timesheet
    /// </summary>
    public double? RequiredHoursWeekly { get; set; }

    public IReadOnlyCollection<Expense> Expenses => _expenses.AsReadOnly();

    public IReadOnlyCollection<Activity> Activities => _activities.AsReadOnly();

    public IReadOnlyCollection<Entry> Entries => _entries.AsReadOnly();

    public IReadOnlyCollection<ProjectMembership> Memberships => _memberships.AsReadOnly();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}