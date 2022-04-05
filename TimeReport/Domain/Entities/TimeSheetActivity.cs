
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class TimeSheetActivity : AuditableEntity, ISoftDelete
{
    private readonly List<Entry> _entries = new List<Entry>();

    public TimeSheetActivity(TimeSheet timeSheet, Project project, Activity activity)
    {
        Id = Guid.NewGuid().ToString();
        TimeSheet = timeSheet;
        Project = project;
        Activity = activity;
    }

    internal TimeSheetActivity()
    {

    }

    public string Id { get; private set; } = null!; 

    public TimeSheet TimeSheet { get; private set; } = null!;

    public Project Project { get; private set; } = null!;

    public Activity Activity { get; private set; } = null!;

    public IReadOnlyList<Entry> Entries => _entries.AsReadOnly();

    // public decimal? HourlyRate { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public Entry AddEntry(DateOnly date, double? hours, string? description)
    {
        var entry = new Entry(TimeSheet.User, Project, Activity, TimeSheet, this, date, hours, description);
        _entries.Add(entry);
        return entry;
    }

    public Entry? GetEntry(DateOnly date)
    {
        return _entries.FirstOrDefault(e => e.Date == date);
    }
}