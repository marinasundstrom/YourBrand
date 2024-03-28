using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface ITimeSheetRepository
{
    IQueryable<TimeSheet> GetTimeSheets();

    Task<TimeSheet?> GetTimeSheet(string id, CancellationToken cancellationToken = default);

    Task<TimeSheet?> GetTimeSheetByWeek(string userId, int year, int week, CancellationToken cancellationToken = default);
}
