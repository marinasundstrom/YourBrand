
using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record TimeSheetTaskDeletedEvent : DomainEvent
{
    public TimeSheetTaskDeletedEvent(string timeSheetId, string timeSheetTaskId, string activityId)
    {
        TimeSheetId = timeSheetId;
        TimeSheetTaskId = timeSheetTaskId;
        TaskId = activityId;
    }

    public string TimeSheetId { get; }

    public string TimeSheetTaskId { get; }

    public string TaskId { get; }
}