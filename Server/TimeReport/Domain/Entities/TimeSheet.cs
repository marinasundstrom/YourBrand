
using TimeReport.Domain.Common;
using TimeReport.Domain.Common.Interfaces;

namespace TimeReport.Domain.Entities;

public class TimeSheet : AuditableEntity, ISoftDelete
{
    public string Id { get; set; } = null!;

    public User? User { get; set; }

    public int Year { get; set; }

    public int Week { get; set; }

    public DateTime From { get; set; }

    public DateTime To { get; set; }

    public TimeSheetStatus Status { get; set; }

    public List<TimeSheetActivity> Activities { get; set; } = new List<TimeSheetActivity>();

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public DateTime? Deleted { get; set; }
    public string? DeletedBy { get; set; }
}