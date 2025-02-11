using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record EntryCreatedEvent : DomainEvent
{
    public EntryCreatedEvent(string projectId, string timeSheetId, string taskId, string entryId, double? hours)
    {
        ProjectId = projectId;
        TimeSheetId = timeSheetId;
        TaskId = taskId;
        EntryId = entryId;
        Hours = hours;
    }

    public string ProjectId { get; }

    public string TimeSheetId { get; }

    public string TaskId { get; }

    public string EntryId { get; }

    public double? Hours { get; }
}