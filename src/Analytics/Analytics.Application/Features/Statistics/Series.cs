using System.ComponentModel;

namespace YourBrand.Analytics.Application.Features.Statistics;

[Description("Represents a named series values.")]
public record class Series(string Name, IEnumerable<decimal> Data);