
using TimeReport.Domain.Common;

namespace TimeReport.Domain.Entities;

public class MonthEntryGroup : AuditableEntity
{
    public string Id { get; set; } = null!;

    public User User { get; set; } = null!;

    public int Year { get; set; }

    public int Month { get; set; }

    public List<Entry> Entries { get; set; } = new List<Entry>();

    public EntryStatus Status { get; set; } = EntryStatus.Unlocked;
}