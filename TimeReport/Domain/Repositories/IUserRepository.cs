using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetUser(string id, CancellationToken cancellationToken = default);
}