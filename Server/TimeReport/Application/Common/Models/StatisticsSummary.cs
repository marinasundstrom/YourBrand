namespace TimeReport.Application.Common.Models;

public record class StatisticsSummary(IEnumerable<StatisticsSummaryEntry> Entries);