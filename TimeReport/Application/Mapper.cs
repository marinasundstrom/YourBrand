using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using YourBrand.TimeReport.Application.Activities;
using YourBrand.TimeReport.Application.Projects.Expenses;
using YourBrand.TimeReport.Application.Projects;
using YourBrand.TimeReport.Application.TimeSheets;
using YourBrand.TimeReport.Application.Users;
using YourBrand.TimeReport.Application.Users.Absence;
using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Application.Activities.ActivityTypes;
using YourBrand.TimeReport.Application.Projects.Expenses.ExpenseTypes;
using YourBrand.TimeReport.Application.Projects.ProjectGroups;
using YourBrand.TimeReport.Application.Organizations;
using YourBrand.TimeReport.Application.Teams;

namespace YourBrand.TimeReport.Application;

public static class Mapper
{
    public static UserDto ToDto(this Domain.Entities.User user)
    {
        return new (user.Id, user.FirstName, user.LastName, user.DisplayName, user.SSN, user.Email, user.Teams.Select(t => t.ToDto()), user.Created, user.Deleted);
    }

    public static ProjectDto ToDto(this Domain.Entities.Project project)
    {
        return new (project.Id, project.Name, project.Description, project.Organization.ToDto(), project.Teams.Select(t => t.ToDto()));
    }

    public static ProjectGroupDto ToDto(this Domain.Entities.ProjectGroup projectGroup)
    {
        return new (projectGroup.Id, projectGroup.Name, projectGroup.Description, projectGroup.Project?.ToDto());
    }

    public static ProjectMembershipDto ToDto(this Domain.Entities.ProjectMembership projectMembership)
    {
        return new ProjectMembershipDto(projectMembership.Id, projectMembership.Project.ToDto(),
                projectMembership.User.ToDto(),
                projectMembership.From, projectMembership.To);
    }
    
    public static ActivityDto ToDto(this Domain.Entities.Activity activity)
    {
        return new (activity.Id, activity.Name, activity.ActivityType.ToDto(), activity.Description, activity.HourlyRate, activity.Project.ToDto());
    }

    public static ActivityTypeDto ToDto(this Domain.Entities.ActivityType activityType)
    {
        return new (activityType.Id, activityType.Name, activityType.Description, activityType.ExcludeHours, activityType.Project?.ToDto());
    }

    public static ExpenseDto ToDto(this Domain.Entities.Expense expense)
    {
        return new (expense.Id, expense.Date.ToDateTime(TimeOnly.Parse("01:00")), expense.ExpenseType.ToDto(), expense.Amount, expense.Description, expense.Attachment, expense.Project.ToDto());
    }

    public static ExpenseTypeDto ToDto(this Domain.Entities.ExpenseType expenseType)
    {
        return new (expenseType.Id, expenseType.Name, expenseType.Description, expenseType.Project?.ToDto());
    }

    public static TimeSheetDto ToDto(this Domain.Entities.TimeSheet timeSheet, IEnumerable<ReportingPeriod> period)
    {
        var activities = timeSheet.Activities
            .OrderBy(e => e.Created)
            .Select(a => a.ToDto())
            .ToArray();

        return new (timeSheet.Id, timeSheet.Year, timeSheet.Week, timeSheet.From, timeSheet.To, (TimeSheetStatusDto)timeSheet.Status, timeSheet.User.ToDto(),
                activities, period.Select(x => new ReportingPeriodDto(x.Month, x.Status == EntryStatus.Locked)));
    }

    public static TimeSheetActivityDto ToDto(this Domain.Entities.TimeSheetActivity activity)
    {
        return new (activity.Activity.Id, activity.Activity.Name, activity.Activity.Description, activity.Project.ToDto(),
                    activity.Entries.OrderBy(e => e.Date).Select(e => e.ToTimeSheetEntryDto()));
    }

    public static TimeSheetEntryDto ToTimeSheetEntryDto(this Domain.Entities.Entry entry)
    {
        return new (entry.Id, entry.Date.ToDateTime(TimeOnly.Parse("01:00")), entry.Hours, entry.Description, (EntryStatusDto)entry.MonthGroup.Status);
    }

    public static EntryDto ToDto(this Domain.Entities.Entry entry)
    {
        return new (entry.Id, entry.Project.ToDto(), entry.Activity.ToDto(), entry.Date.ToDateTime(TimeOnly.Parse("01:00")), entry.Hours, entry.Description, (EntryStatusDto)entry.MonthGroup.Status);
    }

    public static AbsenceDto ToDto(this Domain.Entities.Absence absence)
    {
        return new (absence.Id, absence.Date.GetValueOrDefault().ToDateTime(TimeOnly.Parse("00:01")), absence.Note, absence.Project?.ToDto());
    }

    public static OrganizationDto ToDto(this Domain.Entities.Organization organization)
    {
        return new (organization.Id, organization.Name);
    }

    public static TeamDto ToDto(this Domain.Entities.Team team)
    {
        return new (team.Id, team.Name, team.Memberships.Select(x => x.ToDto()));
    }

    public static TeamMemberDto ToDto(this Domain.Entities.TeamMembership teamMember)
    {
        return new (teamMember.User.Id, teamMember.User.FirstName, teamMember.User.LastName);
    }

    public static TeamMembershipDto ToDto2(this Domain.Entities.TeamMembership teamMembership)
    {
        return new (teamMembership.Id, teamMembership.User.ToDto());
    }
}