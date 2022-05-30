
using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Application;

public static class Extensions
{
    public static async Task<TimeSheet?> GetTimeSheetAsync(this DbSet<TimeSheet> dbSet, string timeSheetId, CancellationToken cancellationToken = default)
    {
            return await dbSet
                .Include(x => x.User)
                .Include(x => x.Activities)
                .Include(x => x.Entries)
                .ThenInclude(x => x.MonthGroup)
                .Include(x => x.Entries)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Project)
                .ThenInclude(x => x.Organization)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.ActivityType)
                .Include(x => x.Entries)
                .ThenInclude(x => x.Activity)
                .ThenInclude(x => x.Project)
                .AsSplitQuery()
                .FirstOrDefaultAsync(x => x.Id == timeSheetId, cancellationToken);
    }

    public static async Task<MonthEntryGroup?> GetMonthGroup(this DbSet<MonthEntryGroup> dbSet, string userId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await dbSet.Include(x => x.User)
                .Include(meg => meg.Entries)
                .ThenInclude(e => e.TimeSheet)
                .AsSplitQuery()
                .FirstOrDefaultAsync(meg =>
                    meg.UserId == userId
                    && meg.Year == year
                    && meg.Month == month, cancellationToken);

    }
}