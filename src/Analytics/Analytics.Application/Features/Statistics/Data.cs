using System.ComponentModel;

namespace YourBrand.Analytics.Application.Features.Statistics;

[Description("Represents the data of a graph, its series, and labels.")]
public record class Data(string[] Labels, IEnumerable<Series> Series);