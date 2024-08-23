namespace YourBrand.Identity;

public interface IUserContext
{
    UserId? UserId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    IEnumerable<string>? Roles { get; }

    bool IsInRole(string role);

    string? GetAccessToken();

    string? ConnectionId { get; }
}