
using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record TimeSheetTaskAddedEvent : DomainEvent
{
    public TimeSheetTaskAddedEvent(string timeSheetId, string timeSheetTaskId, string taskId)
    {
        TimeSheetId = timeSheetId;
        TimeSheetTaskId = timeSheetTaskId;
        TaskId = taskId;
    }

    public string TimeSheetId { get; }

    public string TimeSheetTaskId { get; }

    public string TaskId { get; }
}