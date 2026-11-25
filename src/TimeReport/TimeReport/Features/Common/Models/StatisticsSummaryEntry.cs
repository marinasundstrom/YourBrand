namespace YourBrand.TimeReport.Features.Common.Models;

public record class StatisticsSummaryEntry(string Name, double? Value, decimal? Value2 = null, string? unit = null);