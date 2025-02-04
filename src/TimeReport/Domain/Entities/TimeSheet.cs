
using System.Globalization;

using YourBrand.Domain;
using YourBrand.Identity;
using YourBrand.Tenancy;
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Domain.Entities;

public class TimeSheet : AuditableEntity<string>, IHasTenant, IHasOrganization, ISoftDeletableWithAudit<User>
{
    private readonly HashSet<TimeSheetTask> _activities = new HashSet<TimeSheetTask>();
    private readonly HashSet<Entry> _entries = new HashSet<Entry>();

    public TimeSheet(User user, int year, int week) : base(Guid.NewGuid().ToString())
    {
        User = user;
        Year = year;
        Week = week;

        From = ISOWeek.ToDateTime(year, week, DayOfWeek.Monday);
        To = From.AddDays(6);
    }

    protected TimeSheet()
    {

    }

    public TenantId TenantId { get; set; }

    public OrganizationId OrganizationId { get; set; } = null!;

    public User User { get; private set; } = null!;

    public UserId UserId { get; private set; } = null!;

    public int Year { get; private set; }

    public int Week { get; private set; }

    public DateTime From { get; private set; }

    public DateTime To { get; private set; }

    public TimeSheetStatus Status { get; private set; }

    private void UpdateStatus(TimeSheetStatus status)
    {
        Status = status;
    }

    public IReadOnlyCollection<TimeSheetTask> Tasks => _activities;

    public IReadOnlyCollection<Entry> Entries => _entries;

    public DateTimeOffset? Deleted { get; set; }
    public UserId? DeletedById { get; set; }

    public IEnumerable<Entry> GetEntriesByTaskId(string activityId)
    {
        return Entries.Where(x => x.Task.Id == activityId);
    }

    public bool IsDeleted { get; set; }

    public User? DeletedBy { get; set; }

    public TimeSheetTask AddTask(Task activity)
    {
        var tsTask = new TimeSheetTask(this, activity.Project, activity);
        tsTask.OrganizationId = OrganizationId;

        _activities.Add(tsTask);
        return tsTask;
    }

    public void DeleteTask(TimeSheetTask activity)
    {
        foreach (var entry in _entries.ToArray().Where(e => e.Status == EntryStatus.Unlocked))
        {
            _entries.Remove(entry);
        }

        _activities.Remove(activity);

        AddDomainEvent(new TimeSheetTaskAddedEvent(Id, activity.Id, activity.Task.Id));
    }

    internal void AddEntry(Entry entry)
    {
        _entries.Add(entry);
    }

    public TimeSheetTask? GetTask(string activityId)
    {
        return _activities
               .FirstOrDefault(x => x.TimeSheet.Id == this.Id && x.Task.Id == activityId);
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

    public void Approve()
    {
        foreach (var entry in Entries)
        {
            entry.Lock();
        }

        UpdateStatus(TimeSheetStatus.Approved);
        AddDomainEvent(new TimeSheetApprovedEvent(Id));
    }

    public void Close()
    {
        foreach (var entry in Entries)
        {
            entry.Lock();
        }

        UpdateStatus(TimeSheetStatus.Closed);
        AddDomainEvent(new TimeSheetClosedEvent(Id));
    }

    public void Reopen()
    {
        foreach (var entry in Entries)
        {
            entry.Unlock();
        }

        UpdateStatus(TimeSheetStatus.Open);
        AddDomainEvent(new TimeSheetReoponedEvent(Id));
    }
}