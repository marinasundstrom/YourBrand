
using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application.Common.Interfaces;

public interface ITimeReportContext : IDisposable
{
    DbSet<User> Users { get; set; }
    DbSet<Organization> Organizations { get; set; }
    DbSet<OrganizationUser> OrganizationUsers { get; set; }
    DbSet<Team> Teams { get; set; }
    DbSet<TeamMembership> TeamMemberships { get; set; }
    DbSet<Absence> Absence { get; set; }
    DbSet<AbsenceType> AbsenceTypes { get; set; }
    DbSet<Project> Projects { get; set; }
    DbSet<ProjectGroup> ProjectGroups { get; set; }
    DbSet<ExpenseType> ExpenseTypes { get; set; }
    DbSet<ProjectMembership> ProjectMemberships { get; set; }
    DbSet<ProjectTeam> ProjectTeams { get; set; }
    DbSet<Expense> Expenses { get; set; }
    DbSet<Domain.Entities.Task> Tasks { get; set; }
    DbSet<TaskType> TaskTypes { get; set; }
    DbSet<Entry> Entries { get; set; }
    DbSet<TimeSheet> TimeSheets { get; set; }
    DbSet<ReportingPeriod> ReportingPeriods { get; set; }
    DbSet<TimeSheetTask> TimeSheetTasks { get; set; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}