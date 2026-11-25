namespace YourBrand.TimeReport.Features.Common.Models;

public record class Data(string[] Labels, IEnumerable<Series> Series);