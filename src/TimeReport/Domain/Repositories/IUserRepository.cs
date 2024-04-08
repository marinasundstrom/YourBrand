using YourBrand.Identity;
using YourBrand.TimeReport.Domain.Entities;

namespace YourBrand.TimeReport.Domain.Repositories;

public interface IUserRepository
{
    void AddUser(User user);

    void RemoveUser(User user);

    Task<User?> GetUser(UserId id, CancellationToken cancellationToken = default);
}