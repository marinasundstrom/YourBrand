namespace YourBrand.IdentityManagement.Domain.Exceptions;

public class DomainException(string title, string details) : Exception(title)
{
    public string Title => Message;

    public string Details { get; set; } = details;
}

public class TimeSheetNotFoundException(string timeSheetId) : DomainException("TimeSheet not found", $"TimeSheet with Id {timeSheetId} was not found.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public class TimeSheetClosedException(string timeSheetId) : DomainException("TimeSheet is closed", $"TimeSheet with Id {timeSheetId} is closed.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public class EntryNotFoundException(string entryId) : DomainException("Entry not found", $"Entry with Id {entryId} was not found.")
{
    public string EntryId { get; } = entryId;
}

public class ProjectMembershipNotFoundException(string membershipId) : DomainException("Project Membership not found", $"Project Membership with Id {membershipId} was not found.")
{
    public string ProjectMembershipId { get; } = membershipId;
}

public class UserNotFoundException(string personName) : DomainException("User not found", $"User {personName} was not found.")
{
    public string UserName { get; } = personName;
}

public class UserAlreadyProjectMemberException(string personName, string projectId) : DomainException("User is already a member", $"User {personName} is already a member of {projectId}.")
{
    public string UserName { get; } = personName;

    public string ProjectId { get; } = projectId;
}

public class EntryAlreadyExistsException(string timeSheetId, DateOnly date, string activityId) : DomainException("Entry already exists", $"Entry already registered for {activityId} on {date}.")
{
    public string TimeSheetId { get; } = timeSheetId;

    public DateOnly Date { get; } = date;

    public string ActivityId { get; } = activityId;
}

public class MonthLockedException(string timeSheetId) : DomainException("Month is locked", $"The month is locked for TimeSheet withd Id {timeSheetId}.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public class ProjectNotFoundException(string projectId) : DomainException("Project not found", $"Project with Id {projectId} was not found.")
{
    public string ProjectId { get; } = projectId;
}

public class ActivityNotFoundException(string activityId) : DomainException("Activity not found", $"Activity with Id {activityId} was not found.")
{
    public string ActivityId { get; } = activityId;
}

public class DayHoursExceedPermittedDailyWorkingHoursException(string timeSheetId, DateOnly date) : DomainException("Total daily hours exceed working hours", $"The number of hours for {date} in TimeSheet with Id {timeSheetId} exceeds the permitted daily working hours.")
{
    public string TimeSheetId { get; } = timeSheetId;

    public DateOnly Date { get; } = date;
}

public class WeekHoursExceedPermittedWeeklyWorkingHoursException(string timeSheetId) : DomainException("Total weekly hours exceed working hours", $"The number of hours for TimeSheet with Id {timeSheetId} exceeds the permitted weekly working hours.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public class OrganizationNotFoundException(string organizationName) : DomainException("Organization not found", $"Organization {organizationName} was not found.")
{
    public string OrganizationName { get; } = organizationName;
}