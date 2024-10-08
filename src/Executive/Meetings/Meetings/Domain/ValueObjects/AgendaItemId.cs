using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Meetings.Domain.ValueObjects;

public struct AgendaItemId
{
    public AgendaItemId(string value) => Value = value;

    public AgendaItemId() => Value = Guid.NewGuid().ToString();

    public string Value { get; set; }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return (Value ?? string.Empty).GetHashCode();
    }

    public override string ToString()
    {
        return (Value ?? string.Empty).ToString();
    }

    public static bool operator ==(AgendaItemId lhs, AgendaItemId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(AgendaItemId lhs, AgendaItemId rhs) => lhs.Value != rhs.Value;

    public static implicit operator AgendaItemId(string id) => new AgendaItemId(id);

    public static implicit operator AgendaItemId?(string? id) => id is null ? (AgendaItemId?)null : new AgendaItemId(id);

    public static implicit operator string(AgendaItemId id) => id.Value;

    public static bool TryParse(string? value, out AgendaItemId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out AgendaItemId channelParticipantId)
    {
        if (value is null)
        {
            channelParticipantId = default;
            return false;
        }

        channelParticipantId = value;
        return true;
    }
}