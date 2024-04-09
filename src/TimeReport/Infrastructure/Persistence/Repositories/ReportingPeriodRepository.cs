using Microsoft.EntityFrameworkCore;

using YourBrand.TimeReport.Domain.Entities;
using YourBrand.TimeReport.Domain.Repositories;
using YourBrand.TimeReport.Infrastructure.Persistence;

namespace YourBrand.TimeReport.Domain;

public sealed class ReportingPeriodRepository(TimeReportContext context) : IReportingPeriodRepository
{
    public void AddReportingPeriod(ReportingPeriod monthEntryGroup)
    {
        context.ReportingPeriods.Add(monthEntryGroup);
    }

    public async Task<ReportingPeriod?> GetReportingPeriod(string userId, int year, int month, CancellationToken cancellationToken = default)
    {
        return await context.ReportingPeriods
                .Include(x => x.User)
                .Include(meg => meg.Entries)
                .ThenInclude(e => e.TimeSheet)
                .AsSplitQuery()
                .FirstOrDefaultAsync(meg =>
                    meg.UserId == userId
                    && meg.Year == year
                    && meg.Month == month, cancellationToken);
    }

    public async Task<IEnumerable<ReportingPeriod>> GetReportingPeriodForTimeSheet(TimeSheet timeSheet, CancellationToken cancellationToken = default)
    {
        return await context.ReportingPeriods
                .Where(x => x.UserId == timeSheet.UserId)
                .Where(x => x.Month == timeSheet.From.Month || x.Month == timeSheet.To.Month)
                .ToArrayAsync(cancellationToken);
    }
}