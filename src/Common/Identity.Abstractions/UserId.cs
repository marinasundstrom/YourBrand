using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace YourBrand.Identity;

public struct UserId(string value)
{
    public string Value { get; private set; } = value;

    public override bool Equals([NotNullWhen(true)] object? obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return Value?.GetHashCode() ?? string.Empty.GetHashCode();
    }

    public override string ToString()
    {
        return Value?.ToString() ?? string.Empty;
    }

    public static bool operator ==(UserId lhs, UserId rhs) => lhs.Value == rhs.Value;

    public static bool operator !=(UserId lhs, UserId rhs) => lhs.Value != rhs.Value;

    public static implicit operator UserId(string id) => id is null ? default : new UserId(id);

    public static implicit operator UserId?(string? id) => id is null ? (UserId?)null : new UserId(id);

    public static implicit operator string(UserId id) => id.Value;

    public static bool TryParse(string? value, out UserId userId)
    {
        return TryParse(value, CultureInfo.CurrentCulture, out userId);
    }

    public static bool TryParse(string? value, IFormatProvider? provider, out UserId userId)
    {
        if (value is null)
        {
            userId = default;
            return false;
        }

        userId = value;
        return true;
    }
}