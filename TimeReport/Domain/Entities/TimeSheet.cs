
using System.Globalization;

using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;

namespace YourBrand.TimeReport.Domain.Entities;

public class TimeSheet : AuditableEntity, ISoftDelete
{
    private readonly List<TimeSheetActivity> _activities = new List<TimeSheetActivity>();
    private readonly List<Entry> _entries = new List<Entry>();

    public TimeSheet(User user, int year, int week)
    {
        Id = Guid.NewGuid().ToString();
        User = user;
        Year = year;
        Week = week;

        From = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        To = From.AddDays(6);
    }

    internal TimeSheet()
    {

    }

    public string Id { get; private set; } = null!;

    public User User { get; private set; } = null!;

    public string UserId { get; private set; } = null!;

    public int Year { get; private set; }

    public int Week { get; private set; }

    public DateTime From { get; private set; }

    public DateTime To { get; private set; }

    public TimeSheetStatus Status { get; private set; }

    public void UpdateStatus(TimeSheetStatus status)
    {
        Status = status;
    }

    public IReadOnlyList<TimeSheetActivity> Activities => _activities.AsReadOnly();

    public IReadOnlyList<Entry> Entries => _entries.AsReadOnly();

    public DateTime? Deleted { get; set; }
    public string? DeletedById { get; set; }
    public User? DeletedBy { get; set; }

    public TimeSheetActivity AddActivity(Activity activity)
    {
        var tsActivity = new TimeSheetActivity(this, activity.Project, activity);
        _activities.Add(tsActivity);
        return tsActivity;
    }

    public TimeSheetActivity? GetActivity(Activity activity)
    {
        return _activities
               .FirstOrDefault(x => x.TimeSheet.Id == this.Id && x.Activity.Id == activity.Id);
    }

    public double GetTotalHours()
    {
        return _entries.Sum(x => x.Hours.GetValueOrDefault());
    }

    public double GetTotalHoursForDate(DateOnly date)
    {
        return _entries
            .Where(e => e.Date == date)
            .Sum(e => e.Hours.GetValueOrDefault());
    }
}