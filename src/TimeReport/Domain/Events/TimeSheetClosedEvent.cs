
using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record TimeSheetClosedEvent : DomainEvent
{
    public TimeSheetClosedEvent(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}