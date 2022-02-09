namespace TimeReport.Application.Common.Models;

public record class Data(string[] Labels, IEnumerable<Series> Series);