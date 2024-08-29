namespace YourBrand.Portal.Services;

public interface IUserContext
{
    Task<string?> GetUserId();
    Task<string?> GetTenantId();
    Task<IEnumerable<string>> GetRoles();
    Task<bool> IsUserInRole(string role);
}