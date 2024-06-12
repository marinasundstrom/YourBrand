using System.Diagnostics.CodeAnalysis;

namespace ChatApp.Domain.ValueObjects;

public struct UserId
{
    public UserId(string value) => Value = value;

    public string Value { get; private set; }

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
}
