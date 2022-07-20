namespace YourBrand.Portal.Services;

public interface ICurrentUserService
{
    Task<string?> GetUserId();
    Task<bool> IsUserInRole(string role);

    Task<string?> GetOrganizationId();
}