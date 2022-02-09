namespace Skynet.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }

    string? GetAccessToken();
}