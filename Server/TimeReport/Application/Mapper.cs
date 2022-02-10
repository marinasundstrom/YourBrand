using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using Skynet.TimeReport.Application.Activities;
using Skynet.TimeReport.Application.Expenses;
using Skynet.TimeReport.Application.Projects;
using Skynet.TimeReport.Application.TimeSheets;
using Skynet.TimeReport.Application.Users;
using Skynet.TimeReport.Domain.Entities;

namespace Skynet.TimeReport.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new UserDto(user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Created, user.Deleted);
    }

    public static ProjectDto ToDto(this Domain.Entities.Project project)
    {
        return new ProjectDto(project.Id, project.Name, project.Description);
    }

    public static ActivityDto ToDto(this Domain.Entities.Activity activity)
    {
        return new ActivityDto(activity.Id, activity.Name, activity.Description, activity.HourlyRate, activity.Project.ToDto());
    }

    public static ExpenseDto ToDto(this Domain.Entities.Expense expense)
    {
        return new ExpenseDto(expense.Id, expense.Date.ToDateTime(TimeOnly.Parse("01:00")), expense.Amount, expense.Description, expense.Attachment, expense.Project.ToDto());
    }

    public static TimeSheetDto ToDto(this Domain.Entities.TimeSheet timeSheet, IEnumerable<MonthEntryGroup> monthInfo)
    {
        var activities = timeSheet.Activities
            .OrderBy(e => e.Created)
            .Select(a => a.ToDto())
            .ToArray();

        return new TimeSheetDto(timeSheet.Id, timeSheet.Year, timeSheet.Week, timeSheet.From, timeSheet.To, (TimeSheetStatusDto)timeSheet.Status, timeSheet.User.ToDto(),
                activities, monthInfo.Select(x => new MonthInfoDto(x.Month, x.Status == EntryStatus.Locked)));
    }

    public static TimeSheetActivityDto ToDto(this Domain.Entities.TimeSheetActivity activity)
    {
        return new TimeSheetActivityDto(activity.Activity.Id, activity.Activity.Name, activity.Activity.Description, activity.Project.ToDto(),
                    activity.Entries.OrderBy(e => e.Date).Select(e => e.ToTimeSheetEntryDto()));
    }

    public static TimeSheetEntryDto ToTimeSheetEntryDto(this Domain.Entities.Entry entry)
    {
        return new TimeSheetEntryDto(entry.Id, entry.Date.ToDateTime(TimeOnly.Parse("01:00")), entry.Hours, entry.Description, (EntryStatusDto)entry.MonthGroup.Status);
    }
}