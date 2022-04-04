namespace YourBrand.Showroom.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }
    string? Role { get; }

    string? GetAccessToken();

    void SetCurrentUser(string userId);
}
