namespace YourBrand.Analytics.Application.Features.Statistics;

public record class Series(string Name, IEnumerable<decimal> Data);