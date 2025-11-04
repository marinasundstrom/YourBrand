using System;

namespace YourBrand.Inventory.Domain.ValueObjects;

/// <summary>
/// Represents a physical location within a warehouse using a concise label.
/// Wraps the label so that formatting and validation are centralized instead of
/// relying on ad-hoc strings throughout the domain.
/// </summary>
public sealed class StorageLocation : IEquatable<StorageLocation>
{
    private StorageLocation(string label)
    {
        Label = label;
    }

    public string Label { get; }

    public static StorageLocation Create(string label)
    {
        if (string.IsNullOrWhiteSpace(label))
        {
            throw new ArgumentException("Location label cannot be empty.", nameof(label));
        }

        var trimmed = label.Trim();

        return new StorageLocation(trimmed);
    }

    public bool Equals(StorageLocation? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return string.Equals(Label, other.Label, StringComparison.OrdinalIgnoreCase);
    }

    public override bool Equals(object? obj) => Equals(obj as StorageLocation);

    public override int GetHashCode() => StringComparer.OrdinalIgnoreCase.GetHashCode(Label);

    public override string ToString() => Label;
}
