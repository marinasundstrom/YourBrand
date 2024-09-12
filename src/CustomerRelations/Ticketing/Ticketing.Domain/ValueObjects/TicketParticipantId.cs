using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Ticketing.Domain.ValueObjects;

public struct TicketParticipantId
{
    public TicketParticipantId(string value) => Value = value;

    public TicketParticipantId() => Value = Guid.NewGuid().ToString();

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

    public static bool operator ==(TicketParticipantId lhs, TicketParticipantId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(TicketParticipantId lhs, TicketParticipantId rhs) => lhs.Value != rhs.Value;

    public static implicit operator TicketParticipantId(string id) => new TicketParticipantId(id);

    public static implicit operator TicketParticipantId?(string? id) => id is null ? (TicketParticipantId?)null : new TicketParticipantId(id);

    public static implicit operator string(TicketParticipantId id) => id.Value;

    public static bool TryParse(string? value, out TicketParticipantId channelParticipantId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out channelParticipantId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out TicketParticipantId channelParticipantId)
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