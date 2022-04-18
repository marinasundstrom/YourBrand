namespace YourBrand.ApiKeys.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }

    void SetCurrentUser(string userId);
}