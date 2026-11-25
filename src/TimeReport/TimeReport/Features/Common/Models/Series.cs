namespace YourBrand.TimeReport.Features.Common.Models;

public record class Series(string Name, IEnumerable<decimal> Data);