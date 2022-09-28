using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class MonthGroupRepository : IMonthGroupRepository
{
    private readonly TimeReportContext _context;

    public MonthGroupRepository(TimeReportContext context)
    {
        _context = context;
    }

    public void AddMonthGroup(MonthEntryGroup monthEntryGroup)
    {
        _context.TimeSheetMonths.Add(monthEntryGroup);
    }

    public async Task<MonthEntryGroup?> GetMonthGroupForUser(string userId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSheetMonths
                .Include(x => x.User)
                .Include(meg => meg.Entries)
                .ThenInclude(e => e.TimeSheet)
                .AsSplitQuery()
                .FirstOrDefaultAsync(meg =>
                    meg.UserId == userId
                    && meg.Year == year
                    && meg.Month == month, cancellationToken);
    }

    public async Task<IEnumerable<MonthEntryGroup>> GetMonthGroupsForTimeSheet(TimeSheet timeSheet, CancellationToken cancellationToken = default)
    {
        return await _context.TimeSheetMonths
                .Where(x => x.UserId == timeSheet.UserId)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);
    }
}