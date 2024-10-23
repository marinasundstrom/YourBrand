using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.ChatApp.Domain.ValueObjects;

public struct MessageId
{
    public MessageId(string value) => Value = value;

    public MessageId() => Value = Guid.NewGuid().ToString();

    public string Value { get; set; }

    public override int GetHashCode()
    {
        return (Value ?? string.Empty).GetHashCode();
    }

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override string ToString()
    {
        return (Value ?? string.Empty).ToString();
    }

    public static bool operator ==(MessageId lhs, MessageId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(MessageId lhs, MessageId rhs) => lhs.Value != rhs.Value;

    public static implicit operator MessageId(string id) => new MessageId(id);

    public static implicit operator string(MessageId id) => id.Value;

    public static bool TryParse(string? value, out MessageId messageId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out messageId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out MessageId messageId)
    {
        if (value is null)
        {
            messageId = default;
            return false;
        }

        messageId = value;
        return true;
    }
}