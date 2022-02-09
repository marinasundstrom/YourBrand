using System;
namespace TimeReport.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string title, string details)
        : base(title)
    {
        Details = details;
    }

    public string Title => Message;

    public string Details { get; set; }
}

public class TimeSheetNotFoundException : DomainException
{
    public TimeSheetNotFoundException(string timeSheetId)
        : base("TimeSheet not found", $"TimeSheet with Id {timeSheetId} was not found.")
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}

public class TimeSheetClosedException : DomainException
{
    public TimeSheetClosedException(string timeSheetId)
        : base("TimeSheet is closed", $"TimeSheet with Id {timeSheetId} is closed.")
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}

public class EntryNotFoundException : DomainException
{
    public EntryNotFoundException(string entryId)
         : base("Entry not found", $"Entry with Id {entryId} was not found.")
    {
        EntryId = entryId;
    }

    public string EntryId { get; }
}

public class ProjectMembershipNotFoundException : DomainException
{
    public ProjectMembershipNotFoundException(string membershipId)
         : base("Project Membership not found", $"Project Membership with Id {membershipId} was not found.")
    {
        ProjectMembershipId = membershipId;
    }

    public string ProjectMembershipId { get; }
}

public class UserNotFoundException : DomainException
{
    public UserNotFoundException(string userName)
         : base("User not found", $"User {userName} was not found.")
    {
        UserName = userName;
    }

    public string UserName { get; }
}

public class UserAlreadyProjectMemberException : DomainException
{
    public UserAlreadyProjectMemberException(string userName, string projectId)
         : base("User is already a member", $"User {userName} is already a member of {projectId}.")
    {
        UserName = userName;
        ProjectId = projectId;
    }

    public string UserName { get; }

    public string ProjectId { get; }
}

public class EntryAlreadyExistsException : DomainException
{
    public EntryAlreadyExistsException(string timeSheetId, DateOnly date, string activityId)
        : base("Entry already exists", $"Entry already registered for {activityId} on {date}.")
    {
        TimeSheetId = timeSheetId;
        Date = date;
        ActivityId = activityId;
    }

    public string TimeSheetId { get; }

    public DateOnly Date { get; }

    public string ActivityId { get; }
}

public class MonthLockedException : DomainException
{
    public MonthLockedException(string timeSheetId)
        : base("Month is locked", $"The month is locked for TimeSheet withd Id {timeSheetId}.")
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}

public class ProjectNotFoundException : DomainException
{
    public ProjectNotFoundException(string projectId)
        : base("Project not found", $"Project with Id {projectId} was not found.")
    {
        ProjectId = projectId;
    }

    public string ProjectId { get; }
}

public class ActivityNotFoundException : DomainException
{
    public ActivityNotFoundException(string activityId)
        : base("Activity not found", $"Activity with Id {activityId} was not found.")
    {
        ActivityId = activityId;
    }

    public string ActivityId { get; }
}

public class DayHoursExceedPermittedDailyWorkingHoursException : DomainException
{
    public DayHoursExceedPermittedDailyWorkingHoursException(string timeSheetId, DateOnly date)
        : base("Total daily hours exceed working hours", $"The number of hours for {date} in TimeSheet with Id {timeSheetId} exceeds the permitted daily working hours.")
    {
        TimeSheetId = timeSheetId;
        Date = date;
    }

    public string TimeSheetId { get; }

    public DateOnly Date { get; }
}

public class WeekHoursExceedPermittedWeeklyWorkingHoursException : DomainException
{
    public WeekHoursExceedPermittedWeeklyWorkingHoursException(string timeSheetId)
        : base("Total weekly hours exceed working hours", $"The number of hours for TimeSheet with Id {timeSheetId} exceeds the permitted weekly working hours.")
    {
        TimeSheetId = timeSheetId;
    }

    public string TimeSheetId { get; }
}
