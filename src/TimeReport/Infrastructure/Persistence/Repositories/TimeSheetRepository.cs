using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class TimeSheetRepository(TimeReportContext context) : ITimeSheetRepository
{
    public async Task<TimeSheet?> GetTimeSheet(string id, CancellationToken cancellationToken = default)
    {
        return await context.TimeSheets
                .Include(x => x.User)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.TaskType)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
    }

    public async Task<TimeSheet?> GetTimeSheetByWeek(string userId, int year, int week, CancellationToken cancellationToken = default)
    {
        return await context.TimeSheets
                .Include(x => x.User)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.TaskType)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .FirstOrDefaultAsync(x => x.UserId == userId && x.Year == year && x.Week == week, cancellationToken);
    }

    public IQueryable<TimeSheet> GetTimeSheets()
    {
        return context.TimeSheets
                .Include(x => x.User)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.TaskType)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Tasks)
                .ThenInclude(x => x.Task)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .OrderByDescending(x => x.Year)
                .ThenByDescending(x => x.Week)
                .AsQueryable();
    }
}