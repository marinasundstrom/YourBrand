namespace Catalog.Application.Common.Interfaces;

public interface ICurrentUserService
{
    string? UserId { get; }

    string? GetAccessToken();
}