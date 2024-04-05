namespace YourBrand.StoreFront.API;

public interface IUserContext
{
    string? UserId { get; }

    string? ClientId { get; }

    string? SessionId { get; }

    int? CustomerNo { get; }

    string? UserAgent { get; }

    string? Host { get; }

    string? GetRemoteIPAddress(bool allowForwarded = true);
}