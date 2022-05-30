
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public class TimeSheetApprovedEvent : DomainEvent
{
    public TimeSheetApprovedEvent(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}
