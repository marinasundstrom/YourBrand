
using YourBrand.Messenger.Domain.Entities;

namespace YourBrand.Messenger.Domain.Repositories;

public interface IUserRepository
{
    Task<User?> GetUserById(string id, CancellationToken cancellationToken = default);
}