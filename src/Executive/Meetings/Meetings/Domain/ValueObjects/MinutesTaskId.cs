using System;

namespace YourBrand.Meetings.Domain.ValueObjects;

public readonly struct MinutesTaskId : IEquatable<MinutesTaskId>
{
    public MinutesTaskId(string value)
    {
        Value = value;
    }

    public MinutesTaskId()
        : this(Guid.NewGuid().ToString())
    {
    }

    public string Value { get; }

    public bool Equals(MinutesTaskId other) => Value == other.Value;

    public override bool Equals(object? obj) => obj is MinutesTaskId other && Equals(other);

    public override int GetHashCode() => Value.GetHashCode(StringComparison.Ordinal);

    public override string ToString() => Value;

    public static bool operator ==(MinutesTaskId lhs, MinutesTaskId rhs) => lhs.Equals(rhs);

    public static bool operator !=(MinutesTaskId lhs, MinutesTaskId rhs) => !lhs.Equals(rhs);

    public static implicit operator MinutesTaskId(string id) => new(id);

    public static implicit operator string(MinutesTaskId id) => id.Value;

    public static bool TryParse(string? value, out MinutesTaskId minutesTaskId)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            minutesTaskId = default;
            return false;
        }

        minutesTaskId = new MinutesTaskId(value);
        return true;
    }
}
