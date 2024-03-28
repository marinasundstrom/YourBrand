using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IReportingPeriodRepository
{
    Task<ReportingPeriod?> GetReportingPeriod(string userId, int year, int month, CancellationToken cancellationToken = default);

    void AddReportingPeriod(ReportingPeriod monthEntryGroup);

    Task<IEnumerable<ReportingPeriod>> GetReportingPeriodForTimeSheet(TimeSheet timeSheet, CancellationToken cancellationToken = default);
}
