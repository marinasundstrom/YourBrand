namespace TimeReport.Application.Common.Models;

public record class Series(string Name, IEnumerable<decimal> Data);