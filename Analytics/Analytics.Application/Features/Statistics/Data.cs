namespace YourBrand.Analytics.Application.Features.Statistics;

public record class Data(string[] Labels, IEnumerable<Series> Series);
