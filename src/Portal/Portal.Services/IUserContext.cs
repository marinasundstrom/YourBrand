namespace YourBrand.Portal.Services;

public interface IUserContext
{
    Task<string?> GetUserId();
    Task<IEnumerable<string>> GetRoles();
    Task<bool> IsUserInRole(string role);

    Task<string?> GetOrganizationId();
}