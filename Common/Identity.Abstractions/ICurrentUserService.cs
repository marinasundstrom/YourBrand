namespace YourBrand.Identity;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? FirstName { get; }
    string? LastName { get; }
    string? Email { get; }
    string? Role { get; }

    string? GetAccessToken();

    void SetCurrentUser(string userId);
}