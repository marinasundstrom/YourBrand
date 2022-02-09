using System;

using TimeReport.Domain.Common;

namespace TimeReport.Domain.Events;

public class EntryHoursUpdatedEvent : DomainEvent
{
    public EntryHoursUpdatedEvent(string projectId, string timeSheetId, string activityId, string entryId, double? hours)
    {
        ProjectId = projectId;
        TimeSheetId = timeSheetId;
        ActivityId = activityId;
        EntryId = entryId;
        Hours = hours;
    }

    public string ProjectId { get; }

    public string TimeSheetId { get; }

    public string ActivityId { get; }

    public string EntryId { get; }

    public double? Hours { get; }
}