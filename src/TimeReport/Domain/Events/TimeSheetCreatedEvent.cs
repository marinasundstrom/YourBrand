
using YourBrand.Domain;
using YourBrand.TimeReport.Domain.Common;

namespace YourBrand.TimeReport.Domain.Events;

public record TimeSheetCreatedEvent : DomainEvent
{
    public TimeSheetCreatedEvent(string timeSheetId)
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}