
using TimeReport.Domain.Common;
using TimeReport.Domain.Common.Interfaces;

namespace TimeReport.Domain.Entities;

public class Project : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public List<Expense> Expenses { get; set; } = new List<Expense>();

    public List<Activity> Activities { get; set; } = new List<Activity>();

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public List<ProjectMembership> Memberships { get; set; } = new List<ProjectMembership>();

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}