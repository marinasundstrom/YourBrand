using System.ComponentModel;

namespace YourBrand.Analytics.Domain.ValueObjects;

[Description("Represents a set of geo-coordinates.")]
public record Coordinates(double Latitude, double Longitude);