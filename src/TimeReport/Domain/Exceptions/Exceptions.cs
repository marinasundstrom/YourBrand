namespace YourBrand.TimeReport;

public record TimeSheetNotFound(string timeSheetId) : Error(nameof(TimeSheetNotFound), "TimeSheet not found", $"TimeSheet with Id {timeSheetId} was not found.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public record TimeSheetClosed(string timeSheetId) : Error(nameof(TimeSheetClosed), "TimeSheet is closed", $"TimeSheet with Id {timeSheetId} is closed.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public record EntryNotFound(string entryId) : Error(nameof(EntryNotFound), "Entry not found", $"Entry with Id {entryId} was not found.")
{
    public string EntryId { get; } = entryId;
}

public record ProjectMembershipNotFound(string membershipId) : Error(nameof(ProjectMembershipNotFound), "Project Membership not found", $"Project Membership with Id {membershipId} was not found.")
{
    public string ProjectMembershipId { get; } = membershipId;
}

public record UserNotFound(string userName) : Error(nameof(UserNotFound), "User not found", $"User {userName} was not found.")
{
    public string UserName { get; } = userName;
}

public record UserAlreadyProjectMember(string userName, string projectId) : Error(nameof(UserAlreadyProjectMember), "User is already a member", $"User {userName} is already a member of {projectId}.")
{
    public string UserName { get; } = userName;

    public string ProjectId { get; } = projectId;
}

public record EntryAlreadyExists(string timeSheetId, DateOnly date, string taskId) : Error(nameof(EntryAlreadyExists), "Entry already exists", $"Entry already registered for {taskId} on {date}.")
{
    public string TimeSheetId { get; } = timeSheetId;

    public DateOnly Date { get; } = date;

    public string TaskId { get; } = taskId;
}

public record MonthLocked(string timeSheetId) : Error(nameof(MonthLocked), "Month is locked", $"The month is locked for TimeSheet withd Id {timeSheetId}.")
{
    public string TimeSheetId { get; } = timeSheetId;
}

public record ProjectNotFound(string projectId) : Error(nameof(ProjectNotFound), "Project not found", $"Project with Id {projectId} was not found.")
{
    public string ProjectId { get; } = projectId;
}

public record TaskNotFound(string taskId) : Error(nameof(TaskNotFound), "Task not found", $"Task with Id {taskId} was not found.")
{
    public string TaskId { get; } = taskId;
}

public record DayHoursExceedPermittedDailyWorkingHours(string timeSheetId, DateOnly date) : Error(nameof(DayHoursExceedPermittedDailyWorkingHours), "Total daily hours exceed working hours", $"The number of hours for {date} in TimeSheet with Id {timeSheetId} exceeds the permitted daily working hours.")
{
    public string TimeSheetId { get; } = timeSheetId;

    public DateOnly Date { get; } = date;
}

public record WeekHoursExceedPermittedWeeklyWorkingHours(string timeSheetId) : Error(nameof(WeekHoursExceedPermittedWeeklyWorkingHours), "Total weekly hours exceed working hours", $"The number of hours for TimeSheet with Id {timeSheetId} exceeds the permitted weekly working hours.")
{
    public string TimeSheetId { get; } = timeSheetId;
}