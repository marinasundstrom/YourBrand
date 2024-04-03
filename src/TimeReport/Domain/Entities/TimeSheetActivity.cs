
using System.Globalization;

using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Domain.Entities;

public class TimeSheetActivity : AuditableEntity, IHasTenant, ISoftDelete
{
    private readonly HashSet<Entry> _entries = new HashSet<Entry>();

    public TimeSheetActivity(TimeSheet timeSheet, Project project, Activity activity)
    {
        Id = Guid.NewGuid().ToString();
        TimeSheet = timeSheet;
        Project = project;
        Activity = activity;

        AddDomainEvent(new TimeSheetActivityAddedEvent(TimeSheet.Id, Id, Activity.Id));
    }

    internal TimeSheetActivity()
    {

    }

    public string Id { get; private set; } = null!;

    public TenantId TenantId { get; set; }

    public TimeSheet TimeSheet { get; private set; } = null!;

    public Project Project { get; private set; } = null!;

    public Activity Activity { get; private set; } = null!;

    public IReadOnlyCollection<Entry> Entries => _entries;

    // public decimal? HourlyRate { get; set; }

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public Entry AddEntry(DateOnly date, double? hours, string? description)
    {
        int week = GetWeekFromDate(date);

        if (TimeSheet.Year != date.Year || TimeSheet.Week != week)
        {
            throw new InvalidOperationException("Date is not in Week.");
        }

        if (TimeSheet.Entries.Any(e => e.Date == date))
        {
            throw new InvalidOperationException("Entry for this date already exists");
        }

        if (hours < 0)
        {
            throw new InvalidOperationException("Hours must not be negative,");
        }

        if (hours > 8)
        {
            throw new InvalidOperationException("Hours must not be greater than 8.");
        }

        var entry = new Entry(TimeSheet.User, Project, Activity, TimeSheet, this, date, hours, description);
        _entries.Add(entry);
        TimeSheet.AddEntry(entry);
        return entry;
    }

    private static int GetWeekFromDate(DateOnly date)
    {
        return ISOWeek.GetWeekOfYear(
            date.ToDateTime(
                TimeOnly.FromTimeSpan(
                    TimeSpan.FromMinutes(10))));
    }

    public Entry? GetEntryByDate(DateOnly date)
    {
        return _entries.FirstOrDefault(e => e.Date == date);
    }
}