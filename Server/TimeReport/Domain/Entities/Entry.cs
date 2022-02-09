
using TimeReport.Domain.Common;
using TimeReport.Domain.Common.Interfaces;
using TimeReport.Domain.Events;

namespace TimeReport.Domain.Entities;

public class Entry : AuditableEntity, IHasDomainEvent
{
    public string Id { get; set; } = null!;

    public User User { get; set; } = null!;

    public Project Project { get; set; } = null!;

    public Activity Activity { get; set; } = null!;

    public TimeSheet TimeSheet { get; set; } = null!;

    public TimeSheetActivity TimeSheetActivity { get; set; } = null!;

    public MonthEntryGroup? MonthGroup { get; set; }

    public DateOnly Date { get; set; }

    public double? Hours { get; set; }

    public string? Description { get; set; }

    public EntryStatus Status { get; set; } = EntryStatus.Unlocked;

    public List<DomainEvent> DomainEvents { get; set; } = new List<DomainEvent>();

    public void UpdateHours(double? value)
    {
        if (Hours == value) return;

        Hours = value;

        DomainEvents.Add(new EntryHoursUpdatedEvent(Project.Id, TimeSheet.Id, Activity.Id, Id, value));
    }
}