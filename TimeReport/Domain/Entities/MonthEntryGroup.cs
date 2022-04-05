
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Entities;

public class MonthEntryGroup : AuditableEntity
{
    private List<Entry> _entries = new List<Entry>();

    public MonthEntryGroup(User user, int year, int month)
    {
        Id = Guid.NewGuid().ToString();
        User = user;
        Year = year;
        Month = month;
        Status = EntryStatus.Unlocked;
    }

    internal MonthEntryGroup()
    {
    }

    public string Id { get; set; } = null!;

    public User User { get; private set; } = null!;

    public string UserId { get; private set; } = null!;

    public int Year { get; private set; }

    public int Month { get; private set; }

    public IReadOnlyList<Entry> Entries => _entries.AsReadOnly();

    public EntryStatus Status { get; private set; } = EntryStatus.Unlocked;

    public void UpdateStatus(EntryStatus status)
    {
        Status = status;
    }

    public void AddEntry(Entry entry)
    {
        _entries.Add(entry);
    }
}