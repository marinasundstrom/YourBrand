namespace YourBrand.Identity;

public interface IUserContext
{
    UserId? UserId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    string? Role { get; }

    bool IsInRole(string role);

    string? GetAccessToken();

    string? ConnectionId { get; }
}