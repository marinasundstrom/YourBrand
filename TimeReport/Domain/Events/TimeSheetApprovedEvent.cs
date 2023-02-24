
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record TimeSheetApprovedEvent : DomainEvent
{
    public TimeSheetApprovedEvent(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}
