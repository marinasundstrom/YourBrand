
using YourBrand.TimeReport.Domain.Common;
using YourBrand.TimeReport.Domain.Common.Interfaces;
using YourBrand.TimeReport.Domain.Events;

namespace YourBrand.TimeReport.Domain.Entities;

public class Entry : AuditableEntity
{
    public Entry(User user, Project project, Activity activity, TimeSheet timeSheet, TimeSheetActivity timeSheetActivity,
        DateOnly date, double? hours, string? description)
    {
        Id = Guid.NewGuid().ToString();
        User = user;
        Project = project;
        Activity = activity;
        TimeSheet = timeSheet;
        TimeSheetActivity = timeSheetActivity;
        Date = date;
        Hours = hours;
        Description = description;

        AddDomainEvent(new EntryCreatedEvent(project.Id, timeSheet.Id, activity.Id, Id, hours));
    }

    internal Entry()
    {

    }

    public string Id { get; private set; } = null!;

    public User User { get; private set; } = null!;

    public string UserId { get; private set; } = null!;

    public Project Project { get; private set; } = null!;

    public Activity Activity { get; private set; } = null!;

    public TimeSheet TimeSheet { get; private set; } = null!;

    public TimeSheetActivity TimeSheetActivity { get; private set; } = null!;

    public ReportingPeriod? MonthGroup { get; set; }

    public DateOnly Date { get; set; }

    public double? Hours { get; set; }

    public string? Description { get; set; }

    public EntryStatus Status { get; private set; } = EntryStatus.Unlocked;

    private void UpdateStatus(EntryStatus status)
    {
        Status = status;
    }

    public void Lock()
    {
        UpdateStatus(EntryStatus.Locked);
    }

    public void Unlock()
    {
        UpdateStatus(EntryStatus.Unlocked);
    }

    public void UpdateHours(double? value)
    {
        if (Hours == value) return;

        Hours = value;

        AddDomainEvent(new EntryHoursUpdatedEvent(Project.Id, TimeSheet.Id, Activity.Id, Id, value));
    }
}