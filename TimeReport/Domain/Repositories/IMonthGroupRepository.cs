using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IMonthGroupRepository
{
    Task<MonthEntryGroup?> GetMonthGroupForUser(string userId, int year, int month, CancellationToken cancellationToken = default);

    void AddMonthGroup(MonthEntryGroup monthEntryGroup);

    Task<IEnumerable<MonthEntryGroup>> GetMonthGroupsForTimeSheet(TimeSheet timeSheet, CancellationToken cancellationToken = default);
}
