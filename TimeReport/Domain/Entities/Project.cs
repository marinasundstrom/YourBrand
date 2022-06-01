
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class Project : AuditableEntity, ISoftDelete
{
    readonly HashSet<Team> _teams = new HashSet<Team>();
    readonly HashSet<Expense> _expenses = new HashSet<Expense>();
    readonly HashSet<Activity> _activities = new HashSet<Activity>();
    readonly HashSet<Entry> _entries = new HashSet<Entry>();
    readonly HashSet<ProjectMembership> _memberships = new HashSet<ProjectMembership>();
    readonly HashSet<ProjectTeam> _projectTeams = new HashSet<ProjectTeam>();

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

    public IReadOnlyCollection<Team> Teams => _teams;

    public IReadOnlyCollection<Expense> Expenses => _expenses;

    public IReadOnlyCollection<Activity> Activities => _activities;

    public IReadOnlyCollection<Entry> Entries => _entries;

    public IReadOnlyCollection<ProjectMembership> Memberships => _memberships;

    public IReadOnlyCollection<ProjectTeam> ProjectTeams => _projectTeams;

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }
}
